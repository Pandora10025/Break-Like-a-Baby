using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;

public class PlayerController : MonoBehaviour
{
    private AudioManager audioSearch;
    private OutlineManager itemOutline;
    private TimerManager timerManager;
    public TaskList tasks;
    private CameraFollow cameraFollow;
    private Interaction interact;
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

    private float currentVelocity = 0f;


    private Rigidbody rb;
    private Camera mainCamera;
    private Animator anim;

    // Store the target rotation angle
    private float targetAngle = 0f;

    // Input Action variables
    private PlayerInputs inputActions;
    private InputAction spamAction;// Reference to input actions
    private Vector2 moveInput;
    public bool isSprinting;
    private bool isInteracting;



    void Awake()
    {
        inputActions = new PlayerInputs();
        spamAction = inputActions.Player.Spam;
        // Assuming you have created PlayerInputActions
        inputActions.Enable();
        // Subscribe to the interaction event
        inputActions.Player.Interact.performed += OnInteract; // Detect when the button is pressed
        inputActions.Player.Tasks.performed += OnTask;
        inputActions.Player.Restart.performed += OnRestart;
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
        audioSearch = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        itemOutline = FindAnyObjectByType<OutlineManager>();
        timerManager = FindAnyObjectByType<TimerManager>();
        //QTE = FindAnyObjectByType<QuickTimeEvent>();
        tasks = FindAnyObjectByType<TaskList>();
        mainCamera = Camera.main;
        cameraFollow = GetComponent<CameraFollow>();
        anim = GetComponent<Animator>();
        interact = GetComponent<Interaction>();
    }

    void Update()
    {
        // Fetch input data from the input system
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();  // Get the movement direction
        isSprinting = inputActions.Player.Sprint.ReadValue<float>() > 0.5f;  // Check if sprinting
        //isInteracting = inputActions.Player.Interact.ReadValue<float>() > 0f; // Check if interacting

        MovePlayer();
        RotatePlayerToMovementDirection();


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

        if (spamAction.triggered && interact.currentCircle != null)
        {
            interact.TriggerBar();
            ScreenShake.instance.TriggerShake(0.2f, 0.3f);
            TriggerInteractHapticFeedback();
            AudioManager.instance.PlaySFX(audioSearch.breakSFX);
        }

    }
    private void OnRestart(InputAction.CallbackContext context)
    {
        if (timerManager.timesUp)
        {
            StartCoroutine(StopHapticFeedbackAfterDelay());
            timerManager.ReloadScene();
        }
    }
    private void OnInteract(InputAction.CallbackContext context)
    {
        // Called when the "Interact" action is performed (button press)
        itemOutline.ToggleOutline();  // Trigger the outline toggle
        ScreenShake.instance.TriggerShake(0.25f, 0.5f);
        TriggerInteractHapticFeedback();
        AudioManager.instance.PlaySFX(audioSearch.interactSFX);
       
    }

    private void OnTask(InputAction.CallbackContext context)
    {
        tasks.ToggleTaskList();
        AudioManager.instance.PlaySFX(audioSearch.taskSFX);
    }
    
   
    private void MovePlayer()
    {
        // Determine target speed based on sprinting state
        float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;

        // Gradually increase or decrease velocity
        if (moveInput != Vector2.zero)  // Player is moving
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, targetSpeed, acceleration * Time.deltaTime);
        }
        else  // Player is not moving
        {
            currentVelocity = Mathf.MoveTowards(currentVelocity, 0f, deceleration * Time.deltaTime);
        }

        // Calculate the movement direction and apply to Rigidbody
        Vector3 targetMovement = new Vector3(moveInput.x, moveInput.y, 0f).normalized;
        if (targetMovement.magnitude >= 0.1f)
        {
            Vector3 desiredVelocity = targetMovement * currentVelocity;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, desiredVelocity, 0.15f);
            anim.SetFloat("Velocity", currentVelocity);
        }
        else
        {
            rb.linearVelocity = new Vector3(0f, 0f, rb.linearVelocity.z);
            anim.SetFloat("Velocity", 0f);
        }

    }

    void RotatePlayerToMovementDirection()
    {
        if (moveInput != Vector2.zero)
        {
            Vector3 direction = new Vector3(moveInput.x, moveInput.y, 0f).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
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