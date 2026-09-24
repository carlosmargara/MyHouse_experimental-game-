using UnityEngine;
using UnityEngine.InputSystem;

public enum CameraMode
{
    FPS,
    FixedRE
}

public class CameraModeManager : MonoBehaviour
{
    public static CameraModeManager Instance;

    public CameraMode currentMode = CameraMode.FPS;

    [Header("Controllers")]
    public FPSController fpsController;
    public TankController tankController;

    [Header("Cinemachine")]
    public GameObject fpsVirtualCamera; // CM_Player

    [Header("UI")]
    public CrosshairController crosshairController;

    private PlayerInputActions input;

    void Awake()
    {
        Instance = this;
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Enable();
        input.Player.SwitchCameraMode.performed += OnSwitchCameraMode;
    }

    void OnDisable()
    {
        input.Player.SwitchCameraMode.performed -= OnSwitchCameraMode;
        input.Disable();
    }

    void Start()
    {
        ApplyMode();
    }

    void OnSwitchCameraMode(InputAction.CallbackContext ctx)
    {
        currentMode = currentMode == CameraMode.FPS
            ? CameraMode.FixedRE
            : CameraMode.FPS;

        ApplyMode();
    }

    void ApplyMode()
    {
        bool isFPS = currentMode == CameraMode.FPS;

        fpsController.enabled = isFPS;
        tankController.enabled = !isFPS;

        fpsVirtualCamera.SetActive(isFPS);

        crosshairController.ShowCrosshair(isFPS);

        if (isFPS)
        {
            FixedCameraManager.Instance.DisableFixedCamera();
        }
        else
        {
            FixedCameraManager.Instance.RefreshCurrentFixedCamera();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Camera Mode: " + currentMode);
    }

}


