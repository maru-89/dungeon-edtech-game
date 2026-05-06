using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    private PlayerInput playerInput;
    private InputAction moveAction;
    private CharacterController controller;

    private Vector3 velocity;

    private Vector3 lastMoveDirection = Vector3.forward;


    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Player/Move"];
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(direction.x, 0, direction.y);

        if (!controller.isGrounded)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            velocity.y = -2f;
        }

        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);

        if (moveDirection != Vector3.zero)
        {
            lastMoveDirection = moveDirection;
            transform.rotation = Quaternion.LookRotation(lastMoveDirection);
        }
    }

    // Push rigidbodies on collision, but only from the sides, not from above
    [Header("Push Settings")]
    [SerializeField] private float pushForce = 3f;
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        
        if (rb == null || rb.isKinematic) return;
        
        // Only push objects on the sides, not below
        if (hit.moveDirection.y < -0.3f) return;
        
        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
    }

}
