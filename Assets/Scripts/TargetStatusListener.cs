using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TargetStatusListener : MonoBehaviour
{
    public GameObject gameManager; // kéo reference vào Inspector

    private ObserverBehaviour observer;

    void Awake() => observer = GetComponent<ObserverBehaviour>();

    void OnEnable() => observer.OnTargetStatusChanged += OnTargetStatusChanged;
    void OnDisable() => observer.OnTargetStatusChanged -= OnTargetStatusChanged;

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        bool tracked = (status.Status == Status.TRACKED ||
                        status.Status == Status.EXTENDED_TRACKED);

        gameManager.SetActive( tracked);
        if (tracked)
            Debug.Log("Image target found → GameManager enabled");
    }
}
