using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    [Header("Player Controllers")]
    [SerializeField] private FPSController fpsController;
    [SerializeField] private TankController tankController;

    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;

    [Header("UI")]
    [SerializeField] private CrosshairUI crosshairUI;

    [Header("Doors")]
    private Dictionary<string, bool> doorStates = new Dictionary<string, bool>();

    private bool isPlayerLocked = false;

    private bool fpsWasActive;
    private bool tankWasActive;

    public bool IsPlayerLocked => isPlayerLocked;

    private void Awake()
    {
        Instance = this;
    }

    public void LockPlayer()
    {
        if (isPlayerLocked)
            return;

        isPlayerLocked = true;

        fpsWasActive = fpsController != null && fpsController.enabled;
        tankWasActive = tankController != null && tankController.enabled;

        HideCrosshair();

        if (fpsController != null)
        {
            fpsController.ResetInput();
            fpsController.enabled = false;
        }

        if (tankController != null)
            tankController.enabled = false;

        if (playerInput != null)
            playerInput.SwitchCurrentActionMap("UI");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void UnlockPlayer()
    {
        if (!isPlayerLocked)
            return;

        isPlayerLocked = false;

        ShowCrosshair();

        if (fpsController != null)
        {
            fpsController.ResetInput();
            fpsController.enabled = fpsWasActive;
        }

        if (tankController != null)
            tankController.enabled = tankWasActive;

        if (playerInput != null)
            playerInput.SwitchCurrentActionMap("Player");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void HideCrosshair()
    {
        if (crosshairUI != null)
            crosshairUI.Hide();
    }

    public void ShowCrosshair()
    {
        if (crosshairUI != null)
            crosshairUI.Show();
    }

    public void SaveDoorState(string doorID, bool isAtA)
    {
        if (string.IsNullOrEmpty(doorID)) return;

        doorStates[doorID] = isAtA;
    }

    public bool GetDoorState(string doorID, bool defaultValue = true)
    {
        if (string.IsNullOrEmpty(doorID)) return defaultValue;

        if (doorStates.TryGetValue(doorID, out bool value))
            return value;

        return defaultValue;
    }
}