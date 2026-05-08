using System.Collections.Generic;
using UnityEngine;

public class GemSocketLogic : MonoBehaviour
{
    private GemSO requiredGem; // Set in inspector for now, Will be assigned during generation later
    private bool isFilled = false;

    public void Initialize(GemSO gem)
    {
        requiredGem = gem;
        if (requiredGem == null)
        {
            Debug.LogError("No required gem found for this socket!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.name} tag: {other.tag}");   //
        if (other.CompareTag("Player"))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
                List<ItemSO> items = playerInventory.GetInventoryItems();
                foreach (ItemSO item in items)
                {
                    if (item is GemSO gem)
                    {
                        TryInsertGem(gem);
                        if (isFilled)
                        {
                            playerInventory.RemoveItem(gem);
                            Debug.Log("Correct gem removed from inventory after insertion.");

                            // Optionally, you could also trigger some event here to indicate the socket has been filled
                        }
                        else
                        {
                            Debug.Log("Player has item: " + item.itemName + " but it's not the required gem.");
                        }

                    }
                }
            }
        }
    }

    public void TryInsertGem(GemSO gem)
    {
        if (isFilled)
        {
            Debug.Log("Socket already filled.");
            return;
        }

        if (requiredGem == gem) 
        {
            isFilled = true;
            Debug.Log("Correct gem inserted!");
            // Here you could also trigger some visual change to indicate the socket is filled
        }
        else
        {
            Debug.Log("Incorrect gem inserted.");
        }
    }
}