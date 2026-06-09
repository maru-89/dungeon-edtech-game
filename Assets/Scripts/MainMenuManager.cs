using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class MainMenuManager : MonoBehaviour
{
    // Logic for the main menu, such as starting the game, selecting curriculum pack, etc.
    // MainMenuScene will have a canvas, panel with background image, button for play and exit and a dropdown for curriculum pack selection. 

    [SerializeField] private List<CurriculumPackSO> availablePacks; // List of available curriculum packs to select from
    private int packIndex = 0; // Index to track the currently selected curriculum pack
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TMP_Dropdown curriculumDropdown; // Dropdown to select curriculum pack, options will be populated from availablePacks list

    private void Start()
    {
        curriculumDropdown.ClearOptions();
    
        List<string> options = new List<string>();
        foreach (CurriculumPackSO pack in availablePacks)
        {
            options.Add(pack.curriculumName); // Assuming CurriculumPackSO has a curriculumName field to display in the dropdown
        }
        
        curriculumDropdown.AddOptions(options);
        curriculumDropdown.onValueChanged.AddListener(OnCurriculumChanged);
        playButton.onClick.AddListener(OnPlayClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnPlayClicked()
    {
        StartGame(packIndex);
        SceneManager.LoadScene("DungeonScene");
    }

    private void OnCurriculumChanged(int newIndex)
    {
        packIndex = newIndex;
    }

    private void OnQuitClicked()
    {
        Application.Quit();
    }


    public void StartGame(int packIndex)
    {
        if (packIndex < 0 || packIndex >= availablePacks.Count)
        {
            Debug.LogError("Invalid pack index selected!");
            return;
        }
        if (PlayerPrefs.GetInt("HasPlayed", 0) == 0)
        {
            Debug.Log("First time playing - starting TutorialScene");
            PlayerPrefs.SetInt("HasPlayed", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("TutorialScene");
        }
        else
        {
            Debug.Log("Game starting with curriculum pack: " + availablePacks[packIndex].curriculumName);

            // Load the dungeon scene
            // Set the selected curriculum pack in the DungeonManager before loading the game scene
            GameConfig.ActivePack = availablePacks[packIndex]; // Assuming GameConfig is a static class to hold game-wide settings
            SceneManager.LoadScene("DungeonScene");
        }
    }  
}
