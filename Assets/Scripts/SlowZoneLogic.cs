using UnityEngine;

public class SlowZoneLogic : MonoBehaviour
{
    private PotionProjectileSO potionData;
    private PlayerMovement playerMovement;
    private float damageTickTimer;
    private bool hasSlowedPlayer = false;

    public void Initialize(PotionProjectileSO potionProjectile)
    {
        potionData = potionProjectile;
        Destroy(gameObject, potionData.lingerDuration);
    }

    void Update()
    {
        if (playerMovement == null) return;
        
        damageTickTimer += Time.deltaTime;
        if (damageTickTimer >= 1f)
        {
            damageTickTimer = 0f;
            PlayerHealth health = playerMovement.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage((int)potionData.damageAmount);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement = other.GetComponent<PlayerMovement>();
            playerMovement?.ApplySlow(potionData.slowAmount);
            hasSlowedPlayer = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement?.RemoveSlow();
            playerMovement = null;
            hasSlowedPlayer = false;
        }
    }

    void OnDestroy()
    {
        if (hasSlowedPlayer && playerMovement != null)
        {
            playerMovement.RemoveSlow();
        }
    }
}
