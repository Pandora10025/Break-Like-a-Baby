using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using System;
using UnityEditor;

public class Crib : MonoBehaviourPunCallbacks
{
    [SerializeField] Sprite[] babySleeping;
    [SerializeField] GameObject[] babys;
    BreakableObject breakable;
    int babyBeddedCount;
    [SerializeField] float respawnTime;
    public Transform placePos;
    void Awake()
    {

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.crib = this;
        breakable = GetComponent<BreakableObject>();
        placePos = breakable.explosionPosition;
        foreach(GameObject baby in babys)
        {
            baby.SetActive(false);
        }

        breakable.Inactive();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void babyBedded(int colorID)
    {

        photonView.RPC("babyBed", RpcTarget.All, colorID);
    }

    public void Break()
    {
        photonView.RPC("RPC_BreakCrib", RpcTarget.All);
    }

    [PunRPC]
    void RPC_BreakCrib()
    {
        // now this runs on ALL clients!
        babyBeddedCount = Mathf.Max(0, babyBeddedCount - 1);

        foreach (GameObject baby in babys)
        {
            baby.SetActive(false);
        }

        if (GameManager.instance.playerCaught != null)
        {
            GameManager.instance.playerCaught.GetComponent<PlayerCatching>().changeState(PlayerCatching.playerCatchState.free);
        }
    }

    [PunRPC]
    public void babyBed(int colorID)
    {
        breakable.resetHealth();
        breakable.Active();
        babys[babyBeddedCount].SetActive(true);
        babys[babyBeddedCount].GetComponent<SpriteRenderer>().sprite = babySleeping[colorID];
        babyBeddedCount++;
        if (PhotonNetwork.IsMasterClient && babyBeddedCount >= PhotonNetwork.CurrentRoom.PlayerCount) { GameManager.instance.GameOver(false); }

    }
    void CheckGameOver()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        int totalCaught = 0;
        foreach (GameObject player in players)
        {
            PlayerCatching catching = player.GetComponent<PlayerCatching>();
            if (catching != null && catching.catchState == PlayerCatching.playerCatchState.roomed)
            {
                totalCaught++;
            }
        }

        if (totalCaught >= players.Length)
        {
            GameManager.instance.GameOver(false);
        }
    }
    void respawnCrib()
    {

    }
}
