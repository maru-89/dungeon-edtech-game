using System.Collections.Generic;
using UnityEngine;

public class DungeonDoorLogic : MonoBehaviour
{
    // This is the logic for the locked doors in the dungeon, which will require gems to open.
    // For now, we will just have a simple logic that checks if the player has the required gems to open the door, and if so, opens the door and allows the player to "win".

    private List<ItemSO> requiredGems; // get from DungeonManager when the door is initialized

    [SerializeField] private UnityEngine.UI.Image wordDisplayImg; // assign in inspector, this will show the vocab word associated with the door
    [SerializeField] private GameObject gemSocketPrefab; // assign in inspector

    private List<GemSocketLogic> sockets = new List<GemSocketLogic>();

    
    [SerializeField] private float arcRadius = 1.5f;
    [SerializeField] private float arcAngle = 90f;
    [SerializeField] private Transform socketAnchor; // assign in inspector, this is the point around which the sockets will be arranged

    public void Initialise(VocabWordSO vocabWordSO)
    {
        requiredGems = new List<ItemSO>(vocabWordSO.requiredGems);
        wordDisplayImg.sprite = vocabWordSO.displayImage; // assign door image based on vocab word, for now just a placeholder

        int socketCount = requiredGems.Count;

        for (int i = 0; i < socketCount; i++)
        {
            float angle = socketCount > 1 ? (arcAngle / (socketCount - 1)) * i - (arcAngle / 2f) : 0f;
            Vector3 offset = Quaternion.AngleAxis(angle, socketAnchor.forward) * socketAnchor.up * arcRadius;

            GameObject socket = Instantiate(gemSocketPrefab,
                socketAnchor.position + offset,
                Quaternion.identity,
                socketAnchor);

            GemSocketLogic socketLogic = socket.GetComponent<GemSocketLogic>();
            if (socketLogic != null)
            {
                socketLogic.Initialize(requiredGems[i] as GemSO, this);
                sockets.Add(socketLogic);
            }
        }
    }

    public void OnSocketFilled()
    {
        foreach (GemSocketLogic socket in sockets)
        {
            if (!socket.IsFilled) return; // not all filled yet
        }
        
        // All filled
        OpenDoor();
    }

    void OpenDoor()
    {
        Debug.Log("All sockets filled, door unlocked!");
        transform.localScale = Vector3.zero;
    }
}
