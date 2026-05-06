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
        // 1. Disable the controller so it stops fighting the transform
        controller.enabled = false;

        // 2. Move the player
        transform.position += offset;

        // 3. Re-enable the controller
        controller.enabled = true;
        
        Debug.Log("Teleported to: " + transform.position);
    }
}
