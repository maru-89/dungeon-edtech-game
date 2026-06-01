using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Define our move vector
        Vector3 teleportOffset = Vector3.zero;

        if (other.CompareTag("DoorTop")) teleportOffset = Vector3.forward * 10;
        else if (other.CompareTag("DoorBottom")) teleportOffset = Vector3.back * 10;
        else if (other.CompareTag("DoorLeft")) teleportOffset = Vector3.left * 10;
        else if (other.CompareTag("DoorRight")) teleportOffset = Vector3.right * 10;

        if (teleportOffset != Vector3.zero)
        {
            Teleport(teleportOffset);
        }
    }

    private void Teleport(Vector3 offset)
    {
        controller.enabled = false; // Disable the controller to stop fighting the teleport
        transform.position += offset; // Teleport
        controller.enabled = true; // Re-renable the controller

        // For tutorial
        TutorialDungeonManager tutorial = FindAnyObjectByType<TutorialDungeonManager>();
        tutorial?.OnDoorTeleport();
        
        Vector2Int newGridPos = DungeonManager.Instance.GetGridPositionFromWorld(transform.position);
        Debug.Log($"Teleported to grid position: {newGridPos}");
        MinimapManager.Instance.RevealRoom(newGridPos);
        
        Debug.Log("Teleported to: " + transform.position);
    }
}
