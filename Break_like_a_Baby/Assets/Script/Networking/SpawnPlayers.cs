using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public Transform[] spawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerspawned =PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition[0].position, Quaternion.identity);
        playerspawned.transform.Rotate(90, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
