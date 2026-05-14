using System.Collections.Generic;
using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private PlayerHealth playerHealth;

    private List<HeartUI> hearts = new List<HeartUI>();

    private void Start()
    {
        int heartCount = playerHealth.GetMaxHealth() / 4;
        
        for (int i = 0; i < heartCount; i++)
        {
            GameObject heart = Instantiate(heartPrefab, container);
            hearts.Add(heart.GetComponent<HeartUI>());
        }

        playerHealth.OnHealthChanged += UpdateHearts;
        UpdateHearts(playerHealth.GetCurrentHealth(), playerHealth.GetMaxHealth());
    }

    private void UpdateHearts(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            int heartHealth = Mathf.Clamp(currentHealth - (i * 4), 0, 4);
            hearts[i].SetHealth(heartHealth);
        }
    }

    private void OnDestroy()
    {
        playerHealth.OnHealthChanged -= UpdateHearts;
    }
}