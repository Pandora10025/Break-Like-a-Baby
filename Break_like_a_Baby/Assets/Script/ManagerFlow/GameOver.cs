using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class GameOver : MonoBehaviourPunCallbacks
{
    [SerializeField] string GameRoom = "Arnav_Implement";
    [SerializeField] GameObject wonOverlay,lossOverlay, setButton;
    [SerializeField] string waitingForMaster = "Waiting for host to restart!";


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            setButton.GetComponent<Button>().enabled = false;
            setButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = waitingForMaster;

        }
        wonOverlay.SetActive(false);
        lossOverlay.SetActive(false);

        gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameSet(bool won)
    {
        //photonView.RPC("gameSetRPC", RpcTarget.All, won);
        gameSetRPC(won);
    }

    //[PunRPC]
    void gameSetRPC(bool won)
    {
        
        if (won)
        {
            wonOverlay.SetActive(true);
            Debug.Log("won");
        }
        else
        {
            lossOverlay.SetActive(true);
        }
    }

   public void gameWon()
    {
        wonOverlay.SetActive(true);
        Debug.Log("won");
    }
    public void gameLost()
    {
        lossOverlay.SetActive(true);
        Debug.Log("lost");
    }
    public void GameRestart()
    {
        GameManager.instance.restartLevel();
    }
}
