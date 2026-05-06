using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public int itemID;
    public Sprite itemIcon;
    public GameObject itemPrefab;
}

[CreateAssetMenu(fileName = "HeartSO", menuName = "Scriptable Objects/HeartSO")]
public class HeartSO : ItemSO
{
    public int healAmount = 25;
}

[CreateAssetMenu(fileName = "GemSO", menuName = "Scriptable Objects/GemSO")]
public class GemSO : ItemSO
{
    public string gemCharacter;
}