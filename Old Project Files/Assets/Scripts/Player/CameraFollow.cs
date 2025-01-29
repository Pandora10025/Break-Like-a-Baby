using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("CAMERA FOLLOW")]
    [Space]
    public Transform player;  // Reference to the player
    public Vector3 offset = new Vector3(0f, 10f, -10f);  // Base offset from the player (distance behind and above)
    public float smoothSpeed = 0.125f;  // How quickly the camera moves to follow the player
    public float rotationSpeed = 5f;  // Speed at which the camera rotates to follow the player

    private Vector3 velocity = Vector3.zero;  // For storing the velocity in SmoothDamp method

    void Update()
    {
        FollowPlayer();
        RotateCamera();
    }

    // Smoothly move the camera to the player's position
    void FollowPlayer()
    {
        // Adjust the offset based on the player's facing direction
        Vector3 adjustedOffset = GetOffsetBasedOnDirection();

        // The desired position is player's position + the adjusted offset
        Vector3 desiredPosition = player.position + adjustedOffset;

        // Move the camera smoothly to the desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
    }

    // Rotate the camera to always look at the player
    void RotateCamera()
    {
        // Get the direction from the camera to the player
        Vector3 direction = player.position - transform.position;

        // Calculate the desired rotation
        Quaternion desiredRotation = Quaternion.LookRotation(direction);

        // Smoothly rotate the camera towards the player
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }

    // Get an offset based on the direction the player is facing
    Vector3 GetOffsetBasedOnDirection()
    {
        // Get the player's forward direction 
        Vector3 playerForward = player.forward;

        // Depending on the direction the player is facing, adjust the offset
        // Example: if player is facing forward, the camera stays behind; if player faces right, the camera moves to the side
        if (Vector3.Dot(playerForward, Vector3.forward) > 0.5f)  // Player is mostly facing forward
        {
            return offset;  // Default offset (behind the player)
        }
        else if (Vector3.Dot(playerForward, Vector3.back) > 0.5f)  // Player is facing backward
        {
            return new Vector3(0f, 40f, 40f);  // Camera moves in front of the player
        }
        else if (Vector3.Dot(playerForward, Vector3.right) > 0.5f)  // Player is facing right
        {
            return new Vector3(40f, 40f, 0f);  // Camera moves to the right of the player
        }
        else if (Vector3.Dot(playerForward, Vector3.left) > 0.5f)  // Player is facing left
        {
            return new Vector3(-40f, 40f, 0f);  // Camera moves to the left of the player
        }

        return offset;  // Default if none of the above directions match
    }
}
