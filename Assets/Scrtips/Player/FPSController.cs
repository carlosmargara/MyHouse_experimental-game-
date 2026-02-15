using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool hasRun = false; // por si después agregás sprint
    
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
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float yVelocity;
    private float xRotation = 0f;

    private PlayerInputActions input; //vatiable de tipo PlayerInputAction basia 

    public Vector2 MoveInput => moveInput;
    public bool IsGrounded => controller.isGrounded;

    private Vector3 moveDirection;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = new PlayerInputActions(); //acá creás una instancia real del input, creas la instancia y la guardas 
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void OnEnable()
    {
        input.Enable(); //es como que lo prendes, empezas a escuchar piezas
        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); //performed, cuando se ejecuta
        input.Player.Move.canceled += _ => moveInput = Vector2.zero; //canceled, cuando se suelta termina 

        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += _ => lookInput = Vector2.zero;
        
        input.Player.Interact.performed += OnInteract;
    }

    void OnDisable()
    {
        input.Player.Interact.performed -= OnInteract;
        input.Disable();
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Guardamos dirección NORMALIZADA para animaciones
        moveDirection = move.normalized;

        if (controller.isGrounded && yVelocity < 0)
            yVelocity = -2f;

        yVelocity += gravity * Time.deltaTime;
        move.y = yVelocity;

        controller.Move(move * moveSpeed * currentSpeedMultiplier * Time.deltaTime);
    }


    void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        // Rotación vertical (Pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 60f);
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación horizontal (Yaw)
        transform.Rotate(Vector3.up * mouseX);
    }

    void UpdateAnimator()
    {
        if (!animator) return;

        // Forward / Back
        animator.SetFloat("InputY", moveInput.y, 0.15f, Time.deltaTime);

        // Rotación (usamos el valor absoluto)
        animator.SetFloat("InputX", Mathf.Abs(moveInput.x), 0.15f, Time.deltaTime);

        bool isMoving =
            Mathf.Abs(moveInput.y) > 0.1f ||
            Mathf.Abs(moveInput.x) > 0.1f;

        animator.SetBool("IsMoving", isMoving);
    }

    void OnInteract(InputAction.CallbackContext ctx)
    {
        if (crosshairController != null)
            crosshairController.TryInteract();
    }

    public void SetLighterActive(bool isActive)
    {
        Debug.Log("LAMADA!!!!");
        currentSpeedMultiplier = isActive ? lighterSpeedMultiplier : 1f;
    }
}
