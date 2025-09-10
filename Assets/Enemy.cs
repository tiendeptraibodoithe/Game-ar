using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Settings")]
    public float damage = 1f; // Damage khi chạm Player

    private EnemyMove enemyMove;

    void Start()
    {
        enemyMove = GetComponent<EnemyMove>();
    }

    // Gọi khi Enemy bị bắn
    public void TakeDamage(float damage)
    {
        if (enemyMove != null)
            enemyMove.TakeDamage(damage);
    }

    // Gọi khi Enemy va chạm với Player
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
