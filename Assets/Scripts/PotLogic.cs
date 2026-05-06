using UnityEngine;

public class PotLogic : MonoBehaviour
{
    [SerializeField] private PotSO potData;
    
    private bool isCarried = false;
    private bool isThrown = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Called by player interaction
    public void OnPickup(Transform carryPoint)
    {
        isCarried = true;
        rb.isKinematic = true; // disable physics while carried
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OnPutDown(Transform playerTransform)
    {
        isCarried = false;
        transform.SetParent(null);
        // Place in front of player
        Vector3 putDownPosition = playerTransform.position + playerTransform.forward * 1.5f;
        putDownPosition.y = playerTransform.position.y; // match player height
        transform.position = putDownPosition;
            rb.isKinematic = false;
    }

    public void OnThrow(Vector3 throwForce)
    {
        isCarried = false;
        isThrown = true;
        rb.isKinematic = false;
        transform.SetParent(null);
        rb.AddForce(throwForce, ForceMode.Impulse);
    }

    // Break on weapon hit via this public method
    public void OnWeaponHit()
    {
        Break();
    }

    // Break on landing after throw
    void OnCollisionEnter(Collision collision)
    {
        if (isThrown)
        {
            Break();
        }
    }

    void Break()
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
        }
        Destroy(gameObject);
    }
}
