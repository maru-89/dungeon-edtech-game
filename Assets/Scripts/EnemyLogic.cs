using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    public virtual void Initialise(EnemySO enemy) { }
    public virtual void TakeDamage(int damage) { }
    public virtual void ApplyKnockback(Vector3 direction, float force) { }

    public virtual void Die(float coinDropChance)
    {
        ItemSO drop = DungeonManagerLocator.Instance.GetEnemyDrop(coinDropChance);
        if (drop != null)
        {
            GameObject droppedItem = Instantiate(drop.itemPrefab, transform.position, Quaternion.identity);
            ItemDropLogic dropLogic = droppedItem.GetComponent<ItemDropLogic>();
            if (dropLogic != null)
            {
                dropLogic.Initialise(drop);
                Rigidbody dropRb = droppedItem.GetComponent<Rigidbody>();
                if (dropRb != null)
                {
                    Vector3 randomDirection = new Vector3(
                        (float)DungeonManagerLocator.Instance.SeededRandom.NextDouble() * 2f - 1f,
                        0,
                        (float)DungeonManagerLocator.Instance.SeededRandom.NextDouble() * 2f - 1f
                    ).normalized;
                    
                    dropRb.AddForce(randomDirection * 3f + Vector3.up * 4f, ForceMode.Impulse);
                    dropRb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
                }
            }
        }
        Destroy(gameObject);
    }
}
