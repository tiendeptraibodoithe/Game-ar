using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LivesUI : MonoBehaviour
{
    public Health playerHealth;
    public GameObject lifeIconPrefab;
    public Transform livesContainer;

    private List<GameObject> lifeIcons = new List<GameObject>();

    void Start()
    {
        RefreshUI();
    }

    void Update()
    {
        UpdateLives(playerHealth.currentLives);
    }

    public void RefreshUI()
    {
        // Xóa icon cũ
        foreach (var icon in lifeIcons)
        {
            Destroy(icon);
        }
        lifeIcons.Clear();

        // Tạo lại icon mới theo maxLives
        for (int i = 0; i < playerHealth.maxLives; i++)
        {
            GameObject icon = Instantiate(lifeIconPrefab, livesContainer);
            lifeIcons.Add(icon);
        }

        UpdateLives(playerHealth.currentLives);
    }

    void UpdateLives(int currentLives)
    {
        for (int i = 0; i < lifeIcons.Count; i++)
        {
            lifeIcons[i].SetActive(i < currentLives);
        }
    }
}
