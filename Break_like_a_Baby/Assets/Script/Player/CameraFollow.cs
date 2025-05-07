using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraFollow : MonoBehaviour
{
    [Header("CAMERA FOLLOW")]
    [Space]
    private Transform player;  // Reference to the player
    private PlayerControllerr playerController;
    public Vector3 offset = new Vector3(-0.3f, 0.3f, -20f);  // Base offset from the player (distance behind and above)
    public float smoothSpeed = 0.125f;  // How quickly the camera moves to follow the player
    public float rotationSpeed = 10f;  // Speed at which the camera rotates to follow the player
    GameObject pl;

    private Vector3 velocity = Vector3.zero;  // For storing the velocity in SmoothDamp method

    [Header("ScreenShake Values")]
    private float shakeDuration;
    private float shakeMagnitude;
    private float shakeFadeout;
    private float shakeRotation;

    private float rotationMultiplier = 15;

    private float randomXrange;
    private float randomYrange;
    public Camera cam;
    public float baseFOV = 60f;
    public float maxFOV = 80f;
    public float fovSmoothSpeed = 5f;
    public float speedThreshold;
    public float maxSpeed;
    Rigidbody rb;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player != null)
        {
            FollowPlayer();
            RotateCamera();
        }
        else
        {
            pl = FindLocalPlayer();
            if (pl)
            {
                player = pl.transform;
                rb = pl.GetComponent<Rigidbody>();
                speedThreshold = pl.GetComponent<PlayerControllerr>().moveSpeed;
                maxSpeed = pl.GetComponent<PlayerControllerr>().sprintSpeed;

            }
               
            else
                this.enabled = false;
          
        }

        if (shakeDuration > 0)
        {
            shakeDuration -= Time.deltaTime;

            randomXrange = Random.Range(-0.3f, 0.3f) * shakeMagnitude;
            randomYrange = Random.Range(-0.3f, 0.3f) * shakeMagnitude;

            transform.position += new Vector3(randomXrange, randomYrange, 0f);

            shakeMagnitude = Mathf.MoveTowards(shakeMagnitude, 0f, shakeFadeout * Time.deltaTime);
            shakeRotation = Mathf.MoveTowards(shakeRotation, 0f, shakeFadeout * rotationMultiplier * Time.deltaTime);

            transform.rotation = Quaternion.Euler(90f, 0f, shakeRotation * Random.Range(-1f, 1f));
        }
        

    }

   
    void FollowPlayer()
    {
        // Adjust the offset based on the player's facing direction
        Vector3 adjustedOffset = GetOffsetBasedOnDirection();
      
        // The desired position is player's position + the adjusted offset
        Vector3 desiredPosition = player.position + adjustedOffset;

        // Move the camera smoothly to the desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

        if (GetComponent<PhotonView>().IsMine)
        {
            float speed = rb.linearVelocity.magnitude;

            float targetFOV = baseFOV;

            if (speed > speedThreshold)
            {
               
                float t = Mathf.InverseLerp(speedThreshold, maxSpeed, speed);
                targetFOV = Mathf.Lerp(baseFOV, maxFOV, t);
            }

            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);
        }
    }

  
    void RotateCamera()
    {
        if (playerController == null)
        {
            playerController = player.GetComponent<PlayerControllerr>();
            if (playerController == null) return; // Exit if PlayerController is not found
        }

        // Get the player's movement direction (moveInput)
        Vector2 moveInput = playerController.GetMoveInput();

        // Calculate tilt angles based on the player's movement direction
        float tiltAngleX = -moveInput.y * 5f; // Tilt around X-axis (forward/backward movement)
        float tiltAngleZ = moveInput.x * 5f;  // Tilt around Z-axis (left/right movement)

        // Clamp the tilt angles to prevent excessive tilting
        tiltAngleX = Mathf.Clamp(tiltAngleX, -25f, 25f); // Limit X-axis tilt to ?15 degrees
        tiltAngleZ = Mathf.Clamp(tiltAngleZ, -25f, 25f); // Limit Z-axis tilt to ?15 degrees

        // Start from the top-down view
        Quaternion topDownRotation = Quaternion.Euler(90f, 0f, 0f);

        // Apply tilts relative to camera?s local space
        Quaternion tiltRotation = Quaternion.Euler(tiltAngleX, 0f, 0f) * Quaternion.Euler(0f, 0f, tiltAngleZ);

        // Combine them (reversed order)
        Quaternion targetRotation = tiltRotation * topDownRotation;

        // Smoothly interpolate the camera's rotation towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Get an offset based on the direction the player is facing
    Vector3 GetOffsetBasedOnDirection()
    {

        if (playerController == null)
        {
            playerController = player.GetComponent<PlayerControllerr>();
            if (playerController == null) return offset; // Exit if PlayerController is not found
        }

        // Get the player's movement direction (moveInput)
        Vector2 moveInput = playerController.GetMoveInput();

        // Normalize the moveInput to ensure consistent offset magnitude
        if (moveInput.magnitude > 1f)
        {
            moveInput.Normalize();
        }

        // Calculate the adjusted offset based on the player's movement direction
        Vector3 adjustedOffset = new Vector3(moveInput.x, 0f, moveInput.y) * offset.z;

        // Add the fixed Y-axis offset to ensure the camera looks down
        adjustedOffset.y = offset.y;

        Debug.Log("Adjusted Offset is " + adjustedOffset);

        return adjustedOffset;



    }


    public void TriggerScreenShake(float length, float power)
    {
        shakeDuration = length;
        shakeMagnitude = power;
        shakeFadeout = power / length;
        shakeRotation = power * rotationMultiplier;
    }

    GameObject FindLocalPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject localPlayer = null;
        foreach (GameObject player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                localPlayer = player;
                break;
            }
        }
        return localPlayer;
    }

}