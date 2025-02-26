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
    [SerializeField] public Material mat1, mat2;
    private Transform startPos;
    private MeshRenderer meshRenderer;
    [SerializeField] private Collider objectCollider;

   List<GameObject> playersInRange = new List<GameObject>();
    
    void Start()
    {
        //instantiate sliders and stuff
        slider = this.transform.parent.GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Slider>();
        slider.maxValue = health;
        slider.value = health;
        slider.minValue = 0;

        meshRenderer = GetComponent<MeshRenderer>();
        //objectCollider = GetComponent<Collider>();

        // Ensure the MasterClient owns the object
        //|| photonView.Owner != PhotonNetwork.MasterClient
        if (SceneManager.GetActiveScene().name != "ObjectInfrastructure")
        {
            if (photonView.Owner == null)
            {
                photonView.TransferOwnership(PhotonNetwork.MasterClient);
            }
        }
    }

    /// <summary>
    /// Method <c>SetCollider</c> toggles the outer collider and slider of the Breakable Object
    /// </summary>
    /// <param name="b"></param> takes a boolean :)
    public void SetCollider(Boolean b)
    {
        objectCollider.enabled = b;
        Debug.Log(this.name + " changed to " + b);
        
    }
    public void TakeDamage()
    {
        if (SceneManager.GetActiveScene().name != "ObjectInfrastructure")
        {
            photonView.RPC("DamageObject", RpcTarget.AllBuffered);
        }
        else
        {
            DamageObject();
        }
    }

    [PunRPC]
    public void DamageObject()
    {
        if (SceneManager.GetActiveScene().name != "ObjectInfrastructure")
        {
            if (photonView == null)
            {
                Debug.LogWarning("photonView is null in DamageObject");
                return;
            }
        }


        health--;
        slider.value = health;
        Debug.Log("Health: " + health);

        if (health <= 0)//when the object is broken
        {
            SetCollider(false);
            //adjust list
            ObjectManager.instance.Break(this.gameObject);

            //set child.animState
            //set child MeshRenderer

            //change material to "broken"

            this.GetComponent<MeshRenderer>().material = mat2;

        
            foreach (GameObject player in playersInRange)
            {
                player.GetComponent<PlayerBreak>().breakableInRange(false, gameObject);
            }

            playersInRange.Clear();

            
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