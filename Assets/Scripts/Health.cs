using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public int maxLives = 3;
    private int currentLives;

    private Rigidbody rb;
    private GameManager gameManager;
    private AudioSource audioSource;

    [Header("Sound")]
    public AudioClip hitSound;

    void Start()
    {
        currentLives = maxLives;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Nhận damage từ đạn (EnemyBullet sẽ gọi hàm này)
    public void TakeDamage(float damage)
    {
        LoseLife();
    }

    // Xử lý va chạm trực tiếp với Enemy
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            LoseLife();
        }
    }

    private void LoseLife()
    {
        currentLives--;

        // Phát âm thanh bị trúng
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (currentLives <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (gameManager != null)
        {
            gameManager.stillAlive = false;
        }

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None; // Không điều khiển được nữa
        }

        Debug.Log("Player đã chết!");
    }

    public void ResetHealth()
    {
        currentLives = maxLives;
        rb.constraints = RigidbodyConstraints.FreezeRotation; // khóa lại như ban đầu (nếu cần)
    }
}
