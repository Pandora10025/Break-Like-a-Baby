using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerBreak : MonoBehaviourPunCallbacks
{
    Rigidbody rb;
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float rotationSpeed = 10f;
    [SerializeField] private Vector3 moveDirection;

    private PlayerControllerr playerControl;

    public bool inRange = false;
    GameObject breakable;
    [SerializeField] public AudioSource aud;
    public string brokenList;
    public int viewId;
    public int breakCount;
    
    private void Awake()
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
            breakable = breakableObj;
        }
        if (!isInRange && breakable == breakableObj)
        {
            inRange = false; 
        }
    }

    public void AddToList(string objectName)
    {
        brokenList = objectName +", " +brokenList;
        breakCount++;
    }
}
