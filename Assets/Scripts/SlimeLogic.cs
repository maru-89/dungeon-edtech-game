using UnityEngine;

public class SlimeLogic : EnemyLogic
{
    [SerializeField] private SlimeSO slimeData;
    private Transform playerTransform; // Cache player transform for chasing logic

    private int maxHealth;
    private int currentHealth;

    private float damageCooldown = 1f; // Time in seconds between damage instances to player
    private float lastDamageTime; // Timestamp of last damage instance to player
    private Rigidbody rb;
    private bool isGrounded = false; // Track if slime is on the ground to control jumping
    private SphereCollider triggerSphere;
    private Vector3 wanderDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Cache Rigidbody reference for movement
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // Cache player transform for chasing logic
        maxHealth = slimeData.enemyHealth; // Store max health for potential future use (e.g. health bars)
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
    }

    public override void Initialise(EnemySO data)
    {
        slimeData = data as SlimeSO; // Cast to SlimeSO to access slime-specific properties
    }

    void Update()
    {
        if (isGrounded)
        {
            Jump();
        }

        if (Time.time - lastDamageTime < damageCooldown) return;

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            triggerSphere.radius); // use the actual trigger sphere radius
            
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Slime collided with player, dealing damage. Update with OverlapSphere");
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(slimeData.enemyDamage);
                    lastDamageTime = Time.time;
                }
                break;
            }
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * Mathf.Sqrt(slimeData.slimeJumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        if (IsPlayerInRange())
        {
            Chase();
        }
        else
        {
            Wander();
        }
        isGrounded = false;
    }

    void Wander()
    {
        // Pick random wander direction on each jump
        float randomAngle = Random.Range(0f, 360f);
        wanderDirection = new Vector3(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad),
            0,
            Mathf.Sin(randomAngle * Mathf.Deg2Rad));
        rb.AddForce(wanderDirection * slimeData.enemyMoveSpeed, ForceMode.VelocityChange);
    }

    void Chase()
    {
        if (playerTransform != null)
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            directionToPlayer.y = 0; // keep horizontal only
            rb.AddForce(directionToPlayer * slimeData.enemyMoveSpeed, ForceMode.VelocityChange);
        }
    }

    bool IsPlayerInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, slimeData.slimeChaseRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player")) return true;
        }
        return false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0); // Reset horizontal velocity on landing to prevent acceleration buildup
            isGrounded = true;
        }
    }

    public override void TakeDamage(int damage)
    {
        // Implement slime-specific damage logic here, e.g. reduce health, play hit animation, etc.
        if (slimeData != null)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        Debug.Log($"Slime took {damage} damage. Implement health reduction and death logic.");
    }

    public override void Die()
    {
        // Implement slime-specific death logic here, e.g. play death animation, destroy object, etc.
        Debug.Log("Slime died. Implement death effects and cleanup.");
        Destroy(gameObject);
    }
}
