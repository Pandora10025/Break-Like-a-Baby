using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class BabySitterAI : MonoBehaviour
{


    public enum BabysitterAIState
    {
        PREIDLE,
        IDLE,
        PREPATROL,
        PATROL,
        PRECHASE,
        CHASE,
        PREPICKUP,
        PICKUP

    }

    public BabysitterAIState currentState = BabysitterAIState.IDLE;

    NavMeshAgent nav;



    public Vector2 idleWaitMinAndMax = new Vector2(3, 10);
    public Vector2 patrolPercentEachPatrolMinAndMax = new Vector2(.1f, 1f);

    private float calculatedWaitDelay = 0;
    private float waitTimer = 0;

    public List<Transform> patrolPoints = new List<Transform>();
    private int calculatedPatrolPointsToVisit = 0;



    public string tagToChase;

    public float fieldOfViewDegrees = 60;
    public float viewingDistance = 1;
    public float escapeDistance = 1;
    public float catchingDistance = 1;

    public Transform playerWeAreCurrentlyChasing;
    public Transform playerCloseEnoughToBeGrabbed;


    public LayerMask LayersWeCanSee;







    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();

        currentState = BabysitterAIState.PREIDLE;

    }

    // Update is called once per frame
    void Update()
    {

        //Code for when we spot a player
        Transform spottedPlayer = ScanForPlayers();



        switch (currentState) {

            case BabysitterAIState.PREIDLE:

                calculatedWaitDelay = UnityEngine.Random.Range(idleWaitMinAndMax.x, idleWaitMinAndMax.y);

                waitTimer = calculatedWaitDelay;



                Debug.Log("The babysitter will wait " + calculatedWaitDelay + " seconds!");


                currentState = BabysitterAIState.IDLE;

                break;

            case BabysitterAIState.IDLE:


                // 'waitTimer' decreases every frame to act as a timer.
                if (waitTimer > 0)
                {

                    
                    waitTimer -= Time.deltaTime;



                    
                    if (spottedPlayer)
                    {
                        currentState = BabysitterAIState.CHASE;

                        playerWeAreCurrentlyChasing = spottedPlayer;

                    }


                }
                else
                {
                    //when waitTimer = 0, then the babysitter decides to patrol for a bit.

                    currentState = BabysitterAIState.PREPATROL;

                }



                break;


            case BabysitterAIState.PREPATROL:

                


                //Pre-patrol behavior:

                RandomAmountOfPointsToPatrol();

                currentState = BabysitterAIState.PATROL;



                break;

            case BabysitterAIState.PATROL:


                //Code for when we spot a player
                
                if (spottedPlayer)
                {
                    currentState = BabysitterAIState.CHASE;

                    playerWeAreCurrentlyChasing = spottedPlayer;

                    return;

                }




                //Moving between the different patrol points...
                //When we reach a destination, we will set the current destination to null.
                //Down here we're checking if there's no destination yet. If not, it means we're ready to move again!
                // Check if we've reached the destination
                // Series of if statements take from https://discussions.unity.com/t/how-can-i-tell-when-a-navmeshagent-has-reached-its-destination/52403/5
                if (!nav.pathPending)
                {
                    if (nav.remainingDistance <= nav.stoppingDistance)
                    {
                        if (!nav.hasPath || nav.velocity.sqrMagnitude == 0f)
                        {
                             

                            //The patrol function is mean to keep going based on how many spots are lef to visit. 
                            //We should only keep visiting more spots if we have more move left!


                            if (calculatedPatrolPointsToVisit > 0)
                            {
                                //We're going another point!

                                calculatedPatrolPointsToVisit--;


                                //First we find the closest patrol point! (the one we are at)
                                //Then the destination will be the one after that.


                                float closestDist = 88888; //My favorite number is 8!
                                Transform closestPatrolPoint = null;
                                int closestIndex = 0;

                                foreach (Transform patrolPoint in patrolPoints)
                                {
                                    float currentDist = Vector3.Distance(transform.position, patrolPoint.position);

                                    if (currentDist < closestDist)
                                    {
                                        closestDist = currentDist;
                                        closestPatrolPoint = patrolPoint;
                                        closestIndex = patrolPoints.IndexOf(patrolPoint);

                                    }

                                }



                                nav.SetDestination(patrolPoints[(closestIndex + 1) % patrolPoints.Count].position);


                            }
                            else {
                                //We're done patrolling for now!

                                currentState = BabysitterAIState.PREIDLE;


                            
                            }


                            
                        }
                    }
                }


                //if ( !nav.hasPath || !nav.pathPending)
                //{
                    
                //}








                break;

            case BabysitterAIState.CHASE:

                float playerDist = Vector3.Distance(playerWeAreCurrentlyChasing.position, transform.position);

                if (playerDist > escapeDistance)
                {
                    playerWeAreCurrentlyChasing = null;

                    currentState = BabysitterAIState.PREIDLE;

                    StopMoving();

                }
                else
                {

                    float angleToPlayer = Vector3.Angle( transform.forward , ( playerWeAreCurrentlyChasing.position - transform.position ).normalized );


                    if (playerDist < catchingDistance && angleToPlayer < fieldOfViewDegrees/2)
                    {

                        Debug.Log("GOTCHA GOTCHA GOTCHA, " + playerWeAreCurrentlyChasing.name + "!");

                        StopMoving();

                        //currentState = BabysitterAIState.PICKUP;


                    }
                    else {
                        nav.SetDestination(playerWeAreCurrentlyChasing.position); 
                    }


                   

                }


                break;

            case BabysitterAIState.PICKUP:




                break;





        }







    }


    public void StopMoving()
    {
        nav.isStopped = true;
        nav.ResetPath();
    }


    void RandomAmountOfPointsToPatrol() {

        float randomPercent = UnityEngine.Random.Range( patrolPercentEachPatrolMinAndMax.x , patrolPercentEachPatrolMinAndMax.y );


        float roundedPercent = Mathf.Ceil(randomPercent * patrolPoints.Count);


        calculatedPatrolPointsToVisit = (int)(roundedPercent);

        Debug.Log("The babysitter will patrol " + calculatedPatrolPointsToVisit + " points!");


    }


    Transform ScanForPlayers() {
        Transform playerReturnVariable = null;


        GameObject[] players = GameObject.FindGameObjectsWithTag(tagToChase);


        GameObject closestPlayer = null;
        float closestDist = viewingDistance;


        for (int i = 0; i < players.Length; i++)
        {
            GameObject currentPlayer = players[i];
            Vector3 lookDirection = currentPlayer.transform.position - transform.position;

            float currentDist = Vector3.Magnitude( lookDirection);

            float angleDifference = Vector3.Angle(transform.forward, lookDirection);



            RaycastHit hit;


            if ( Mathf.Abs(angleDifference) <= fieldOfViewDegrees/2f)
            {
                if (Physics.Raycast(transform.position, lookDirection.normalized * currentDist, out hit, 888, LayersWeCanSee))
                {


                    //&& hit.rigidbody.name == currentPlayer.name
                    if (currentDist < closestDist && hit.transform.tag == "Player")
                    {
                        closestDist = currentDist;
                        closestPlayer = currentPlayer;

                        playerReturnVariable = closestPlayer.GetComponent<Transform>();

                        //Debug.Log("The closest player..." + playerReturnVariable.name + "!");

                          


                    }


                }
            }

        }


        
        
        return playerReturnVariable;
    }





    private void OnDrawGizmos()
    {

        Vector3 rightNormalFOV = Quaternion.Euler(0, fieldOfViewDegrees/2f, 0) * transform.forward;
        Vector3 leftNormalFOV = Quaternion.Euler(0, -fieldOfViewDegrees/2f, 0) * transform.forward;

        Vector3 sweepingNormalFOV = Quaternion.Euler(0, Mathf.Sin(Time.realtimeSinceStartup) * fieldOfViewDegrees / 2f, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position+rightNormalFOV * viewingDistance);
        Gizmos.DrawLine(transform.position, transform.position+leftNormalFOV * viewingDistance);
        Gizmos.DrawLine(transform.position, transform.position+sweepingNormalFOV * viewingDistance);

        int arcSteps = 8;

        for (int i = 0; i < arcSteps; i++)
        {
            Vector3 from = Quaternion.Euler( 0, (float)i /(float)arcSteps * fieldOfViewDegrees ,0 ) * leftNormalFOV;
            Vector3 to = Quaternion.Euler(0, (float)(i+1) / (float)arcSteps * fieldOfViewDegrees, 0) * leftNormalFOV;


            Gizmos.DrawLine( transform.position + from * viewingDistance, transform.position + to * viewingDistance);
            
            
            Gizmos.DrawLine( transform.position + from * catchingDistance, transform.position + to * catchingDistance);

            




        }


        for (int i = 0; i < patrolPoints.Count; i++)
        {

            Transform currentPoint = patrolPoints[i];

            Transform nextPoint = patrolPoints[ (i+1)%patrolPoints.Count];

            Gizmos.DrawSphere(currentPoint.position, .5f);

            Gizmos.DrawLine(currentPoint.position, nextPoint.position);

            

        }



        //Now we'll make a little visual but for the babysitter's escape range.
        //I want to do a striped circle around the babysitter, to represent the escape range.


        arcSteps = 32;
        Gizmos.color = Color.red;

        for (int i = 0; i < arcSteps; i++)
        {

            Vector3 from = Quaternion.Euler(0, (float)i / (float)arcSteps * 360  +Time.realtimeSinceStartup*2, 0) * Vector3.forward;
            Vector3 to = Quaternion.Euler(0, (float)(i + 1) / (float)arcSteps * 360 + Time.realtimeSinceStartup*2, 0) * Vector3.forward;


            Gizmos.DrawLine(transform.position + from * escapeDistance, transform.position + to * viewingDistance);

            Gizmos.DrawLine(transform.position + from * escapeDistance, transform.position + to * escapeDistance);




        }



    }


}
