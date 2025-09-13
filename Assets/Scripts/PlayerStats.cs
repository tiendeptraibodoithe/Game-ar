using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Current Stats")]
    public float baseDamage = 10f;
    public float baseFireRate = 0.5f;

    [Header("Current Modifiers")]
    public float currentDamageBonus = 0f;
    public float currentFireRateReduction = 0f;

    // Properties để lấy stats cuối cùng
    public float CurrentDamage => baseDamage + currentDamageBonus;
    public float CurrentFireRate => Mathf.Max(0.1f, baseFireRate - currentFireRateReduction);

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Subscribe to upgrade events
        GameEvents.Instance.OnDamageUpgrade += IncreaseDamage;
        GameEvents.Instance.OnFireRateUpgrade += IncreaseFireRate;
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnDamageUpgrade -= IncreaseDamage;
            GameEvents.Instance.OnFireRateUpgrade -= IncreaseFireRate;
        }
    }

    private void IncreaseDamage(float amount)
    {
        currentDamageBonus += amount;
        Debug.Log($"Total damage now: {CurrentDamage}");
    }

    private void IncreaseFireRate(float reduction)
    {
        currentFireRateReduction += reduction;
        Debug.Log($"Fire rate now: {CurrentFireRate}");
    }
}