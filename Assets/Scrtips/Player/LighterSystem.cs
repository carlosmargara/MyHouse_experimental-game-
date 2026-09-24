using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using FMOD.Studio;
using FMODUnity;

public class LighterSystem : MonoBehaviour
{
    public static LighterSystem Instance { get; private set; }

    [Header("Referencia al objeto encendedor")]
    [SerializeField] private GameObject cigaretteLighter;

    [Header("Referencia a la luz del encendedor")]
    [SerializeField] private Light lighterLight;

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
    [SerializeField] private Color baseColor = new Color(1f, 0.85f, 0.55f);
    [SerializeField] private float colorFlickerAmount = 0.08f;

    [Header("Controllers")]
    [SerializeField] private FPSController fpsController;
    [SerializeField] private TankController tankController;

    [Header("FMOD")]
    [SerializeField] private EventReference zippoEvent;

    private EventInstance zippoInstance;

    public float CurrentIntensity { get; private set; }

    public bool IsEquipped { get; private set; }
    public bool IsOn { get; private set; }

    private PlayerInputActions inputActions;
    private float noiseSeed;

    private void Awake()
    {
        Instance = this;

        inputActions = new PlayerInputActions();
        noiseSeed = Random.Range(0f, 100f);
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
        TurnOffVisuals();

        zippoInstance = RuntimeManager.CreateInstance(zippoEvent);
    }

    private void Update()
    {
        UpdateAnimatorLayer();

        if (IsOn && lighterLight != null)
        {
            UpdateLightFlicker();
            UpdateColorFlicker();
        }
    }

    public void SetEquipped(bool value)
    {
        IsEquipped = value;

        if (!IsEquipped)
            TurnOff();
    }

    private void OnFlashlight(InputAction.CallbackContext context)
    {
        ToggleLighter();
    }

    public void ToggleLighter()
    {
        if (!IsEquipped)
        {
            Debug.Log("No tenés el encendedor equipado.");
            return;
        }

        if (IsOn)
            TurnOff();
        else
            TurnOn();
    }

    public void ToggleFromInventory()
    {
        SetEquipped(true);

        if (IsOn)
            TurnOff();
        else
            TurnOn();
    }

    private void TurnOn()
    {
        IsOn = true;

        if (lighterLight != null)
        {
            lighterLight.enabled = true;
            lighterLight.intensity = baseIntensity;
        }

        if (cigaretteLighter != null)
            cigaretteLighter.SetActive(true);

        fpsController?.SetLighterActive(true);
        tankController?.SetLighterActive(true);

        zippoInstance.setParameterByName("Zippo_Closed", 0f);
        zippoInstance.start();

        StartCoroutine(LighterTransition(true));
    }

    private void TurnOff()
    {
        IsOn = false;

        TurnOffVisuals();

        fpsController?.SetLighterActive(false);
        tankController?.SetLighterActive(false);

        zippoInstance.setParameterByName("Zippo_Closed", 1f);

        StartCoroutine(LighterTransition(false));
    }

    private void TurnOffVisuals()
    {
        if (lighterLight != null)
            lighterLight.enabled = false;

        if (cigaretteLighter != null)
            cigaretteLighter.SetActive(false);
    }

    private void UpdateAnimatorLayer()
    {
        if (animator == null)
            return;

        currentLayerWeight = Mathf.Lerp(
            currentLayerWeight,
            targetLayerWeight,
            Time.deltaTime * layerSmoothSpeed
        );

        animator.SetLayerWeight(1, currentLayerWeight);
    }

    private void UpdateLightFlicker()
    {
        float noise = Mathf.PerlinNoise(noiseSeed, Time.time * flickerSpeed);
        float flicker = (noise - 0.5f) * flickerAmount * 2f;

        CurrentIntensity = baseIntensity + flicker;
        lighterLight.intensity = CurrentIntensity;
    }

    private void UpdateColorFlicker()
    {
        float colorNoise = Mathf.PerlinNoise(noiseSeed + 50f, Time.time * flickerSpeed * 0.8f);
        float colorShift = (colorNoise - 0.5f) * colorFlickerAmount;

        lighterLight.color = new Color(
            baseColor.r,
            baseColor.g + colorShift,
            baseColor.b - colorShift * 0.5f
        );
    }

    private IEnumerator LighterTransition(bool turnOn)
    {
        yield return new WaitForSeconds(0.1f);
        targetLayerWeight = turnOn ? 1f : 0f;
    }
}


