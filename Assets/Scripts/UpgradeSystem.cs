using UnityEngine;
using System.Collections.Generic;

public class UpgradeSystem : MonoBehaviour
{
    public static UpgradeSystem Instance;

    [Header("Upgrade UI Settings")]
    public Transform[] slots;                 // 3 slot để spawn UI
    public List<GameObject> upgradeUIPrefabs; // List prefab để random
    private List<GameObject> currentUIs = new List<GameObject>();

    [Header("Attack Speed Upgrade Settings")]
    public float fireRateReduction = 0.1f; // Mức giảm fireRate mỗi lần nâng cấp
    public float minFireRate = 0.1f; // Giới hạn tối thiểu của fireRate

    [Header("Damage Upgrade Settings")]
    // Có thể thêm các settings cho nâng cấp khác ở đây
    public float damageIncrease = 5f;
    public float maxDamage = 30f;
    public float healthIncrease = 20f;

    [Header("Shield Upgrade Settings")]
    public int shieldDamageIncrease = 5;
    public float shieldSizeIncrease = 0.5f;

    private BirdShooting birdShooting;
    private Health health;
    [SerializeField] private PlayerShield playerShield;

    public bool isUpgrading = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Không tìm BirdShooting ở đây vì GameParent chưa active
        // Sẽ tìm khi cần thiết trong UpgradeAttackSpeed()
    }

    private BirdShooting GetBirdShooting()
    {
        // Tìm BirdShooting mỗi khi cần (vì có thể GameParent chưa active lúc Start)
        if (birdShooting == null)
        {
            birdShooting = FindObjectOfType<BirdShooting>();

            if (birdShooting != null)
            {
                Debug.Log($"BirdShooting found! Current fire rate: {birdShooting.fireRate}s");
            }
        }

        return birdShooting;
    }

    private Health GetHealth()
    {
        if (health == null)
        {
            health = FindObjectOfType<Health>();
        }
        return health;
    }

    private PlayerShield GetPlayerShield()
    {
        if (playerShield == null)
        {
            playerShield = FindObjectOfType<PlayerShield>();
        }
        return playerShield;
    }

    public void ShowUpgradeOptions()
    {
        ClearCurrentUIs();
        if (upgradeUIPrefabs.Count < slots.Length)
        {
            Debug.LogWarning("Không đủ prefab để spawn số lượng slot!");
            return;
        }
        // Tạo 1 list index shuffle để random không trùng
        List<int> indices = new List<int>();
        for (int i = 0; i < upgradeUIPrefabs.Count; i++) indices.Add(i);
        Shuffle(indices);
        for (int i = 0; i < slots.Length; i++)
        {
            int prefabIndex = indices[i];
            GameObject chosenPrefab = upgradeUIPrefabs[prefabIndex];
            GameObject ui = Instantiate(chosenPrefab, slots[i]);
            currentUIs.Add(ui);
        }
        Time.timeScale = 0f; // pause game
        isUpgrading = true;
    }

    public void OnUpgradeSelected()
    {
        ClearCurrentUIs();
        Time.timeScale = 1f; // resume game
        isUpgrading = false;
    }

    // ============== UPGRADE FUNCTIONS ==============

    public void UpgradeAttackSpeed()
    {
        // Tìm BirdShooting khi cần (vì GameParent có thể chưa active lúc Start)
        BirdShooting shooting = GetBirdShooting();

        if (shooting == null)
        {
            Debug.LogError("BirdShooting component not found! Đảm bảo GameParent đã active và Bird có BirdShooting script.");
            return;
        }

        // Giảm fireRate (tăng tốc độ bắn)
        float oldFireRate = shooting.fireRate;
        float newFireRate = shooting.fireRate - fireRateReduction;

        // Đảm bảo fireRate không nhỏ hơn giới hạn tối thiểu
        newFireRate = Mathf.Max(newFireRate, minFireRate);

        // Cập nhật fireRate mới
        shooting.fireRate = newFireRate;

        Debug.Log($"Attack Speed upgraded! Fire rate: {oldFireRate}s → {shooting.fireRate}s");

    }

    public void UpgradeDamage()
    {
        // Tìm BirdShooting khi cần
        BirdShooting shooting = GetBirdShooting();

        if (shooting == null || shooting.bulletPrefab == null)
        {
            Debug.LogError("BirdShooting hoặc bulletPrefab không tìm thấy! Không thể nâng cấp damage.");
            return;
        }

        // Lấy Bullet component từ prefab
        Bullet bulletComponent = shooting.bulletPrefab.GetComponent<Bullet>();
        if (bulletComponent == null)
        {
            Debug.LogError("Bullet prefab không có Bullet component!");
            return;
        }

        // Tăng damage
        float oldDamage = bulletComponent.damage;
        float newDamage = bulletComponent.damage + damageIncrease;

        // Giới hạn damage tối đa
        newDamage = Mathf.Min(newDamage, maxDamage);

        bulletComponent.damage = newDamage;

        // 👉 Bật particle trong prefab
        ParticleSystem particle = shooting.bulletPrefab.GetComponentInChildren<ParticleSystem>(true); // true = tìm cả inactive
        if (particle != null && !particle.gameObject.activeSelf)
        {
            particle.gameObject.SetActive(true);
            Debug.Log("Bullet particle đã được kích hoạt khi nâng cấp!");
        }

        Debug.Log($"Damage upgraded! Bullet damage: {oldDamage} → {bulletComponent.damage}");
    }


    public void UpgradeHealth()
    {
        Debug.Log($"Health upgraded by {healthIncrease}!");
        Health playerHealth = GetHealth();

        int oldMaxLives = playerHealth.maxLives;

        // Giới hạn maxLives tối đa = 6
        int newMaxLives = Mathf.Min(oldMaxLives + 1, 6);

        // Nếu chưa đạt maxLives thì mới cộng currentLives
        if (newMaxLives > oldMaxLives)
        {
            playerHealth.maxLives = newMaxLives;
            playerHealth.currentLives = Mathf.Min(playerHealth.currentLives + 1, newMaxLives);

            LivesUI livesUI = FindObjectOfType<LivesUI>();
            if (livesUI != null)
            {
                livesUI.RefreshUI();
            }
        }
        else
        {
            Debug.Log("Đã đạt giới hạn số mạng tối đa (6). Không thể nâng cấp thêm!");
        }
    }


    public void UpgradeDoubleBullet()
    {
        BirdShooting shooting = GetBirdShooting();

        if (!shooting.doubleBullet)
        {
            shooting.doubleBullet = true;
            Debug.Log("Double Bullet upgrade activated! Giờ Bird sẽ bắn ra 2 viên đạn cùng lúc.");
        }
        else
        {
            Debug.Log("Double Bullet đã được kích hoạt trước đó!");
        }
    }

    public void UpgradeShield()
    {
        PlayerShield shield = GetPlayerShield();
        if (shield == null) return;

        GameObject shieldObj = shield.gameObject;

        if (!shieldObj.activeSelf) // lần đầu
        {
            shieldObj.SetActive(true); // bật shield
            Debug.Log("Shield activated for the first time!");
        }
        else
        {
            // Đã active thì nâng cấp
            shield.UpgradeShieldDamage(shieldDamageIncrease);
            shield.UpgradeShieldSize(shieldSizeIncrease);
        }
    }

    public void ResetUpgrades()
    {
        // Reset BirdShooting
        birdShooting = FindObjectOfType<BirdShooting>();
        if (birdShooting != null)
        {
            birdShooting.fireRate = 0.8f;  // gốc tuỳ bạn set
            birdShooting.doubleBullet = false;
        }

        // Reset Bullet damage
        if (birdShooting != null && birdShooting.bulletPrefab != null)
        {
            Bullet bullet = birdShooting.bulletPrefab.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.damage = 10f; // gốc tuỳ bạn set
            }
        }

        // Reset Shield
        playerShield = FindObjectOfType<PlayerShield>(true);
        if (playerShield != null)
        {
            playerShield.gameObject.SetActive(false); // shield lại tắt từ đầu
            playerShield.damageOnHit = 10;            // gốc
            playerShield.shieldRadius = 2f;           // gốc
        }

        // Reset Health
        Health health = FindObjectOfType<Health>();
        if (health != null)
        {
            health.maxLives = 3; // gốc tuỳ bạn set
            health.currentLives = health.maxLives;
        }

        Debug.Log("All upgrades reset to default.");
    }



    private void ClearCurrentUIs()
    {
        foreach (var ui in currentUIs)
        {
            if (ui != null) Destroy(ui);
        }
        currentUIs.Clear();
    }

    // Hàm shuffle Fisher–Yates
    private void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}