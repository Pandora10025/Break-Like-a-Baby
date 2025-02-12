using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class BreakableObject : MonoBehaviourPunCallbacks
{
    [SerializeField] public UnityEngine.UI.Slider slider;
    [SerializeField] private int health = 10;
    [SerializeField] public Material mat1, mat2;
    private Transform startPos;
    private MeshRenderer meshRenderer;
    private Collider objectCollider;

   List<GameObject> playersInRange = new List<GameObject>();
    
    void Start()
    {
        //instantiate sliders and stuff
        slider = this.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Slider>();
        slider.maxValue = health;
        slider.value = health;
        slider.minValue = 0;

        meshRenderer = GetComponent<MeshRenderer>();
        objectCollider = GetComponent<Collider>();

        // Ensure the MasterClient owns the object
        //|| photonView.Owner != PhotonNetwork.MasterClient
        if (photonView.Owner == null)
        {
            photonView.TransferOwnership(PhotonNetwork.MasterClient);
        }
    }

    public void takeDamage()
    {

        if (true)
        {
            photonView.RPC("DamageObject", RpcTarget.AllBuffered);
        }
        else
        {
            Debug.Log("takeDamage called but photonView is not owned by this client.");
        }
    }

    [PunRPC]
    public void DamageObject()
    {
        if (photonView == null)
        {
            Debug.LogWarning("photonView is null in DamageObject");
            return;
        }

        slider.value = health;
        health--;
        Debug.Log("Health: " + health);

        if (health <= 0)//when the object is broken
        {
            //change material to "broken"
            this.GetComponent<MeshRenderer>().material = mat2;

            foreach (GameObject player in playersInRange)
            {
                player.GetComponent<PlayerBreak>().breakableInRange(false, gameObject);
            }
            playersInRange.Clear();
            meshRenderer.enabled = false;
            objectCollider.enabled = false;
            gameObject.SetActive(false);
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