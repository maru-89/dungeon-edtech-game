using System.Collections.Generic;
using UnityEngine;

public class GemSocketLogic : MonoBehaviour
{
    [SerializeField] private MeshRenderer redVisual; // For visual feedback, assign in inspector
    [SerializeField] private MeshRenderer greenVisual;

    private GemSO requiredGem; // Set in inspector for now, Will be assigned during generation later
    private bool isFilled = false;
    public bool IsFilled => isFilled;

    private DungeonDoorLogic parentDoor;

    public void Initialize(GemSO gem, DungeonDoorLogic door)
    {
        requiredGem = gem;
        parentDoor = door;
        if (requiredGem == null)
        {
            Debug.LogError("No required gem found for this socket!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isFilled) return; // already filled, ignore
        
        if (other.CompareTag("Player"))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
                List<ItemSO> items = playerInventory.GetInventoryItems();
                foreach (ItemSO item in items)
                {
                    if (item is GemSO gem && gem == requiredGem)
                    {
                        // Only try if this is actually the required gem
                        TryInsertGem(gem);
                        if (isFilled)
                        {
                            playerInventory.RemoveItem(gem);
                            Debug.Log("Correct gem removed from inventory.");
                        }
                        return; // stop looking regardless of outcome
                    }
                }
                Debug.Log("Player does not have the required gem.");
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
            redVisual.enabled = false;
            greenVisual.enabled = true;
            parentDoor.OnSocketFilled();
        }
        else
        {
            Debug.Log("Incorrect gem inserted.");
        }
    }
}