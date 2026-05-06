using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerLogic : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction interactAction;
    private PotLogic carriedPot;

    private float pickupButtonHeldTime = 0f;
    private float throwHoldThreshold = 1f; // one second hold = throw

    private bool canInteract = true; // to prevent spamming interactions
    [SerializeField] private float interactCooldown = 0.2f; // cooldown time after an interaction
    

    [SerializeField] private Transform potCarryPoint; // the head point you already added
    [SerializeField] private float pickupRadius = 2f;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        interactAction = playerInput.actions["Player/Interact"];
    }


    void Update()
    {
        if (!canInteract) return; // block all interact input during cooldown

        if (carriedPot != null)
        {
            if (interactAction.IsPressed())
            {
                pickupButtonHeldTime += Time.deltaTime;
            }

            if (interactAction.WasReleasedThisFrame())
            {
                if (pickupButtonHeldTime >= throwHoldThreshold)
                {
                    Throw(transform);
                }
                else
                {
                    PutDown(transform);
                }
                pickupButtonHeldTime = 0f;
                StartCoroutine(InteractCooldown());
            }
        }
        else
        {
            if (interactAction.WasPressedThisFrame())
            {
                TryPickup();
                StartCoroutine(InteractCooldown());
            }
        }
    }

    IEnumerator InteractCooldown()
    {
        canInteract = false;
        yield return new WaitForSeconds(interactCooldown);
        canInteract = true;
    }

    void TryPickup()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRadius);
        PotLogic nearestPot = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            PotLogic pot = hit.GetComponent<PotLogic>();
            Debug.Log($"Found collider: {hit.name}, PotLogic: {(pot != null ? "Yes" : "No")}");
            if (pot != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPot = pot;
                }
            }
        }

        if (nearestPot != null)
        {
            nearestPot.OnPickup(potCarryPoint);
            carriedPot = nearestPot;
        }
    }

    void PutDown(Transform transform)
    {
        carriedPot.OnPutDown(transform);
        carriedPot = null;
    }

    void Throw(Transform transform)
    {
        Vector3 throwDirection = transform.forward;
        float throwForce = 8f;
        carriedPot.OnThrow(throwDirection * throwForce);
        carriedPot = null;
    }
}
