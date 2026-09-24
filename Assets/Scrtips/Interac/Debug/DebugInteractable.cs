using UnityEngine;

public class DebugInteractable : Interactable
{
    [SerializeField] private GameObject panelIU;
    public override void Interact()
    {
        Debug.Log("Interactuaste con: " + gameObject.name);

        panelIU.SetActive(true);
    }
}
