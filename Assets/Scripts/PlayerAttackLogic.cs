using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackLogic : MonoBehaviour
{
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float knockbackForce = 1.5f;
    //[SerializeField] private LayerMask enemyLayer;

    private PlayerInput playerInput;
    private InputAction attackAction;

    private float attackCooldown = 0.5f; // Time in seconds between attacks
    private float lastAttackTime; // Timestamp of last attack

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        attackAction = playerInput.actions["Player/Attack"];
    }

    private void OnEnable()
    {
        attackAction.Enable();
    }

    private void OnDisable()
    {
        attackAction.Disable();
    }

    void Update()
    {
        if (attackAction.WasPressedThisFrame() && Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    public void Attack()
    {
        // Check for enemies in range
        Collider[] weaponHits = Physics.OverlapSphere(transform.position, attackRange);

        // Damage each enemy hit
        foreach (Collider entity in weaponHits)
        {
            EnemyLogic enemyHealth = entity.GetComponent<EnemyLogic>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                Rigidbody enemyRb = entity.GetComponent<Rigidbody>(); // Get the enemy's Rigidbody for knockback
                AttackKnockback(enemyRb, knockbackForce);

                Debug.Log("Player attacked enemy, dealing damage and knockback");
            }
            PotLogic pot = entity.GetComponent<PotLogic>();
            if (pot != null)
            {
                pot.OnWeaponHit();
            }
        }
    }

    public void AttackKnockback(Rigidbody rb, float knockbackForce)
    {
        if (rb == null) return;

        Vector3 knockbackDirection = (rb.transform.position - transform.position).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the attack range in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
