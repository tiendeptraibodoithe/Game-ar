using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceManager : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve;
    int currentLevel, totalExperience;
    int previousLevelsExperience, nextLevelsExperience;

    [Header("Enemy Experience Values")]
    [SerializeField] int[] enemyExpValues = { 5, 8, 12, 15 }; // exp cho enemy type 1,2,3,4

    [Header("Interface")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] Image experienceFill;

    // Singleton pattern để dễ truy cập từ Enemy
    public static ExperienceManager Instance;

    void Awake()
    {
        // Đảm bảo chỉ có 1 ExperienceManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateLevel();
    }

    void Update()
    {
    }

    public void AddExperience(int amount)
    {
        totalExperience += amount;
        CheckForLevelUp();
        UpdateInterface();

        Debug.Log($"Gained {amount} exp. Total: {totalExperience}");
    }

    // Method mới để add exp dựa trên enemy type
    public void AddExperienceFromEnemy(int enemyType)
    {
        // enemyType bắt đầu từ 0 (enemy type 1 = index 0)
        if (enemyType >= 0 && enemyType < enemyExpValues.Length)
        {
            int expGained = enemyExpValues[enemyType];
            AddExperience(expGained);
            Debug.Log($"Enemy type {enemyType + 1} killed! Gained {expGained} exp");
        }
        else
        {
            Debug.LogWarning($"Invalid enemy type: {enemyType}");
        }
    }

    void CheckForLevelUp()
    {
        if (totalExperience >= nextLevelsExperience)
        {
            currentLevel++;
            UpdateLevel();
            Debug.Log($"LEVEL UP! New level: {currentLevel}");
            // Start level up sequence... Possibly vfx?
        }
    }

    void UpdateLevel()
    {
        previousLevelsExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelsExperience = (int)experienceCurve.Evaluate(currentLevel + 1);
        UpdateInterface();
    }

    void UpdateInterface()
    {
        int start = totalExperience - previousLevelsExperience;
        int end = nextLevelsExperience - previousLevelsExperience;

        levelText.text = currentLevel.ToString();
        experienceText.text = start + " exp / " + end + " exp";
        experienceFill.fillAmount = (float)start / (float)end;
    }

    // Getter methods (nếu cần)
    public int GetCurrentLevel() => currentLevel;
    public int GetTotalExperience() => totalExperience;
}