using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DungeonManager : MonoBehaviour 
{
    
    // The singleton instance
    public static DungeonManager Instance { get; private set; }

    // Expose seeded random generator for use in other classes
    public System.Random SeededRandom { get; private set; }
    
    public static Dictionary<Vector2Int, RoomController> dungeonMap = new Dictionary<Vector2Int, RoomController>();

    // Dungeon generation parameters
    [SerializeField] private int dungeonSeed = 0;
    [SerializeField] private bool useRandomSeed = true;
    
    [SerializeField] private int maxRooms = 10;

    [SerializeField] private float heartDropChance = 0.5f; // Chance for a heart drop instead of a gem
    [SerializeField] private ItemSO heartItem; // Reference to heart item for drops
    private int roomCount = 0;

    [SerializeField] private LanguagePackSO currentPack;
    // For item drops, hardcoded for now, from pack later
    //[SerializeField] private List<ItemSO> requiredGems; // hardcoded for now, from pack later
    //[SerializeField] private List<ItemSO> possibleDrops; // all optional drops

    [SerializeField] private List<PotSO> possiblePots; // For random pot generation in rooms
    private List<GemSO> remainingRequiredGems; // tracked during generation

    // For spawning locked door in furthest room
    [SerializeField] private GameObject lockedDoorPrefab;

    // Fields for room content spawning, can be adjusted in inspector
    [SerializeField] private int minPotsPerRoom = 2;
    [SerializeField] private int maxPotsPerRoom = 5;
    [SerializeField] private GameObject potPrefab;
    [SerializeField] private List<GameObject> enemyPrefabs; // List of enemy types to spawn


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
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return; // don't run anything else on the duplicate
        }

        SetDungeonSeed();
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
                enemyPrefabs[SeededRandom.Next(0, enemyPrefabs.Count)], // pick random enemy type for this room
                minPotsPerRoom,
                maxPotsPerRoom
            );
        }
    }

    void SetDungeonSeed()
    {
        // Set seed for reproducibility
        int seed = useRandomSeed ? System.DateTime.Now.Millisecond : dungeonSeed;
        SeededRandom = new System.Random(seed);
        Debug.Log($"Dungeon seed: {seed}");
    }

    public void InitialiseDungeon()
    {
        remainingRequiredGems = new List<GemSO>(currentPack.vocabWordList[0].requiredGems); // copy required gems from pack for tracking
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

    public GemSO GetRequiredDrop()
    {
        if (remainingRequiredGems.Count > 0)
        {
            GemSO gem = remainingRequiredGems[SeededRandom.Next(0, remainingRequiredGems.Count)];
            remainingRequiredGems.Remove(gem);
            return gem;
        }
        return null;
    }

    public ItemSO GetDrop(PotSO potData)
    {
        if (remainingRequiredGems.Count > 0)
        {
            Debug.Log($"Required gems remaining: {remainingRequiredGems.Count}, dropping gem");
            return GetRequiredDrop();
        }

        if (SeededRandom.NextDouble() <= potData.dropChance)
        {
            Debug.Log($"Random roll: {SeededRandom.NextDouble():F2}, Heart drop chance: {heartDropChance}");
            if (SeededRandom.NextDouble() <= heartDropChance)
            {
                Debug.Log("Dropping heart item");
                return heartItem;
            }
            Debug.Log("Random Gem drop");
            return currentPack.fullGemList[SeededRandom.Next(0, currentPack.fullGemList.Count)];
        }

        return null;
    }

    public List<ItemSO> GetRequiredGems()
    {
        return new List<ItemSO>(currentPack.vocabWordList[0].requiredGems); // return copy not reference
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
                doorLogic.Initialise(currentPack.vocabWordList[0].requiredGems ); // pass required gems for unlocking
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
