using UnityEngine;

public class BabysitterCamera : MonoBehaviour
{

    public Transform followTarget;

    public Vector3 offset = new Vector3( 0, 4, 2);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (!followTarget)
        {
            followTarget = GameObject.FindGameObjectWithTag("Babysitter").transform;


        }


    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotatedOffset = followTarget.rotation * offset;



        Vector3 targetPos = followTarget.position + rotatedOffset;


        transform.position = Vector3.Lerp( transform.position  , targetPos  ,   0.1f );




        
    }
}
