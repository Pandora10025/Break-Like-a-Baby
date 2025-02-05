using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private Vector3 moveDirection;

    public PhotonView view;

    public GameObject camera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();
        if (!view.IsMine)
        {
            camera.SetActive(false);

        }
        // Only allow control for the local player
    }

    void Update()
    {
        if (view.IsMine)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");
            moveDirection = new Vector3(moveX, 0, moveZ).normalized;

            // Rotate to face movement direction
            if (moveDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * rotationSpeed);
            }
        }
        // Get Input
     
    }

    void FixedUpdate()
    {
        // Move the baby
        rb.linearVelocity = moveDirection * moveSpeed + new Vector3(0, rb.linearVelocity.y, 0);
    }
}
