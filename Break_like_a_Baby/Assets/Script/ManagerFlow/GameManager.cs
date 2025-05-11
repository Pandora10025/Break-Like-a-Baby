using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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

    [SerializeField] TextMeshProUGUI totalS;

    [SerializeField] GameObject chaseUI;
    public GameObject cribSymbol;

    public TextMeshProUGUI[] playerNames, itemsBrokenC, timeBonus, itemsBrokenList, breakoutBonus, timesCaught, brokenT;

    public CameraFollow cam;

    public Image[] scoreP;

    public Sprite[] playerP;

    public TextMeshProUGUI wLevel,lLevel;
    bool timerOn = true;

    bool tOn = false;
    public GameObject bT;

    float bTime = 5f;
    float bCount;
    public string[] bTi;
    int bTii;
    public GameObject sliderr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        chaseUI.SetActive(false);
        cribSymbol.SetActive(false);
        gameStarted = true;
        playerCount= GameObject.FindGameObjectsWithTag("Player").Length;
        
        for(int i = 0; i < 2; i++)
        {
            playerNames[i].text = "";
            itemsBrokenC[i].text = "";
            itemsBrokenList[i].text = "";
            breakoutBonus[i].text = "";
            timesCaught[i].text ="";
            scoreP[i].color = Color.clear;
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Difficulty", out object difficultyObj))
        {
            int difficulty = (int)difficultyObj;

            switch (difficulty)
            {
                case 0:
                    totalTime = 20;
                    timerOn = false;
                    timerText.gameObject.transform.parent.gameObject.SetActive(false);
                    tOn = true;
                    bT.SetActive(true);
                    bCount = bTime;
                    break;
                case 1:
                    Debug.Log("Normal Mode");
                    totalTime = 120;
                    timerOn = true;
                    tOn = false;
                    bT.SetActive(false);
                    break;
                case 2:
                    Debug.Log("Hard Mode");
                    totalTime = 120;
                    timerOn = true;
                    tOn = false;
                    bT.SetActive(false);
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (true)
        {
            if (gameStarted)
            {
                if(timerOn)
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

        if (tOn)
        {
            bCount += Time.deltaTime;
            if (bTime <= bCount)
            {
                bCount = 0;
                if (bTii < bTi.Length)
                    bT.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = bTi[bTii++];
                else bT.SetActive(false);
            }
        }

        timerUItext = string.Format("{0:00}:{1:00}", minutes, seconds);

        timerText.text = timerUItext;
    }

    public void GameOver(bool won)
    {
        if (gameOver)
            return;
        if (!PhotonNetwork.IsMasterClient) return;

        PlayerBreak[] allPbreaks = Object.FindObjectsOfType<PlayerBreak>();
        int len = Mathf.Min(2, allPbreaks.Length);

        int[] breakCounts = new int[len];
        int[] catchCounts = new int[len];
        int[] cribCounts = new int[len];
        string[] brokenLists = new string[len];
        string[] names = new string[len];
        int[] colorIds = new int[len];
        int[] viewIds = new int[len];

        for (int i = 0; i < len; i++)
        {
            breakCounts[i] = allPbreaks[i].breakCount;
            catchCounts[i] = allPbreaks[i].GetComponent<PlayerCatching>().catchCount;
            cribCounts[i] = allPbreaks[i].cribCount;
            brokenLists[i] = allPbreaks[i].brokenList;
            names[i] = allPbreaks[i].photonView.Owner.NickName;
            colorIds[i] = allPbreaks[i].GetComponent<PlayerControllerr>().colorId;
            viewIds[i] = allPbreaks[i].viewId;
        }

        photonView.RPC("GameOverRPC", RpcTarget.All, won, breakCounts, catchCounts, cribCounts, brokenLists, names, colorIds, viewIds, timerUItext, (int)totalTime);
       

    }
    public void toggleGOverlay(bool t)
    {


        gOverlay.SetActive(t);
        
    }

    public void enableChaseUI(bool on,int actorN)
    {
        photonView.RPC("enableChaseui", RpcTarget.AllBuffered, on,actorN);
        
        
    }
    [PunRPC]
    void enableChaseui(bool on , int actorN)
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber==actorN)
        chaseUI.SetActive(on);
    }

  
    [PunRPC]
    void GameOverRPC(bool won, int[] breakCounts, int[] catchCounts, int[] cribCounts, string[] brokenLists, string[] names, int[] colorIds, int[] viewIds, string timerU, int t)
    {
        gameOver = true;
        gameOverScreen.SetActive(true);

        int score = 0;

        for (int i = 0; i < breakCounts.Length; i++)
        {
            playerNames[i].text = names[i];
            itemsBrokenC[i].text = breakCounts[i].ToString();
            itemsBrokenList[i].text = brokenLists[i].Substring(0, Mathf.Max(brokenLists[i].Length - 2, 0));
            breakoutBonus[i].text = cribCounts[i].ToString();
            timesCaught[i].text = catchCounts[i].ToString();
            scoreP[i].sprite = playerP[colorIds[i]];
            scoreP[i].color = Color.white;

            score += breakCounts[i] * 10 - (catchCounts[i] * 10) + (cribCounts[i] * 20) + (int)(t * (won ? 1 : -1));
        }

        string diff = "";
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Difficulty", out object difficultyObj))
        {
            int difficulty = (int)difficultyObj;
            diff = difficulty switch
            {
                0 => "Easy",
                1 => "Normal",
                2 => "Cry",
                _ => "Unknown"
            };
        }

        diff += " ";
        wLevel.text = diff + wLevel.text;
        lLevel.text = diff + lLevel.text;

        timeBonus[0].text = "Time Bonus: " + "[" + timerU + "]";
        timeBonus[0].color = won ? brokenT[0].color : timesCaught[0].color;

        brokenT[0].text = "Items Broken: " + (ObjectManager.instance.numOfStartObjects - ObjectManager.instance.numOfActiveObjects) + "/" + ObjectManager.instance.numOfStartObjects;
        totalS.text = "Total Score:\t" + score;

        if (won)
            gameOverScreen.GetComponent<GameOver>().gameWon();
        else
            gameOverScreen.GetComponent<GameOver>().gameLost();

        Debug.Log("Game Over RPC executed: " + won);
    }

    public void ToggleText(bool b)
    {
        timerText.enabled = b;
    }

    public void toggleGOv()
    {
        
            toggleGOverlay(!gOverlay.activeInHierarchy);
        
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
