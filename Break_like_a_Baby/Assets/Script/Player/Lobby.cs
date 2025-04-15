using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using Photon.Realtime;

public class Lobby : MonoBehaviourPunCallbacks
{
    public static Lobby Instance;

    public string gameSceneName;
    [SerializeField] TextMeshProUGUI readyText;

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

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Ready"))
        {
            UpdateReadyDisplay();
            if (PhotonNetwork.IsMasterClient && AllReady())
            {
              
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
}