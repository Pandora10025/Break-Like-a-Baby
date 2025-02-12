using Unity.VisualScripting;
using UnityEngine;

public class PlayerBreak : MonoBehaviour
{
    //Rigidbody rb;
    //[SerializeField] public float moveSpeed = 5f;
    //[SerializeField] public float rotationSpeed = 10f;
    //[SerializeField] private Vector3 moveDirection;
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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Breakable" && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("pressed space");
            other.gameObject.GetComponent<BreakableObject>().takeDamage();

        }
    }


}
