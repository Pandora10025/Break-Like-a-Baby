using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using Photon.Realtime;

public class Lobby : MonoBehaviourPunCallbacks
{
    public static Lobby Instance;

    public Dictionary<int, bool> playerReady = new Dictionary<int, bool>();
    public string gameSceneName;
    int readyCount, totalCount;
    [SerializeField] TextMeshProUGUI readyText;
    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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

    public void UpdateReadyDisplay()
    {
        if (readyText == null || Lobby.Instance == null)
            return;

        int readyCount = 0;
        int totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (Lobby.Instance.playerReady.TryGetValue(player.ActorNumber, out bool isReady) && isReady)
            {
                readyCount++;
            }
        }

        readyText.text = $"Players Ready: {readyCount} / {totalPlayers}";
    }

    public void SetReady()
    {
       
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        playerReady[actorNumber] = true;
        Debug.Log($"Player {actorNumber} set ready: {true}");
        UpdateReadyDisplay();
        // Only MasterClient should check and start the game
        if (PhotonNetwork.IsMasterClient && AllReady())
        {
            Debug.Log("All players ready. Starting game...");
            ResetReadyFlags();
            PhotonNetwork.LoadLevel(gameSceneName);
        }
    }

    private bool AllReady()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!playerReady.ContainsKey(player.ActorNumber) || !playerReady[player.ActorNumber])
                return false;
        }
        return true;
    }

    public void ResetReadyFlags()
    {
        playerReady.Clear();
    }
}