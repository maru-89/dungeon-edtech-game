using UnityEngine;

public class CoinDropLogic : ItemDropLogic
{
    public CoinSO coinData; // Reference to the Coin Scriptable Object
    private Rigidbody rb; // Rigidbody for physics interactions

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false; // Ensure the coin is affected by physics
    }

    public override void Initialise(ItemSO data)
    {
        coinData = data as CoinSO;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Assuming the player has a PlayerWallet component to manage collected coins
            PlayerWallet playerWallet = collision.GetComponent<PlayerWallet>();
            if (playerWallet != null)
            {
                playerWallet.AddCoins(coinData.value); // Add the coin's value to the player's wallet
                Destroy(gameObject); // Destroy the coin after collection
            }
        }
    }
}
