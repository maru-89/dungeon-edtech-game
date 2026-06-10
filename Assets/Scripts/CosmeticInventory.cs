using UnityEngine;
using System.Collections.Generic;

public static class CosmeticInventory
{
    public static HashSet<string> UnlockedCosmeticIDs = new HashSet<string>();
    public static string EquippedCosmeticID = "";

    public static void Unlock(string id)
    {
        UnlockedCosmeticIDs.Add(id);
        Save();
    }

    public static void Save()
    {
        PlayerPrefs.SetString("UnlockedCosmetics", string.Join(",", UnlockedCosmeticIDs));
        PlayerPrefs.Save();
    }

    public static void Load()
    {
        string saved = PlayerPrefs.GetString("UnlockedCosmetics", "");
        if (!string.IsNullOrEmpty(saved))
            UnlockedCosmeticIDs = new HashSet<string>(saved.Split(','));
        
        EquippedCosmeticID = PlayerPrefs.GetString("EquippedCosmetic", "");
    }

    public static void Equip(string id)
    {
        EquippedCosmeticID = id;
        PlayerPrefs.SetString("EquippedCosmetic", id);
        PlayerPrefs.Save();
    }

    public static void UnEquip(string id)
    {
        EquippedCosmeticID = "";
        PlayerPrefs.SetString("EquippedCosmetic", "");
        PlayerPrefs.Save();
    }
}
