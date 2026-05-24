using System.Collections.Generic;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    public static MinimapManager Instance { get; private set; }

    [SerializeField] private GameObject overlayPrefab; // black plane prefab
    [SerializeField] private float overlayHeight = 20f; // height above rooms on minimap camera layer
    [SerializeField] private float overlayScale = 100f; // should match room size
    [SerializeField] private Color visitedRoomColor = new Color(0.3f, 0.3f, 0.3f);

    public float GetOverlayScale() => overlayScale;

    private Dictionary<Vector2Int, GameObject> minimapOverlays = new Dictionary<Vector2Int, GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void GenerateOverlays()
    {
        Debug.Log($"GenerateOverlays called, dungeonMap has {DungeonManager.dungeonMap.Count} rooms");

        foreach (var kvp in DungeonManager.dungeonMap)
        {
            Vector2Int gridPos = kvp.Key;
            Vector3 worldPos = new Vector3(gridPos.x * overlayScale, overlayHeight, gridPos.y * overlayScale);
            GameObject overlay = Instantiate(overlayPrefab, worldPos, Quaternion.identity);
            minimapOverlays[gridPos] = overlay;
        }
        Debug.Log($"Generated {minimapOverlays.Count} overlays");
    }

    public void RevealRoom(Vector2Int gridPosition)
    {
        Debug.Log($"RevealRoom called for {gridPosition}, overlays count: {minimapOverlays.Count}");
        if (minimapOverlays.TryGetValue(gridPosition, out GameObject overlay))
        {
            Debug.Log($"Found overlay, changing color");
            overlay.GetComponent<Renderer>().material.color = visitedRoomColor;
            //Destroy(overlay);
            //minimapOverlays.Remove(gridPosition);
        }
        else
        {
            Debug.Log($"No overlay found for {gridPosition}");
        }
    }
}