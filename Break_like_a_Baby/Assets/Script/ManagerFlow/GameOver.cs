using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameOver : MonoBehaviourPunCallbacks
{
    [SerializeField] string GameRoom = "Arnav_Implement";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameRestart()
    {


        if (PhotonNetwork.IsMasterClient)
        {
         
            
                PhotonNetwork.LoadLevel(GameRoom);
            
            

        }
    }
}
