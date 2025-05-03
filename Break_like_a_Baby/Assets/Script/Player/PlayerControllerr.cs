using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine.UI;

public class PlayerControllerr : MonoBehaviourPunCallbacks, IInRoomCallbacks
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
    [SerializeField] RuntimeAnimatorController[] animatorControllers;


    // Store the target rotation angle
    private float targetAngle = 0f; 

    // Input Action variables
    private PlayerControls inputActions;  // Reference to input actions
    private InputAction spamInput;
    private InputAction tasksInput;

  

    public Vector2 moveInput;
    public bool isSprinting;
    private bool isInteracting;
    public bool isRotating;


    public PhotonView view;

    [SerializeField] TextMeshProUGUI nameTag;
    [SerializeField] Color[] shadowColors;

    public bool enableMove = true;

    public int colorId;

    public float sprintCoolDown = 5f;
    public float sprintRecoveryRate = 2f;
    public float sprintDrainRate = 1f;

    public float currentSprintTime;
    private bool canSprint = true;

    private ParticleSystem dustTrail;
    public float trailM=200f;
    public Slider sprintBar;
    public float minEmiss=15f;
    bool clDown = true;

    
    void Awake()
    {
        inputActions = new PlayerControls();
        inputActions.Enable();
        // Subscribe to the interaction event
        //inputActions.Player.Interact.performed += OnInteract; // Detect when the button is pressed
        inputActions.Player.Tasks.performed += OnTask;
        //inputActions.Player.Tasks.performed += OnSpam;

        spamInput = inputActions.Player.Spam;
        tasksInput = inputActions.Player.Tasks;
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
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Difficulty", out object difficultyObj))
        {
            int difficulty = (int)difficultyObj;

            switch (difficulty)
            {
                case 0:
                    Debug.Log("Easy Mode");
                    clDown = false;
                    break;
                case 1:
                    Debug.Log("Normal Mode");
                    clDown = true;
                    sprintRecoveryRate = 2.5f;
                    sprintCoolDown = 10f;
                    break;
                case 2:
                    Debug.Log("Hard Mode");
                    clDown = true;
                    sprintRecoveryRate = 1f;
                    sprintCoolDown = 5f;
                    break;
            }
        }

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        //audioSearch = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        //itemOutline = FindAnyObjectByType<OutlineManager>();
        //timerManager = FindAnyObjectByType<TimerManager>();
        //QTE = FindAnyObjectByType<QuickTimeEvent>();
        //tasks = FindAnyObjectByType<TaskList>();
        mainCamera = Camera.main;
        dustTrail = GetComponentInChildren<ParticleSystem>();
        //cameraFollow = GetComponent<CameraFollow>();
      
           anim = GetComponent<Animator>();

        if (photonView.IsMine)
        {
            nameTag.gameObject.SetActive(false);
            StartCoroutine(SetAnimatorDelayed());
       
        }

        view = GetComponent<PhotonView>();

        currentSprintTime = sprintCoolDown;

        photonView.RPC("toggleTrail", RpcTarget.AllBuffered, false);
        
        sprintBar.maxValue = sprintCoolDown;
        
        sprintBar.gameObject.SetActive(false);
    }

    IEnumerator SetAnimatorDelayed()
    {
        yield return new WaitForSeconds(0.1f); // Small delay to ensure all components are initialized

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("CharacterID", out object characterID))
        {
            int id = (int)characterID;
            Debug.Log(id);
            photonView.RPC("RPC_SetAnimator", RpcTarget.AllBuffered, id);
        }
        
    }

    [PunRPC]
    void RPC_SetAnimator(int characterID)
    {
        nameTag.text = photonView.Owner.NickName;
        colorId = characterID;
        
        if (anim == null)
        {
            anim = GetComponent<Animator>();  // Ensure anim is assigned
            if (anim == null)
            {
                Debug.LogError("Animator component is missing on " + gameObject.name);
                return;
            }

        }

        Debug.Log($"RPC_SetAnimator called on {photonView.Owner.NickName} with CharacterID {characterID}");

        if (characterID >= 0 && characterID < animatorControllers.Length)
        {
            anim.runtimeAnimatorController = animatorControllers[characterID];
            GetComponent<NetworkedPlayer>().anim.runtimeAnimatorController= animatorControllers[characterID];
            Debug.Log($"{photonView.Owner.NickName} now using Animator {characterID}");

            Material mat = nameTag.fontSharedMaterial; 

            if (mat.HasProperty("_UnderlayColor")) 
            {
                mat.SetColor("_UnderlayColor", shadowColors[characterID]);
            }
        }
        else
        {
            Debug.LogWarning("Invalid CharacterID received!");
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("CharacterID"))
        {
            int characterID = (int)changedProps["CharacterID"];
            Debug.Log($"OnPlayerPropertiesUpdate called for {targetPlayer.NickName} with CharacterID: {characterID}");

            if (targetPlayer == photonView.Owner)
            {
                photonView.RPC("RPC_SetAnimator", RpcTarget.AllBuffered, characterID);
            }
        }
    }

    void Update()
    {
        if (view.IsMine && enableMove )
        {
            // Fetch input data from the input system
            moveInput = inputActions.Player.Move.ReadValue<Vector2>();  // Get the movement direction
            isSprinting = canSprint && inputActions.Player.Sprint.ReadValue<float>() > 0.5f;  // Check if sprinting
                                                                                              //isInteracting = inputActions.Player.Interact.ReadValue<float>() > 0f; // Check if interacting
            HandleSprint();
            // Storing previous player angle
            previousAngle = transform.eulerAngles.y;
            MovePlayer();
            RotatePlayerToMovementDirection();
        }

        if (spamInput.triggered)
        {
            Debug.Log("Button pressed" + spamInput.ToString());
        }

        if (tasksInput.triggered)
        {
            Debug.Log("Task button is " +  tasksInput.ToString());  
        }
        
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

    //private void OnInteract(InputAction.CallbackContext context)
    //{
        // Called when the "Interact" action is performed (button press)
        //itemOutline.ToggleOutline();  // Trigger the outline toggle
        //ScreenShake.instance.TriggerShake(0.25f, 0.5f);
        //TriggerInteractHapticFeedback();


        /*
        if (timerManager.timesUp)
        {
            StartCoroutine(StopHapticFeedbackAfterDelay());
            timerManager.ReloadScene();
        }
        */
    //}
    void HandleSprint()
    {
        //Check if player is sprinting
        if(isSprinting)
        {
           
            // Drain the current sprin time
            if(clDown)
            currentSprintTime -=sprintDrainRate * Time.deltaTime;
            photonView.RPC("trailSpeed", RpcTarget.AllBuffered, currentSprintTime);
            // If it is completely drained, 
            if (currentSprintTime <= 0f)
            {
                // Player is unable to sprint
                currentSprintTime = 0f;
                canSprint = false;
                isSprinting = false;
            }
            if (!dustTrail.isPlaying)
            {
                dustTrail.Play();
                photonView.RPC("toggleTrail", RpcTarget.AllBuffered, true);
            }
           
        }
        else
        {
          
            if (currentSprintTime < sprintCoolDown)
            {
                canSprint = false;
                if (photonView.IsMine)
                {
                    if (!sprintBar.gameObject.active)
                    {
                        sprintBar.gameObject.SetActive(true);
                        sprintBar.minValue = currentSprintTime;
                    }
                      
                }
                currentSprintTime += sprintRecoveryRate * Time.deltaTime;
                sprintBar.value = currentSprintTime;
                if (currentSprintTime >= sprintCoolDown)
                {
                    if (photonView.IsMine)
                    {
                        if (sprintBar.gameObject.active)
                            sprintBar.gameObject.SetActive(false);
                    }
                    currentSprintTime = sprintCoolDown;
                    canSprint = true;
                }
            }
            if (dustTrail.isPlaying)
            {
                dustTrail.Stop();
                photonView.RPC("toggleTrail", RpcTarget.AllBuffered, false);
            }
            
           
        
    }
    }

    [PunRPC]
    void toggleTrail(bool t)
    {
        if (t)
        {
            dustTrail.Play();
        }
        else
        {
            dustTrail.Stop();
        }
    }

    [PunRPC]
    void trailSpeed(float t)
    {
        float falloff = Mathf.Pow(t / sprintCoolDown, 2);
        float scaledEmission = trailM * falloff;

      
        var emission = dustTrail.emission;
        emission.rateOverTime = scaledEmission;

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
        Vector3 targetMovement = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

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
            rb.linearVelocity = Vector3.zero;
            // Stop movement animation
            anim.SetFloat("Velocity", 0f);
            
        }

    }

    void RotatePlayerToMovementDirection()
    {

        if (moveInput != Vector2.zero)
        {

            Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            currentAngle = Mathf.Atan2(direction.z,direction.x) * Mathf.Rad2Deg;

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

            Quaternion targetRotation = Quaternion.Euler(new Vector3(90f, 0f, currentAngle));
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


    public void TriggerInteractHapticFeedback(float low, float high)
    {
        if (Gamepad.current != null) // Ensure there's a gamepad connected
        {
            Gamepad.current.SetMotorSpeeds(low, high);  // Light vibration for feedback
            StartCoroutine(StopHapticFeedbackAfterDelay());  // Wait for some time before stopping vibration
        }
    }

    // Coroutine to stop haptic feedback after a short delay
    IEnumerator StopHapticFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(0.2f);  // Delay duration (adjust as needed)
        Gamepad.current.SetMotorSpeeds(0f, 0f);  // Stop vibration after the delay
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.gameObject.CompareTag("interact"))
    //    {
    //        Debug.Log("This is an item");
    //    }
    //}

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }
    
    public bool isSpamTriggered()
    {
        return spamInput.triggered;
    }

    public bool isTaskTriggered()
    {
        return tasksInput.IsPressed();
    }
}
