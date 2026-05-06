using UnityEngine;

public class RoomController : MonoBehaviour
{
    public Vector2Int gridPosition;

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
}
