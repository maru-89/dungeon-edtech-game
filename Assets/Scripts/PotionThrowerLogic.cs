using System.Collections;
using UnityEngine;

public class PotionThrowerLogic : EnemyLogic
{
    [SerializeField] private PotionThrowerSO potionThrowerData;
    [SerializeField] private GameObject potionProjectilePrefab;
    [SerializeField] private float wanderInterval = 2f;
    private PotionProjectileSO potionProjectileData; // Store the potion data for initializing projectiles
    private Transform playerTransform; // Cache player transform for chasing logic

    private int maxHealth;
    private int currentHealth;

    private float damageCooldown = 1f; // Time in seconds between damage instances to player
    private float lastDamageTime; // Timestamp of last damage instance to player
    private Rigidbody rb;
    private SphereCollider triggerSphere;
    private Vector3 wanderDirection;
    private bool isKnockedBack = false;
    private bool isDead = false;
    private float hitCooldown = 0.2f;
    private float lastHitTime;
    private float knockbackResetTime;
    private float lastThrowTime;
    private float wanderTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Cache Rigidbody reference for movement
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Cache player transform for chasing logic
        maxHealth = potionThrowerData.enemyHealth; // Store max health for potential future use (e.g. health bars)
        currentHealth = maxHealth; // Initialize current health to max health

        // Get the trigger sphere specifically
        SphereCollider[] colliders = GetComponents<SphereCollider>();
        foreach (SphereCollider col in colliders)
        {
            if (col.isTrigger)
            {
                triggerSphere = col;
                break;
            }
        }
        Initialise(potionThrowerData);
    }

    public override void Initialise(EnemySO data)
    {
        base.Initialise(data); // Call base method to set enemyData
        potionThrowerData = data as PotionThrowerSO; // Cast to PotionThrowerSO to access potion thrower-specific properties
        potionProjectileData = potionThrowerData.potionData; // Access the potion data for throwing logic
    }

    void Update()
    {
        if (isDead) return;
    
        // Fall back to finding player transform if it was not found in Awake (e.g. player spawned after slime)
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
            return;
        }

        StateCheck();

        if (isKnockedBack && Time.time >= knockbackResetTime)
        {
            isKnockedBack = false;
        }

        if (Time.time - lastDamageTime < damageCooldown) return;

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            triggerSphere.radius); // use the actual trigger sphere radius
            
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                //Debug.Log("PotionThrower collided with player, dealing damage. Update with OverlapSphere");
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(potionThrowerData.enemyDamage);
                    lastDamageTime = Time.time;
                }
                break;
            }
        }
    }

    void StateCheck()
    {
        if (!IsPlayerInRange())
        {
            //Debug.Log("Player not in range, wandering.");
            Wander();
            return;
        }

        if (IsPlayerTooClose())
        {
            //Debug.Log("Player too close, retreating.");
            Retreat();
        }
        else if (IsPlayerInThrowingRange())
        {
            //Debug.Log("Player in throwing range, throwing potion.");
            ThrowPotion(); // stay still and throw
        }
        else
        {
            //Debug.Log("Chasing player.");
            Chase();
        }
    }

    void Retreat()
    {
        if (playerTransform != null)
        {
            Vector3 directionAwayFromPlayer = (transform.position - playerTransform.position).normalized;
            directionAwayFromPlayer.y = 0; // keep horizontal only

            // Clamp velocity to prevent excessive speed buildup from continuous force application
            if (rb.linearVelocity.magnitude > potionThrowerData.enemyMoveSpeed)            {
                rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, potionThrowerData.enemyMoveSpeed);
            }
            rb.AddForce(directionAwayFromPlayer * potionThrowerData.enemyMoveSpeed, ForceMode.VelocityChange);
        }
    }

    bool IsPlayerTooClose()
    {
        //Debug.Log($"IsPlayerTooClose check, threshold: {potionThrowerData.playerDistanceThreshold * 0.5f}");
        Collider[] hits = Physics.OverlapSphere(transform.position, potionThrowerData.playerDistanceThreshold * 0.5f);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player")) return true;
        }
        return false;
    }

    void ThrowPotion()
    {
        if (Time.time - lastThrowTime < potionThrowerData.potionThrowCooldown) return;
        
        lastThrowTime = Time.time;
        rb.linearVelocity = Vector3.zero; // stop moving before throwing
        
        GameObject potion = Instantiate(potionProjectilePrefab, transform.position + Vector3.up, Quaternion.identity);
        Physics.IgnoreCollision(potion.GetComponent<Collider>(), GetComponent<Collider>());
        PotionProjectileLogic potionLogic = potion.GetComponent<PotionProjectileLogic>();
        if (potionLogic != null)
        {
            potionLogic.Initialize(potionProjectileData);
        }

        Rigidbody potionRb = potion.GetComponent<Rigidbody>();
        if (potionRb != null)
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.y = 0; // keep horizontal direction only
            float distance = directionToPlayer.magnitude;

            // Calculate force needed to reach target
            float throwForce = Random.Range(potionThrowerData.potionThrowForceMin, potionThrowerData.potionThrowForceMax);
            Vector3 horizontalForce = directionToPlayer.normalized * throwForce;
            Vector3 verticalForce = Vector3.up * throwForce * 0.5f;

            potionRb.AddForce(horizontalForce + verticalForce, ForceMode.Impulse);
        }
    }


    void Wander()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderInterval)
        {
            wanderTimer = 0f;
            float randomAngle = Random.Range(0f, 360f);
            wanderDirection = new Vector3(
                Mathf.Cos(randomAngle * Mathf.Deg2Rad),
                0,
                Mathf.Sin(randomAngle * Mathf.Deg2Rad));
        }

        if (rb.linearVelocity.magnitude < potionThrowerData.enemyMoveSpeed)
        {
            rb.AddForce(wanderDirection * potionThrowerData.enemyMoveSpeed, ForceMode.VelocityChange);
        }
    }

    void Chase()
    {
        if (playerTransform != null)
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            directionToPlayer.y = 0; // keep horizontal only

            // Clamp velocity to prevent excessive speed buildup from continuous force application
            if (rb.linearVelocity.magnitude > potionThrowerData.enemyMoveSpeed)            {
                rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, potionThrowerData.enemyMoveSpeed);
            }
            rb.AddForce(directionToPlayer * potionThrowerData.enemyMoveSpeed, ForceMode.VelocityChange);

            // Attempt to maintain distance from player by applying force in opposite direction if too close
            Vector3 toPlayer = playerTransform.position - transform.position;
            if (toPlayer.magnitude < potionThrowerData.playerDistanceThreshold * 0.5f) // if too close, try to back away
            {
                Vector3 awayFromPlayer = -toPlayer.normalized;
                rb.AddForce(awayFromPlayer * potionThrowerData.enemyMoveSpeed, ForceMode.VelocityChange);
            }
        }
    }

    bool IsPlayerInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, potionThrowerData.enemyDetectionRange);
        //Debug.Log($"IsPlayerInRange check, detectionRange: {potionThrowerData.enemyDetectionRange}, hits: {hits.Length}");
        foreach (Collider hit in hits)
        {
            //Debug.Log($"Hit: {hit.name} tag: {hit.tag}");
            if (hit.CompareTag("Player")) return true;
        }
        return false;
    }

    bool IsPlayerInThrowingRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, potionThrowerData.playerDistanceThreshold);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                float distanceToPlayer = Vector3.Distance(transform.position, hit.transform.position);
                return distanceToPlayer <= potionThrowerData.playerDistanceThreshold;
            }
        }
        return false;
    }


    public override void ApplyKnockback(Vector3 direction, float force)
    {
        if (isKnockedBack) return;
        
        isKnockedBack = true;
        knockbackResetTime = Time.time + 0.1f;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    public override void TakeDamage(int damage)
    {
        // Slime-specific damage logic
        if (isDead) return;
        if (Time.time - lastHitTime < hitCooldown) return; // this might be blocking the kill
        
        lastHitTime = Time.time;
        currentHealth -= damage;
        
        //Debug.Log($"PotionThrower health: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            isDead = true;
            Die();
        }
        //Debug.Log($"PotionThrower took {damage} damage. Implement health reduction and death logic.");
    }

    public void Die()
    {
        //Debug.Log("PotionThrower died.");
        base.Die(potionThrowerData.coinDropChance);
    }
}
