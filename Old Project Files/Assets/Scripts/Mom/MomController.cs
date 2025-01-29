using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MomController : MonoBehaviour
{
    [SerializeField] private PathNode targetNode;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float minIdleDuration;
    [SerializeField] private float maxIdleDuration;

    // AI tracking variables
    [SerializeField] private Transform player; // Reference to player
    [SerializeField] private float trackingRange = 10f; // Range within which the mom starts chasing the player

    private Rigidbody rb;
    private NavMeshAgent agent;
    private IEnumerator behaviorRoutine;
    private SlowMotion slowMo;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        slowMo = FindAnyObjectByType<SlowMotion>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }

        if (targetNode == null)
        {
            Debug.LogError("Target Node has not yet been assigned.");
            return;
        }

        if (player == null)
        {
            Debug.LogError("Player has not been assigned.");
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }



        SetAgentSpeed(moveSpeed);
        agent.stoppingDistance = 0.5f; // Adjust stopping distance as needed

        // Freeze rotation to keep the mom's orientation fixed
        agent.updateRotation = false;
        agent.updatePosition = true;
        agent.autoRepath = true;

        transform.position = targetNode.transform.position;

        StartCoroutine(BehaviorRoutine());
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= trackingRange)
        {
            // Get the direction to the player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // Keep X and Y rotations fixed to 0, only change Z rotation
            float targetZRotation = targetRotation.eulerAngles.z;

            // Apply the rotation, freezing X and Y while allowing Z to rotate
            transform.rotation = Quaternion.Euler(0, 0, targetZRotation);
        }
        else
        {
            // Keep rotation fixed at zero for idle behavior
            transform.rotation = Quaternion.identity;
        }
    }

    private void SetAgentSpeed(float speed)
    {
        if ( agent != null)
        {
            agent.speed = speed;
        }
    }

    private IEnumerator BehaviorRoutine()
    {
        float nextCheckTime = 0f;
        float checkInterval = 0.2f; // Check every 0.2 seconds for efficiency

        while (true)
        {
            if (Time.time >= nextCheckTime)
            {
                nextCheckTime = Time.time + checkInterval;

                if (Vector3.Distance(transform.position, player.position) <= trackingRange)
                {
                    // If the player is within range, chase the player
                    MoveToPlayer();
                    SetAgentSpeed(chaseSpeed);
                    AudioManager.instance.SwitchMusic(AudioManager.instance.chaseSFX);
                }
                else
                {
                    AudioManager.instance.SwitchMusic(AudioManager.instance.backgroundSFX);
                    // Otherwise, follow the path
                    yield return StartCoroutine(FollowPath());
                    SetAgentSpeed(moveSpeed);
                    
                }
            }
            yield return null;  // Wait one frame before checking again
        }
    }

    private void MoveToPlayer()
    {
        if (agent != null)
        {
            agent.SetDestination(player.position);
            agent.isStopped = false;
        }
    }

    private IEnumerator FollowPath()
    {
        // Idle behavior at current target node
        float idleDuration = Random.Range(minIdleDuration, maxIdleDuration);
        yield return new WaitForSeconds(idleDuration);

        targetNode = targetNode.GetRandomAdjacent();
        Vector3 nextPos = targetNode.transform.position;

        // Set the agent's destination to the next target node
        if (agent != null)
        {
            agent.SetDestination(nextPos);

            // Wait until the agent reaches the target node
            while (Vector3.Distance(transform.position, nextPos) > agent.stoppingDistance)
            {
                yield return null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("BABY CAUGHT!");

            slowMo.StartSlowMotion();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            slowMo.StopSlowMotion();

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Detect collision with environmental objects
        Debug.Log($"Collided with {collision.gameObject.name}");
    }

    // Draw gizmo to visualize the tracking range
    private void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, trackingRange);

            // Visualize the current state (chasing or idle)
            if (Vector3.Distance(transform.position, player.position) <= trackingRange)
            {
                Gizmos.color = Color.green;  // Chasing state
            }
            else
            {
                Gizmos.color = Color.yellow; // Idle state
            }
            Gizmos.DrawWireSphere(transform.position, 1f);  // Visualization of current position/state
        }
    }
}
