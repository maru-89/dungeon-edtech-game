using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DungeonManager : MonoBehaviour, IDungeonManager
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

    [SerializeField] private float heartDropChance = 0.25f; // Chance for a heart drop instead of a gem
    [SerializeField] private float coinDropChance = 0.25f; // Chance for a coin drop instead of a gem
    [SerializeField] private ItemSO heartItem; // Reference to heart item for drops
    [SerializeField] private ItemSO coinItem; // Reference to coin item for drops
    private int roomCount = 0;

    [SerializeField] private CurriculumPackSO currentPack;
    private CurriculumItemSO selectedWord;
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

    private float roomScale; // For minimap calculations

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
        CosmeticInventory.Load(); // Load cosmetic inventory to ensure any unlocked cosmetics are available when starting the game
        roomScale = MinimapManager.Instance.GetOverlayScale(); // Cache the room scale
        StartCoroutine(LogAfterGeneration());
        InitialiseDungeon();
    }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            dungeonMap.Clear(); // clear before any RoomController registers
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        SetDungeonSeed();
        SetCurriculumPack(GameConfig.ActivePack); // Set the curriculum pack from the static GameConfig
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
        
        Debug.Log($"MinimapManager instance: {MinimapManager.Instance != null}");
        MinimapManager.Instance.GenerateOverlays(); // Generate minimap overlay
        MinimapManager.Instance.RevealRoom(Vector2Int.zero); // Reveal the center room
    }

    public void SetCurriculumPack(CurriculumPackSO pack)
    {
        if (pack != null)
        {
            currentPack = pack;
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
        int wordIndex = SeededRandom.Next(0, currentPack.curriculumItemList.Count);
        Debug.Log($"Word index selected: {wordIndex}");
        selectedWord = currentPack.curriculumItemList[wordIndex];
        remainingRequiredGems = new List<GemSO>(selectedWord.requiredGems); // copy required gems from pack for tracking
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

    public Vector2Int GetGridPositionFromWorld(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x / roomScale), Mathf.RoundToInt(worldPosition.z / roomScale));
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
            if (SeededRandom.NextDouble() <= heartDropChance)
            {
                Debug.Log("Dropping heart item");
                return heartItem;
            }
            if (SeededRandom.NextDouble() <= coinDropChance)
            {
                Debug.Log("Dropping coin item");
                return coinItem;
            }
            Debug.Log("Random Gem drop");
            return currentPack.fullGemList[SeededRandom.Next(0, currentPack.fullGemList.Count)];
        }

        return null;
    }
    public ItemSO GetEnemyDrop(float dropChance)
    {
        if (SeededRandom.NextDouble() <= dropChance)
        {
            return coinItem;
        }
        return null;
    }

    public List<ItemSO> GetRequiredGems()
    {
        return new List<ItemSO>(selectedWord.requiredGems); // return copy not reference
    }

    void SpawnLockedDoor()
    {
        RoomController furthestRoom = GetFurthestRoom();
        if (furthestRoom != null)
        {
            GameObject door = Instantiate(lockedDoorPrefab,
                furthestRoom.transform.position,
                Quaternion.Euler(0f, 180f, 0f));
                
            DungeonDoorLogic doorLogic = door.GetComponent<DungeonDoorLogic>();
            if (doorLogic != null)
            {
                doorLogic.Initialise(selectedWord); // pass required gems for unlocking
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

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    
    // Expose maxRooms and roomCount via properties
    public int MaxRooms => maxRooms;
    public int RoomCount => roomCount;
    public void IncrementRoomCount() => roomCount++;
}
