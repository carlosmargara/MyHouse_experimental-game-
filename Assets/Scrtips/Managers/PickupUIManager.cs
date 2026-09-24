using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using FMODUnity;

public class PickupUIManager : MonoBehaviour
{
    public static PickupUIManager Instance { get; private set; }

    private enum PickupUIState
    {
        Description,
        Decision,
        Confirmation
    }

    [Header("References")]
    [SerializeField] private PlayerInput playerInput;

    [Header("UI")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private GameObject yesNoPanel;

    [SerializeField] private TextMeshProUGUI nameTMP;
    [SerializeField] private TextMeshProUGUI descriptionTMP;

    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [Header("Typing Effect")]
    [SerializeField] private float typingSpeed = 0.03f;

    [Header("Audio")]
    [SerializeField] private EventReference pickupSound;

    private InputAction submitAction;
    private InputAction cancelAction;

    private PickupItemInteraction currentPickup;
    private PickupUIState currentState;

    private Coroutine typingCoroutine;
    private string currentFullText;

    private bool isOpen;
    private bool isTyping;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (playerInput == null)
            playerInput = FindObjectOfType<PlayerInput>();

        submitAction = playerInput.actions.FindAction("UI/Submit");
        cancelAction = playerInput.actions.FindAction("UI/Cancel");
    }

    private void Start()
    {
        descriptionPanel.SetActive(false);
        yesNoPanel.SetActive(false);

        yesButton.onClick.AddListener(ConfirmPickup);
        noButton.onClick.AddListener(CancelPickup);
    }

    private void OnEnable()
    {
        if (submitAction != null)
            submitAction.performed += OnSubmit;

        if (cancelAction != null)
            cancelAction.performed += OnCancel;
    }

    private void OnDisable()
    {
        if (submitAction != null)
            submitAction.performed -= OnSubmit;

        if (cancelAction != null)
            cancelAction.performed -= OnCancel;
    }

    public void ShowPickupPrompt(PickupItemInteraction pickup)
    {
        if (pickup == null) return;
        if (pickup.ItemToPickup == null) return;

        currentPickup = pickup;
        isOpen = true;
        currentState = PickupUIState.Description;

        descriptionPanel.SetActive(true);
        yesNoPanel.SetActive(false);

        nameTMP.text = pickup.ItemToPickup.itemName;

        GameStateManager.Instance.LockPlayer();
        playerInput.SwitchCurrentActionMap("UI");

        StartTyping(pickup.ItemToPickup.pickupText);
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (!isOpen) return;

        if (isTyping)
        {
            CompleteTextInstantly();
            return;
        }

        switch (currentState)
        {
            case PickupUIState.Description:
                ShowDecisionStep();
                break;

            case PickupUIState.Decision:
                break;

            case PickupUIState.Confirmation:
                ClosePickupPrompt();
                break;
        }
    }

    private void ShowDecisionStep()
    {
        if (currentPickup == null) return;
        if (currentPickup.ItemToPickup == null) return;

        currentState = PickupUIState.Decision;

        yesNoPanel.SetActive(false);

        StartTyping(currentPickup.ItemToPickup.pickupText02, () =>
        {
            yesNoPanel.SetActive(true);
            yesButton.Select();
        });
    }

    private void ConfirmPickup()
    {
        if (!isOpen) return;
        if (currentState != PickupUIState.Decision) return;
        if (isTyping) return;

        bool picked = false;

        if (currentPickup != null)
            picked = currentPickup.Pickup();

        if (picked)
        {
            RuntimeManager.PlayOneShot(pickupSound);
            ShowConfirmationStep();
        }
        else
        {
            ClosePickupPrompt();
        }
    }

    private void ShowConfirmationStep()
    {
        if (currentPickup == null) return;
        if (currentPickup.ItemToPickup == null) return;

        currentState = PickupUIState.Confirmation;

        yesNoPanel.SetActive(false);

        StartTyping(currentPickup.ItemToPickup.confirmationText);
    }

    private void CancelPickup()
    {
        if (!isOpen) return;

        ClosePickupPrompt();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (!isOpen) return;

        CancelPickup();
    }

    private void StartTyping(string text, System.Action onComplete = null)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        currentFullText = text;
        typingCoroutine = StartCoroutine(TypeText(text, onComplete));
    }

    private IEnumerator TypeText(string text, System.Action onComplete)
    {
        isTyping = true;
        descriptionTMP.text = "";

        foreach (char letter in text)
        {
            descriptionTMP.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;

        onComplete?.Invoke();
    }

    private void CompleteTextInstantly()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        descriptionTMP.text = currentFullText;

        isTyping = false;
        typingCoroutine = null;

        if (currentState == PickupUIState.Decision)
        {
            yesNoPanel.SetActive(true);
            yesButton.Select();
        }
    }

    private void ClosePickupPrompt()
    {
        isOpen = false;
        isTyping = false;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        descriptionPanel.SetActive(false);
        yesNoPanel.SetActive(false);

        currentPickup = null;

        GameStateManager.Instance.UnlockPlayer();
        playerInput.SwitchCurrentActionMap("Player");
    }
}
