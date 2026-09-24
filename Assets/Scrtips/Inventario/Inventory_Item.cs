using UnityEngine;

public enum TypeItem
{
    Use,
    Equip
}

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Inventory/Item")]
public class Inventory_Item : ScriptableObject
{
    [Header("Basic Info")]
    public string ID;
    public string itemName;

    [TextArea]
    public string description;

    [Header("3D Inventory")]
    public GameObject prefabModel;

    [Header("Pickup Text")]
    [TextArea] public string pickupText;
    [TextArea] public string pickupText02;
    [TextArea] public string confirmationText;

    [Header("Item Type")]
    public TypeItem typeItem;

    public virtual bool UseItem()
    {
        return true;
    }

    public virtual bool EquipItem()
    {
        return true;
    }
}
