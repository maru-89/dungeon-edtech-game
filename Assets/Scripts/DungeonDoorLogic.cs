using System.Collections.Generic;
using UnityEngine;

public class DungeonDoorLogic : MonoBehaviour
{
    // This is the logic for the locked doors in the dungeon, which will require gems to open.
    // For now, we will just have a simple logic that checks if the player has the required gems to open the door, and if so, opens the door and allows the player to "win".

    private List<ItemSO> requiredGems; // get from DungeonManager when the door is initialized

    
}
