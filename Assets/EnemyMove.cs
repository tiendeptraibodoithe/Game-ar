using UnityEngine;
using System.Collections;

public class EnemyMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float size = 5f; // Khoảng cách trigger spawn enemy tiếp theo
    public Vector3 moveSpeed = new Vector3(-3f, 0f, 0f);

    [Header("Scale Animation")]
    public Vector3 resize = new Vector3(2f, 2f, 2f);
    private bool grown = false;
    public Vector3 targetScale = new Vector3(1.3f, 1.3f, 1.3f);
    private bool calledSpawn = false;

    [Header("Enemy Behavior")]
    public float health = 30f; // HP của enemy
    public GameObject deathEffect; // Hiệu ứng khi chết (optional)

    // References
    private Vector3 destroyPos;
    public EnemySpawnManager parentManager; // Được gán từ SpawnManager
    public GameManager gameManager;

    // Để tính toán position đúng khi là child của anchor
    private bool isAnchorChild = false;
    private Transform anchorParent;

    // Enemy behavior - UPDATED
    [Header("AI Behavior (Optional)")]
    public bool enableAI = true;
    public float detectionRange = 5f;
    public float chaseSpeed = 4f;
    private Transform player;
    private Vector3 originalMoveSpeed;

    // NEW: Target position system
    private bool hasTarget = false;
    private Vector3 targetPosition;
    private bool wasPlayerInRange = false; // Để track khi player ra khỏi range

    private EnemyShoot enemyShoot;

    void Start()
    {
        gameManager = Object.FindObjectOfType<GameManager>();
        enemyShoot = GetComponent<EnemyShoot>();

        // Kiểm tra nếu parent là anchor (không phải EnemySpawnManager)
        if (transform.parent != null)
        {
            // Nếu parent không phải là EnemySpawnManager thì đây là anchor child
            EnemySpawnManager parentSpawnManager = transform.parent.GetComponent<EnemySpawnManager>();
            if (parentSpawnManager == null)
            {
                isAnchorChild = true;
                anchorParent = transform.parent;
                Debug.Log("Enemy spawned as anchor child");
            }
            else
            {
                parentManager = parentSpawnManager;
            }
        }

        // Lấy destroyPos từ parentManager
        if (parentManager != null)
            destroyPos = parentManager.destroyPos;

        // Gốc vận tốc, đảm bảo z = 0
        originalMoveSpeed = new Vector3(moveSpeed.x, moveSpeed.y, 0f);

        // Tìm player (Bird)
        if (enableAI)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void ResizeUpCheck()
    {
        if (this.transform.localScale.x >= targetScale.x)
        {
            grown = true;
            this.transform.localScale = targetScale; // Dùng target scale thay vì Vector3.one
        }
        else
        {
            ResizeUp();
        }
    }

    void ResizeDownCheck()
    {
        // Khi là anchor child, cần dùng local position để so sánh
        float currentX = isAnchorChild ? transform.localPosition.x : transform.localPosition.x;

        if (currentX <= destroyPos.x)
        {
            if (this.transform.localScale.x <= 0)
                DestroyEnemy();
            else
                ResizeDown();
        }
    }

    void ResizeUp()
    {
        this.transform.localScale += resize * Time.deltaTime;
    }

    void ResizeDown()
    {
        this.transform.localScale -= resize * Time.deltaTime;
    }

    void AIBehavior()
    {
        if (!enableAI || player == null)
        {
            // Trở về vận tốc gốc, z luôn 0
            moveSpeed = new Vector3(originalMoveSpeed.x, originalMoveSpeed.y, 0f);
            if (enemyShoot != null) enemyShoot.SetShootingEnabled(true);
            return;
        }

        // Kiểm tra khoảng cách đến player
        float distanceToPlayer = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(player.position.x, player.position.y)
        );

        bool playerInRange = distanceToPlayer <= detectionRange;

        // Chỉ lấy target position khi player VỪA MỚI vào range (không phải đang ở trong range)
        if (playerInRange && !wasPlayerInRange && !hasTarget)
        {
            // Lấy vị trí player ngay lúc này làm target
            targetPosition = new Vector3(player.position.x, player.position.y, 0f);
            hasTarget = true;
            if (enemyShoot != null) enemyShoot.SetShootingEnabled(false);
            Debug.Log($"Enemy locked onto target position: {targetPosition}");
        }

        // Update trạng thái range tracking
        wasPlayerInRange = playerInRange;

        // Nếu có target, di chuyển về phía target
        if (hasTarget)
        {
            // Tính hướng từ enemy đến target position
            Vector2 direction = new Vector2(
                targetPosition.x - transform.position.x,
                targetPosition.y - transform.position.y
            ).normalized;

            // Kiểm tra nếu đã đến gần target position
            float distanceToTarget = Vector2.Distance(
                new Vector2(transform.position.x, transform.position.y),
                new Vector2(targetPosition.x, targetPosition.y)
            );

            if (distanceToTarget < 0.5f) // Threshold để coi như đã đến target
            {
                // Đã đến target, quay về di chuyển bình thường
                hasTarget = false;
                moveSpeed = new Vector3(originalMoveSpeed.x, originalMoveSpeed.y, 0f);
                if (enemyShoot != null) enemyShoot.SetShootingEnabled(true);
                Debug.Log("Enemy reached target, resuming normal movement");
            }
            else
            {
                // Vẫn chưa đến target, tiếp tục lao về phía target
                moveSpeed = new Vector3(direction.x * chaseSpeed, direction.y * chaseSpeed, 0f);
            }
        }
        else
        {
            // Không có target, di chuyển bình thường
            moveSpeed = new Vector3(originalMoveSpeed.x, originalMoveSpeed.y, 0f);
            if (enemyShoot != null) enemyShoot.SetShootingEnabled(true);
        }
    }

    void FixedUpdate()
    {
        if (gameManager.startedGame && gameManager.stillAlive)
        {
            // AI behavior (nếu có)
            AIBehavior();

            // Di chuyển - sử dụng localPosition khi là anchor child
            if (isAnchorChild)
            {
                // Khi là con của anchor, dùng local position
                transform.localPosition += new Vector3(moveSpeed.x, moveSpeed.y, 0f) * Time.deltaTime;
            }
            else
            {
                // Khi không phải anchor child, dùng world position
                transform.localPosition += moveSpeed * Time.deltaTime;

                // Giữ Z luôn = 0
                Vector3 pos = transform.localPosition;
                pos.z = 0f;
                transform.localPosition = pos;
            }

            // Trigger spawn mới khi enemy đi qua giới hạn
            // Sử dụng local position cho cả 2 trường hợp
            float checkPositionX = transform.localPosition.x;

            if (checkPositionX < (size * -1) && !calledSpawn && parentManager != null)
            {
                parentManager.SpawnCall();
                calledSpawn = true;
                Debug.Log($"Enemy triggered spawn at position: {checkPositionX}");
            }

            // Resize animation
            if (!grown)
                ResizeUpCheck();
            else
                ResizeDownCheck();
        }
    }

    // Nhận damage từ bullet
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Enemy nhận {damage} damage. HP còn lại: {health}");

        if (health <= 0)
        {
            DestroyEnemy();
        }
    }

    void DestroyEnemy()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Debug.Log("Enemy đã chết!");

        // Đảm bảo gọi OnEnemyDestroyed để giảm counter
        if (parentManager != null)
        {
            parentManager.OnEnemyDestroyed();
            Debug.Log("Called OnEnemyDestroyed()");
        }
        else
        {
            Debug.LogWarning("parentManager is null! Counter won't decrease!");
        }

        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enemy va chạm với Player!");
            // Xử lý va chạm với player nếu cần
        }
    }

    void OnDrawGizmosSelected()
    {
        if (enableAI)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Vẽ target position nếu có
            if (hasTarget)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(targetPosition, 0.3f);
                Gizmos.DrawLine(transform.position, targetPosition);
            }
        }
    }

    // Method để manually set parentManager (gọi từ SpawnManager)
    public void SetParentManager(EnemySpawnManager manager)
    {
        parentManager = manager;

        if (parentManager != null)
            destroyPos = parentManager.destroyPos;

        Debug.Log("ParentManager set successfully");
    }
}