using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject inventoryRoot;
    [SerializeField] private InventoryCarousel inventoryCarousel;
    [SerializeField] private CrosshairController crosshairController;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera gameplayCamera;
    [SerializeField] private CinemachineVirtualCamera inventoryCamera;

    [Header("Camera Priorities")]
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 0;

    private InputAction openInventoryPlayerAction;
    private InputAction openInventoryUIAction;
    private InputAction cancelAction;

    private bool isOpen;
    private bool isClosing;

    private void Awake()
    {
        Instance = this;

        if (playerInput == null)
            playerInput = FindObjectOfType<PlayerInput>();

        openInventoryPlayerAction = playerInput.actions.FindAction("Player/OpenInventory");
        openInventoryUIAction = playerInput.actions.FindAction("UI/OpenInventory");
        cancelAction = playerInput.actions.FindAction("UI/Cancel");
    }

    private void Start()
    {
        inventoryRoot.SetActive(false);
        inventoryPanel.SetActive(false);

        gameplayCamera.Priority = activePriority;
        inventoryCamera.Priority = inactivePriority;
        inventoryCamera.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (openInventoryPlayerAction != null)
            openInventoryPlayerAction.performed += OnOpenInventory;

        if (openInventoryUIAction != null)
            openInventoryUIAction.performed += OnOpenInventory;

        if (cancelAction != null)
            cancelAction.performed += OnCancel;
    }

    private void OnDisable()
    {
        if (openInventoryPlayerAction != null)
            openInventoryPlayerAction.performed -= OnOpenInventory;

        if (openInventoryUIAction != null)
            openInventoryUIAction.performed -= OnOpenInventory;

        if (cancelAction != null)
            cancelAction.performed -= OnCancel;
    }

    private void OnOpenInventory(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (isClosing) return;

        if (isOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (isOpen)
            CloseInventory();
    }

    private void OpenInventory()
    {
        isOpen = true;

        crosshairController.SaveInventoryTarget(); //llamamos esta funcion antes de abrir el inventario porque despues cambia de camara 

        inventoryRoot.SetActive(true);
        inventoryPanel.SetActive(true);

        gameplayCamera.Priority = inactivePriority;

        if (FixedCameraManager.Instance.currentFixedCamera != null)
            FixedCameraManager.Instance.currentFixedCamera.Priority = inactivePriority;

        inventoryCamera.gameObject.SetActive(true);
        inventoryCamera.Priority = activePriority;

        GameStateManager.Instance.LockPlayer();
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void CloseInventory(System.Action onComplete = null)
    {
        if (!isOpen) return;
        if (isClosing) return;

        isClosing = true;
        isOpen = false;

        inventoryCarousel.CloseCarouselAnimation(() =>
        {
            inventoryRoot.SetActive(false);
            inventoryPanel.SetActive(false);

            inventoryCamera.Priority = inactivePriority;
            inventoryCamera.gameObject.SetActive(false);

            if (CameraModeManager.Instance.currentMode == CameraMode.FPS)
            {
                gameplayCamera.Priority = activePriority;
            }
            else
            {
                gameplayCamera.Priority = inactivePriority;

                if (FixedCameraManager.Instance.currentFixedCamera != null)
                    FixedCameraManager.Instance.currentFixedCamera.Priority = 10;
            }

            GameStateManager.Instance.UnlockPlayer();
            playerInput.SwitchCurrentActionMap("Player");

            isClosing = false;

            onComplete?.Invoke();
        });
    }
}