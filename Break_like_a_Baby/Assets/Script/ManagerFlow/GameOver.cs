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
    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            setButton.GetComponent<Button>().enabled = false;
            setButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = waitingForMaster;

        }
        wonOverlay.SetActive(false);
        lossOverlay.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GameSet(bool won)
    {
        if (won)
        {
            wonOverlay.SetActive(true);
        }
        else
        {
            lossOverlay.SetActive(true);
        }
    }
    public void GameRestart()
    {
        
      photonView.RPC("RequestRestart", RpcTarget.All);
     

    }

    [PunRPC]
    void RequestRestart()
    {
        
     PhotonNetwork.LoadLevel(GameRoom);
      
    }
}
