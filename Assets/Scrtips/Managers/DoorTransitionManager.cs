using UnityEngine;
using System.Collections;

public enum AmbientZone
{
    General,
    Apartment10
}

public class DoorTransitionManager : MonoBehaviour
{
    public static DoorTransitionManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private AmbientController ambientController;

    [Header("Timing")]
    [SerializeField] private float waitBeforeFade = 0.15f;
    [SerializeField] private float waitOnBlack = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.25f;
    [SerializeField] private float fadeInDuration = 0.25f;

    private bool isTransitioning;

    [Space]

    // Ambiente en el que creemos que está actualmente el jugador.
    [SerializeField] private AmbientZone currentAmbientZone = AmbientZone.General;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (ambientController != null)
        {
            ambientController.InitializeAmbient(currentAmbientZone);
        }
    }

    public void TransitionTo(Transform teleportTarget, AmbientZone destinationZone, AudioSource audioSource = null,
    AudioClip doorSound = null)
    {
        if (isTransitioning)
            return;

        if (teleportTarget == null)
        {
            Debug.LogWarning(
                "DoorTransitionManager: falta asignar teleportTarget."
            );

            return;
        }

        StartCoroutine(TransitionRoutine(teleportTarget, destinationZone, audioSource, doorSound));
    }


    private IEnumerator TransitionRoutine(Transform teleportTarget, AmbientZone destinationZone, AudioSource audioSource,
        AudioClip doorSound)
    {
        isTransitioning = true;

        GameStateManager.Instance.LockPlayer();


        // Sonido físico de la puerta, si se utiliza.
        if (audioSource != null && doorSound != null)
            audioSource.PlayOneShot(doorSound);


        yield return new WaitForSeconds(waitBeforeFade);


        // --------------------------------------------------
        // FADE OUT
        // --------------------------------------------------

        bool fadeOutDone = false;

        FadeManager.Instance.FadeOut(fadeOutDuration, () =>

            {
                fadeOutDone = true;
            }
        );

        yield return new WaitUntil(() => fadeOutDone);


        // Pantalla completamente negra.
        yield return new WaitForSeconds(waitOnBlack);


        // --------------------------------------------------
        // TELEPORT
        // --------------------------------------------------

        TeleportPlayer(teleportTarget);


        // --------------------------------------------------
        // AMBIENTE
        // --------------------------------------------------

        ChangeAmbientIfNecessary(destinationZone);


        // --------------------------------------------------
        // FADE IN
        // --------------------------------------------------

        bool fadeInDone = false;

        FadeManager.Instance.FadeIn(
            fadeInDuration,
            () =>
            {
                fadeInDone = true;
            }
        );

        yield return new WaitUntil(() => fadeInDone);


        GameStateManager.Instance.UnlockPlayer();

        isTransitioning = false;
    }


    private void ChangeAmbientIfNecessary(AmbientZone destinationZone)
    {
        // Si el destino pertenece al mismo ambiente en el que
        // ya estamos, NO hacemos absolutamente nada.
        if (destinationZone == currentAmbientZone)
        {
            Debug.Log(
                $"DoorTransitionManager: mismo ambiente ({destinationZone}). " +
                "No se realiza transición."
            );

            return;
        }


        Debug.Log(
            $"DoorTransitionManager: cambio de ambiente " +
            $"{currentAmbientZone} → {destinationZone}"
        );


        if (ambientController != null)
        {
            switch (destinationZone)
            {
                case AmbientZone.General:

                    ambientController.TransitionToGeneral();

                    break;


                case AmbientZone.Apartment10:

                    ambientController.TransitionToApartment10();

                    break;
            }
        }


        // Actualizamos el estado solamente después de realizar
        // el cambio.
        currentAmbientZone = destinationZone;
    }


    private void TeleportPlayer(Transform teleportTarget)
    {
        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        player.position = teleportTarget.position;
        player.rotation = teleportTarget.rotation;

        if (controller != null)
            controller.enabled = true;
    }
}
