using System.Collections.Generic;
using UnityEngine;

public class DungeonDoorLogic : MonoBehaviour
{
    // This is the logic for the locked doors in the dungeon, which will require gems to open.
    // For now, we will just have a simple logic that checks if the player has the required gems to open the door, and if so, opens the door and allows the player to "win".

    private List<GemSO> requiredGems; // get from DungeonManager when the door is initialized

    [SerializeField] private UnityEngine.UI.Image wordDisplayImg; // assign in inspector, this will show the vocab word associated with the door
    [SerializeField] private GameObject gemSocketPrefab; // assign in inspector

    private List<GemSocketLogic> sockets = new List<GemSocketLogic>();

    
    [SerializeField] private float arcRadius = 1.5f;
    [SerializeField] private float arcAngle = 90f;
    [SerializeField] private Transform socketAnchor; // assign in inspector, this is the point around which the sockets will be arranged
    [SerializeField] private Transform cameraAnchor; // assign in inspector, this is the point from which the camera will look at the door when player approaches with gem
    [SerializeField] private Transform doorInteractionAnchor; // assign in inspector, this is the point to which the handUI will spawn to place gems

    private bool playerInRange = false; // Track if player is in range to interact with the door, to control camera lerping and hand UI
    public bool PlayerInRange => playerInRange;
    public void Initialise(CurriculumItemSO vocabWordSO)
    {
        requiredGems = new List<GemSO>(vocabWordSO.requiredGems);
        requiredGems.RemoveAll(g => g == null || string.IsNullOrEmpty(g.gemCharacter)); // remove any gems without a character
        Debug.Log($"Required vocab word: {vocabWordSO.displayWord}, gems: {vocabWordSO.requiredGems.Count}");
        foreach (GemSO gem in requiredGems)
        {
            if (gem != null)
            {
                Debug.Log($"Required gem for door: {gem.name}");
            }
            else
            {
                Debug.LogError("A required gem in the vocab word is null!");
            }
        }

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

            Debug.Log("Socket instantiated at position: " + socket.transform.position);

            GemSocketLogic socketLogic = socket.GetComponent<GemSocketLogic>();
            Debug.Log("SocketLogic found: " + (socketLogic != null));

            if (socketLogic != null)
            {
                Debug.Log($"Initializing socket {i} with gem: {requiredGems[i].name}");
                socketLogic.Initialize(requiredGems[i], this);
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
        GameManager.Instance.OnDungeonDoorOpened(); // Notify GameManager that the door has been opened (win condition)
    }

    void OnTriggerEnter(Collider other)
    {        
        Debug.Log($"Trigger entered by: {other.name} tag: {other.tag}");
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            InventoryLogic inventoryLogic = other.GetComponent<InventoryLogic>();
            if (inventoryLogic != null && inventoryLogic.GemHold.childCount > 0 && !inventoryLogic.IsInventoryOpen)
            {
                other.GetComponent<CharacterController>().enabled = false;
                other.GetComponent<PlayerMovement>().enabled = false;

                Camera.main.GetComponent<CameraLogic>().LerpToDoorPosition (cameraAnchor);
                inventoryLogic.OpenDoorInteraction(doorInteractionAnchor);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            other.GetComponent<CharacterController>().enabled = true;
            other.GetComponent<PlayerMovement>().enabled = true;

            Camera.main.GetComponent<CameraLogic>().ReturnToDefault();
            InventoryLogic inventoryLogic = other.GetComponent<InventoryLogic>();
            if (inventoryLogic != null)
            {
                inventoryLogic.CloseDoorInteraction();
            }
        }
    }

    public void TriggerDoorInteraction(InventoryLogic inventoryLogic)
    {
        Camera.main.GetComponent<CameraLogic>().LerpToDoorPosition(cameraAnchor);
        inventoryLogic.OpenDoorInteraction(doorInteractionAnchor);
    }
}
