using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;
    public TMP_Text statusText;
    [SerializeField]
    string roomJoin;

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

        UpdateStatus($"Creating room: {createInput.text}...");
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(joinInput.text))
        {
            UpdateStatus("Room name cannot be empty.");
            return;
        }

        UpdateStatus($"Joining room: {joinInput.text}...");
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        UpdateStatus($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
        PhotonNetwork.LoadLevel(roomJoin);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        UpdateStatus($"Failed to join room: {message}");
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
}
