using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviourPunCallbacks
{
    public static Lobby Instance;

    public string gameSceneName;
    [SerializeField] TextMeshProUGUI readyText;

    public GameObject bSet;
    public RectTransform[] b,b1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            bSet.SetActive(false);
        }
        else
        {
            SetDifficulty(1);
        }
        //ClearReadyFlag();
        UpdateReadyDisplay();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateReadyDisplay();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateReadyDisplay();
    }
    public override void OnJoinedRoom()
    {
        ClearReadyFlag(); 
        UpdateReadyDisplay();
    }

    public void SetDifficulty(int level)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        b[0].position = b1[level].position;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
    {
        { "Difficulty", level }
    };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        Debug.Log("Difficulty set to: " + level);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Ready"))
        {
            UpdateReadyDisplay();
            if (PhotonNetwork.IsMasterClient && AllReady())
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                photonView.RPC("clearAllFlags", RpcTarget.All);
                PhotonNetwork.LoadLevel(gameSceneName);
            }
        }
    }

    public void UpdateReadyDisplay()
    {
        if (readyText == null)
            return;

        int readyCount = 0;
        int totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("Ready", out object isReady) && (bool)isReady)
            {
                readyCount++;
            }
        }

        readyText.text = $"Players Ready: {readyCount} / {totalPlayers}";
    }

    public void SetReady()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "Ready", true }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        Debug.Log($"Player {PhotonNetwork.LocalPlayer.ActorNumber} is ready!");
    }
    public void exitG()
    {
        photonView.RPC("clearAllFlags", RpcTarget.All);
        if (PhotonNetwork.IsMasterClient)
        {
            LeaveGameForAll();
        }
        else
        {
            ReturnToLobby();
        }
      
    }
    public void ReturnToLobby()
    { 

        PhotonNetwork.LeaveRoom();
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

    public override void OnLeftRoom()
    {

        SceneManager.LoadScene("Lobby");
    }
    private bool AllReady()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.TryGetValue("Ready", out object isReady) || !(bool)isReady)
            {
                return false;
            }
        }
        return true;
    }
    public void ClearReadyFlag()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["Ready"] = null; // Removing the key
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        UpdateReadyDisplay();

    }
    [PunRPC]
    void clearAllFlags()
    {
        ClearReadyFlag();
    }
}