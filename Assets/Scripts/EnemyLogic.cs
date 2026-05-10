using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
   public virtual void Initialise(EnemySO enemy) { }

   public virtual void TakeDamage(int damage)
   {
       // Default implementation does nothing, can be overridden by specific enemy types
   }

   public virtual void Die()
   {
       // Default implementation does nothing, can be overridden by specific enemy types
   }
}
