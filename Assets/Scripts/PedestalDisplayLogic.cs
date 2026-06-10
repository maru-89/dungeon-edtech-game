using UnityEngine;

public class PedestalDisplayLogic : MonoBehaviour
{
    // Logic to add a bob and rotation effect to cosmetics on the pedestal displays
    [SerializeField] private float bobHeight = 0.25f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float rotationSpeed = 30f;

    private Vector3 startPos;
    private float bobOffset;
    private GameObject cosmetic;

    private void Start()
    {
        startPos = cosmetic.transform.position;
        bobOffset = Random.Range(0f, Mathf.PI * 2f);

        cosmetic.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        Debug.Log($"PedestalDisplayLogic Start: startPos={startPos}, bobOffset={bobOffset}");
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * bobSpeed + bobOffset) * bobHeight;

        Debug.Log($"PedestalDisplayLogic Update: time={Time.time}, yOffset={yOffset}");

        cosmetic.transform.position =  startPos + Vector3.up * yOffset;

        cosmetic.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void SetCosmetic(GameObject cosmeticObject)
    {
        cosmetic = cosmeticObject;
    }

}
