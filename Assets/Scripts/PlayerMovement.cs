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
    private float baseSpeed; // set once, never modified by slow zones
    private bool isSlowed = false;

    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

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

    public void ApplySlow(float slowAmount)
    {
        if (isSlowed) return;
        isSlowed = true;
        baseSpeed = MoveSpeed;
        MoveSpeed *= slowAmount;
    }

    public void RemoveSlow()
    {
        if (!isSlowed) return;
        isSlowed = false;
        MoveSpeed = baseSpeed;
    }

    private void MovePlayer()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(direction.x, 0, direction.y);

        if (controller.enabled)
        {
            if (!controller.isGrounded)
            {
                velocity.y += Physics.gravity.y * Time.deltaTime;
            }
            else
            {
                velocity.y = -2f;
            }

            Vector3 finalMovement = (moveDirection * moveSpeed) + velocity;
            controller.Move(finalMovement * Time.deltaTime); // Move the player based on input and gravity

            if (moveDirection != Vector3.zero)
            {
                lastMoveDirection = moveDirection;
                Quaternion targetRotation = Quaternion.LookRotation(lastMoveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime); // Smoothly rotate towards movement direction
            }
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
