using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;   // Prefab đạn
    public Transform firePoint;       // Vị trí bắn
    public float bulletSpeed = 10f;   // Tốc độ bay của đạn
    public float bulletLifetime = 5f; // Thời gian tồn tại của đạn
    public float fireRate = 1f;       // Số viên/giây
    public Transform bulletContainer;
    private bool canShoot = true;

    [Header("Sound (Optional)")]
    public AudioSource audioSource;
    public AudioClip shootSound;

    private float fireTimer;

    void Start()
    {
        if (bulletContainer == null)
        {
            GameObject containerObj = GameObject.Find("BulletContainer");
            if (containerObj != null)
            {
                bulletContainer = containerObj.transform;
            }
        }
    }

    void Update()
    {
        if (!canShoot) return; // Ngừng bắn nếu bị tắt

        fireTimer += Time.deltaTime;
        if (fireTimer >= 1f / fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    public void SetShootingEnabled(bool enabled)
    {
        canShoot = enabled;
    }

    public void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Enemy chưa gán Bullet Prefab hoặc FirePoint!");
            return;
        }

        // Tạo đạn tại fire point
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.transform.SetParent(bulletContainer.transform, true);


        // Lấy Rigidbody của đạn
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb == null)
        {
            bulletRb = bullet.AddComponent<Rigidbody>();
        }

        bulletRb.useGravity = false;
        bulletRb.drag = 0f;
        bulletRb.angularDrag = 0f;

        // Enemy bắn sang trái (trục X âm)
        bulletRb.velocity = firePoint.forward * bulletSpeed;

        // Phát âm thanh bắn (nếu có)
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // Tự động xóa đạn sau một thời gian
        Destroy(bullet, bulletLifetime);

        Debug.Log("Enemy đã bắn!");
    }
}
