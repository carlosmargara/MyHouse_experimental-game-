using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MessageUIManager : MonoBehaviour
{
    public static MessageUIManager Instance;

    [Header("UI")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI npcName;
    [SerializeField] private TextMeshProUGUI text;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.03f;

    private Queue<string> objectMessageQueue = new Queue<string>();
    private Queue<DialogueLine> npcDialogueQueue = new Queue<DialogueLine>();

    private Coroutine typingCoroutine;

    private bool isTextAnimating = false;
    private string currentText = "";

    private PlayerInput playerInput;
    private InputAction submitAction;
    private InputAction cancelAction;

    private NPCDialogueData currentNPCDialogue;

    private enum MessageMode
    {
        None,
        ObjectDescription,
        NPCDialogue
    }

    private MessageMode currentMode = MessageMode.None;

    public bool IsDialogueActive => descriptionPanel.activeSelf;

    private void Awake()
    {
        Instance = this;

        playerInput = FindObjectOfType<PlayerInput>();

        submitAction = playerInput.actions["Submit"];
        cancelAction = playerInput.actions["Cancel"];
    }

    private void Start()
    {
        descriptionPanel.SetActive(false);
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
        if (!IsDialogueActive)
            return;

        ShowNextMessage();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (!IsDialogueActive)
            return;

        EndDialogue();
    }

    // =====================================================
    // INICIAR DESCRIPCION DE OBJETO
    // =====================================================

    public void StartObjectDescription(string objectName, string[] descriptions)
    {
        currentMode = MessageMode.ObjectDescription;

        descriptionPanel.SetActive(true);

        GameStateManager.Instance.LockPlayer();

        npcName.text = objectName;

        objectMessageQueue.Clear();
        npcDialogueQueue.Clear();

        portraitImage.sprite = null;
        portraitImage.gameObject.SetActive(false);

        foreach (string description in descriptions)
        {
            objectMessageQueue.Enqueue(description);
        }

        ShowNextMessage();
    }

    // =====================================================
    // INICIAR DIALOGO NPC
    // =====================================================

    public void StartNPCDialogue(NPCDialogueData npcDialogue)
    {
        currentMode = MessageMode.NPCDialogue;
        currentNPCDialogue = npcDialogue;

        descriptionPanel.SetActive(true);

        GameStateManager.Instance.LockPlayer();

        objectMessageQueue.Clear();
        npcDialogueQueue.Clear();

        portraitImage.gameObject.SetActive(true);
        portraitImage.sprite = npcDialogue.npcPortrait;

        foreach (DialogueLine line in npcDialogue.dialogueLines)
        {
            npcDialogueQueue.Enqueue(line);
        }

        ShowNextMessage();
    }

    // =====================================================
    // MOSTRAR SIGUIENTE MENSAJE
    // =====================================================

    private void ShowNextMessage()
    {
        if (isTextAnimating)
        {
            CompleteTextInstantly();
            return;
        }

        switch (currentMode)
        {
            case MessageMode.ObjectDescription:
                ShowNextObjectDescription();
                break;

            case MessageMode.NPCDialogue:
                ShowNextNPCDialogueLine();
                break;
        }
    }

    private void ShowNextObjectDescription()
    {
        if (objectMessageQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentText = objectMessageQueue.Dequeue();

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(currentText));
    }

    private void ShowNextNPCDialogueLine()
    {
        if (npcDialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = npcDialogueQueue.Dequeue();

        if (currentLine.speaker == Speaker.NPC)
        {
            npcName.text = currentNPCDialogue.npcName;
        }
        else
        {
            npcName.text = "Player";
        }

        currentText = currentLine.text;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(currentText));
    }

    // =====================================================
    // EFECTO MAQUINA DE ESCRIBIR
    // =====================================================

    private IEnumerator TypeText(string message)
    {
        isTextAnimating = true;

        text.text = "";

        foreach (char letter in message)
        {
            text.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTextAnimating = false;
    }

    // =====================================================
    // COMPLETAR TEXTO INSTANTE
    // =====================================================

    private void CompleteTextInstantly()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        text.text = currentText;

        isTextAnimating = false;
    }

    // =====================================================
    // TERMINAR DIALOGO
    // =====================================================

    private void EndDialogue()
    {
        descriptionPanel.SetActive(false);

        GameStateManager.Instance.UnlockPlayer();

        text.text = "";
        npcName.text = "";

        portraitImage.sprite = null;
        portraitImage.gameObject.SetActive(false);

        currentMode = MessageMode.None;
        currentNPCDialogue = null;

        objectMessageQueue.Clear();
        npcDialogueQueue.Clear();
    }
}