using System.Collections;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    [SerializeField] private Transform defaultAnchor;
    [SerializeField] private Transform inventoryAnchor;
    [SerializeField] private float duration = 0.5f;

    private bool isInventoryOpen = false;
    private bool isLerping = false;
    private bool isDoorMode = false;

    void LateUpdate()
    {
        if (!isInventoryOpen && !isLerping && !isDoorMode)
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

    public IEnumerator LerpToPosition(Transform target)
    {
        Debug.Log("Starting camera lerp to " + target.name);
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

    public bool IsLerping()
    {
        return isLerping;
    }

    public void LerpToDoorPosition(Transform anchor)
    {
        isDoorMode = true;
        StartCoroutine(LerpToPosition(anchor));
    }

    public void ReturnToDefault()
    {
        isDoorMode = false;
        StartCoroutine(LerpToPosition(defaultAnchor));
    }
}