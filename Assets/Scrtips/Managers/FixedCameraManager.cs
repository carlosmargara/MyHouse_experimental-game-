using UnityEngine;
using Cinemachine;

public class FixedCameraManager : MonoBehaviour
{
    public static FixedCameraManager Instance;

    private CameraTrigger currentTrigger;
    [Header("Current Fixed Camera")]
    public CinemachineVirtualCamera currentFixedCamera;

    [Header("References")]
    public TankController tankController;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetCurrentTrigger(CameraTrigger trigger)
    {
        currentTrigger = trigger;
    }

    public void SetFixedCamera(CinemachineVirtualCamera newCamera)
    {
        if (newCamera == null)
            return;

        if (currentFixedCamera != null && currentFixedCamera != newCamera)
            currentFixedCamera.Priority = 0;

        currentFixedCamera = newCamera;
        currentFixedCamera.Priority = 10;

        if (tankController != null)
            tankController.SetCameraTransform(currentFixedCamera.transform);
    }

    public void RefreshCurrentFixedCamera()
    {
        if (currentTrigger == null)
            return;

        SetFixedCamera(currentTrigger.fixedCamera);
    }

    public void DisableFixedCamera()
    {
        if (currentFixedCamera != null)
        {
            currentFixedCamera.Priority = 0;
            currentFixedCamera = null;
        }
    }
}
