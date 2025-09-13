using UnityEngine;

public class UpgradeAttackSpeed : MonoBehaviour
{
    [Header("Attack Speed Upgrade Settings")]
    public float fireRateReduction = 0.1f; // Mức giảm fireRate mỗi lần nâng cấp
    public float minFireRate = 0.1f; // Giới hạn tối thiểu của fireRate

    private BirdShooting birdShooting;

    void Start()
    {
        // Tìm BirdShooting component trong scene
        birdShooting = FindObjectOfType<BirdShooting>();

        if (birdShooting == null)
        {
            Debug.Log("Không tìm thấy BirdShooting component trong scene!");
        }
    }

    public void UpgradeAttackSpeedFunction()
    {
        if (birdShooting == null)
        {
            Debug.LogError("BirdShooting reference is null!");
            return;
        }

        // Giảm fireRate (tăng tốc độ bắn)
        float newFireRate = birdShooting.fireRate - fireRateReduction;

        // Đảm bảo fireRate không nhỏ hơn giới hạn tối thiểu
        newFireRate = Mathf.Max(newFireRate, minFireRate);

        // Cập nhật fireRate mới
        birdShooting.fireRate = newFireRate;

        Debug.Log($"Attack Speed upgraded! New fire rate: {birdShooting.fireRate}s");

        // Có thể thêm hiệu ứng âm thanh hoặc visual effect ở đây
        PlayUpgradeEffect();
    }

    private void PlayUpgradeEffect()
    {
        // Thêm hiệu ứng âm thanh hoặc visual effect khi nâng cấp
        Debug.Log("Attack speed upgrade effect played!");

        // Ví dụ: có thể phát âm thanh nâng cấp
        // AudioSource.PlayClipAtPoint(upgradeSound, transform.position);
    }
}