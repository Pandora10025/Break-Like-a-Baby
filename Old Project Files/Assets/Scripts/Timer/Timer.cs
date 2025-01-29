using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Ensure TextMeshPro is in your project.

public class TimerManager : MonoBehaviour
{
    // Public variables to be set in the Unity editor
    public float countdown = 10f; // Countdown time in seconds
    public TextMeshProUGUI messageText; // Reference to the TextMeshPro UI element
    public GameObject gameOverScreen; // Game Over screen UI element
   // public GameObject itemsRemainingUI; // UI element showing items remaining
    public GameObject timerUI; // Timer UI element
    private AudioManager audioSearch;

    // Private variables to manage internal states
    public bool timesUp = false; // Tracks if time is up

    private void Start()
    {

        // Set initial UI states
        UpdateCountdownText();
        gameOverScreen.SetActive(false);
        //itemsRemainingUI.SetActive(true);
        timerUI.SetActive(true);


        audioSearch = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Update()
    {
        /*
        if (!timesUp)
        {
            // Count down the time
            countdown -= Time.deltaTime;

            if (countdown <= 0f)
            {
                // Time is up, freeze the scene
                timesUp = true;
                countdown = 0f;
                FreezeScene();
            }
            else
            {
                // Update countdown text
                UpdateCountdownText();
            }
        }
        else
        {
            // Allow the player to reload the scene by pressing the spacebar
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ReloadScene();
            }
        }
        */
    }


    // Update the countdown UI text to show remaining time
    public void UpdateCountdownText()
    {
        messageText.text = $"{Mathf.CeilToInt(countdown):00}";
    }

    // Freeze the scene and display the Game Over screen
    public void FreezeScene()
    {
        Time.timeScale = 0f; // Pause the game time
        messageText.text = ""; // Clear the countdown text
        gameOverScreen.SetActive(true); // Show Game Over screen
        //itemsRemainingUI.SetActive(false); // Hide remaining items UI
        timerUI.SetActive(false); // Hide timer UI

        AudioManager.instance.PauseMusic();
        AudioManager.instance.PlaySFX(audioSearch.gameOverSFX);
    }

    // Reload the scene and resume the game
    public void ReloadScene()
    {
        Time.timeScale = 1f; // Resume game time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }
}
