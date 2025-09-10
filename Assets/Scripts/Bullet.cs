using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float damage = 10f;
    public GameObject hitEffect; // Hiệu ứng khi trúng đích (optional)

    [Header("Collision Detection")]
    public LayerMask hitLayers = -1; // Layers mà đạn có thể va chạm
    public bool destroyOnHit = true;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Đảm bảo đạn có Collider
        if (GetComponent<Collider>() == null)
        {
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
        }

        // Đặt tag cho đạn
        gameObject.tag = "Bullet";
    }

    void OnTriggerEnter(Collider other)
    {
        // Bỏ qua va chạm với Bird (người bắn)
        if (other.CompareTag("Player"))
        {
            return;
        }

        // Kiểm tra layer
        if (((1 << other.gameObject.layer) & hitLayers) == 0)
        {
            return;
        }

        // Xử lý va chạm
        HandleHit(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Backup collision detection
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Bird"))
        {
            return;
        }

        HandleHit(collision.collider);
    }

    private void HandleHit(Collider hitTarget)
    {
        Debug.Log($"Đạn trúng: {hitTarget.name}");

        // Tạo hiệu ứng va chạm
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        // Gây damage (nếu target có component nhận damage)
        IDamageable damageable = hitTarget.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        // Xóa đạn
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}

// Interface cho các object có thể nhận damage
public interface IDamageable
{
    void TakeDamage(float damage);
}
