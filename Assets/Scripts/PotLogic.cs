using UnityEngine;

public class PotLogic : MonoBehaviour
{
    [SerializeField] private PotSO potData;

    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material interactabletMaterial;
    
    private bool isThrown = false;
    private bool isBroken = false;
    private Rigidbody rb;

    private MeshRenderer currentRenderer;
    private bool isHighlighted = false;



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentRenderer = GetComponentInChildren<MeshRenderer>();
    }

    // Called by player interaction
    public void OnPickup(Transform carryPoint)
    {
        rb.isKinematic = true; // disable physics while carried
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OnPutDown(Transform playerTransform)
    {
        transform.SetParent(null);
        // Place in front of player
        Vector3 putDownPosition = playerTransform.position + playerTransform.forward * 1.5f;
        putDownPosition.y = playerTransform.position.y; // match player height
        transform.position = putDownPosition;
            rb.isKinematic = false;
    }

    public void OnThrow(Vector3 throwForce)
    {
        isThrown = true;
        rb.isKinematic = false;
        transform.SetParent(null);
        rb.AddForce(throwForce, ForceMode.Impulse);
        // Spin forward in throw direction
        Vector3 spinAxis = Vector3.Cross(throwForce.normalized, -Vector3.up);
        rb.AddTorque(spinAxis * (throwForce.magnitude / 2f), ForceMode.Impulse);
    }

    // Break on weapon hit via this public method
    public void OnWeaponHit(Vector3 hitDirection)
    {
        if (isBroken) return;
        isBroken = true;
        Break(hitDirection);
    }

    // Break on landing after throw
    void OnCollisionEnter(Collision collision)
    {
        if (isBroken) return;
        if (isThrown)
        {
            isBroken = true;
            Break();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Logic for changing ht pot material to show its interactable
        if (isHighlighted) return;
        if (other.CompareTag("Player") && currentRenderer != null)
        {
            currentRenderer.material = interactabletMaterial;
            isHighlighted = true;
        }
        else
        {
            Debug.Log("No renderer found.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Logic for changing ht pot material back to default
        if (!isHighlighted) return;
        if (other.CompareTag("Player") && currentRenderer != null)
        {
            currentRenderer.material = defaultMaterial;
            isHighlighted = false;
        }
        else
        {
            Debug.Log("No renderer found.");
        }
    }

    void Break(Vector3 hitDirection = default)
    {
        Debug.Log("Pot broken!");
        Debug.Log($"DungeonManager: {DungeonManager.Instance}, TutorialDungeonManager: {TutorialDungeonManager.Instance}");

        ItemSO drop = DungeonManagerLocator.Instance.GetDrop(potData);
        if (drop != null)
        {
            GameObject droppedItem = Instantiate(drop.itemPrefab, transform.position, Quaternion.identity);
            ItemDropLogic dropLogic = droppedItem.GetComponent<ItemDropLogic>();
            if (dropLogic != null)
            {
                dropLogic.Initialise(drop);
            }

            Rigidbody dropRb = droppedItem.GetComponent<Rigidbody>();
            if (dropRb != null)
            {
                Vector3 randomSpin = Random.insideUnitSphere * 3f;
                Vector3 force = rb.linearVelocity.magnitude > 0.1f 
                    ? rb.linearVelocity * 0.5f
                    : hitDirection * 4f;
                    
                dropRb.AddForce(force + Vector3.up * 3f, ForceMode.Impulse);
                dropRb.AddTorque(randomSpin, ForceMode.Impulse);
            }
            DungeonManagerLocator.Instance.OnPotBroken(); // For tutorial
        }
        Destroy(gameObject);
    }
}
