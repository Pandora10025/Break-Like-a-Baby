using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using System;
using UnityEditor;

public class Crib : MonoBehaviourPunCallbacks
{
    //[SerializeField] Sprite[] babySleeping;
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
        GameManager.instance.playerCaught.GetComponent<PlayerCatching>().changeState(PlayerCatching.playerCatchState.free);
        foreach (GameObject baby in babys)
        {
            baby.SetActive(false);
        }

        Invoke("respawnCrib", respawnTime);
    }

    [PunRPC]
    public void babyBed(int colorID)
    {
       
        babys[colorID].SetActive(true);
        babyBeddedCount++;
        breakable.Active();
    }

    void respawnCrib()
    {

    }
}
