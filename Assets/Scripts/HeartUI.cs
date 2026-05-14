using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    // This script is responsible for updating the heart UI based on the player's health.
    [SerializeField] Sprite fullHeartSprite;
    [SerializeField] Sprite threeQuarterHeartSprite;
    [SerializeField] Sprite halfHeartSprite;
    [SerializeField] Sprite quarterHeartSprite;
    [SerializeField] Sprite emptyHeartSprite;
    private Image heartImage;

    void Awake()
    {
        heartImage = GetComponent<Image>();
    }

    public void SetHealth(int healthInThisHeart)
    {
        switch (healthInThisHeart)
        {
            case 4: heartImage.sprite = fullHeartSprite; break;
            case 3: heartImage.sprite = threeQuarterHeartSprite; break;
            case 2: heartImage.sprite = halfHeartSprite; break;
            case 1: heartImage.sprite = quarterHeartSprite; break;
            case 0: heartImage.sprite = emptyHeartSprite; break;
        }
    }
}
