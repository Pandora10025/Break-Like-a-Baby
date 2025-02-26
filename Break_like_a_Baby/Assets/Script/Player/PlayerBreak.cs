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
    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // Rotate to face movement direction
        //THIS IS FOR RAY'S OBJECT INFRASTRUCTURE SCENE. NO PHOTON/
        if (SceneManager.GetActiveScene().name == "ObjectInfrastructure")
        {
            if (moveDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * rotationSpeed);
            }


            if (inRange && Input.GetKeyDown(KeyCode.Space))
            {

                Debug.Log("pressed space");
                breakable.GetComponent<BreakableObject>().TakeDamage();


            }

            if (Input.GetKey(KeyCode.Tab))
            {
                ObjectManager.ToggleText(true);
            }
            else
            {
                ObjectManager.ToggleText(false);
            }
        }
        else//THIS IS FOR EVERYTHING NORMAL-- WILL BE REMOVED LATER
        {
            if (inRange && Input.GetKeyDown(KeyCode.Space) && photonView.IsMine)
            {

                Debug.Log("pressed space");
                breakable.GetComponent<BreakableObject>().TakeDamage();


            }
            //check to show the tablist
            if(Input.GetKey(KeyCode.Tab) && photonView.IsMine)
            {
                ObjectManager.ToggleText(true);
            }
            else
            {
                ObjectManager.ToggleText(false);
            }
        }




    }

    void FixedUpdate()
     {
        // Move the baby
        if (SceneManager.GetActiveScene().name == "ObjectInfrastructure")
            rb.linearVelocity = moveDirection * moveSpeed + new Vector3(0, rb.linearVelocity.y, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Initial: " + collision.gameObject.name);

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
