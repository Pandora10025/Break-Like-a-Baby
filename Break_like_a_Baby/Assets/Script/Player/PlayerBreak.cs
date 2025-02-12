using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

public class PlayerBreak : MonoBehaviourPunCallbacks
{
    //Rigidbody rb;
    //[SerializeField] public float moveSpeed = 5f;
    //[SerializeField] public float rotationSpeed = 10f;
    //[SerializeField] private Vector3 moveDirection;

    public bool inRange = false;
    GameObject breakable;
    private void Start()
    {
        //rb = this.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        //float moveX = Input.GetAxisRaw("Horizontal");
        //float moveZ = Input.GetAxisRaw("Vertical");
        //moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        //// Rotate to face movement direction
        //if (moveDirection != Vector3.zero)
        //{
        //    Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * rotationSpeed);
        //}
        if(inRange && Input.GetKeyDown(KeyCode.Space) && photonView.IsMine)
        {
           
                Debug.Log("pressed space");
                breakable.GetComponent<BreakableObject>().takeDamage();

       
        }

    }

    void FixedUpdate()
     {
            //// Move the baby
            //rb.linearVelocity = moveDirection * moveSpeed + new Vector3(0, rb.linearVelocity.y, 0);
        }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Initial: " + collision.gameObject.name);

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Breakable" )
    //    {

    //        breakable = other.gameObject;
    //        inRange = true;
                
    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject == breakable)
    //    {
    //        inRange = false;

    //    }
    //}

    public void breakableInRange(bool isInRange , GameObject breakableObj)
    {
        if (isInRange)
        {
            Debug.Log(gameObject.name + isInRange);
            inRange = true;
            breakable = breakableObj;
        }
        if( !isInRange && breakable == breakableObj)
        {
            Debug.Log(gameObject.name + isInRange);
            inRange = false;
        }
    }

}
