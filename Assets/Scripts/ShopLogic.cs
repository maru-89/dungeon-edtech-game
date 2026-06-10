using UnityEngine;
using System.Collections.Generic;

public class ShopLogic : MonoBehaviour
{
    [SerializeField] private List<CosmeticSO> availableCosmetics;
    [SerializeField] private List<Transform> pedestalTransforms;
    [SerializeField] private GameObject pedestalPrefab;
    [SerializeField] private float pedestalHeight = 1f;
    private List<ShopItemLogic> shopItems = new List<ShopItemLogic>();
    private PlayerWallet playerWallet;
    private PlayerCosmeticLogic playerCosmeticLogic;

    private void Start()
    {
        playerWallet = FindAnyObjectByType<PlayerWallet>();
        playerCosmeticLogic = FindAnyObjectByType<PlayerCosmeticLogic>();
        CosmeticInventory.Load();
        SpawnShopItems();
    }

    private void SpawnShopItems()
    {
        Debug.Log($"Spawning {availableCosmetics.Count} shop items");
        for (int i = 0; i < availableCosmetics.Count; i++)
        {
            Debug.Log($"Spawning item {i}: {availableCosmetics[i].displayName}");
            if (i >= pedestalTransforms.Count) break;

            GameObject pedestal = Instantiate(pedestalPrefab, pedestalTransforms[i].position, Quaternion.identity);
            GameObject item = Instantiate(availableCosmetics[i].cosmeticPrefab, pedestal.transform); 
            item.transform.localPosition = Vector3.up * pedestalHeight; // offset item upward so it sits on top of pedestal
            ShopItemLogic itemLogic = item.GetComponent<ShopItemLogic>();
            itemLogic.index = i;
            itemLogic.Initialise(availableCosmetics[i]);
            shopItems.Add(itemLogic);

            if (CosmeticInventory.UnlockedCosmeticIDs.Contains(availableCosmetics[i].cosmeticID))
                shopItems[i].SetSoldState();
        }
    }

    public void Purchase(int index)
    {
        CosmeticSO cosmetic = availableCosmetics[index];

        if (CosmeticInventory.UnlockedCosmeticIDs.Contains(cosmetic.cosmeticID))
            return;

        if (!playerWallet.Spend(cosmetic.cost))
            return;

        CosmeticInventory.Unlock(cosmetic.cosmeticID);
        CosmeticInventory.Equip(cosmetic.cosmeticID);
        playerCosmeticLogic.EquipCosmetic(cosmetic);
        shopItems[index].SetSoldState();
    }
}