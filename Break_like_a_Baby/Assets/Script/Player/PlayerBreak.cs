using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerBreak : MonoBehaviourPunCallbacks
{
    Rigidbody rb;
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float rotationSpeed = 10f;
    [SerializeField] private Vector3 moveDirection;

    private PlayerControllerr playerControl;

    public bool inRange = false;
    GameObject breakable;
    [SerializeField]
    List<GameObject> breakables=new List<GameObject>();
    [SerializeField] public AudioSource aud;
    public string brokenList;
    public int viewId;
    public int breakCount;

    public bool canBreak = true;
    
    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        aud = this.GetComponent<AudioSource>();
        playerControl = this.GetComponent<PlayerControllerr>();
        brokenList = "";
        viewId = photonView.ViewID;

    }

    private void Update()
    {
        if (inRange && (Input.GetKeyDown(KeyCode.Space) || playerControl.isSpamTriggered()) && photonView.IsMine){

            aud.Play();
            breakable.GetComponent<BreakableObject>().TakeDamage(photonView.ViewID);
            
        }

        //check to show the tablist
        if (photonView.IsMine)
        {
            if (Input.GetKey(KeyCode.Tab) || playerControl.isTaskTriggered())
            {
                ObjectManager.instance.ToggleText(true);
                //GameManager.instance.ToggleText(true);
            }
            else
            {
                ObjectManager.instance.ToggleText(false);
                //GameManager.instance.ToggleText(false);
            }
        }
    }

    public void breakableInRange(bool isInRange, GameObject breakableObj)
    {
        if (isInRange)
        {
            //Debug.Log(gameObject.name + isInRange);
            inRange = true;
            addToList(breakableObj);
            //if (breakable)
            //{
                //breakable.GetComponent<BreakableObject>().removeSelf(gameObject);
                //breakable.GetComponent<BoxRockerTest>().DisableOutlines();
            //}

            breakable = breakableObj;
            if(photonView.IsMine)
            breakable.GetComponent<BoxRockerTest>().EnabledOutlines();
        }
        if (!isInRange )
        {
            if(breakable == breakableObj)
            removeFromList(breakable);
            else
            {
                breakables.Remove(breakableObj);
                if (photonView.IsMine)
                    breakableObj.GetComponent<BoxRockerTest>().DisableOutlines();
            }
            //inRange = false;
            //breakable.GetComponent<BoxRockerTest>().DisableOutlines();
            //breakable = null;
        }


    }

    public void AddToList(string objectName)
    {
        brokenList = objectName +", " +brokenList;
        breakCount++;
    }

    void addToList(GameObject b)
    {
        
        for(int i = 0; i < breakables.Count; i++)
        {
            if (photonView.IsMine)
            {
                breakables[i].GetComponent<BoxRockerTest>().DisableOutlines();
            }
           
        }
        breakables.Add(b);

    }

    void removeFromList(GameObject b)
    {
        int newIndex = breakables.IndexOf(b)-1;
        if (newIndex < 0)
        {
            breakable = null;
            inRange = false;
        }
        else
        {
            breakable = breakables[newIndex];
            if(photonView.IsMine)
            breakable.GetComponent<BoxRockerTest>().EnabledOutlines();
        }
        if(photonView.IsMine)
        b.GetComponent<BoxRockerTest>().DisableOutlines();

        breakables.Remove(b);
       
    }

}
