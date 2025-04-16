using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using System;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [SerializeField]
    GameObject shatterable;
    List<Rigidbody> breakableRb = new List<Rigidbody>();
    public float explosionForce = 10f;  
    public float explosionRadius = 5f;
    public float upwardsModifier = 1f;

    [SerializeField] public Transform explosionPosition;

    [SerializeField] GameObject shatteredMesh, originalMesh;
    [SerializeField] float shatterWeight = 10f;
    public Sprite breakImage;
    //enum and state manager
    private enum objectState
    {
        inactive,
        active,
        broken
    }


    [SerializeField] bool debugShowRadii = false;
    [SerializeField] float onHitAlarmRadius = 8;
    //Thought I'd also add a super duper radius for when you break an object. It's obviously going to be a bit louder!
    [SerializeField] float onShatterAlarmRadius = 88;

    private int myState = (int)objectState.inactive;

    private void Awake()
    {
        if (shatterable != null)
        {
            // Collect all Rigidbody components in the children of the public object
            breakableRb.AddRange(shatterable.GetComponentsInChildren<Rigidbody>());
        }
      

        foreach (Rigidbody rb in breakableRb)
        {
            rb.isKinematic = true;
            rb.mass = shatterWeight;
        }

        if(shatteredMesh!=null)
        shatteredMesh.SetActive(false);
    }
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
        GetComponent<BoxRockerTest>().DisableOutlines();

    }
    public void Active()
    {
        this.GetComponent<MeshRenderer>().material = activeMat;
        myState = (int)objectState.active;
        canvas.enabled = true;
        GetComponent<BoxRockerTest>().EnabledOutlines();

    }
    public void resetHealth()
    {
        health = maxHealth;
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

        if (Input.GetKeyDown(KeyCode.B))
        {
            Explode();
        }

    }
    public void TakeDamage(int pvId)
    {
        Debug.Log("taking damage!");
        

        photonView.RPC("DamageObject", RpcTarget.AllBuffered, pvId);
    }

    [PunRPC]
    public void DamageObject(int pvId)
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
            PhotonView playerPhotonView = PhotonView.Find(pvId);
            if (playerPhotonView != null)
            {
                Transform playerT = playerPhotonView.transform;
                Debug.Log("Player has been sent over!: " + playerT.name);



                //THIS IS LUKAS!!! Here, I'm making it so that the babysitter is alarmed.
                alarmBabysitter(onHitAlarmRadius);
                
                Vector3 playerPos = playerT.position;
                Vector3 playerRight = playerT.right;



                this.GetComponent<BoxRockerTest>().Shake(playerPos, playerRight);
            }

            health--;
            Debug.Log("Health: " + health);

            if (health <= 0)//when the object is broken
            {
                if (playerPhotonView != null)
                {
                    Transform playerT = playerPhotonView.transform;
                    playerT.gameObject.GetComponent<PlayerBreak>().AddToList(transform.parent.name);
                }
                if (GetComponent<Crib>() != null)
                {
                    GameManager.instance.crib.Break();
                    Break();
                }
                else
                {
                    ObjectManager.instance.Break(this.gameObject);
                }
                Explode();
               
                
                foreach (GameObject player in playersInRange)
                {
                    player.GetComponent<PlayerBreak>().breakableInRange(false, gameObject);
                }
                playersInRange.Clear();
            }
        }
    }


    void Explode()
    {
       

        if(originalMesh!=null && shatteredMesh != null)
        {
            originalMesh.SetActive(false);
            shatteredMesh.SetActive(true);
        }

        foreach (Rigidbody rb in breakableRb)
        {
          

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(explosionForce, explosionPosition.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }

            Invoke("disableColOnFragments", 0.5f);
        }


        //THIS IS LUKAS!!! HERE IM SETTING IT SO THAT THE BABYSITTER IS ALARMED AND ITS ESPECIALLY LOUD WHEN YOU BREAK THE OBJECT

        alarmBabysitter(onShatterAlarmRadius);


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

    void disableColOnFragments()
    {
        foreach( Rigidbody rb in breakableRb)
        {
            Collider col = rb.gameObject.GetComponent<Collider>();
            if (col!=null)
            {
               
            }
            rb.mass = 1f;
        }
    }

    void alarmBabysitter(float radius) {

        if ( Vector3.Distance( GameManager.instance.babySitter.transform.position, transform.position) < radius   )
        {
            GameManager.instance.babySitter.GetComponent<BabySitterAI>().PathfindToPos(transform.position);

        }



    }


    //THIS IS LUKAS!!! JUST THOUGHT ID DRAW A GIZMO TO SHOW THE RADIUS OF AN OBJECT!
    private void OnDrawGizmos()
    {
        if (debugShowRadii)
        {
            int arcSteps = 32;
            for (int i = 0; i < arcSteps; i++)
            {
                Gizmos.color = Color.yellow;

                Vector3 from = Quaternion.Euler(0, (float)i / (float)arcSteps * 360 + Time.realtimeSinceStartup * 2, 0) * Vector3.forward;
                Vector3 to = Quaternion.Euler(0, (float)(i + 1) / (float)arcSteps * 360 + Time.realtimeSinceStartup * 2, 0) * Vector3.forward;

                Gizmos.DrawLine(transform.position + from * onHitAlarmRadius, transform.position + to * onHitAlarmRadius);
                Gizmos.DrawLine(transform.position + from * onShatterAlarmRadius, transform.position + to * onShatterAlarmRadius);

            }
        }

        


    }

}