using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryLogic : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction inventoryAction;
    private PlayerInventory playerInventory;
    private CameraLogic cameraLogic;

    [SerializeField] private Transform inventoryAnchor;
    [SerializeField] private float arcRadius = 2f;

    private List<GameObject> spawnedItems = new List<GameObject>();
    private bool isOpen = false;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        inventoryAction = playerInput.actions["Player/Inventory"];
        playerInventory = GetComponent<PlayerInventory>();
        cameraLogic = Camera.main.GetComponent<CameraLogic>();
    }

    void Update()
    {
        if (inventoryAction.WasPressedThisFrame())
        {
            isOpen = !isOpen;
            cameraLogic.ToggleCameraPosition();

            if (isOpen)
                ShowInventoryItems();
            else
                DestroyInventoryItems();
        }
    }

    void ShowInventoryItems()
    {
        var items = playerInventory.GetInventoryItems();
        int count = items.Count;

        for (int i = 0; i < count; i++)
        {
            float angle = (360f / count) * i;
            Vector3 offset = Quaternion.Euler(0, angle, 0) * transform.forward * arcRadius;
            Vector3 spawnPosition = inventoryAnchor.position + offset;

            GameObject spawnedItem = Instantiate(items[i].itemPrefab, spawnPosition, Quaternion.identity);

            spawnedItems.Add(spawnedItem);
        }
    }

    void DestroyInventoryItems()
    {
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
        spawnedItems.Clear();
    }
}