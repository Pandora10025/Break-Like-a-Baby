using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using System;
using UnityEditor;
using UnityEngine.SceneManagement;

public class BreakableObject : MonoBehaviourPunCallbacks
{
    [SerializeField] public UnityEngine.UI.Slider slider;
    [SerializeField] private int health = 10;
    [SerializeField] public Material activeMat, inactiveMat, boykissedMaterialSponsoredByJayVik;
    private Transform startPos;
    private MeshRenderer meshRenderer;
    [SerializeField] private Collider objectCollider;
    PhotonView pv;
    List<GameObject> playersInRange = new List<GameObject>();

    //enum and state manager
    private enum objectState
    {
        inactive,
        active,
        broken
    }
    private int myState = (int) objectState.inactive;

    
    void Start()
    {
        //instantiate sliders and stuff
        slider = this.transform.parent.GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Slider>();
        slider.maxValue = health;
        slider.value = health;
        slider.minValue = 0;
        pv = GetComponent<PhotonView>();
        Debug.Log((pv==null) + gameObject.transform.parent.name);
        meshRenderer = GetComponent<MeshRenderer>();
        //this.GetComponent<MeshRenderer>().material = inactiveMat;
        if (photonView.Owner == null)
        {
            photonView.TransferOwnership(PhotonNetwork.MasterClient);
        }
        
    }

    /// <summary>
    /// 
    /// </summary>
    public void Inactive()
    {
        this.GetComponent<MeshRenderer>().material = inactiveMat;
        myState = (int)objectState.inactive;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Active()
    {
        this.GetComponent<MeshRenderer>().material = activeMat;
        myState = (int)objectState.active;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Break()
    {
        this.GetComponent<MeshRenderer>().material = boykissedMaterialSponsoredByJayVik;
        myState = (int)objectState.broken;
    }

    public void TakeDamage()
    {
        photonView.RPC("DamageObject", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void DamageObject()
    {
        if (myState == (int)objectState.active)
        {
            if (photonView == null)
            {
                Debug.LogWarning("photonView is null in DamageObject");
                return;
            }
            health--;
            slider.value = health;
            Debug.Log("Health: " + health);

            if (health <= 0)//when the object is broken
            {
                ObjectManager.instance.Break(this.gameObject);
                foreach (GameObject player in playersInRange)
                {
                    player.GetComponent<PlayerBreak>().breakableInRange(false, gameObject);
                }
                playersInRange.Clear();
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (int)stream.ReceiveNext();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playersInRange.Add(other.gameObject);
            other.GetComponent<PlayerBreak>().breakableInRange(true, gameObject);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
           
            other.GetComponent<PlayerBreak>().breakableInRange(false, gameObject);
            playersInRange.Remove(other.gameObject);

        }
    }
}