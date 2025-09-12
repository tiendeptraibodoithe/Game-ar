using UnityEngine;
using System.Collections;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] enemyPrefabs;
    private int unlockedTypes = 1;
    private float elapsedTime = 0f;
    [Tooltip("Điểm gốc để tính vị trí spawn")]
    public Transform spawnAnchor;
    public Vector3 spawnOffset = new Vector3(12f, 0f, 0f);
    public Vector3 destroyPos = new Vector3(-10f, 0f, 0f);

    [Header("Random Y Offset Settings")]
    private float lastYOffset = Mathf.Infinity;
    public float minYOffset = -2f;
    public float maxYOffset = 2f;

    [Header("Spawn Control")]
    public float spawnIntervalStart = 3.5f;
    public float spawnIntervalMin = 1.5f;
    public float spawnInterval;
    public bool autoSpawn = true;
    public int maxEnemiesStart = 3;
    public int maxEnemiesMax = 5;
    public int maxEnemies;
    public float difficultyRampTime = 30f;

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

    void Update()
    {
        if (autoSpawn)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= 30f)
                unlockedTypes = 4;
            else if (elapsedTime >= 20f)
                unlockedTypes = 3;
            else if (elapsedTime >= 10f)
                unlockedTypes = 2;
            else
                unlockedTypes = 1;
        }

        // ---- Điều chỉnh độ khó ----
        float t = Mathf.Clamp01(elapsedTime / difficultyRampTime);

        // Giảm spawnInterval từ start → min
        spawnInterval = Mathf.Lerp(spawnIntervalStart, spawnIntervalMin, t);

        // Tăng maxEnemies từ start → max
        maxEnemies = Mathf.RoundToInt(Mathf.Lerp(maxEnemiesStart, maxEnemiesMax, t));
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
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("Enemy prefabs array is empty!");
            return;
        }

        // Chọn prefab random trong số unlocked
        int index = Random.Range(0, unlockedTypes);
        GameObject prefabToSpawn = enemyPrefabs[index];

        // Random Y offset, đảm bảo cách ít nhất 2 đơn vị so với lần trước
        float randomYOffset;
        int safety = 0; // tránh vòng lặp vô hạn
        do
        {
            randomYOffset = Random.Range(minYOffset, maxYOffset);
            safety++;
            if (safety > 20) break; // thoát nếu quá nhiều vòng lặp
        }
        while (Mathf.Abs(randomYOffset - lastYOffset) < 2f);

        // Cập nhật giá trị mới
        lastYOffset = randomYOffset;

        GameObject spawnedEnemy = null;

        if (spawnAsAnchorChild && spawnAnchor != null)
        {
            Vector3 localSpawnPos = new Vector3(
                spawnOffset.x,
                spawnOffset.y + randomYOffset,
                spawnOffset.z
            );

            spawnedEnemy = Instantiate(prefabToSpawn, spawnAnchor);
            spawnedEnemy.transform.localPosition = localSpawnPos;

            // Giữ scale gốc
            Vector3 desiredWorldScale = prefabToSpawn.transform.localScale;
            Vector3 parentScale = spawnAnchor.lossyScale;
            Vector3 requiredLocalScale = new Vector3(
                desiredWorldScale.x / parentScale.x,
                desiredWorldScale.y / parentScale.y,
                desiredWorldScale.z / parentScale.z
            );
            spawnedEnemy.transform.localScale = requiredLocalScale;

        }

        currentEnemyCount++;

        // Setup enemy component cho đúng spawned instance (FIXED)
        if (spawnedEnemy != null)
        {
            // Set up EnemyMove component
            EnemyMove move = spawnedEnemy.GetComponent<EnemyMove>();
            if (move != null)
            {
                move.parentManager = this;
            }

            // Set up Enemy component với enemy type
            Enemy enemyComponent = spawnedEnemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.SetEnemyType(index); // index 0-3 tương ứng với enemy type 1-4
            }

            Debug.Log($"Enemy type {index + 1} spawned and configured with experience system");
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
        elapsedTime = 0f;   // reset lại thời gian
        unlockedTypes = 1;

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