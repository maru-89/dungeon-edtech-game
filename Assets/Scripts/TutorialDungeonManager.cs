using System.Collections.Generic;
using UnityEngine;

public class TutorialDungeonManager : MonoBehaviour, IDungeonManager
{
    public static TutorialDungeonManager Instance { get; private set; }
    public System.Random SeededRandom { get; private set; }

    [SerializeField] private float heartDropChance = 0.25f;
    [SerializeField] private float coinDropChance = 0.25f;
    [SerializeField] private ItemSO heartItem;
    [SerializeField] private ItemSO coinItem;

    [SerializeField] private LanguagePackSO currentPack;

    [SerializeField] private DungeonDoorLogic dungeonDoor;

    [SerializeField] private TutorialArrowLogic tutorialArrow;
    [SerializeField] private Transform potTarget;
    [SerializeField] private Transform doorTarget;

    private VocabWordSO selectedWord;
    private List<GemSO> remainingRequiredGems;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SeededRandom = new System.Random(1337); // fixed seed for consistent tutorial
    }

    void Start()
    {
        InitialiseDungeon();

        tutorialArrow.SetTarget(potTarget);
        
        if (dungeonDoor != null)
        {
            dungeonDoor.Initialise(selectedWord);
        }
    }

    public void InitialiseDungeon()
    {
        selectedWord = currentPack.vocabWordList[0]; // always first word for tutorial
        remainingRequiredGems = new List<GemSO>(selectedWord.requiredGems);
    }

    public GemSO GetRequiredDrop()
    {
        if (remainingRequiredGems.Count > 0)
        {
            GemSO gem = remainingRequiredGems[SeededRandom.Next(0, remainingRequiredGems.Count)];
            remainingRequiredGems.Remove(gem);
            return gem;
        }
        return null;
    }

    public ItemSO GetDrop(PotSO potData)
    {
        if (remainingRequiredGems.Count > 0)
        {
            return GetRequiredDrop();
        }

        if (SeededRandom.NextDouble() <= potData.dropChance)
        {
            if (SeededRandom.NextDouble() <= heartDropChance)
            {
                return heartItem;
            }
            if (SeededRandom.NextDouble() <= coinDropChance)
            {
                return coinItem;
            }
            return currentPack.fullGemList[SeededRandom.Next(0, currentPack.fullGemList.Count)];
        }

        return null;
    }

    public ItemSO GetEnemyDrop(float dropChance)
    {
        if (SeededRandom.NextDouble() <= dropChance)
        {
            return coinItem;
        }
        return null;
    }

    public List<ItemSO> GetRequiredGems()
    {
        return new List<ItemSO>(selectedWord.requiredGems);
    }

    public void OnPotBroken()
    {
        GemDropLogic gem = FindAnyObjectByType<GemDropLogic>();
        if (gem != null)
        {
            tutorialArrow.SetTarget(gem.transform);
        }
    }

    public void OnGemCollected()
    {
        tutorialArrow.SetTarget(doorTarget);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}