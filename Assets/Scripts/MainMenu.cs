using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Buttons")]
    public Button startButton;
    public Button optionsButton;
    public Button quitButton;

    [Header("Hover Settings")]
    public float scaleFactor = 1.1f;   // Hệ số phóng to khi hover
    public float lerpSpeed = 10f;      // Tốc độ mượt

    private Transform currentHoveredButton;
    private Vector3 originalScale;
    private Vector3 targetScale;

    void Start()
    {
        // Gắn sự kiện click cho các button
        startButton.onClick.AddListener(PlayGame);
        optionsButton.onClick.AddListener(OpenOptions);
        quitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        // Nếu có button đang được hover, scale nó mượt về targetScale
        if (currentHoveredButton != null)
        {
            currentHoveredButton.localScale = Vector3.Lerp(
                currentHoveredButton.localScale,
                targetScale,
                Time.deltaTime * lerpSpeed
            );
        }
    }

    public void PlayGame()
    {
        // Load scene game (đặt đúng tên Scene của bạn trong Build Settings)
        SceneManager.LoadScene("Scene");
    }

    public void OpenOptions()
    {
        // Sau này bạn có thể bật panel Options ở đây
        Debug.Log("Options Clicked");
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // --- Hiệu ứng Hover ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject hoveredObject = eventData.pointerEnter;
        if (hoveredObject != null && hoveredObject.GetComponent<Button>())
        {
            currentHoveredButton = hoveredObject.transform;
            originalScale = currentHoveredButton.localScale;
            targetScale = originalScale * scaleFactor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentHoveredButton != null)
        {
            targetScale = originalScale;
        }
    }
}
