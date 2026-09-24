using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InspectionManager : MonoBehaviour
{
    public static InspectionManager Instance;

    [Header("UI")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private Image inspectionImage;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI titleTMP;
    [SerializeField] private TextMeshProUGUI textTMP;
    [SerializeField] private GameObject yesNoPanel;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.03f;
    [SerializeField] private float fadeDuration = 0.5f;

    private Queue<string> messageQueue = new Queue<string>();

    private Coroutine typingCoroutine;

    private bool isInspectionActive = false;
    private bool isTextAnimating = false;
    private bool isTransitioning = false;

    private string currentText = "";

    private PlayerInput playerInput;
    private InputAction submitAction;
    private InputAction cancelAction;

    private void Awake()
    {
        Instance = this;

        playerInput = FindObjectOfType<PlayerInput>();

        submitAction = playerInput.actions["Submit"];
        cancelAction = playerInput.actions["Cancel"];
    }

    private void Start()
    {
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.gameObject.SetActive(false);

        inspectionImage.gameObject.SetActive(false);
        descriptionPanel.SetActive(false);
        yesNoPanel.SetActive(false);
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

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (!isInspectionActive || isTransitioning)
            return;

        ShowNextMessage();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (!isInspectionActive || isTransitioning)
            return;

        EndInspection();
    }

    public void StartInspection(string inspectionTitle, Sprite imageToShow, string[] messages)
    {
        isInspectionActive = true;

        titleTMP.text = inspectionTitle;
        inspectionImage.sprite = imageToShow;

        messageQueue.Clear();

        foreach (string message in messages)
        {
            messageQueue.Enqueue(message);
        }

        StartCoroutine(BeginInspectionRoutine());
    }

    private void ShowNextMessage()
    {
        if (isTextAnimating)
        {
            CompleteTextInstantly();
            return;
        }

        if (messageQueue.Count == 0)
        {
            EndInspection();
            return;
        }

        currentText = messageQueue.Dequeue();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(currentText));
    }

    private void CompleteTextInstantly()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        textTMP.text = currentText;
        isTextAnimating = false;
    }

    private void EndInspection()
    {
        if (!isInspectionActive)
            return;

        StartCoroutine(EndInspectionRoutine());
    }

    #region Corrutinas
    //Corrutina Fade
    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / fadeDuration;

            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            yield return null;
        }

        fadeCanvasGroup.alpha = endAlpha;
    }

    //Corrunita Fx_Maquina de escribir 
    private IEnumerator TypeText(string message)
    {
        isTextAnimating = true;

        textTMP.text = "";

        foreach (char letter in message)
        {
            textTMP.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTextAnimating = false;
    }

    private IEnumerator BeginInspectionRoutine()
    {
        isTransitioning = true;

        GameStateManager.Instance.LockPlayer();

        fadeCanvasGroup.gameObject.SetActive(true);
        yesNoPanel.SetActive(false);

        yield return StartCoroutine(Fade(0f, 1f));

        inspectionImage.gameObject.SetActive(true);
        descriptionPanel.SetActive(true);

        isTransitioning = false;

        ShowNextMessage();
    }

    private IEnumerator EndInspectionRoutine()
    {
        isTransitioning = true;

        inspectionImage.gameObject.SetActive(false);
        descriptionPanel.SetActive(false);
        yesNoPanel.SetActive(false);

        yield return StartCoroutine(Fade(1f, 0f));

        fadeCanvasGroup.gameObject.SetActive(false);

        textTMP.text = "";
        titleTMP.text = "";

        inspectionImage.sprite = null;

        messageQueue.Clear();

        isInspectionActive = false;
        isTransitioning = false;

        GameStateManager.Instance.UnlockPlayer();
    }
    #endregion
}
