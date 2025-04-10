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
    void Awake()
    {

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.crib = this;
        breakable = GetComponent<BreakableObject>();
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

        photonView.RPC("babyBed", RpcTarget.AllBuffered, colorID);
    }

    public void Break()
    {
        babyBeddedCount--;
        foreach (GameObject baby in babys)
        {
            baby.SetActive(false);
        }
        GameManager.instance.playerCaught.GetComponent<PlayerCatching>().changeState(PlayerCatching.playerCatchState.free);

    }

    [PunRPC]
    public void babyBed(int colorID)
    {

        
        babys[babyBeddedCount].SetActive(true);
        babys[babyBeddedCount].GetComponent<SpriteRenderer>().sprite = babySleeping[colorID];
        babyBeddedCount++;
        if (babyBeddedCount >= 2) { GameManager.instance.GameOver(false); }
        breakable.Active();
        breakable.resetHealth();
    }

    void respawnCrib()
    {

    }
}
