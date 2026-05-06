using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth / 2; // Start with half health for testing heart pickups
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public bool Heal(int healAmount)
    {
        if (currentHealth >= maxHealth) return false;
        
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        return true;
    }

    private void Die()
    {
        // Handle player death (e.g., play animation, reload scene, etc.)
        Debug.Log("Player has died.");
    }
}
