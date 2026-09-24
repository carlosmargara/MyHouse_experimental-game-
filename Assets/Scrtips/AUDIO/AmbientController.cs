using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Collections;

public class AmbientController : MonoBehaviour
{
    [Header("Ambient Events")]
    [SerializeField] private EventReference ambienceEvent;
    [SerializeField] private EventReference apartment10Event;


    [Header("Apartment 10 Transition")]
    [SerializeField] private float apartmentTransitionDuration = 3f;

    private EventInstance ambienceInstance;
    private EventInstance apartment10Instance;

    private Coroutine transitionCoroutine;

    private bool isOutdoor;
    private bool terraceDoorActive;

    public bool IsOutdoor => isOutdoor;
    public bool TerraceDoorActive => terraceDoorActive;

    private void Awake()
    {
        // --------------------------------------------------
        // CREAR INSTANCIAS
        // --------------------------------------------------

        ambienceInstance = RuntimeManager.CreateInstance(ambienceEvent);
        apartment10Instance = RuntimeManager.CreateInstance(apartment10Event);
    }

    public void InitializeAmbient(AmbientZone initialZone)
    {
        // --------------------------------------------------
        // AMBIENTE GENERAL
        // --------------------------------------------------

        ambienceInstance.setVolume(
            initialZone == AmbientZone.General ? 1f : 0f
        );

        ambienceInstance.start();


        // --------------------------------------------------
        // DPTO 10
        // --------------------------------------------------

        if (initialZone == AmbientZone.Apartment10)
        {
            apartment10Instance.setVolume(1f);
            apartment10Instance.start();
        }
        else
        {
            apartment10Instance.setVolume(0f);
        }
    }

    /*
    private void Start()
    {
        // --------------------------------------------------
        // AMBIENTE GENERAL
        // --------------------------------------------------

        ambienceInstance = RuntimeManager.CreateInstance(ambienceEvent);

        ambienceInstance.setVolume(1f);
        ambienceInstance.start();


        // --------------------------------------------------
        // DPTO 10
        // --------------------------------------------------

        apartment10Instance = RuntimeManager.CreateInstance(apartment10Event);

        apartment10Instance.setVolume(0f);
    }
    */

    // =========================================================
    // AMBIENTE GENERAL
    // =========================================================

    public void SetExteriorExposure(float value)
    {
        if (isOutdoor)
        {
            value = 1f;
        }

        ambienceInstance.setParameterByName("Exterior_Exposure", value);
    }


    public void SetOutdoor(bool value)
    {
        isOutdoor = value;

        if (isOutdoor)
        {
            ambienceInstance.setParameterByName("Exterior_Exposure", 1f);

            ambienceInstance.setParameterByName("Rooftop_On_Off", 1f);
        }
        else
        {
            ambienceInstance.setParameterByName("Exterior_Exposure", 0f);

            ambienceInstance.setParameterByName("Rooftop_On_Off", 0f);
        }
    }


    public void SetRooftop(bool value)
    {
        ambienceInstance.setParameterByName("Rooftop_On_Off", value ? 1f : 0f);
    }


    public void SetTerraceDoorActive(bool value)
    {
        terraceDoorActive = value;
    }


    // =========================================================
    // DPTO 10
    // =========================================================

    public void TransitionToApartment10()
    {
        StartAmbientTransition(true);
    }


    public void TransitionToGeneral()
    {
        StartAmbientTransition(false);
    }


    private void StartAmbientTransition(bool goingToApartment)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(AmbientTransition(goingToApartment));
    }


    private IEnumerator AmbientTransition(bool goingToApartment)
    {
        // --------------------------------------------------
        // ENTRANDO AL DPTO 10
        // --------------------------------------------------

        if (goingToApartment)
        {
            // El evento arranca en silencio.
            apartment10Instance.setVolume(0f);
            apartment10Instance.start();
        }


        // Tomamos los volúmenes actuales para que la transición
        // también sea segura si alguna vez se interrumpe.
        float startGeneralVolume;
        float startApartmentVolume;

        ambienceInstance.getVolume(out startGeneralVolume);
        apartment10Instance.getVolume(out startApartmentVolume);


        float endGeneralVolume =
            goingToApartment ? 0f : 1f;

        float endApartmentVolume =
            goingToApartment ? 1f : 0f;


        float time = 0f;


        // --------------------------------------------------
        // CROSSFADE
        // --------------------------------------------------

        while (time < apartmentTransitionDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / apartmentTransitionDuration);

            ambienceInstance.setVolume(Mathf.Lerp(startGeneralVolume, endGeneralVolume, t));

            apartment10Instance.setVolume(Mathf.Lerp(startApartmentVolume, endApartmentVolume, t));

            yield return null;
        }

        // --------------------------------------------------
        // VALORES FINALES
        // --------------------------------------------------

        ambienceInstance.setVolume(endGeneralVolume);

        apartment10Instance.setVolume(endApartmentVolume);

        // -------------------------------------------------
        // SALIENDO DEL DPTO 10
        // --------------------------------------------------

        if (!goingToApartment)
        {
            apartment10Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }


        transitionCoroutine = null;
    }

    private void OnDestroy()
    {
        ambienceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        ambienceInstance.release();


        apartment10Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        apartment10Instance.release();
    }
}
