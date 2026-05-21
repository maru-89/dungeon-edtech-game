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
        if (heldGem == null)
            TryGrab();
        else
            TryPlace();
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
                heldGem.transform.SetParent(socket.transform);
                heldGem.transform.localPosition = Vector3.zero;
                socket.OnGemPlaced(heldGem.GetComponent<GemDropLogic>().GetGemData());
                heldGem = null;
                SetHandColour(redMaterial);
                return;
            }
        }

        // No socket found, drop in world
        heldGem.transform.SetParent(null);
        heldGem.transform.position = transform.position;
        inventoryLogic.TrackSpawnedItem(heldGem);
        heldGem = null;
        SetHandColour(redMaterial);
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