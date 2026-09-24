using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [SerializeField] private int numberSlot = 7;
    public int NumberSlot => numberSlot;

    [Header("Items")]
    [SerializeField] private Inventory_Item[] items;
    public Inventory_Item[] Items => items;

    public int CurrentIndex { get; private set; }

    private void Awake()
    {
        Instance = this;

        items = new Inventory_Item[numberSlot];
        CurrentIndex = 0;
    }

    public bool AddItem(Inventory_Item itemToAdd)
    {
        if (itemToAdd == null)
            return false;

        if (HasItem(itemToAdd.ID))
            return false;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = itemToAdd;

                Debug.Log($"Item agregado: {itemToAdd.itemName}");

                return true;
            }
        }

        Debug.Log("Inventario lleno");
        return false;
    }

    public bool RemoveItem(Inventory_Item itemToRemove)
    {
        if (itemToRemove == null)
            return false;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == itemToRemove)
            {
                items[i] = null;

                Debug.Log($"Item removido: {itemToRemove.itemName}");

                return true;
            }
        }

        return false;
    }

    public bool HasItem(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return false;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].ID == itemID)
                return true;
        }

        return false;
    }

    public Inventory_Item GetCurrentItem()
    {
        if (items == null || items.Length == 0)
            return null;

        if (CurrentIndex < 0 || CurrentIndex >= items.Length)
            return null;

        return items[CurrentIndex];
    }

    public void SetCurrentIndex(int index)
    {
        if (index < 0 || index >= items.Length)
            return;

        CurrentIndex = index;
    }
}
