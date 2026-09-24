using UnityEngine;

public class InspectionInteraction : Interactable
{
    [Header("Inspection")]
    [SerializeField] private InspectionData inspectionData;

    public override void Interact()
    {
        if (inspectionData == null)
        {
            Debug.LogWarning("No hay InspectionData asignado en " + gameObject.name);
            return;
        }

        InspectionManager.Instance.StartInspection(
            inspectionData.inspectionTitle,
            inspectionData.inspectionImage,
            inspectionData.messages
        );
    }
}
