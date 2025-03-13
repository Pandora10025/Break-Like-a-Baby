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
    [SerializeField] public Canvas canvas;
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] public Material activeMat, inactiveMat, boykissedMaterialSponsoredByJayVik;
    private float health;
    private Transform startPos;
    private MeshRenderer meshRenderer;
    [SerializeField] private Collider objectCollider;
    PhotonView pv;
    List<GameObject> playersInRange = new List<GameObject>();
    private Transform playerTransform;
    private Vector3 playerPos;
    private Vector3 playerRight;



    //enum and state manager
    private enum objectState
    {
        inactive,
        active,
        broken
    }
    private int myState = (int)objectState.inactive;


    void Start()
    {
        //instantiate sliders and stuff
        slider = this.transform.parent.GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Slider>();
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        slider.minValue = 0;
        health = maxHealth;
        pv = GetComponent<PhotonView>();
        Debug.Log((pv == null) + gameObject.transform.parent.name);
        meshRenderer = GetComponent<MeshRenderer>();
        //this.GetComponent<MeshRenderer>().material = inactiveMat;
        if (photonView.Owner == null)
        {
            photonView.TransferOwnership(PhotonNetwork.MasterClient);
        }

    }
    #region state changer
    public void Inactive()
    {
        this.GetComponent<MeshRenderer>().material = inactiveMat;
        myState = (int)objectState.inactive;
        canvas.enabled = false;
    }
    public void Active()
    {
        this.GetComponent<MeshRenderer>().material = activeMat;
        myState = (int)objectState.active;
        canvas.enabled = true;
    }
    public void Break()
    {
        this.GetComponent<MeshRenderer>().material = boykissedMaterialSponsoredByJayVik;
        myState = (int)objectState.broken;
    }

    #endregion

    void FixedUpdate()
    {//all slider adjustments will be here
        if (health <= maxHealth && health > 0)
            health += 0.05f;
        slider.value = health;

    }
    public void TakeDamage(Transform playerT)
    {
        Debug.Log("taking damage!");
        playerTransform = playerT;
        playerPos = playerT.position;
        playerRight = playerT.right;

        photonView.RPC("DamageObject", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void DamageObject()
    {
        Debug.Log("among us");
        if (myState == (int)objectState.active)
        {
            if (photonView == null)
            {
                Debug.LogWarning("photonView is null in DamageObject");
                return;
            }

            //Debug.Log("player has been sent over!: " + playerTransform.name);
            //shake it!
            this.GetComponent<BoxRockerTest>().Shake(playerPos, playerRight);


            health--;
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