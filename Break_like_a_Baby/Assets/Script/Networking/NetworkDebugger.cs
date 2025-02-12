using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class NetworkDebugger : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI debugText; // UI Text component to display the status

    private void Start()
    {
        debugText = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        if (debugText != null)
        {
            if (Input.GetKeyDown(KeyCode.I)) // Press F1 to toggle
            {
                debugText.gameObject.SetActive(!debugText.gameObject.activeSelf);
            }


            debugText.text =
                $"Photon Status: {PhotonNetwork.NetworkClientState}\n" +
                $"Ping: {PhotonNetwork.GetPing()} ms\n" +
                $"Players in Room: {PhotonNetwork.CurrentRoom?.PlayerCount ?? 0}\n" +
                $"Region: {PhotonNetwork.CloudRegion}\n" +
                $"Server: {PhotonNetwork.ServerAddress}";
        }
    }
}
