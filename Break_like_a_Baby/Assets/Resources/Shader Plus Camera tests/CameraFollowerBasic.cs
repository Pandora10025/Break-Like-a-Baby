using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowerBasic : MonoBehaviour
{
    public float camDist = 10f;

    public Vector3 offsetDirection = Vector3.up;

    public Transform followTarget;


    // Start is called before the first frame update
    void Start()
    {


        
    }

    // Update is called once per frame
    void Update()
    {
        if (followTarget != null)
        {
            transform.position = followTarget.position + offsetDirection.normalized * camDist;

            transform.LookAt(followTarget.position);




        }
    }
}
