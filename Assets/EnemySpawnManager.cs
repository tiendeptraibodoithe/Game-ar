using UnityEngine;
using System.Collections;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    [Tooltip("Điểm gốc để tính vị trí spawn")]
    public Transform spawnAnchor;
    public Vector3 spawnOffset = new Vector3(12f, 0f, 0f);
    public Vector3 destroyPos = new Vector3(-10f, 0f, 0f);

    [Header("Random Y Offset Settings")]
    public float minYOffset = -2f;
    public float maxYOffset = 2f;

    [Header("Spawn Control")]
    public float spawnInterval = 3.5f;
    public bool autoSpawn = true;
    public int maxEnemies = 3;

    [Header("Spawn Method")]
    [Tooltip("Spawn enemy như con của spawnAnchor để tự động theo tracking")]
    public bool spawnAsAnchorChild = true;

    private bool isSpawning = false;
    private int currentEnemyCount = 0;

    void Start()
    {
        spawnAnchor.localScale = Vector3.one;
        Debug.Log($"SpawnAnchor scale: {spawnAnchor.lossyScale}");
    }

    private IEnumerator AutoSpawnCoroutine()
    {
        while (autoSpawn)
        {
            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy prefab is null!");
            return;
        }

        // Random Y offset
        float randomYOffset = Random.Range(Mathf.RoundToInt(minYOffset), Mathf.RoundToInt(maxYOffset) + 1);

        if (spawnAsAnchorChild && spawnAnchor != null)
        {
            Vector3 localSpawnPos = new Vector3(
                spawnOffset.x,
                spawnOffset.y + randomYOffset,
                spawnOffset.z
            );

            GameObject enemy = Instantiate(enemyPrefab, spawnAnchor);
            enemy.transform.localPosition = localSpawnPos;

            // 🔑 Giữ nguyên scale gốc của prefab
            enemy.transform.localScale = enemyPrefab.transform.localScale;
            Vector3 desiredWorldScale = enemyPrefab.transform.localScale;
            Vector3 parentScale = spawnAnchor.lossyScale;
            Vector3 requiredLocalScale = new Vector3(
                desiredWorldScale.x / parentScale.x,
                desiredWorldScale.y / parentScale.y,
                desiredWorldScale.z / parentScale.z
            );
            enemy.transform.localScale = requiredLocalScale;

            Debug.Log($"Enemy spawned as anchor child at local pos: {localSpawnPos}, scale: {enemy.transform.localScale}");
        }

        currentEnemyCount++;

        // Setup enemy component
        EnemyMove move = GameObject.FindObjectOfType<EnemyMove>(); // Get the newest spawned enemy
        if (move != null)
        {
            move.parentManager = this;
        }
    }

    public void OnEnemyDestroyed()
    {
        currentEnemyCount--;
        Debug.Log($"Enemy destroyed. Remaining enemies: {currentEnemyCount}");
    }

    public void ManualSpawn()
    {
        if (!isSpawning && currentEnemyCount < maxEnemies)
        {
            StartCoroutine(SingleSpawnCoroutine());
        }
    }

    public void SpawnCall()
    {
        ManualSpawn();
    }

    private IEnumerator SingleSpawnCoroutine()
    {
        isSpawning = true;
        SpawnEnemy();
        yield return new WaitForSeconds(0.5f);
        isSpawning = false;
    }

    public void StopSpawning()
    {
        autoSpawn = false;
        StopAllCoroutines();
    }

    public void StartSpawning()
    {
        if (!autoSpawn)
        {
            autoSpawn = true;
            StartCoroutine(AutoSpawnCoroutine());
        }
    }

    public void ResetEnemies(bool restartSpawning = true)
    {
        // Xóa toàn bộ enemy con của spawnAnchor hoặc của chính EnemySpawnManager
        if (spawnAsAnchorChild && spawnAnchor != null)
        {
            foreach (Transform child in spawnAnchor)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        currentEnemyCount = 0;

        // Reset coroutine spawn nếu được cho phép
        StopAllCoroutines();
        if (autoSpawn && restartSpawning)
        {
            StartCoroutine(AutoSpawnCoroutine());
        }

        Debug.Log("EnemySpawnManager reset!");
    }

    public void BeginSpawning()
    {
        if (!isSpawning && autoSpawn)
        {
            StartCoroutine(AutoSpawnCoroutine());
        }
    }
}