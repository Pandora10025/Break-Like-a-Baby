using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    bool gameOver=true;
    public string timerUItext;
    [SerializeField] float totalTime;
    [SerializeField] bool gameStarted;

    [SerializeField] TextMeshProUGUI timerText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;

        gameStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        int minutes = Mathf.FloorToInt(totalTime / 60);
        int seconds = Mathf.FloorToInt(totalTime % 60);
        if (gameStarted)
        {
            totalTime -= Time.deltaTime;
            
            if (totalTime <= 0)
            {
                totalTime = 0;
                GameOver(true);

            }
           
        }

        timerUItext = string.Format("{0:00}:{1:00}", minutes, seconds);

        timerText.text = timerUItext;
    }

    public void GameOver(bool timer)
    {
        gameOver = true;
    }

    public void ToggleText(bool b)
    {
        timerText.enabled = b;
    }






}
