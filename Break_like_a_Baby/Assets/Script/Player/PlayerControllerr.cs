using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;

public class PlayerControllerr   : MonoBehaviour
{
    //private AudioManager audioSearch;
    //private OutlineManager itemOutline;
    //private TimerManager timerManager;
    //public TaskList tasks;
    //private CameraFollow cameraFollow;
    //private QuickTimeEvent QTE;

    [Header("MOVEMENT VALUES")]
    [Space]
    public float currentSpeed;
    [Range(0, 1000)]
    public float moveSpeed;
    [Range(0, 2000)]
    public float sprintSpeed;
    public float rotationSpeed;
    public float acceleration = 5f;
    public float deceleration = 5f;

    public float previousAngle = 0f;
    public float currentAngle = 0f;
    public float maxAngleDistance;

    public Transform playerTransform;
    public Transform spineIKTarget;
    public float rswayAmount = 0.1f;
    public float rlerpSpeed = 5f;


    private float currentVelocity = 0f;


    private Rigidbody rb;
    private Camera mainCamera;
    private Animator anim;

    // Store the target rotation angle
    private float targetAngle = 0f;

    // Input Action variables
    private PlayerControls inputActions;  // Reference to input actions
    private Vector2 moveInput;
    public bool isSprinting;
    private bool isInteracting;
    public bool isRotating;

    void Awake()
    {
        inputActions = new PlayerControls();
        inputActions.Enable();
        // Subscribe to the interaction event
        inputActions.Player.Interact.performed += OnInteract; // Detect when the button is pressed
        inputActions.Player.Tasks.performed += OnTask;
        //inputActions.Player.Tasks.performed += OnSpam;
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        //audioSearch = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        //itemOutline = FindAnyObjectByType<OutlineManager>();
        //timerManager = FindAnyObjectByType<TimerManager>();
        //QTE = FindAnyObjectByType<QuickTimeEvent>();
        //tasks = FindAnyObjectByType<TaskList>();
        mainCamera = Camera.main;
        //cameraFollow = GetComponent<CameraFollow>();
        anim = GetComponent<Animator>();

       
    }

    void Update()
    {
        // Fetch input data from the input system
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();  // Get the movement direction
        isSprinting = inputActions.Player.Sprint.ReadValue<float>() > 0.5f;  // Check if sprinting
        //isInteracting = inputActions.Player.Interact.ReadValue<float>() > 0f; // Check if interacting

        // Storing previous player angle
        previousAngle = transform.eulerAngles.z;

        MovePlayer();
        RotatePlayerToMovementDirection();

        /*
        if (!timerManager.timesUp)
        {
            timerManager.countdown -= Time.deltaTime;
            if (timerManager.countdown <= 0)
            {
                timerManager.timesUp = true;
                timerManager.countdown = 0;
                timerManager.FreezeScene();
            }
            else
            {
                timerManager.UpdateCountdownText();
            }
        }
        */
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        // Called when the "Interact" action is performed (button press)
        //itemOutline.ToggleOutline();  // Trigger the outline toggle
        //ScreenShake.instance.TriggerShake(0.25f, 0.5f);
        TriggerInteractHapticFeedback();


        /*
        if (timerManager.timesUp)
        {
            StartCoroutine(StopHapticFeedbackAfterDelay());
            timerManager.ReloadScene();
        }
        */
    }

    private void OnTask(InputAction.CallbackContext context)
    {
        //tasks.ToggleTaskList();
    }
    /*
    void OnSpam(InputAction.CallbackContext context) 
    {
        QTE.StartQTE("SPAM!");
    }
    */
    private void MovePlayer()
    {
        // Determine target speed based on if player is sprinting 
        // If player is sprinting then the target speed is set to spring speed. vice-versa
        float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;


        if (moveInput != Vector2.zero)  // Player is moving
        {
            // The current velocity lerps to target speed at the acceleration rate
            currentVelocity = Mathf.MoveTowards(currentVelocity, targetSpeed, acceleration * Time.deltaTime);
        }
        else  // Player is not moving
        {
            // The current velocity lerps to zero at the acceleration rate
            currentVelocity = Mathf.MoveTowards(currentVelocity, 0f, deceleration * Time.deltaTime);
        }

        // Calculate the movement direction 
        Vector3 targetMovement = new Vector3(-moveInput.x, 0f, -moveInput.y).normalized;

        // If the movement input is over a threshold, similar to (if keyPressed)
        if (targetMovement.magnitude >= 0.1f)
        {
            // create a desired velocity using current velocity and the movement direction
            Vector3 desiredVelocity = targetMovement * currentVelocity;
            // Smoothly transition to the desired velocity 
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, desiredVelocity, 0.15f);
            // Play movement animation.
            anim.SetFloat("Velocity", currentVelocity);
        }
        else
        {
            // Stop player movement
            rb.linearVelocity = new Vector3(0f, 0f, rb.linearVelocity.z);
            // Stop movement animation
            anim.SetFloat("Velocity", 0f);
        }

    }

    void RotatePlayerToMovementDirection()
    {

        if (moveInput != Vector2.zero)
        {

            Vector3 direction = new Vector3(-moveInput.x, 0f, -moveInput.y).normalized;
            currentAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

            float angleDif = Mathf.Abs(Mathf.DeltaAngle(previousAngle, currentAngle));

            float swayOffset = Mathf.Sin(Time.time * 4f) * rswayAmount * angleDif * 0.1f;
            Vector3 targetPos = playerTransform.position + new Vector3(moveInput.x, 0f, moveInput.y) + new Vector3(0, swayOffset, 0f);
            isRotating = angleDif > maxAngleDistance;

            /*
            if (isRotating)
            {
                spineIKTarget.position = Vector3.Lerp(spineIKTarget.position, targetPos, Time.deltaTime * rlerpSpeed);
                //anim.SetBool("isRotating", true);
            }
            */

            Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, currentAngle, 0f));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


        }


    }

    void TriggerHapticFeedback()
    {
        if (Gamepad.current != null) // Ensure there's a gamepad connected
        {
            // Intensity can be adjusted, here it is set to 0.5f (medium strength)
            Gamepad.current.SetMotorSpeeds(0.5f, 0.5f); // Left motor (low frequency) and right motor (high frequency)
        }
    }

    // Stop haptic feedback


    void TriggerInteractHapticFeedback()
    {
        if (Gamepad.current != null) // Ensure there's a gamepad connected
        {
            Gamepad.current.SetMotorSpeeds(0.3f, 0.3f);  // Light vibration for feedback
            StartCoroutine(StopHapticFeedbackAfterDelay());  // Wait for some time before stopping vibration
        }
    }

    // Coroutine to stop haptic feedback after a short delay
    IEnumerator StopHapticFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(0.2f);  // Delay duration (adjust as needed)
        Gamepad.current.SetMotorSpeeds(0f, 0f);  // Stop vibration after the delay
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("interact"))
        {
            Debug.Log("This is an item");
        }
    }
}
