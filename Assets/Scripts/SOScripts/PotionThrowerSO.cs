using UnityEngine;

[CreateAssetMenu(fileName = "PotionThrowerSO", menuName = "Scriptable Objects/PotionThrowerSO")]
public class PotionThrowerSO : EnemySO
{
    public float potionThrowCooldown = 3f;
    public float playerDistanceThreshold = 2f; // Distance at which Potion Thrower will attempt to keep to the player and throw potions
    public float potionThrowForceMax = 5f; // Force applied to the potion when thrown
    public float potionThrowForceMin = 2f; // Minimum force applied to the potion when thrown, to prevent very weak throws

    public PotionProjectileSO potionData; // Reference to the potion data for throwing logic
}
