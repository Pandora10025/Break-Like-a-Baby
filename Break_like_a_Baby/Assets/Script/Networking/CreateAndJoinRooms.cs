using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;
    public TMP_InputField playerInput;
    public TMP_Text statusText;
    [SerializeField]
    string roomJoin;
    bool characterSet;
    [SerializeField]
    RectTransform highlight, buttonA, buttonB;

    void Start()
    {
        UpdateStatus("Waiting for input...");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(createInput.text))
        {
            UpdateStatus("Room name cannot be empty.");
            return;
        }

        string roomName = createInput.text.ToLower(); 
        UpdateStatus($"Creating room: {roomName}...");
        PhotonNetwork.CreateRoom(roomName);
    }

    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(joinInput.text))
        {
            UpdateStatus("Room name cannot be empty.");
            return;
        }

        string roomName = joinInput.text.ToLower();
        UpdateStatus($"Joining room: {roomName}...");
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        UpdateStatus($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
        setName();
        if (!characterSet)
        {
            SelectCharacter(0);
        }
        PhotonNetwork.LoadLevel(roomJoin);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (returnCode == ErrorCode.GameClosed)
        {
            UpdateStatus("Room is closed. Game has already started.");
        }
        else
        {
            UpdateStatus($"Failed to join room: {message}");
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        UpdateStatus($"Failed to create room: {message}");
    }

    public override void OnConnectedToMaster()
    {
        UpdateStatus("Connected to Photon server.");
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        UpdateStatus($"Disconnected: {cause}");
    }

    private void UpdateStatus(string message)
    {
        Debug.Log(message);
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    public void SelectCharacter(int characterID)
    {
        characterSet = true;
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
        playerProperties["CharacterID"] = characterID;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        Debug.Log($"CharacterID set to: {characterID} for {PhotonNetwork.LocalPlayer.NickName}");
    }

    public void characterA()
    {
        SelectCharacter(0);
        highlight.position = buttonA.position;
        
        
    }

    public void characterB()
    {
        SelectCharacter(1);
        highlight.position = buttonB.position;

    }

    public void setName()
    {
        if (string.IsNullOrEmpty(playerInput.text))
        {
            PhotonNetwork.NickName = "Player_" + Random.Range(1000, 9999);
        }
        else
        {
            PhotonNetwork.NickName = playerInput.text;
        }
    }

    public void exitG()
    {
        Application.Quit();
    }
}
