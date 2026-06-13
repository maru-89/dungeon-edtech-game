using UnityEngine;
using System.Collections.Generic;

public class PlayerCosmeticLogic : MonoBehaviour
{
    [SerializeField] private List<Transform> attachPoints; // { Head - 0, Face - 1, Body - 2, Weapon - 3 }
    [SerializeField] private List<CosmeticSO> allCosmetics;

    private void Start()
    {
        if (string.IsNullOrEmpty(CosmeticInventory.EquippedCosmeticID)) return;
        
        CosmeticSO equipped = allCosmetics.Find(c => c.cosmeticID == CosmeticInventory.EquippedCosmeticID);
        if (equipped != null)
            EquipCosmetic(equipped);
    }
    
    public void EquipCosmetic(CosmeticSO cosmetic)
    {
        Transform attach = attachPoints[(int)cosmetic.cosmeticSlot];
        Instantiate(cosmetic.cosmeticPrefab, attach);
    }
}
