using UnityEngine;

public class DebugInteractable : Interactable
{
    public override void Interact()
    {
        Debug.Log("Interactuaste con: " + gameObject.name);
    }
}
