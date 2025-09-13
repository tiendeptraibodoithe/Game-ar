using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static GameEvents Instance { get; private set; }

    [Header("Upgrade Events")]
    public Action<float> OnDamageUpgrade;
    public Action<float> OnFireRateUpgrade;
    // Thêm các event khác khi cần

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
}