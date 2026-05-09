using UnityEngine;
using System.Collections.Generic;

public class RoomController : MonoBehaviour
{
    public Vector2Int gridPosition;
    [SerializeField] private Transform spawnCenter;    
    private int[] oddOffsets = { -45, -30, -15, -3, 3, 15, 30, 45 };
    private int[] evenOffsets = { -40, -20, 0, 20, 40 };

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
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>(); // to track used grid positions for spawning and checking overlaps

        // Spawn pots
        int potCount = Random.Range(minPots, maxPots + 1);
        int potsSpawned = 0;
        int attempts = 0; // safety to prevent infinite loops
        
        while (potsSpawned < potCount && attempts < 20)
        {
            attempts++;
            
            int xOffset = oddOffsets[Random.Range(0, oddOffsets.Length)];
            int zOffset = oddOffsets[Random.Range(0, oddOffsets.Length)];
            Vector2Int gridPos = new Vector2Int(xOffset, zOffset);
            
            if (usedPositions.Contains(gridPos)) continue;
            
            usedPositions.Add(gridPos);
            Vector3 position = spawnCenter.position + new Vector3(xOffset, 0, zOffset);
            Instantiate(potPrefab, position, Quaternion.identity);
            Debug.Log($"Spawning contents in room: {gameObject.name} at {gridPosition}");
            potsSpawned++;
        }

        // Spawn enemies
        int enemyCount = Random.Range(1, 3);
        int enemiesSpawned = 0;
        attempts = 0;
        
        while (enemiesSpawned < enemyCount && attempts < 20)
        {
            attempts++;
            
            int xOffset = evenOffsets[Random.Range(0, evenOffsets.Length)];
            int zOffset = evenOffsets[Random.Range(0, evenOffsets.Length)];
            Vector2Int gridPos = new Vector2Int(xOffset, zOffset);
            
            if (usedPositions.Contains(gridPos)) continue;
            
            usedPositions.Add(gridPos);
            Vector3 position = spawnCenter.position + new Vector3(xOffset, 0, zOffset);
            //Instantiate(enemyPrefab, position, Quaternion.identity);
            enemiesSpawned++;
        }
    }
}
