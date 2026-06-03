using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int maxInventorySize = 10;
    public event Action<float> OnInventoryChanged; // passes the scale factor for the inventory visual
    private List<ItemSO> inventoryItems = new List<ItemSO>();

    public bool AddItem(ItemSO item)
    {
        if (inventoryItems.Count >= maxInventorySize)
        {
            Debug.Log("Inventory is full! Cannot add item: " + item.itemName);
            return false;
        }
        if (item ==  null)
        {
            Debug.Log("Attempted to add null item to inventory.");
            return false;
        }
        inventoryItems.Add(item);
        Debug.Log("Added item to inventory: " + item.itemName);
        OnInventoryChanged?.Invoke(1.1f); // Notify subscribers of the inventory change
        return true;
    }

    public void RemoveItem(ItemSO item)
    {
        if (inventoryItems.Contains(item))
        {
            inventoryItems.Remove(item);
            Debug.Log("Removed item from inventory: " + item.itemName);
            OnInventoryChanged?.Invoke(0.9f); // Notify subscribers of the inventory change
        }
        else
        {
            Debug.Log("Item not found in inventory: " + item.itemName);
        }
    }

    public List<ItemSO> GetInventoryItems()
    {
        return new List<ItemSO>(inventoryItems); // Return a copy to prevent external modification
    }

    public int GetInventoryCount()
    {
        return inventoryItems.Count;
    }

}
