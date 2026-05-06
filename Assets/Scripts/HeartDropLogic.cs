using UnityEngine;

public class HeartDropLogic : ItemDropLogic
{
    private HeartSO heartData;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    public override void Initialise(ItemSO data)
    {
        heartData = data as HeartSO;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.name} tag: {other.tag}");   //
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                if (playerHealth.Heal(heartData.healAmount))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
