using System.Collections.Generic;
using UnityEngine;

public class DungeonDoorLogic : MonoBehaviour
{
    // This is the logic for the locked doors in the dungeon, which will require gems to open.
    // For now, we will just have a simple logic that checks if the player has the required gems to open the door, and if so, opens the door and allows the player to "win".

    private List<ItemSO> requiredGems; // get from DungeonManager when the door is initialized

    [SerializeField] private GameObject gemSocketPrefab;
    private List<GemSocketLogic> sockets = new List<GemSocketLogic>();

    public void Initialise(List<ItemSO> gems)
    {
        int socketCount = gems.Count;
        float totalWidth = (socketCount - 1) * socketSpacing;
        float startOffset = -totalWidth / 2f; // center the sockets on the door

        for (int i = 0; i < socketCount; i++)
        {
            float xOffset = startOffset + (i * socketSpacing);
            Vector3 offset = transform.right * xOffset;
            
            GameObject socket = Instantiate(gemSocketPrefab,
                transform.position + offset,
                Quaternion.identity,
                transform);

            GemSocketLogic socketLogic = socket.GetComponent<GemSocketLogic>();
            if (socketLogic != null)
            {
                socketLogic.Initialize(gems[i] as GemSO);
                sockets.Add(socketLogic);
            }
        }
    }

    [SerializeField] private float socketSpacing = 2f; // adjustable in inspector
    }
