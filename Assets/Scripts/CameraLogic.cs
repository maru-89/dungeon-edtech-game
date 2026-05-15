using System.Collections;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    [SerializeField] private Transform defaultAnchor;
    [SerializeField] private Transform inventoryAnchor;
    [SerializeField] private float duration = 0.5f;

    private bool isInventoryOpen = false;
    private bool isLerping = false;

    void LateUpdate()
    {
        if (!isInventoryOpen && !isLerping)
        {
            transform.position = defaultAnchor.position;
        }
    }

    public void ToggleCameraPosition()
    {
        if (isLerping) return;

        isInventoryOpen = !isInventoryOpen;
        Transform target = isInventoryOpen ? inventoryAnchor : defaultAnchor;
        StartCoroutine(LerpToPosition(target));
    }

    private IEnumerator LerpToPosition(Transform target)
    {
        isLerping = true;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, target.position, t);
            transform.rotation = Quaternion.Lerp(startRotation, target.rotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target.position;
        transform.rotation = target.rotation;
        isLerping = false;
    }
}