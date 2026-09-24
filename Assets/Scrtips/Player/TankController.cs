using UnityEngine;
using UnityEngine.InputSystem;

public class TankController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float rotateSpeed = 120f;
    public float gravity = -9.8f;

    [Header("Lighter Movement")]
    [SerializeField] private float lighterSpeedMultiplier = 0.9f;

    [Header("Interaction")]
    [SerializeField] private CrosshairController crosshairController;

    private float currentSpeedMultiplier = 1f;

    private Vector2 moveInput;
    private CharacterController controller;
    private float yVelocity;

    private Transform currentCameraTransform;

    // INPUT
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction interactAction;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        interactAction = playerInput.actions["Interact"];
    }

    void OnEnable()
    {
        moveAction.performed += OnMove;
        moveAction.canceled += OnMoveCanceled;

        interactAction.performed += OnInteract;
    }

    void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMoveCanceled;

        interactAction.performed -= OnInteract;
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        Debug.Log("TANK INTERACT LLAMADO");

        if (crosshairController != null)
        {
            Debug.Log("Llamando a TryInteract()");
            crosshairController.TryInteract();
        }
        else
        {
            Debug.LogWarning("CrosshairController NO asignado");
        }
    }

    void Update()
    {
        HandleMovement();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        float rotate = moveInput.x;
        transform.Rotate(Vector3.up * rotate * rotateSpeed * Time.deltaTime);

        Vector3 move = transform.forward * moveInput.y;

        if (controller.isGrounded && yVelocity < 0)
            yVelocity = -2f;

        yVelocity += gravity * Time.deltaTime;
        move.y = yVelocity;

        controller.Move(move * moveSpeed * currentSpeedMultiplier * Time.deltaTime);
    }

    void UpdateAnimator()
    {
        if (!animator) return;

        animator.SetFloat("InputY", moveInput.y, 0.15f, Time.deltaTime);
        animator.SetFloat("InputX", Mathf.Abs(moveInput.x), 0.15f, Time.deltaTime);

        bool isMoving =
            Mathf.Abs(moveInput.y) > 0.1f ||
            Mathf.Abs(moveInput.x) > 0.1f;

        animator.SetBool("IsMoving", isMoving);
    }

    public void SetCameraTransform(Transform cam)
    {
        currentCameraTransform = cam;
    }

    public void SetLighterActive(bool isActive)
    {
        currentSpeedMultiplier = isActive ? lighterSpeedMultiplier : 1f;
    }
}




