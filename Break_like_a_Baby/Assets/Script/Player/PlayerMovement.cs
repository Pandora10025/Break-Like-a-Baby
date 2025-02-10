using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] public int mode = 0;
    [SerializeField] public Vector3 w;   
    Vector3 startPos;
    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        w = new Vector3(5f, 5f, 5f);
        startPos = this.transform.position;
    }
    private void Update()
    {
        if (mode == 0)
        {
            if (Input.GetKey(KeyCode.D))
            {
                transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, transform.position.z);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y, transform.position.z);
            }
            if (Input.GetKey(KeyCode.W))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.1f);
            }
        }
        if(mode == 1)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(w, ForceMode.Acceleration);
                Debug.Log("accel");
            }
        }
        if(mode == 2)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(w, ForceMode.Impulse);
                Debug.Log("impulse");
            }
        }
        if(mode == 3)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(w, ForceMode.Force);
                Debug.Log("force");
            }
        }
        if(mode == 4)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(w, ForceMode.VelocityChange);
                Debug.Log("veloc");
            }
        }
        if(mode == 5)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(transform.forward * 5.0f);
                Debug.Log("forward");
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            rb.linearVelocity = new Vector3(0, 0, 0);
            transform.position = startPos;
        }

    }

    private void Physics_ContactEvent()
    {

    }

    

}
