using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private void Update()
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

    private void Physics_ContactEvent()
    {

    }

    

}
