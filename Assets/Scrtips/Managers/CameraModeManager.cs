using UnityEngine;
using UnityEngine.InputSystem;

public enum CameraMode
{
    FPS,
    FixedRE
}

public class CameraModeManager : MonoBehaviour
{
    public CameraMode currentMode = CameraMode.FPS;

    [Header("Controllers")]
    public FPSController fpsController;
    public TankController tankController;

    [Header("Cinemachine")]
    public GameObject fpsVirtualCamera; // CM_Player

    private PlayerInputActions input;

    void Awake()
    {
        input = new PlayerInputActions(); //creas la intancia real del input 
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

        if (isFPS)
        {
            // Apagamos cualquier cámara fija activa
            FixedCameraManager.Instance.DisableFixedCamera();
        }

        Cursor.lockState = isFPS ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isFPS;

        Debug.Log("Camera Mode: " + currentMode);
    }

}


