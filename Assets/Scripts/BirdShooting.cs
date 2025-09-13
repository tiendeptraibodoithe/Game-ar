using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 100f;
    public float bulletLifetime = 3f;
    public float fireRate = 0.5f;

    [Header("Upgrade Settings")]
    public bool doubleBullet = false; // bật/tắt chế độ bắn đôi
    public float doubleBulletOffset = 0.5f; // khoảng cách ngang giữa 2 viên đạn

    [Header("Audio (Optional)")]
    public AudioClip shootSound;
    private AudioSource audioSource;

    private float fireTimer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (GameManager.Instance == null ||
        !GameManager.Instance.startedGame ||
        !GameManager.Instance.stillAlive)
        {
            return;
        }

        // Tự động bắn
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    public void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Thiếu Bullet Prefab hoặc FirePoint!");
            return;
        }

        if (doubleBullet)
        {
            // Spawn 2 viên đạn sang trái và phải một chút
            Vector3 leftOffset = firePoint.right * -doubleBulletOffset;
            Vector3 rightOffset = firePoint.right * doubleBulletOffset;

            SpawnBullet(firePoint.position + leftOffset);
            SpawnBullet(firePoint.position + rightOffset);
        }
        else
        {
            // Spawn 1 viên đạn bình thường
            SpawnBullet(firePoint.position);
        }

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        Debug.Log("Bird đã bắn!");
    }

    private void SpawnBullet(Vector3 spawnPos)
    {
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, firePoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

        if (bulletRb == null)
        {
            bulletRb = bullet.AddComponent<Rigidbody>();
        }

        bulletRb.useGravity = false;
        bulletRb.drag = 0f;
        bulletRb.angularDrag = 0f;
        bulletRb.velocity = firePoint.forward * bulletSpeed;

        Destroy(bullet, bulletLifetime);
    }
}
