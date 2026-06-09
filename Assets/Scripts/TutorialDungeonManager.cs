using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialDungeonManager : MonoBehaviour, IDungeonManager
{
    public static TutorialDungeonManager Instance { get; private set; }
    public System.Random SeededRandom { get; private set; }

    [SerializeField] private float heartDropChance = 0.25f;
    [SerializeField] private float coinDropChance = 0.25f;
    [SerializeField] private ItemSO heartItem;
    [SerializeField] private ItemSO coinItem;

    [SerializeField] private CurriculumPackSO currentPack;

    [SerializeField] private DungeonDoorLogic dungeonDoor;

    [SerializeField] private TutorialArrowLogic tutorialArrow;
    [SerializeField] private List<Transform> nextTargetList;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CharacterController controller;
    [SerializeField] private CameraLogic cameraLogic;
    [SerializeField] private Transform inventoryZoomAnchor;
    [SerializeField] private GameObject tabKeyPromptUI;
    [SerializeField] private GameObject eKeyPromptUI;
    [SerializeField] public List<GemSO> allGems; // This will be set from the inspector with all gems available in the game, used for random drops after required gems are exhausted

    private bool teleported = false;

    private CurriculumItemSO selectedItem;
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

        if (nextTargetList != null && nextTargetList.Count > 0)
        {
            tutorialArrow.SetTarget(nextTargetList[0]);
        }

        if (dungeonDoor != null)
        {
            Debug.Log($"Initialising door with word: {selectedItem.displayWord}, gems: {selectedItem.requiredGems.Count}");
            dungeonDoor.Initialise(selectedItem);
        }
    }

    public void InitialiseDungeon()
    {
        selectedItem = currentPack.curriculumItemList[0]; // always first item for tutorial
        remainingRequiredGems = new List<GemSO>(selectedItem.requiredGems);
        Debug.Log($"Tutorial initialised, required gems: {remainingRequiredGems.Count}");
    }

    public GemSO GetRequiredDrop()
    {
        Debug.Log($"GetRequiredDrop called, remaining: {remainingRequiredGems.Count}");
        if (remainingRequiredGems.Count > 0)
        {
            GemSO gem = remainingRequiredGems[SeededRandom.Next(0, remainingRequiredGems.Count)];
            Debug.Log($"Dropping gem: {gem}");
            remainingRequiredGems.Remove(gem);
            return gem;
        }
        return null;
    }

    public ItemSO GetDrop(PotSO potData)
    {
        Debug.Log($"GetDrop called for pot: {potData.name}, remaining required gems: {remainingRequiredGems.Count}");
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
        return new List<ItemSO>(selectedItem.requiredGems);
    }

    public void OnPotBroken()
    {
        GemDropLogic gem = FindAnyObjectByType<GemDropLogic>();
        if (gem != null)
        {
            tutorialArrow.SetTarget(gem.transform);
            if (nextTargetList != null && nextTargetList.Count > 0)
            {            
                nextTargetList.RemoveAt(0);
                CheckForNextTarget();
            }
        }
    }

    public void OnGemCollected()
    {
        Debug.Log("Gem collected, remaining required gems: " + remainingRequiredGems.Count);
        if (nextTargetList != null && nextTargetList.Count > 0)
        {
            tutorialArrow.SetTarget(nextTargetList[0]);
            Debug.Log("Gem collected, moving arrow to next target: " + nextTargetList[0].name);
        }
        if (remainingRequiredGems.Count == 0)
        {
            StartCoroutine(InventoryPromptSequence());
        }
    }

    public void OnDoorTeleport()
    {
        if (nextTargetList != null && nextTargetList.Count > 0 && !teleported)
        {            
            nextTargetList.RemoveAt(0);
            tutorialArrow.SetTarget(nextTargetList[0]);
            teleported = true;

            CheckForNextTarget();
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void CheckForNextTarget()
    {
        if (nextTargetList == null || nextTargetList.Count == 0)
        {
            tutorialArrow.gameObject.SetActive(false); // Hide arrow if no more targets
        }
    }

    IEnumerator InventoryPromptSequence()
    {
        controller.enabled = false;
        playerMovement.enabled = false;
        
        cameraLogic.SetTutorialMode(true);
        StartCoroutine(cameraLogic.LerpToPosition(inventoryZoomAnchor));
        
        yield return new WaitForSeconds(0.5f);
    
        tabKeyPromptUI.SetActive(true);
    }

    public void OnInventoryOpened()
    {
        tabKeyPromptUI.SetActive(false);
        cameraLogic.SetTutorialMode(false);
    }

    public void ShowEKeyPrompt()
    {
        eKeyPromptUI.SetActive(true);
    }

    public void HideEKeyPrompt()
    {
        eKeyPromptUI.SetActive(false);
    }
}