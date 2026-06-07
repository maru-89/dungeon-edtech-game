using UnityEngine;
using System.Collections.Generic;

public class RoomController : MonoBehaviour
{
    public Vector2Int gridPosition;
    [SerializeField] private Transform spawnCenter;    
    [SerializeField] private List<Transform> landmarkPositions; // Predefined positions in rooms for landmarks for method of loci
    [SerializeField] private List<GameObject> landmarkPrefabs; // Prefabs for landmarks for method of loci
    [SerializeField] private float wallDistance = 100f; // Distance from center to wall, tune to taste
    [SerializeField] private float torchHeight = 5f; // Height of torches, tune to taste
    [SerializeField] private GameObject torchPrefab; // Prefab for torch for method of loci
    private int[] oddOffsets = { -35, -25, -15, -5, 5, 15, 25, 35 };
    private int[] evenOffsets = { -30, -15, 0, 15, 30 };

    void Awake()
    {
        if (CompareTag("CenterRoom"))
        {
            gridPosition = Vector2Int.zero;
            DungeonManager.dungeonMap[gridPosition] = this;
            DungeonManager.Instance.IncrementRoomCount();
            
            RoomSpawner[] spawners = GetComponentsInChildren<RoomSpawner>();
            foreach (RoomSpawner spawner in spawners)
            {
                spawner.parentGridPosition = Vector2Int.zero;
            }
        }
    }

    public void CleanupDoors(bool hasNorth, bool hasSouth, bool hasWest, bool hasEast)
    {
        Debug.Log($"=== Cleanup for {gameObject.name} at {gridPosition} ===");
        Debug.Log($"Neighbours - N:{hasNorth} S:{hasSouth} W:{hasWest} E:{hasEast}");
        
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        
        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("DoorTop"))
                Debug.Log($"Found DoorTop: {child.name} - hasNorth:{hasNorth} - will delete:{!hasNorth}");
            if (child.CompareTag("DoorBottom"))
                Debug.Log($"Found DoorBottom: {child.name} - hasSouth:{hasSouth} - will delete:{!hasSouth}");
            if (child.CompareTag("DoorLeft"))
                Debug.Log($"Found DoorLeft: {child.name} - hasWest:{hasWest} - will delete:{!hasWest}");
            if (child.CompareTag("DoorRight"))
                Debug.Log($"Found DoorRight: {child.name} - hasEast:{hasEast} - will delete:{!hasEast}");

            if (!hasNorth && child.CompareTag("DoorTop")) Destroy(child.gameObject);
            if (!hasSouth && child.CompareTag("DoorBottom")) Destroy(child.gameObject);
            if (!hasWest && child.CompareTag("DoorLeft")) Destroy(child.gameObject);
            if (!hasEast && child.CompareTag("DoorRight")) Destroy(child.gameObject);
        }
    }

    public void SpawnRoomContents(GameObject potPrefab, GameObject enemyPrefab, int minPots, int maxPots)
    {        
        System.Random rng = DungeonManagerLocator.Instance.SeededRandom; // use seeded random generator from DungeonManager for consistency
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();

        // Spawn pots
        int potCount = rng.Next(minPots, maxPots + 1);
        int potsSpawned = 0;
        int attempts = 0;
        
        while (potsSpawned < potCount && attempts < 20)
        {
            attempts++;
            
            int xOffset = oddOffsets[rng.Next(0, oddOffsets.Length)];
            int zOffset = oddOffsets[rng.Next(0, oddOffsets.Length)];
            Vector2Int gridPos = new Vector2Int(xOffset, zOffset);
            
            if (usedPositions.Contains(gridPos)) continue;
            
            usedPositions.Add(gridPos);
            Vector3 position = spawnCenter.TransformPoint(new Vector3(xOffset, 0, zOffset));
            Instantiate(potPrefab, position, Quaternion.identity);
            Debug.Log($"Spawning contents in room: {gameObject.name} at {gridPosition}");
            potsSpawned++;
        }

        // Spawn enemies
        int enemyCount = rng.Next(1, 3);
        int enemiesSpawned = 0;
        attempts = 0;
        
        while (enemiesSpawned < enemyCount && attempts < 20)
        {
            attempts++;
            
            int xOffset = evenOffsets[rng.Next(0, evenOffsets.Length)];
            int zOffset = evenOffsets[rng.Next(0, evenOffsets.Length)];
            Vector2Int gridPos = new Vector2Int(xOffset, zOffset);
            
            if (usedPositions.Contains(gridPos)) continue;
            
            usedPositions.Add(gridPos);
            Vector3 position = spawnCenter.TransformPoint(new Vector3(xOffset, 0, zOffset));
            Instantiate(enemyPrefab, position, Quaternion.identity);
            enemiesSpawned++;
        }

        // Spawn landmarks
        SpawnLandmark();

        // Spawn torches
        SpawnTorches();
    }

    void SpawnLandmark()
    {
        if (landmarkPositions.Count == 0 || landmarkPrefabs.Count == 0) return;
        System.Random rng = DungeonManagerLocator.Instance.SeededRandom;
        Transform chosenPosition = landmarkPositions[rng.Next(0, landmarkPositions.Count)];
        GameObject chosenPrefab = landmarkPrefabs[rng.Next(0, landmarkPrefabs.Count)];
        Instantiate(chosenPrefab, chosenPosition.position, chosenPosition.rotation);
    }

    void SpawnTorches()
    {
        if (torchPrefab == null) return;
        System.Random rng = DungeonManagerLocator.Instance.SeededRandom;
        
        float r = (float)rng.NextDouble();
        float g = (float)rng.NextDouble();
        float b = (float)rng.NextDouble();
        Color roomColor = new Color(r, g, b);

        Vector3[] torchOffsets = {
            new Vector3(0, torchHeight, wallDistance),   // North
            new Vector3(0, torchHeight, -wallDistance),  // South
            new Vector3(wallDistance, torchHeight, 0),   // East
            new Vector3(-wallDistance, torchHeight, 0)   // West
        };

        foreach (Vector3 offset in torchOffsets)
        {
            Vector3 position = spawnCenter.TransformPoint(offset);
            GameObject torch = Instantiate(torchPrefab, position, Quaternion.identity);
            Light torchLight = torch.GetComponentInChildren<Light>();
            if (torchLight != null)
            {
                torchLight.color = roomColor;
            }
        }
    }
}
