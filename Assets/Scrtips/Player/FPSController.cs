using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool hasRun = false;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float gravity = -9.8f;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Transform cameraPivot;

    [Header("Interaction")]
    [SerializeField] private CrosshairController crosshairController;

    [Header("Lighter Movement")]
    [SerializeField] private float lighterSpeedMultiplier = 0.9f;

    private float currentSpeedMultiplier = 1f;

    private CharacterController controller;
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction interactAction;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float yVelocity;
    private float xRotation = 0f;

    public Vector2 MoveInput => moveInput;
    public bool IsGrounded => controller.isGrounded;

    private Vector3 moveDirection;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        interactAction = playerInput.actions["Interact"];
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        moveAction.performed += OnMove;
        moveAction.canceled += OnMoveCanceled;

        lookAction.performed += OnLook;
        lookAction.canceled += OnLookCanceled;

        interactAction.performed += OnInteract;
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMoveCanceled;

        lookAction.performed -= OnLook;
        lookAction.canceled -= OnLookCanceled;

        interactAction.performed -= OnInteract;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        UpdateAnimator();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext ctx)
    {
        lookInput = Vector2.zero;
    }

    private void HandleMovement()
    {
        Vector3 horizontalMove = transform.right * moveInput.x + transform.forward * moveInput.y;

        if (horizontalMove.magnitude > 1f)
            horizontalMove.Normalize();

        moveDirection = horizontalMove;

        if (controller.isGrounded && yVelocity < 0)
            yVelocity = -2f;

        yVelocity += gravity * Time.deltaTime;

        Vector3 finalMove = horizontalMove * moveSpeed * currentSpeedMultiplier;
        finalMove.y = yVelocity;

        controller.Move(finalMove * Time.deltaTime);
    }

    private void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 60f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void UpdateAnimator()
    {
        if (!animator) return;

        animator.SetFloat("InputY", moveInput.y, 0.15f, Time.deltaTime);

        animator.SetFloat("InputX", Mathf.Abs(moveInput.x), 0.15f, Time.deltaTime);

        bool isMoving =
            Mathf.Abs(moveInput.y) > 0.1f ||
            Mathf.Abs(moveInput.x) > 0.1f;

        animator.SetBool("IsMoving", isMoving);
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (crosshairController != null)
            crosshairController.TryInteract();
    }

    public void SetLighterActive(bool isActive)
    {
        currentSpeedMultiplier = isActive ? lighterSpeedMultiplier : 1f;

        Debug.Log($"Lighter Active: {isActive} / Speed Multiplier: {currentSpeedMultiplier}");
    }

    public void ResetInput()
    {
        moveInput = Vector2.zero;
        lookInput = Vector2.zero;
        moveDirection = Vector3.zero;
        yVelocity = 0f;

        if (animator != null)
        {
            animator.SetFloat("InputY", 0f);
            animator.SetFloat("InputX", 0f);
            animator.SetBool("IsMoving", false);
        }
    }
}
