using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton GameManager instance to manage game-wide states - win / lose conditions, player stats, etc.
    public static GameManager Instance { get; private set; }

    [SerializeField] private Canvas gameStateCanvas; // Reference to the canvas that displays win/lose messages
    [SerializeField] private TMPro.TextMeshProUGUI gameStateText; // Reference to the text element for displaying win/lose messages
    [SerializeField] private UnityEngine.UI.Image fadePanel; // Reference to an image used for fade in/out effects (assign a black image in the inspector)
    [SerializeField] private float fadeDuration = 1f; // Duration for fade in/out effects
    [SerializeField] private float displayDuration = 3f; // Duration to display win/lose message before reloading scene

    private bool isGameOver = false; // Flag to prevent multiple triggers of win/lose conditions

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnDungeonDoorOpened()
    {
        if (isGameOver) return;
        isGameOver = true;
        StartCoroutine(GameEndSequence("You Win!"));
    }

    public void OnPlayerDeath()
    {
        if (isGameOver) return;
        isGameOver = true;
        StartCoroutine(GameEndSequence("You Lose!"));
    }

    private IEnumerator GameEndSequence(string message)
    {
        gameStateCanvas.gameObject.SetActive(true);
        gameStateText.text = message;

        // Fade in
        float elapsed = 0f;
        Color colour = fadePanel.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            colour.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.color = colour;
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
