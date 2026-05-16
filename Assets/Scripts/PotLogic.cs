using UnityEngine;

public class PotLogic : MonoBehaviour
{
    [SerializeField] private PotSO potData;
    
    private bool isThrown = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
    }

    // Break on weapon hit via this public method
    public void OnWeaponHit(Vector3 hitDirection)
    {
        Break(hitDirection);
    }

    // Break on landing after throw
    void OnCollisionEnter(Collision collision)
    {
        if (isThrown)
        {
            Break();
        }
    }

    void Break(Vector3 hitDirection = default)
    {
        ItemSO drop = DungeonManager.Instance.GetDrop(potData);
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
        }
        Destroy(gameObject);
    }
}
