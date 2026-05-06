using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 10f, 0);

    void LateUpdate()
    {
        transform.position = target.position + offset;
        // No rotation update, camera stays fixed looking down
    }
}