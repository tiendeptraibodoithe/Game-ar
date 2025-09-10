using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab; // Kéo sphere prefab vào đây
    public Transform firePoint; // Điểm bắn đạn (có thể là con của Bird)
    public float bulletSpeed = 100f;
    public float bulletLifetime = 3f;

    [Header("Audio (Optional)")]
    public AudioClip shootSound;
    private AudioSource audioSource;

    void Start()
    {
        // Tạo fire point nếu chưa có
        //if (firePoint == null)
        //{
        //    GameObject firePointObj = new GameObject("FirePoint");
        //    firePointObj.transform.SetParent(transform);
        //    firePointObj.transform.localPosition = Vector3.forward; // Phía trước Bird
        //    firePoint = firePointObj.transform;
        //}

        // Lấy AudioSource component
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Test bắn đạn bằng chuột phải trên máy tính
        if (Input.GetMouseButtonDown(1)) // Chuột phải
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet Prefab chưa được gán!");
            return;
        }

        // Tạo đạn tại fire point
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Lấy Rigidbody của đạn
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb == null)
        {
            bulletRb = bullet.AddComponent<Rigidbody>();
        }

        bulletRb.useGravity = false;
        bulletRb.drag = 0f;
        bulletRb.angularDrag = 0f;
        // Bắn đạn theo hướng Bird đang nhìn
        bulletRb.velocity = firePoint.forward * bulletSpeed;

        // Phát âm thanh bắn (nếu có)
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // Tự động xóa đạn sau một thời gian
        Destroy(bullet, bulletLifetime);

        Debug.Log("Bird đã bắn!");
    }
}
