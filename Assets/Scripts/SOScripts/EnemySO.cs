using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/EnemySO")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public int enemyID;
    public int enemyHealth;
    public int enemyDamage;
    public float enemyMoveSpeed;
    public float enemyDetectionRange;
    public Sprite enemyIcon;
    public GameObject enemyPrefab;
    public float coinDropChance = 0.5f; // Chance for this enemy to drop a coin on death
}