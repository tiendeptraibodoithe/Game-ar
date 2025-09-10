using UnityEngine;
using UnityEngine.UI;

public class ShootingUIController : MonoBehaviour
{
    public Button shootButton;
    public BirdShooting birdShooting; // Kéo Bird object vào đây

    public float buttonCooldown = 0.5f; // Thời gian chờ giữa các lần bắn
    private float lastShootTime;

    void Start()
    {
        // Tìm Bird nếu chưa được gán
        if (birdShooting == null)
        {
            birdShooting = FindObjectOfType<BirdShooting>();
        }

        // Kiểm tra button
        if (shootButton == null)
        {
            Debug.LogError("Shoot Button chưa được gán!");
            return;
        }

        // Gán sự kiện click cho button
        shootButton.onClick.AddListener(OnShootButtonClick);
    }

    public void OnShootButtonClick()
    {
        // Kiểm tra cooldown
        if (Time.time - lastShootTime < buttonCooldown)
        {
            return;
        }

        // Kiểm tra Bird
        if (birdShooting == null)
        {
            Debug.LogWarning("Bird Shooting script không tìm thấy!");
            return;
        }

        // Bắn đạn
        birdShooting.Shoot();
        lastShootTime = Time.time;

        // Hiệu ứng button (optional)
        StartCoroutine(ButtonPressEffect());
    }

    private System.Collections.IEnumerator ButtonPressEffect()
    {
        // Scale down effect
        shootButton.transform.localScale = Vector3.one * 0.9f;
        yield return new WaitForSeconds(0.1f);

        // Scale back up
        shootButton.transform.localScale = Vector3.one;
    }

    void OnDestroy()
    {
        // Cleanup event listener
        if (shootButton != null)
        {
            shootButton.onClick.RemoveListener(OnShootButtonClick);
        }
    }
}
