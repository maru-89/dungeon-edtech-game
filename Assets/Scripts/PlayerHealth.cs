using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 12; // 3 full hearts = 12 health points
    [SerializeField] private int currentHealth;
    public event Action<int, int> OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth / 2; // Start with half health for testing heart pickups
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
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
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        return true;
    }

    private void Die()
    {
        // Handle player death (e.g., play animation, reload scene, etc.)
        Debug.Log("Player has died.");
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
