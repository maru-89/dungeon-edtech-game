using System.Collections.Generic;

public interface IDungeonManager
{
    System.Random SeededRandom { get; }
    ItemSO GetDrop(PotSO potData);
    ItemSO GetEnemyDrop(float dropChance);
    List<ItemSO> GetRequiredGems();
}