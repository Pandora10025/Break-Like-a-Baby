using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourPunCallbacks

{
    public static GameManager instance { get; private set; }

    bool gameOver=false;
    public string timerUItext;
    [SerializeField] float totalTime;
    [SerializeField] bool gameStarted;

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject gameOverScreen;

    public Transform respawnPos;
    int roomedCount;
    public int playerCount;

   
    public Color[] playerColors;

    public GameObject babySitter;

    public Crib crib;

    public GameObject playerCaught;

    [SerializeField] CaughtPlayerOverlay caughtOverlay;

    [SerializeField] GameObject gOverlay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
       

        gameStarted = true;
        playerCount= GameObject.FindGameObjectsWithTag("Player").Length;
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
        if(!gameOver)
        photonView.RPC("GameOverRPC", RpcTarget.AllBuffered, won);
    }
    public void toggleGOverlay(bool t)
    {


        gOverlay.SetActive(t);
        
    }
    [PunRPC]
    void GameOverRPC(bool won)
    {
        gameOver = true;
        gameOverScreen.SetActive(true);
        PlayerBreak[] allPbreaks = Object.FindObjectsOfType<PlayerBreak>();
        string stats = "";

        for (int i = 0; i < allPbreaks.Length; i++)
        {

            PhotonView playerPhotonView = PhotonView.Find(allPbreaks[i].viewId);


            stats= stats + playerPhotonView.Owner.NickName+ " Broke " + allPbreaks[i].breakCount +" items: " + allPbreaks[i].brokenList +"\n";
        }

        gameOverScreen.GetComponent<GameOver>().setScore(stats);
        //gameOverScreen.GetComponent<GameOver>().GameSet(won);
        if (won)
        {
            gameOverScreen.GetComponent<GameOver>().gameWon();
        }
        else
        {
            gameOverScreen.GetComponent<GameOver>().gameLost();
        }
        Debug.Log("Game Over RPC: " + won);
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
   
     
    public void caughtPlayerOverlay(int pvID)
    {
        photonView.RPC("caughtPlayerO", RpcTarget.All,pvID);
    }

    [PunRPC]
    void caughtPlayerO(int pvID)
    {
        PhotonView playerPhotonView = PhotonView.Find(pvID);
        if (playerPhotonView)
        {
            caughtOverlay.overlayOn(5f, playerPhotonView.Owner.NickName + " has been caught!");
           
        }
       
    }

[PunRPC]
void RequestRestart()
{

    PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name);

}

    public void playerRoomed()
    {
        roomedCount++;
    }

    public void ReturnToLobby()
    {
        // Close the room if you're the Master Client
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }

        
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        
        SceneManager.LoadScene("Lobby"); 
    }

    public void LeaveGameForAll()
    {
        photonView.RPC("LeaveRoomRPC", RpcTarget.All);
    }

    [PunRPC]
    void LeaveRoomRPC()
    {
        ReturnToLobby();
    }
}
