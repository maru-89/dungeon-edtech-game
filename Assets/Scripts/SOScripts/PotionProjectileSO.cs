using UnityEngine;

[CreateAssetMenu(fileName = "PotionProjectileSO", menuName = "Scriptable Objects/PotionProjectileSO")]
public class PotionProjectileSO : ItemSO
{
    public float damageAmount = 1f;
    public float slowAmount = 0.5f; // Percentage to slow the player (e.g., 0.5 for 50% slow)
    public float lingerDuration = 2f; // How long the potion's effect lingers on the ground after being thrown
}
