using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryLogic : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction inventoryAction;

    private Camera mainCamera;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        inventoryAction = playerInput.actions["Player/Inventory"];
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (inventoryAction.WasPressedThisFrame())
        {
            Debug.Log("Inventory button pressed, toggling camera position.");
            // This will trigger the camera to switch positions
            // The actual camera movement is handled in CameraLogic.cs
            mainCamera.GetComponent<CameraLogic>().ToggleCameraPosition();

        }
    }
}
