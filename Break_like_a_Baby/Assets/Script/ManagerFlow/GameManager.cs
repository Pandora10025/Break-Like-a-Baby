using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourPunCallbacks

{
    public static GameManager instance { get; private set; }

    bool gameOver=true;
    public string timerUItext;
    [SerializeField] float totalTime;
    [SerializeField] bool gameStarted;

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject gameOverScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        instance = this;

        gameStarted = true;
        gameOverScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (true)
        {
            if (gameStarted)
            {
                totalTime -= Time.deltaTime;

                if (totalTime <= 0)
                {
                    totalTime = 0;
                    GameOver(false);

                }

            }
        }
        int minutes = Mathf.FloorToInt(totalTime / 60);
        int seconds = Mathf.FloorToInt(totalTime % 60);

       

        timerUItext = string.Format("{0:00}:{1:00}", minutes, seconds);

        timerText.text = timerUItext;
    }

    public void GameOver(bool won)
    {
        photonView.RPC("GameOverRPC", RpcTarget.AllBuffered, won);
    }

    [PunRPC]
    void GameOverRPC(bool won)
    {
        gameOver = true;
        gameOverScreen.SetActive(true);
        gameOverScreen.GetComponent<GameOver>().GameSet(won);
    }

    public void ToggleText(bool b)
    {
        timerText.enabled = b;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncTimer", newPlayer, totalTime);
        }
    }

   
    [PunRPC]
    void SyncTimer(float time)
    {
        totalTime = time;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(totalTime);
           
        }
        else
        {
            totalTime = (float)stream.ReceiveNext();
            Debug.Log("a");
        }
    }

public void restartLevel()
    { 
        photonView.RPC("RequestRestart", RpcTarget.All);
    }
   
     

    

[PunRPC]
void RequestRestart()
{

    PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name);

}


}
