using UnityEngine;
using Mediapipe;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using System.Collections.Generic;

public class HandTouchDetector : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject imageTarget;
    [SerializeField] public HandLandmarkDetectionConfig config;

    private Vector2? latestFingerScreenPos;
    private HandLandmarker handLandmarker;

    void Start()
    {
        config = new HandLandmarkDetectionConfig();
        var options = config.GetHandLandmarkerOptions(OnHandResult);
        handLandmarker = HandLandmarker.CreateFromOptions(options);
    }

    void Update()
    {
        if (latestFingerScreenPos == null) return;

        Ray ray = mainCamera.ScreenPointToRay(latestFingerScreenPos.Value);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null && hit.collider.name == "VirtualButtonZone")
            {
                Debug.Log("👉 Ngón tay chạm vùng ImageTarget!");
                SimulateClick();
            }
        }
    }

    // ✅ Hàm callback đúng định dạng với ResultCallback delegate
    private void OnHandResult(HandLandmarkerResult result, Image image, long timestamp)
    {
        if (result.handLandmarks == null || result.handLandmarks.Count == 0)
        {
            latestFingerScreenPos = null;
            return;
        }

        // Lấy ngón trỏ (index 8) từ danh sách landmark của bàn tay đầu tiên
        var finger = result.handLandmarks[0].landmarks[8];
        latestFingerScreenPos = new Vector2(finger.x * Screen.width, (1 - finger.y) * Screen.height);
    }

    private void SimulateClick()
    {
        Debug.Log("🖱 Click mô phỏng tại vị trí tay!");
        // Thực hiện hành động tại đây nếu cần
    }
}
