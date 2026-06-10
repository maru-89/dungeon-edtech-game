using UnityEngine;

public class ShopItemLogic : MonoBehaviour
{
    [SerializeField] private Renderer cosmeticRenderer;
    [SerializeField] private Collider interactCollider;

    private CosmeticSO cosmetic;
    public bool IsSold { get; private set; }
    public int index;

    public void Initialise(CosmeticSO cosmeticData)
    {
        cosmetic = cosmeticData;
        IsSold = false;
    }

    public void SetSoldState()
    {
        IsSold = true;
        interactCollider.enabled = false;

        Color c = cosmeticRenderer.material.color;
        c.a = 0.4f;
        cosmeticRenderer.material.color = c;
    }

    public void SetAvailableState()
    {
        IsSold = false;
        interactCollider.enabled = true;

        Color c = cosmeticRenderer.material.color;
        c.a = 1f;
        cosmeticRenderer.material.color = c;
    }
}