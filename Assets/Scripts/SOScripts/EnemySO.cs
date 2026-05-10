using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Scriptable Objects/EnemySO")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public int enemyID;
    public int enemyHealth;
    public int enemyDamage;
    public float enemyMoveSpeed;
    public Sprite enemyIcon;
    public GameObject enemyPrefab;
}

[CreateAssetMenu(fileName = "SlimeSO", menuName = "Scriptable Objects/SlimeSO")]
public class SlimeSO : EnemySO
{
    public float slimeJumpHeight = 2f;
    public float slimeChaseRadius = 10f;
}
