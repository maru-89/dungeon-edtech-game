using UnityEngine;
using TMPro;

public class CoinDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private PlayerWallet playerWallet;

    void Start()
    {
        playerWallet.OnCoinsChanged += UpdateDisplay;
        UpdateDisplay(playerWallet.GetCoinCount());
    }

    void UpdateDisplay(int coinCount)
    {
        coinText.text = coinCount.ToString();
    }

    void OnDestroy()
    {
        playerWallet.OnCoinsChanged -= UpdateDisplay;
    }
}