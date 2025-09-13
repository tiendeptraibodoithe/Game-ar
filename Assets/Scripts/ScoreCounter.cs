using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    // Điểm cho từng loại enemy (index 0 = enemy1, index 1 = enemy2,...)
    [SerializeField] private int[] enemyPoints = { 1, 2, 3, 4 };

    public static ScoreCounter Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Gọi hàm này khi enemy chết, truyền vào enemyType (0 = enemy1, 1 = enemy2,...)
    /// </summary>
    public void AddScoreFromEnemy(int enemyType)
    {
        if (enemyType >= 0 && enemyType < enemyPoints.Length)
        {
            int pointsToAdd = enemyPoints[enemyType];

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddPoints(pointsToAdd);
            }

            Debug.Log($"Enemy type {enemyType + 1} killed! +{pointsToAdd} điểm");
        }
        else
        {
            Debug.LogWarning($"Enemy type {enemyType} không hợp lệ!");
        }
    }
}
