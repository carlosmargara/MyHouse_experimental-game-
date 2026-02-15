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

    private float currentSpeedMultiplier = 1f;

    private Vector2 moveInput;
    private PlayerInputActions input;
    private CharacterController controller;
    private float yVelocity;

    private Transform currentCameraTransform;

    void Awake()
    {
        input = new PlayerInputActions();
        controller = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        input.Enable();
        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += _ => moveInput = Vector2.zero;
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        HandleMovement();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        // ROTACIÓN TANQUE
        float rotate = moveInput.x;
        transform.Rotate(Vector3.up * rotate * rotateSpeed * Time.deltaTime);

        // MOVIMIENTO ADELANTE / ATRÁS (dirección del personaje)
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




