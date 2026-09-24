using UnityEngine;

public class PickupItemInteraction : Interactable
{
    [Header("Item")]
    [SerializeField] private Inventory_Item itemToPickup;

    public Inventory_Item ItemToPickup => itemToPickup;

    public override void Interact()
    {
        if (Inventory.Instance == null) return;
        if (itemToPickup == null) return;

        PickupUIManager.Instance.ShowPickupPrompt(this);
    }

    public bool Pickup()
    {
        bool added = Inventory.Instance.AddItem(itemToPickup);

        if (added)
        {
            Debug.Log("Recogiste: " + itemToPickup.itemName);
            Destroy(gameObject);
            return true;
        }

        Debug.Log("No se pudo recoger: " + itemToPickup.itemName);
        return false;
    }
}
