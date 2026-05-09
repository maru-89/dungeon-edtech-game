using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DungeonManager : MonoBehaviour 
{
    
    // The singleton instance
    public static DungeonManager Instance { get; private set; }
    
    public static Dictionary<Vector2Int, RoomController> dungeonMap = new Dictionary<Vector2Int, RoomController>();
    
    [SerializeField] private int maxRooms = 10;
    private int roomCount = 0;

    // For item drops, hardcoded for now, from pack later
    [SerializeField] private List<ItemSO> requiredGems; // hardcoded for now, from pack later
    [SerializeField] private List<ItemSO> possibleDrops; // all optional drops

    [SerializeField] private List<PotSO> possiblePots; // For random pot generation in rooms
    private List<ItemSO> remainingRequiredGems; // tracked during generation

    // For spawning locked door in furthest room
    [SerializeField] private GameObject lockedDoorPrefab;

    // Fields for room content spawning, can be adjusted in inspector
    [SerializeField] private int minPotsPerRoom = 2;
    [SerializeField] private int maxPotsPerRoom = 5;
    [SerializeField] private GameObject potPrefab;
    [SerializeField] private GameObject enemyPrefab;


    IEnumerator LogAfterGeneration()
    {
        yield return new WaitForSeconds(5f);
        CleanupOrphanedDoors();

        RoomController furthestRoom = GetFurthestRoom();
        
        SpawnLockedDoor(); // spawn locked door in furthestRoom

        SpawnRoomContents(); // spawn pots and enemies in all rooms

        LogDungeonMap(); // Final log to check everything after generation
    }

    void Start()
    {
        StartCoroutine(LogAfterGeneration());
        InitialiseDungeon();
    }
    
    void Awake() 
    {
        // Set the singleton instance
        if (Instance == null) {
            Instance = this;
        } else {
            // If a second DungeonManager somehow exists, destroy it
            Destroy(gameObject);
        }
    }

    void SpawnRoomContents()
    {
        Debug.Log($"Dictionary contains {dungeonMap.Count} rooms");
        foreach (var kvp in dungeonMap)
        {
            RoomController room = kvp.Value;
            
            // Skip central room and locked door room eventually
            room.SpawnRoomContents(
                potPrefab,
                enemyPrefab,
                minPotsPerRoom,
                maxPotsPerRoom
            );
        }
    }

    public void InitialiseDungeon()
    {
        remainingRequiredGems = new List<ItemSO>(requiredGems);
    }

    public void LogDungeonMap()
    {
        Debug.Log($"Total rooms registered: {dungeonMap.Count} (including central room)");
        foreach (var kvp in dungeonMap)
        {
            Debug.Log($"Room at grid position: {kvp.Key}");
        }
    }

    public void CleanupOrphanedDoors()
    {
        foreach (var kvp in dungeonMap)
        {
            Vector2Int pos = kvp.Key;
            RoomController room = kvp.Value;

            room.CleanupDoors(
                dungeonMap.ContainsKey(pos + Vector2Int.up),    // Z+ exists? (DoorTop)
                dungeonMap.ContainsKey(pos + Vector2Int.down),  // Z- exists? (DoorBottom)
                dungeonMap.ContainsKey(pos + Vector2Int.left),  // X- exists? (DoorLeft)
                dungeonMap.ContainsKey(pos + Vector2Int.right)  // X+ exists? (DoorRight)
            );
        }
    }

    public ItemSO GetRequiredDrop()
    {
        if (remainingRequiredGems.Count > 0)
        {
            ItemSO gem = remainingRequiredGems[Random.Range(0, remainingRequiredGems.Count)];
            remainingRequiredGems.Remove(gem);
            return gem;
        }
        return null;
    }

    public ItemSO GetDrop(PotSO potData)
    {
        // Required gems take priority
        if (remainingRequiredGems.Count > 0)
        {
            return GetRequiredDrop();
        }
        
        // Otherwise roll for optional drop
        if (Random.value <= potData.dropChance)
        {
            return possibleDrops[Random.Range(0, possibleDrops.Count)];
        }
        
        return null; // no drop
    }

    public List<ItemSO> GetRequiredGems()
    {
        return new List<ItemSO>(requiredGems); // return copy not reference
    }

    void SpawnLockedDoor()
    {
        RoomController furthestRoom = GetFurthestRoom();
        if (furthestRoom != null)
        {
            GameObject door = Instantiate(lockedDoorPrefab,
                furthestRoom.transform.position,
                Quaternion.identity);
                
            DungeonDoorLogic doorLogic = door.GetComponent<DungeonDoorLogic>();
            if (doorLogic != null)
            {
                doorLogic.Initialise(requiredGems);
            }
        }
    }

    public RoomController GetFurthestRoom()
    {
        RoomController furthestRoom = null;
        float furthestDistance = 0f;

        foreach (var kvp in dungeonMap)
        {
            float distance = Vector3.Distance(Vector3.zero, 
                new Vector3(kvp.Key.x, 0, kvp.Key.y));
            if (distance > furthestDistance)
            {
                furthestDistance = distance;
                furthestRoom = kvp.Value;
            }
        }
        return furthestRoom;
    }
    
    // Expose maxRooms and roomCount via properties
    public int MaxRooms => maxRooms;
    public int RoomCount => roomCount;
    public void IncrementRoomCount() => roomCount++;
}
