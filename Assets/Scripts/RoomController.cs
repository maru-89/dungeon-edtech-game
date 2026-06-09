using UnityEngine;
using System.Collections.Generic;

public class RoomController : MonoBehaviour
{
    public Vector2Int gridPosition;
    [SerializeField] private Transform spawnCenter;    
    [SerializeField] private MethodOfLociSO methodOfLoci;

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

        SpawnRoomVisuals(rng.Next(0, methodOfLoci.roomThemes.Count), rng); // Randomly select a room theme for visual variety - can be overridden later to assign specific themes to specific rooms if desired
    }

    void SpawnRoomVisuals(int themeIndex, System.Random rng)
    {
        RoomThemeSO roomTheme = methodOfLoci.roomThemes[themeIndex];
        SpawnLandmark(roomTheme, rng);
        SpawnTorches(roomTheme);
    }

    void SpawnLandmark(RoomThemeSO roomTheme, System.Random rng)
    {
        Vector3[] landmarkOffsets = {
            new Vector3(methodOfLoci.wallDistance-methodOfLoci.landmarkOffset, 0, methodOfLoci.wallDistance-methodOfLoci.landmarkOffset),   // NorthEast
            new Vector3(-methodOfLoci.wallDistance+methodOfLoci.landmarkOffset, 0, methodOfLoci.wallDistance-methodOfLoci.landmarkOffset),  // NorthWest
            new Vector3(methodOfLoci.wallDistance-methodOfLoci.landmarkOffset, 0, -methodOfLoci.wallDistance+methodOfLoci.landmarkOffset),   // SouthEast
            new Vector3(-methodOfLoci.wallDistance+methodOfLoci.landmarkOffset, 0, -methodOfLoci.wallDistance+methodOfLoci.landmarkOffset)   // SouthWest
        };

        Vector3 position = spawnCenter.TransformPoint(landmarkOffsets[rng.Next(0, landmarkOffsets.Length)]);
        Instantiate(roomTheme.landmarkPrefab, position, Quaternion.identity);
    }

    void SpawnTorches(RoomThemeSO roomTheme)
    {
        if (methodOfLoci.torchPrefab == null) return;

        Vector3[] torchOffsets = {
            new Vector3(0, methodOfLoci.torchHeight, methodOfLoci.wallDistance),   // North
            new Vector3(0, methodOfLoci.torchHeight, -methodOfLoci.wallDistance),  // South
            new Vector3(methodOfLoci.wallDistance, methodOfLoci.torchHeight, 0),   // East
            new Vector3(-methodOfLoci.wallDistance, methodOfLoci.torchHeight, 0)   // West
        };

        foreach (Vector3 offset in torchOffsets)
        {
            Vector3 position = spawnCenter.TransformPoint(offset);
            GameObject torch = Instantiate(methodOfLoci.torchPrefab, position, Quaternion.identity);
            Light torchLight = torch.GetComponentInChildren<Light>();
            if (torchLight != null)
            {
                torchLight.color = roomTheme.torchColor;
            }
        }
    }
}
