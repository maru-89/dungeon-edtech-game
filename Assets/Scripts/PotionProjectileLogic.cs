using UnityEngine;

public class PotionProjectileLogic : MonoBehaviour
{
    // Logic for how the potion interacts with the player and the environment after being thrown by the Potion Thrower
    private PotionProjectileSO potionData; // Reference to the potion data for applying effects
    [SerializeField] private GameObject slowZonePrefab;

    public void Initialize(PotionProjectileSO potionProjectile)
    {
        potionData = potionProjectile; // Store the potion data for use in collision logic
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Potion collided with: " + collision.gameObject.name);
        Debug.Log($"Potion hit: {collision.gameObject.name} tag: {collision.gameObject.tag}");
        if (collision.gameObject.CompareTag("Ground"))
        {
            GameObject slowZone = Instantiate(slowZonePrefab, transform.position, Quaternion.identity);
            SlowZoneLogic slowZoneLogic = slowZone.GetComponent<SlowZoneLogic>();
            if (slowZoneLogic != null)
            {
                slowZoneLogic.Initialize(potionData);
            }
            Destroy(gameObject);
        }
        //Destroy(gameObject);
    }
}
