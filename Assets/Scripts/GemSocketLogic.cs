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

        if (requiredGem == gem) // correct gem inserted
        {
            isFilled = true;
            redVisual.enabled = false;
            greenVisual.enabled = true;
            GetComponent<Collider>().enabled = false;

            // Disable trigger on placed gem so it can't be picked up
            GemDropLogic gemChild = GetComponentInChildren<GemDropLogic>();
            if (gemChild != null)
            {
                foreach (Collider col in gemChild.GetComponents<Collider>())
                {
                    if (col.isTrigger) col.enabled = false;
                }
            }

            parentDoor.OnSocketFilled();
            
            // Return to overhead view
            InventoryLogic inventoryLogic = FindAnyObjectByType<InventoryLogic>();
            if (inventoryLogic != null)
            {
                inventoryLogic.CloseDoorInteraction();
            }
            Camera.main.GetComponent<CameraLogic>().ReturnToDefault();
        }
        else
        {
            Debug.Log("Incorrect gem inserted.");
            // Find the gem child specifically
            GemDropLogic gemChild = GetComponentInChildren<GemDropLogic>();
            if (gemChild != null)
            {
                GameObject gemObject = gemChild.gameObject;
                gemObject.transform.SetParent(null);
                Rigidbody rb = gemObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    Vector3 spitDirection = (-transform.forward + Vector3.up).normalized;
                    rb.AddForce(spitDirection * 5f, ForceMode.Impulse);
                    rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
                }

                PlayerInventory playerInventory = FindAnyObjectByType<PlayerInventory>();
                if (playerInventory != null)
                {
                    playerInventory.RemoveItem(gem); // Ensure the incorrect gem is removed from inventory
                }

                InventoryLogic inventoryLogic = FindAnyObjectByType<InventoryLogic>();
                if (inventoryLogic != null)
                {
                    inventoryLogic.CloseDoorInteraction();
                }
                Camera.main.GetComponent<CameraLogic>().ReturnToDefault();
            }
        }
    }

    public bool OnGemPlaced(GemSO gem)
    {
        TryInsertGem(gem);
        if (isFilled)
        {
            PlayerInventory playerInventory = FindAnyObjectByType<PlayerInventory>();
            if (playerInventory != null)
            {
                playerInventory.RemoveItem(gem);
            }
            return true;
        }
        return false;
}
}