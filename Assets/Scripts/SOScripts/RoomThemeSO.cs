using UnityEngine;

[CreateAssetMenu(fileName = "RoomThemeSO", menuName = "Scriptable Objects/RoomThemeSO")]
public class RoomThemeSO : ScriptableObject
{
    // RoomThemeSO governs the visual theme of a room - landmark, landmark location, torch color, and other visual aspects
    public string roomThemeName; // Name of the room theme (e.g., "Ancient Ruins", "Mystic Forest", etc.)
    public GameObject landmarkPrefab; // Prefab for the room's landmark (e.g., a statue, fountain, etc.)
    public Color torchColor; // Color for the room's torches to create a specific ambiance
    public Sprite landmarkSprite; // Sprite for the room's landmark (e.g., a statue, fountain, etc.)

    // possible future additions: misc visual elements like banners, clutter, floor patterns, wall textures, etc. to further differentiate room themes

}
