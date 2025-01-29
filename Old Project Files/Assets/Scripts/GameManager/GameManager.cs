using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    //public TextMeshProUGUI inactivatedCirclesText; // TextMeshPro component
    public int totalCircles = 6;
    public int activatedCircles = 0;

    public GameObject winScreen;
    public GameObject otherRemainingUI;

    private AudioManager audioSearch;

    private void Start()
    {
        winScreen.SetActive(false);
        audioSearch = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Update()
    {
        if (activatedCircles == totalCircles)
        {
            WinGame();

        }
    }
    public void UpdateActivationCount()
    {
        activatedCircles++;
        
    }


    

    public void WinGame()
    {
        winScreen.SetActive(true);
        otherRemainingUI.SetActive(false);
        Debug.Log("You win!");
        /*
        // Destroy the Timer GameObject
        GameObject timer = GameObject.Find("Timer");
        if (timer != null)
        {
            Destroy(timer);
        }
        else
        {
            Debug.LogWarning("Timer GameObject not found.");
        }
        */
        Time.timeScale = 0f;
        AudioManager.instance.PauseMusic();
        AudioManager.instance.PlaySFX(audioSearch.winSFX);

        // Additional win condition logic
    }
}
