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
    private bool isDoorInteractionOpen = false;
    private CharacterController controller;

    [SerializeField] private GameObject handPrefab; // Prefab for the UI hand that will be used to select and drag gems in the inventory 
    private GameObject spawnedHand;
    private PlayerMovement playerMovement;

    [SerializeField] private Material handRedMaterial;
    [SerializeField] private Material handGreenMaterial;
    [SerializeField] private Transform gemHold; // A transform that will hold any gem that is currently being dragged by the hand when the inventory is closed, to prevent it from being lost

    public Transform InventoryAnchor => inventoryAnchor;
    public Material HandRedMaterial => handRedMaterial;
    public Material HandGreenMaterial => handGreenMaterial;
    public Transform GemHold => gemHold;

    public bool IsInventoryOpen => isOpen;


    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        inventoryAction = playerInput.actions["Player/Inventory"];
        playerInventory = GetComponent<PlayerInventory>();
        cameraLogic = Camera.main.GetComponent<CameraLogic>();
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (inventoryAction.WasPressedThisFrame() && !cameraLogic.IsLerping() && !isDoorInteractionOpen)
        {
            isOpen = !isOpen;
            cameraLogic.ToggleCameraPosition();

            if (isOpen)
            {
                ShowInventoryItems();
                controller.enabled = false; // disable character controller to prevent movement while inventory is open
                playerMovement.enabled = false; // disable player movement script to prevent movement while inventory is open
            }
            else
            {
                DestroyInventoryItems();
                controller.enabled = true; // re-enable character controller when inventory is closed
                playerMovement.enabled = true; // re-enable player movement script when inventory is closed

                // check if player is in door range with a gem
                DungeonDoorLogic door = FindAnyObjectByType<DungeonDoorLogic>();
                if (door != null && door.PlayerInRange && gemHold.childCount > 0)
                {
                    door.TriggerDoorInteraction(this);
                }
            }
        }
    }

    void ShowInventoryItems()
    {
        var items = playerInventory.GetInventoryItems();
        int count = items.Count;

        // check if a gem is already being carried
        GemSO carriedGemData = null;
        if (gemHold.childCount > 0)
        {
            GemDropLogic carried = gemHold.GetChild(0).GetComponent<GemDropLogic>();
            if (carried != null) carriedGemData = carried.GetGemData();
        }

        spawnedHand = Instantiate(handPrefab, inventoryAnchor.position, Quaternion.identity);
        HandLogic handLogic = spawnedHand.GetComponent<HandLogic>();
        if (handLogic != null)
        {
            handLogic.Initialise(playerInput.actions["Player/Move"], playerInput.actions["Player/Interact"], inventoryAnchor.position.y, transform, this);

            if (gemHold.childCount > 0)
            {
                GameObject carriedGem = gemHold.GetChild(0).gameObject;
                carriedGem.transform.SetParent(spawnedHand.transform);
                carriedGem.transform.localPosition = Vector3.zero;

                foreach (Collider col in carriedGem.GetComponents<Collider>())
                {
                    if (col.isTrigger) col.enabled = true;
                }

                handLogic.SetHeldGem(carriedGem);
            }
        }

        for (int i = 0; i < count; i++)
        {
            if (items[i] == carriedGemData) continue; // skip already held gem

            float angle = (360f / count) * i;
            Vector3 offset = Quaternion.Euler(0, angle, 0) * transform.forward * arcRadius;
            Vector3 spawnPosition = inventoryAnchor.position + offset;

            GameObject spawnedItem = Instantiate(items[i].itemPrefab, spawnPosition, transform.rotation);
            GemDropLogic gemLogic = spawnedItem.GetComponent<GemDropLogic>();
            if (gemLogic != null)
            {
                gemLogic.Initialise(items[i]);
            }

            spawnedItems.Add(spawnedItem);
        }
    }

    void DestroyInventoryItems()
    {
        Debug.Log($"Destroying {spawnedItems.Count} items");
        if (spawnedHand != null)
        {
            HandLogic handLogic = spawnedHand.GetComponent<HandLogic>();
            if (handLogic != null && handLogic.IsHoldingGem())
            {
                GameObject heldGem = handLogic.GetHeldGem();
                heldGem.transform.SetParent(gemHold);
                heldGem.transform.localPosition = Vector3.zero;
                spawnedItems.Remove(heldGem); // remove before cleanup loop

                foreach (Collider col in heldGem.GetComponents<Collider>())
                {
                    if (col.isTrigger) col.enabled = false;
                }
            }
            Destroy(spawnedHand);
            spawnedHand = null;
        }

        foreach (GameObject item in spawnedItems)
        {
            if (item != null) Destroy(item);
        }
        spawnedItems.Clear();
    }

    public void TrackSpawnedItem(GameObject item)
    {
        spawnedItems.Add(item);
    }

    public void OpenDoorInteraction(Transform anchor)
    {
        isDoorInteractionOpen = true;
        spawnedHand = Instantiate(handPrefab, anchor.position, anchor.rotation);
        HandLogic handLogic = spawnedHand.GetComponent<HandLogic>();
        if (handLogic != null)
        {
            handLogic.Initialise(playerInput.actions["Player/Move"], playerInput.actions["Player/Interact"], inventoryAnchor.position.y, transform, this);

            if (gemHold.childCount > 0)
            {
                GameObject carriedGem = gemHold.GetChild(0).gameObject;
                carriedGem.transform.SetParent(spawnedHand.transform);
                carriedGem.transform.localPosition = Vector3.zero;

                foreach (Collider col in carriedGem.GetComponents<Collider>())
                {
                    if (col.isTrigger) col.enabled = true;
                }

                handLogic.SetHeldGem(carriedGem);
                handLogic.SwitchToDoorPlane(anchor.transform);
            }
        }
    }

    public void CloseDoorInteraction()
    {
        isDoorInteractionOpen = false;
        controller.enabled = true;
        GetComponent<PlayerMovement>().enabled = true;

        if (spawnedHand != null)
        {
            HandLogic handLogic = spawnedHand.GetComponent<HandLogic>();
            if (handLogic != null && handLogic.IsHoldingGem())
            {
                GameObject heldGem = handLogic.GetHeldGem();
                heldGem.transform.SetParent(gemHold);
                heldGem.transform.localPosition = Vector3.zero;

                foreach (Collider col in heldGem.GetComponents<Collider>())
                {
                    if (col.isTrigger) col.enabled = false;
                }
            }
            Destroy(spawnedHand);
            spawnedHand = null;
        }
    }

    void OnDestroy()
    {
        DestroyInventoryItems(); // Ensure any remaining items are cleaned up if the player object is destroyed
    }
}