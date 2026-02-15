using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class FlashlightToggle : MonoBehaviour
{
    [Header("Referencia a la luz de la linterna")]
    [SerializeField] private Light flashlight;

    [Space]

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [SerializeField] private float layerSmoothSpeed = 6f;
    private float targetLayerWeight = 0f;
    private float currentLayerWeight = 0f;

    [Header("Flicker Settings")]
    [SerializeField] private float baseIntensity = 2.2f;
    [SerializeField] private float flickerAmount = 0.4f;
    [SerializeField] private float flickerSpeed = 8f;

    [Header("Flame Color Variation")]
    [SerializeField] private Color baseColor = new Color(1f, 0.6f, 0.3f);
    [SerializeField] private float colorFlickerAmount = 0.08f;

    [Space]

    [SerializeField] private FPSController fpsController;
    [SerializeField] private TankController tankController;

    public float CurrentIntensity { get; private set; }

    private PlayerInputActions inputActions;
    private bool isOn = false;
    private float noiseSeed;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        noiseSeed = Random.Range(0f, 100f); // Hace que cada linterna sea distinta
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Flashlight.performed += OnFlashlight;
    }

    private void OnDisable()
    {
        inputActions.Player.Flashlight.performed -= OnFlashlight;
        inputActions.Player.Disable();
    }

    private void Start()
    {
        if (flashlight != null)
            flashlight.enabled = false;
    }

    private void Update()
    {
        if (animator != null)
        {
        currentLayerWeight = Mathf.Lerp(
            currentLayerWeight,
            targetLayerWeight,
            Time.deltaTime * layerSmoothSpeed
        );

        animator.SetLayerWeight(1, currentLayerWeight);
        }
        

    
        if (isOn && flashlight != null)
        {
            float noise = Mathf.PerlinNoise(noiseSeed, Time.time * flickerSpeed);
            float flicker = (noise - 0.5f) * flickerAmount * 2f;
            CurrentIntensity = baseIntensity + flicker;
            flashlight.intensity = CurrentIntensity;
        }

        float colorNoise = Mathf.PerlinNoise(noiseSeed + 50f, Time.time * flickerSpeed * 0.8f);
        float colorShift = (colorNoise - 0.5f) * colorFlickerAmount;

        flashlight.color = new Color(
        baseColor.r,
        baseColor.g + colorShift,
        baseColor.b - colorShift * 0.5f);
    
    }

    private void OnFlashlight(InputAction.CallbackContext context)
    {
        ToggleFlashlight();
    }

    private void ToggleFlashlight()
    {
        if (flashlight == null) return;

        isOn = !isOn;

        flashlight.enabled = isOn;

        fpsController?.SetLighterActive(isOn);
        tankController?.SetLighterActive(isOn);

        StartCoroutine(LighterTransition(isOn));

        if (isOn)
            flashlight.intensity = baseIntensity;
    }


    private IEnumerator LighterTransition(bool turnOn)
    {
        yield return new WaitForSeconds(0.1f);
        targetLayerWeight = turnOn ? 1f : 0f;
    }

}


