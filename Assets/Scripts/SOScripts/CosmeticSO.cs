using UnityEngine;

[CreateAssetMenu(fileName = "CosmeticSO", menuName = "Scriptable Objects/CosmeticSO")]
public class CosmeticSO : ScriptableObject
{
    public string cosmeticID; // Unique identifier for the cosmetic item
    public string displayName; // Name to display in the UI
    public int cost; // Cost of the cosmetic item in in-game currency
    public Sprite icon; // Icon to represent the cosmetic item in the UI
    public GameObject cosmeticPrefab; // Prefab to instantiate when the cosmetic is equipped
    public CosmeticSlot cosmeticSlot; // The slot this cosmetic item belongs to (Head - 0, Face - 1, Body - 2, Weapon - 3)
}
