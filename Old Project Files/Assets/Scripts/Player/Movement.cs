
using UnityEngine;

public class Movement : MonoBehaviour
{
    private AudioManager audioSearch;

    [Header("MOVEMENT VALUES")]

    [Space]

    public float currentSpeed;
    [Range(0, 1000)]
    public float moveSpeed;
    [Range(0, 2000)]
    public float sprintSpeed;
    public float rotationSpeed;
    public float moveX;
    public float moveY;
    public bool isSprinting;

    private Rigidbody rb;
    private Camera mainCamera;
    private Animator anim;

    // Store the target rotation angle
    private float targetAngle = 0f; // Default target angle is 0 (facing up)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;  // Prevent Rigidbody from rotating automatically (we will handle rotation manually)
        audioSearch = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        mainCamera = Camera.main;
        anim = GetComponent<Animator>();
    }

    [System.Obsolete]


    void Update()
    {
        MovePlayer();
        //RotatePlayerToMouse();
        RotatePlayerToMovementDirection();
    }

    [System.Obsolete]
    void MovePlayer()
    {
        // Get player input for movement along the X and Y axes
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetButton("R2"))
        {
            currentSpeed = sprintSpeed;
            isSprinting = true;
        }
        else
        {
            currentSpeed = moveSpeed;
            isSprinting = false;
        }

        // Calculate movement direction
        Vector3 targetMovement = new Vector3(moveX, moveY, 0f).normalized;

        // Apply smooth movement to the Rigidbody
        if (targetMovement.magnitude >= 0.1f)
        {
            // Smoothly interpolate the current velocity to the target direction and speed
            Vector3 desiredVelocity = targetMovement * currentSpeed;
            rb.velocity = Vector3.Lerp(rb.velocity, desiredVelocity, 0.15f); // 0.1f is the smoothing factor

            // Set the animation speed based on movement velocity magnitude
            float velocityMagnitude = rb.velocity.magnitude;
            anim.SetFloat("Velocity", velocityMagnitude);
        }
        else
        {
            // Reset the velocity when no input is given, keep the z-axis velocity (if needed)
            rb.velocity = new Vector3(0f, 0f, rb.velocity.z);
            anim.SetFloat("Velocity", 0);
        }
    }

    void RotatePlayerToMouse()
    {
        // Get the mouse position in screen space (in pixels)
        Vector3 mousePosition = Input.mousePosition;

        // Convert mouse position to world position, but keeping z at 0
        mousePosition.z = mainCamera.nearClipPlane; // This ensures a valid z value to convert to world space

        // Convert mouse position from screen space to world space
        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // Calculate the direction vector from the player to the mouse position
        Vector3 direction = (worldMousePosition - transform.position).normalized;

        // Calculate the angle in degrees between the player and the mouse position
        // Swap direction.x and direction.y to fix the angle calculation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Debugging logs to check mouse position and angle
        Debug.Log("Mouse Position (Screen): " + mousePosition);
        Debug.Log("Mouse Position (World): " + worldMousePosition);
        Debug.Log("Calculated Angle: " + angle);

        // Create a target rotation using Quaternion.Euler
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        // Smoothly rotate towards the target angle
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }



    void RotatePlayerToMovementDirection()
    {
        // Check if the player is moving
        if (moveX != 0 || moveY != 0)
        {
            // Calculate the direction vector from the player's current position
            Vector3 direction = new Vector3(moveX, moveY, 0f).normalized;

            // Calculate the angle in degrees between the player and the direction of movement
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Create a target rotation using Quaternion.Euler
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

            // Smoothly rotate the player towards the target angle
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }


}

