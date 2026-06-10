using System;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public event Action<int> OnCoinsChanged;
    private int coinCount = 0;
    private const string CoinKey = "PlayerCoins";

    void Awake()
    {
        coinCount = PlayerPrefs.GetInt(CoinKey, 0);
    }

    public void AddCoins(int amount)
    {
        coinCount += amount;
        PlayerPrefs.SetInt(CoinKey, coinCount);
        PlayerPrefs.Save();
        OnCoinsChanged?.Invoke(coinCount);
        Debug.Log($"Added {amount} coins, total: {coinCount}");
    }

    public bool Spend(int amount)
    {
        if (coinCount < amount) return false;
        
        coinCount -= amount;
        PlayerPrefs.SetInt(CoinKey, coinCount);
        PlayerPrefs.Save();
        OnCoinsChanged?.Invoke(coinCount);
        Debug.Log($"Spent {amount} coins, total: {coinCount}");
        return true;
    }

    public int GetCoinCount()
    {
        return coinCount;
    }
}