using UnityEngine;

public class ObjectInteraction : Interactable
{
    [SerializeField] private ObjectInfoSO objectInfo;

    public override void Interact()
    {
        if (MessageUIManager.Instance != null)
        {
            MessageUIManager.Instance.StartObjectDescription
            (
                objectInfo.objectName,
                objectInfo.descriptions
            );
        }
    }
}