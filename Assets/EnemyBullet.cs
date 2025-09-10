using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 1f;
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime); // Tự hủy sau vài giây
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}

