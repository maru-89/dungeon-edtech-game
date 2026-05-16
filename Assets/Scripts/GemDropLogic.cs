using UnityEngine;

public class GemDropLogic : ItemDropLogic
{
    private GemSO gemData;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    public override void Initialise(ItemSO data)
    {
        gemData = data as GemSO;
    }

    void OnTriggerEnter(Collider other)
    {
        if (gemData == null) return; // safety check to prevent null reference errors
        
        Debug.Log($"Trigger entered by: {other.name} tag: {other.tag}");
        if (other.CompareTag("Player"))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
                if (playerInventory.AddItem(gemData))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
