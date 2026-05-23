using UnityEngine;
using UnityEngine.InputSystem;

public class HandLogic : MonoBehaviour
{
    // This script controls the UI hand that appears in the inventory, allowing the player to select and drag gems around. It is instantiated by InventoryLogic when the inventory is opened, and destroyed when the inventory is closed.
    private InputAction moveAction;
    private InputAction interactAction;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float planeY; // fixed Y position to keep hand on inventory plane

    private GameObject heldGem;
    private Renderer handRenderer;
    private Transform playerTransform;
    private float maxHandRadius = 2f; // maximum distance the hand can move from the center of the inventory
    private float grabRadius = 0.5f; // radius within which the hand can grab or place gems
    private InventoryLogic inventoryLogic;
    private Material redMaterial;
    private Material greenMaterial;
    private Transform inventoryAnchor;

    private bool isDoorMode = false;
    private Transform doorTransform;
    


    public void Initialise(InputAction move, InputAction interact, float yPosition, Transform player, InventoryLogic inventoryLogicBase)
    {
        inventoryLogic = inventoryLogicBase;
        playerTransform = player;
        inventoryAnchor = inventoryLogic.InventoryAnchor;
        moveAction = move;
        interactAction = interact;
        planeY = yPosition;
        handRenderer = GetComponentInChildren<Renderer>();
        redMaterial = inventoryLogic.HandRedMaterial;
        greenMaterial = inventoryLogic.HandGreenMaterial;
    }

    void Update()
    {
        MoveHand();
        if (interactAction.WasPressedThisFrame())
        {
            if (isDoorMode && heldGem == null)
            {
                inventoryLogic.CloseDoorInteraction();
            }
            else if (heldGem == null)
            {
                TryGrab();
            }
            else
            {
                TryPlace();
            }
        }
    }

    public bool IsHoldingGem() => heldGem != null;
    public GameObject GetHeldGem() => heldGem;

    void MoveHand()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        if (isDoorMode)
        {
            Vector3 movement = (doorTransform.right * -input.x + doorTransform.up * input.y) * moveSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + movement;

            // Clamp to door plane radius
            Vector3 offset = newPosition - doorTransform.position;
            Vector3 localOffset = new Vector3(
                Vector3.Dot(offset, doorTransform.right),
                Vector3.Dot(offset, doorTransform.up),
                0);
            if (localOffset.magnitude > maxHandRadius)
            {
                localOffset = localOffset.normalized * maxHandRadius;
            }
            transform.position = doorTransform.position + 
                doorTransform.right * localOffset.x + 
                doorTransform.up * localOffset.y;
        }
        else
        {
            Vector3 forward = playerTransform.forward;
            Vector3 right = playerTransform.right;
            forward.y = 0;
            right.y = 0;
            
            Vector3 movement = (forward * input.y + right * input.x) * moveSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + movement;

            Vector3 offset = newPosition - inventoryAnchor.position;
            offset.y = 0;
            if (offset.magnitude > maxHandRadius)
            {
                offset = offset.normalized * maxHandRadius;
            }
            newPosition = inventoryAnchor.position + offset;
            newPosition.y = planeY;
            transform.position = newPosition;
        }
    }

    void TryGrab()
    {
        if (heldGem != null) return;
        
        Collider[] hits = Physics.OverlapSphere(transform.position, grabRadius);
        foreach (Collider hit in hits)
        {
            GemDropLogic gem = hit.GetComponent<GemDropLogic>();
            if (gem != null)
            {
                heldGem = hit.gameObject;
                heldGem.GetComponent<Rigidbody>().isKinematic = true; // make the gem follow the hand without physics interference
                heldGem.transform.SetParent(transform);
                heldGem.transform.localPosition = Vector3.zero;
                SetHandColour(greenMaterial);
                break;
            }
        }
    }

    void TryPlace()
    {
        if (heldGem == null) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, grabRadius);
        
        // Check for socket first
        foreach (Collider hit in hits)
        {
            GemSocketLogic socket = hit.GetComponent<GemSocketLogic>();
            if (socket != null)
            {
                GameObject gemToPlace = heldGem;
                GemSO gemData = gemToPlace.GetComponent<GemDropLogic>().GetGemData();

                gemToPlace.transform.SetParent(socket.transform);
                gemToPlace.transform.localPosition = Vector3.zero;

                heldGem = null; // clear before OnGemPlaced fires CloseDoorInteraction
                SetHandColour(redMaterial);
                socket.OnGemPlaced(gemData);
                return;
            }
        }
        // No socket found, drop in world
        heldGem.GetComponent<Rigidbody>().isKinematic = false;
        heldGem.transform.SetParent(null);
        heldGem.transform.position = transform.position;
        inventoryLogic.TrackSpawnedItem(heldGem);

        GemSO droppedGemData  = heldGem.GetComponent<GemDropLogic>().GetGemData();
        PlayerInventory playerInventory = FindAnyObjectByType<PlayerInventory>();
        if (playerInventory != null)
        {
            playerInventory.RemoveItem(droppedGemData); // Ensure the dropped gem is removed from inventory
        }

        heldGem = null;
        SetHandColour(redMaterial);
        inventoryLogic.CloseDoorInteraction();
        Camera.main.GetComponent<CameraLogic>().ReturnToDefault();
    }

    void SetHandColour(Material mat)
    {
        if (handRenderer == null)
        {
            Debug.Log("handRenderer is null");
            return;
        }
        if (mat == null)
        {
            Debug.Log("material is null");
            return;
        }
        handRenderer.material = mat;
    }

    public void SetHeldGem(GameObject gem)
    {
        heldGem = gem;
        if (heldGem != null)
        {
            SetHandColour(greenMaterial);
        }
        else
        {
            SetHandColour(redMaterial);
        }
    }

    public void SwitchToDoorPlane(Transform door)
    {
        isDoorMode = true;
        doorTransform = door;
        planeY = transform.position.y; // lock current Y as the vertical centre
    }
}