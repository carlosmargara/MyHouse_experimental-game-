using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class DoorTransitionInteractable : Interactable
{
    [Header("Door Data")]
    [SerializeField] private DoorData doorData;


    [Header("Bidirectional Transition")]
    [SerializeField] private Transform destinationPointA;
    [SerializeField] private Transform destinationPointB;


    [Header("Ambient Zones")]
    [SerializeField] private AmbientZone ambientAtA = AmbientZone.General;
    [SerializeField] private AmbientZone ambientAtB = AmbientZone.General;


    [Header("Audio")]
    [SerializeField] private EventReference closedDoorSound;
    [SerializeField] private EventReference openDoorSound;
    [SerializeField] private EventReference useItemSound;
    [SerializeField] private EventReference keySound;


    [Header("FMOD")]
    [SerializeField] private string doorMaterialParameter = "DoorMaterial";


    private bool isTeleporting = false;
    private bool isUnlockedByItem = false;


    public override void Interact()
    {
        if (isTeleporting)
            return;


        if (doorData == null)
        {
            Debug.LogWarning($"{name}: falta DoorData.");
            return;
        }


        if (isUnlockedByItem)
        {
            StartCoroutine(OpenDoorRoutine());
            return;
        }


        switch (doorData.accessType)
        {
            case DoorAccessType.LockedOnly:

                ShowMessage(doorData.lockedText);
                PlayClosedDoorSound();

                return;


            case DoorAccessType.KeyRequired:

                TryOpenWithKey();

                return;


            case DoorAccessType.AlwaysOpen:

                StartCoroutine(OpenDoorRoutine());

                return;
        }
    }


    private void TryOpenWithKey()
    {
        bool hasKey =
            Inventory.Instance.HasItem(doorData.requiredKeyID);


        if (!hasKey)
        {
            ShowMessage(doorData.lockedText);
            PlayClosedDoorSound();

            return;
        }


        ShowMessage(doorData.unlockedText);
        PlayKeySound();

        StartCoroutine(OpenDoorAfterMessage());
    }


    public bool CanUseItem(Inventory_Item item)
    {
        if (item == null)
            return false;


        if (string.IsNullOrEmpty(doorData.requiredUseItemID))
            return false;


        return item.ID == doorData.requiredUseItemID;
    }


    public void UseItem(Inventory_Item item)
    {
        if (!CanUseItem(item))
            return;


        isUnlockedByItem = true;

        PlayUseItemSound();

        Debug.Log(
            "¡Puerta destrabada con " +
            item.itemName +
            "!"
        );
    }


    private IEnumerator OpenDoorAfterMessage()
    {
        isTeleporting = true;


        while (MessageUIManager.Instance.IsDialogueActive)
            yield return null;


        yield return StartCoroutine(OpenDoorRoutine());


        isTeleporting = false;
    }


    private IEnumerator OpenDoorRoutine()
    {
        isTeleporting = true;


        PlayOpenDoorSound();


        // --------------------------------------------------
        // DETERMINAR DESTINO
        // --------------------------------------------------

        bool isAtA =
            GameStateManager.Instance.GetDoorState(
                doorData.ID,
                true
            );


        Transform destination;
        AmbientZone destinationAmbient;


        if (isAtA)
        {
            // Estamos en A → vamos a B

            GameStateManager.Instance.SaveDoorState(
                doorData.ID,
                false
            );


            destination = destinationPointB;
            destinationAmbient = ambientAtB;
        }
        else
        {
            // Estamos en B → vamos a A

            GameStateManager.Instance.SaveDoorState(
                doorData.ID,
                true
            );


            destination = destinationPointA;
            destinationAmbient = ambientAtA;
        }


        // --------------------------------------------------
        // TRANSICIÓN
        // --------------------------------------------------

        DoorTransitionManager.Instance.TransitionTo(
            destination,
            destinationAmbient
        );


        yield return null;


        isTeleporting = false;
    }


    private void ShowMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;


        MessageUIManager.Instance.StartObjectDescription(
            doorData.ID,
            new string[] { message }
        );
    }


    // --------------------------------------------------
    // AUDIO
    // --------------------------------------------------

    private void PlayClosedDoorSound()
    {
        PlayDoorSound(closedDoorSound);

        Debug.Log($"Sonido puerta cerrada: " + $"{doorData.doorSoundType}");
    }


    private void PlayOpenDoorSound()
    {
        PlayDoorSound(openDoorSound);

        Debug.Log($"Sonido puerta abierta: " + $"{doorData.doorSoundType}");
    }


    private void PlayDoorSound(EventReference eventReference)
    {
        if (eventReference.IsNull)
            return;


        EventInstance instance =
            RuntimeManager.CreateInstance(eventReference);


        instance.setParameterByName(
            doorMaterialParameter,
            (float)doorData.doorSoundType
        );


        instance.start();
        instance.release();
    }


    private void PlayKeySound()
    {
        RuntimeManager.PlayOneShot(keySound);

        Debug.Log(
            "Sonido llave entrando en cerradura"
        );
    }


    private void PlayUseItemSound()
    {
        RuntimeManager.PlayOneShot(useItemSound);
    }
}
