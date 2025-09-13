using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Settings")]
    public float damage = 1f; // Damage khi chạm Player

    [Header("Health Settings")]
    public float health = 30f; // HP của enemy
    public GameObject deathEffect; // Hiệu ứng khi chết (optional)

    [Header("Experience Settings")]
    [SerializeField] int enemyType = 0; // 0=Enemy1, 1=Enemy2, 2=Enemy3, 3=Enemy4

    private EnemyMove enemyMove;
    private EnemyShoot enemyShoot;

    void Start()
    {
        enemyMove = GetComponent<EnemyMove>();
        enemyShoot = GetComponent<EnemyShoot>();
    }

    // Gọi khi Enemy bị bắn
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Enemy nhận {damage} damage. HP còn lại: {health}");

        if (health <= 0)
        {
            DestroyEnemy(true); // true = killed by player (give exp)
            ScoreCounter.Instance.AddScoreFromEnemy(enemyType);
        }
    }

    void DestroyEnemy(bool killedByPlayer = false)
    {
        // Spawn death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Debug.Log("Enemy đã chết!");

        // Give experience nếu bị player tiêu diệt
        if (killedByPlayer && ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.AddExperienceFromEnemy(enemyType);
        }

        // Thông báo cho SpawnManager để giảm counter
        if (enemyMove != null && enemyMove.parentManager != null)
        {
            enemyMove.parentManager.OnEnemyDestroyed();
            Debug.Log("Called OnEnemyDestroyed()");
        }
        else
        {
            Debug.LogWarning("parentManager is null! Counter won't decrease!");
        }

        Destroy(this.gameObject);
    }

    // Gọi khi Enemy va chạm với Player
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Gây damage cho Player
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            Debug.Log("Enemy va chạm với Player!");
            DestroyEnemy(false); // false = not killed by player (no exp)
        }
        else if (other.CompareTag("Shield"))
        {
            // Nếu va chạm shield thì Enemy mất máu
            TakeDamage(damage);
            Debug.Log("Enemy chạm Shield!");
        }
    }

    // Method để set enemy type (gọi từ SpawnManager)
    public void SetEnemyType(int type)
    {
        enemyType = type;
        Debug.Log($"Enemy type set to: {type + 1}");
    }
}