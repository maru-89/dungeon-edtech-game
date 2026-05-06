using UnityEngine;
using System.Collections.Generic;

public class RoomSpawner : MonoBehaviour
{
    public int openingDirection;
    // 1 --> need bottom door
    // 2 --> need top door
    // 3 --> need left door
    // 4 --> need right door

    private RoomTemplates templates;
    private int rand;
    private bool spawned = false;

    public Vector2Int parentGridPosition;

    private float waitTime = 4f;

    void Start()
    {
        Destroy(gameObject, waitTime);
        templates = GameObject.FindGameObjectWithTag("Rooms")
            .GetComponent<RoomTemplates>();
        Invoke("Spawn", 0.1f);
    }

    Vector2Int GetNewGridPosition()
    {
        switch (openingDirection)
        {
            case 1: return parentGridPosition + Vector2Int.up;    // Z+ 
            case 2: return parentGridPosition + Vector2Int.down;  // Z-
            case 3: return parentGridPosition + Vector2Int.right; // X+
            case 4: return parentGridPosition + Vector2Int.left;  // X-
            default: return parentGridPosition;
        }
    }

    void Spawn()
    {
        if (!spawned)
        {
            Vector2Int newGridPosition = GetNewGridPosition();
            Debug.Log($"Spawner at world pos {transform.position} | openingDirection:{openingDirection} | parentGrid:{parentGridPosition} | newGridPos:{newGridPosition}");

            // Room already exists here, just destroy this spawner
            if (DungeonManager.dungeonMap.ContainsKey(newGridPosition))
            {
                spawned = true;
                return;
            }

            // Room limit reached
            if (DungeonManager.Instance.RoomCount >= DungeonManager.Instance.MaxRooms)
            {
                // TODO: delete this door
                spawned = true;
                return;
            }

            GameObject newRoom = null;

            if (openingDirection == 1)
            {
                rand = Random.Range(0, templates.bottomRooms.Length);
                newRoom = Instantiate(templates.bottomRooms[rand],
                    transform.position,
                    templates.bottomRooms[rand].transform.rotation);
            }
            else if (openingDirection == 2)
            {
                rand = Random.Range(0, templates.topRooms.Length);
                newRoom = Instantiate(templates.topRooms[rand],
                    transform.position,
                    templates.topRooms[rand].transform.rotation);
            }
            else if (openingDirection == 3)
            {
                rand = Random.Range(0, templates.leftRooms.Length);
                newRoom = Instantiate(templates.leftRooms[rand],
                    transform.position,
                    templates.leftRooms[rand].transform.rotation);
            }
            else if (openingDirection == 4)
            {
                rand = Random.Range(0, templates.rightRooms.Length);
                newRoom = Instantiate(templates.rightRooms[rand],
                    transform.position,
                    templates.rightRooms[rand].transform.rotation);
            }

            if (newRoom != null)
            {
                RoomController roomController = 
                    newRoom.GetComponent<RoomController>();
                    
                if (roomController != null)
                {
                    // Set grid position on the new room
                    roomController.gridPosition = newGridPosition;

                    // Pass parent grid position to all child spawners
                    RoomSpawner[] childSpawners = 
                        newRoom.GetComponentsInChildren<RoomSpawner>();
                    foreach (RoomSpawner spawner in childSpawners)
                    {
                        spawner.parentGridPosition = newGridPosition;
                    }

                    // Register in dictionary
                    DungeonManager.dungeonMap[newGridPosition] = roomController;
                    DungeonManager.Instance.IncrementRoomCount();
                }
            }

            spawned = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
            Destroy(gameObject);
            Debug.Log("Spawn point deleted at " + transform.position);
        }
    }
}