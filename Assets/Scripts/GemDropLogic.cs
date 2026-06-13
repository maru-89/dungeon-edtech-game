using UnityEngine;

public class GemDropLogic : ItemDropLogic
{
    private GemSO gemData;
    private Rigidbody rb;

    [SerializeField] private TMPro.TextMeshPro gemText; // assign in inspector, this will display the letter on the gem
    [SerializeField] private SpriteRenderer gemSpriteRenderer; // assign in inspector, this will display the sprite on the gem
    private float targetSize = 0.5f; // for sprite size on gems

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    public override void Initialise(ItemSO data)
    {
        Debug.Log($"Initialising GemDropLogic with gem: {data.name}");
        
        // Pattern matching: Checks if data is a GemSO, and if so, assigns it to 'gem'
        if (data is GemSO gem)
        {
            gemData = gem;

            if (gemText != null &&
                !string.IsNullOrEmpty(gemData.gemCharacter))
            {
                gemText.text = gemData.gemCharacter;
            }

            if (gemSpriteRenderer != null &&
                gemData.gemSprite != null)
            {
                gemSpriteRenderer.sprite = gemData.gemSprite;

                Vector2 size = gemSpriteRenderer.sprite.bounds.size;
                float largestDimension = Mathf.Max(size.x, size.y);

                float scale = targetSize / largestDimension;

                gemSpriteRenderer.transform.localScale = Vector3.one * scale;
            }
            else
            {
                Debug.LogError($"GemText UI component is missing on {gameObject.name}");
            }
        }
        else
        {
            // This will tell you exactly what incorrect data type is being passed in
            Debug.LogError($"Type mismatch! Expected GemSO, but received {data.GetType().Name} for item {data.name}");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (gemData == null)
        {
            Debug.LogError("Gem data not set on GemDropLogic!");
            return;
        } 
        
        Debug.Log($"Trigger entered by: {other.name} tag: {other.tag}");
        if (other.CompareTag("Player"))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
                if (playerInventory.AddItem(gemData))
                {
                    // For tutorial
                    TutorialDungeonManager tutorial = FindAnyObjectByType<TutorialDungeonManager>();
                    tutorial?.OnGemCollected();
                    Destroy(gameObject);
                }
            }
        } 
    }

    public GemSO GetGemData()
    {
        return gemData;
    }
}
