using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Lighter")]
public class Lighter_Item : Inventory_Item
{
    public override bool EquipItem()
    {
        if (LighterSystem.Instance == null)
            return false;

        LighterSystem.Instance.ToggleFromInventory();

        return true;
    }
}
