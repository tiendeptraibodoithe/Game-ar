using UnityEngine;
using Mediapipe.Tasks.Core;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Tasks.Vision.Core;

[CreateAssetMenu(menuName = "MediaPipe/Hand Landmark Detection Config")]
public class HandLandmarkDetectionConfig : ScriptableObject
{
    public BaseOptions.Delegate delegateOption = BaseOptions.Delegate.CPU;
    public RunningMode runningMode = RunningMode.LIVE_STREAM;
    public int numHands = 1;
    public float minHandDetectionConfidence = 0.5f;
    public float minHandPresenceConfidence = 0.5f;
    public float minTrackingConfidence = 0.5f;
    public string modelPath = Application.streamingAssetsPath + "/hand_landmarker.bytes";

    public HandLandmarkerOptions GetHandLandmarkerOptions(HandLandmarkerOptions.ResultCallback callback = null)
    {
        return new HandLandmarkerOptions(
            new BaseOptions(delegateOption, modelAssetPath: modelPath),
            runningMode,
            numHands,
            minHandDetectionConfidence,
            minHandPresenceConfidence,
            minTrackingConfidence,
            callback
        );
    }
}
