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
    public float fireRate = 0.5f; // Thời gian giữa mỗi lần bắn (giây)

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

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb == null)
        {
            bulletRb = bullet.AddComponent<Rigidbody>();
        }

        bulletRb.useGravity = false;
        bulletRb.drag = 0f;
        bulletRb.angularDrag = 0f;
        bulletRb.velocity = firePoint.forward * bulletSpeed;

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        Destroy(bullet, bulletLifetime);

        Debug.Log("Bird đã bắn!");
    }
}
