using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hustler : MonoBehaviour
{
    public float speed = 1f;

    public float stoppingSpeed = 1f;

    Rigidbody rigidBunga;



    // Start is called before the first frame update
    void Start()
    {
        
        rigidBunga = GetComponent<Rigidbody>();


    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 moveDir = Vector3.zero;


        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {

            moveDir += new Vector3(-Input.GetAxisRaw("Vertical"), 0, Input.GetAxisRaw("Horizontal"));

            moveDir = moveDir.normalized * speed;
        }
        else { 
        
            moveDir = -rigidBunga.linearVelocity;
            moveDir = moveDir.normalized * stoppingSpeed;

        
        }

        

        

        rigidBunga.AddForce(moveDir, ForceMode.Acceleration);


    }
}
