using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Data")]
    [SerializeField] private NPCDialogueData npcDialogueData;

    [Header("Settings")]
    [SerializeField] private bool playOnlyOnce = true;

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasPlayed && playOnlyOnce)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (npcDialogueData == null)
            return;

        if (MessageUIManager.Instance == null)
            return;

        hasPlayed = true;

        MessageUIManager.Instance.StartNPCDialogue(npcDialogueData);
    }
}
