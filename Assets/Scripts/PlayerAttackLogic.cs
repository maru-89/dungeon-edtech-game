using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackLogic : MonoBehaviour
{
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float knockbackForce = 5f;
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
        Collider[] weaponHits = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider entity in weaponHits)
        {
            EnemyLogic enemy = entity.GetComponent<EnemyLogic>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                Vector3 knockbackDirection = (entity.transform.position - transform.position).normalized;
                enemy.ApplyKnockback(knockbackDirection, knockbackForce);
            }

            PotLogic pot = entity.GetComponent<PotLogic>();
            if (pot != null)
            {
                pot.OnWeaponHit(transform.forward);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the attack range in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
