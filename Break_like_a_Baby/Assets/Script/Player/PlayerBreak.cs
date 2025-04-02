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

    public bool inRange = false;
    GameObject breakable;

    [SerializeField] public AudioSource aud;
    

    
    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        aud = this.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.Space) && photonView.IsMine){

            aud.Play();
            breakable.GetComponent<BreakableObject>().TakeDamage(photonView.ViewID);
            
        }
        //check to show the tablist
        if (photonView.IsMine)
        {
            if (Input.GetKey(KeyCode.Tab))
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

    public void breakableInRange(bool isInRange , GameObject breakableObj)
    {
        if (isInRange)
        {
            //Debug.Log(gameObject.name + isInRange);
            inRange = true;
            breakable = breakableObj;
        }
        if( !isInRange && breakable == breakableObj)
        {
            inRange = false;
        }
    }



}
