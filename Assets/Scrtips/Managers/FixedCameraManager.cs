using UnityEngine;
using Cinemachine;

public class FixedCameraManager : MonoBehaviour
{
    public static FixedCameraManager Instance;

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

    public void SetFixedCamera(CinemachineVirtualCamera newCamera)
    {
        if (newCamera == null)
            return;

        if (currentFixedCamera == newCamera)
            return;

        // Baja la anterior
        if (currentFixedCamera != null)
            currentFixedCamera.Priority = 0;

        // Sube la nueva
        currentFixedCamera = newCamera;
        currentFixedCamera.Priority = 10;

        // Avisamos al tank controller qué cámara usar
        tankController.SetCameraTransform(currentFixedCamera.transform);
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
