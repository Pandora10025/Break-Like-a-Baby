using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;



public class BabySitterAI : MonoBehaviourPunCallbacks
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
        PICKUP,
        PATHFIND


    }

    public bool cribTest = false;

    public bool holdingBaby = false;



    public BabysitterAIState[] statesIntoAnsync;

    public BabysitterAIState currentState = BabysitterAIState.IDLE;

    NavMeshAgent nav;



    public Vector2 idleWaitMinAndMax = new Vector2(3, 10);
    public Vector2 patrolPercentEachPatrolMinAndMax = new Vector2(.1f, 1f);

    public float deathCountdownMaxTime = 1f;
    public float deathTimer = 0f;



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

    // ANIMATION

    private Animator anim;
    private float velocity;

    public Material[] playerMats;

    public GameObject babyOrb;

    void Awake() {

        Debug.Log(gameObject);



    }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.instance)
        {
            GameManager.instance.babySitter = gameObject;

        }
        babyOrb.SetActive(false);
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        currentState = BabysitterAIState.PREIDLE;

        deathTimer = deathCountdownMaxTime;

    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(nav.velocity);
        //Code for when we spot a player

        if (!PhotonNetwork.IsMasterClient)
            return;
        Transform spottedPlayer = ScanForPlayers();

        if (spottedPlayer != playerWeAreCurrentlyChasing)
        {
            deathTimer = deathCountdownMaxTime;
        }


        if (cribTest)
        {
            cribTest = false;

            PathfindToPos(GameObject.FindGameObjectWithTag("Crib").transform.position);

        }


        switch (currentState)
        {

            case BabysitterAIState.PREIDLE:
                anim.SetBool("preidle", true);
                calculatedWaitDelay = UnityEngine.Random.Range(idleWaitMinAndMax.x, idleWaitMinAndMax.y);

                waitTimer = calculatedWaitDelay;



                Debug.Log("The babysitter will wait " + calculatedWaitDelay + " seconds!");


                currentState = BabysitterAIState.IDLE;
                photonView.RPC("changeState", RpcTarget.AllBuffered, (int)BabysitterAIState.IDLE);

                break;

            case BabysitterAIState.IDLE:
                anim.SetBool("preidle", false);

                // 'waitTimer' decreases every frame to act as a timer.
                if (waitTimer > 0)
                {


                    waitTimer -= Time.deltaTime;




                    if (spottedPlayer)
                    {
                        currentState = BabysitterAIState.CHASE;
                        photonView.RPC("changeState", RpcTarget.AllBuffered, (int)BabysitterAIState.CHASE);

                        playerWeAreCurrentlyChasing = spottedPlayer;
                        photonView.RPC("SetTargetPlayer", RpcTarget.AllBuffered, playerWeAreCurrentlyChasing.GetComponent<PhotonView>().ViewID);
                    }


                }
                else
                {
                    //when waitTimer = 0, then the babysitter decides to patrol for a bit.

                    currentState = BabysitterAIState.PREPATROL;
                    photonView.RPC("changeState", RpcTarget.AllBuffered, (int)BabysitterAIState.PREPATROL);

                }



                break;


            case BabysitterAIState.PREPATROL:

                anim.SetBool("prepatrol", true);


                //Pre-patrol behavior:

                RandomAmountOfPointsToPatrol();

                currentState = BabysitterAIState.PATROL;
                photonView.RPC("changeState", RpcTarget.AllBuffered, (int)BabysitterAIState.PATROL);



                break;

            case BabysitterAIState.PATROL:


                //Code for when we spot a player
                anim.SetBool("prepatrol", false);
                if (spottedPlayer)
                {
                    currentState = BabysitterAIState.CHASE;
                    photonView.RPC("changeState", RpcTarget.AllBuffered, (int)BabysitterAIState.CHASE);

                    playerWeAreCurrentlyChasing = spottedPlayer;
                    photonView.RPC("SetTargetPlayer", RpcTarget.AllBuffered, playerWeAreCurrentlyChasing.GetComponent<PhotonView>().ViewID);

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
                                anim.SetBool("patrol", true);

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
                            else
                            {
                                //We're done patrolling for now!

                                currentState = BabysitterAIState.PREIDLE;
                                photonView.RPC("changeState", RpcTarget.AllBuffered, (int)BabysitterAIState.PREIDLE);
                                anim.SetBool("patrol", false);



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
                anim.SetBool("patrol", false);
                anim.SetBool("chasing", true);
                

                if (playerDist > escapeDistance)
                {
                    playerWeAreCurrentlyChasing = null;
                    photonView.RPC("SetTargetPlayer", RpcTarget.AllBuffered, -1);

                    currentState = BabysitterAIState.PREIDLE;
                    photonView.RPC("changeState", RpcTarget.AllBuffered, (int)BabysitterAIState.PREIDLE);

                    StopMoving();

                }
                else
                {

                    float angleToPlayer = Vector3.Angle(transform.forward, (playerWeAreCurrentlyChasing.position - transform.position).normalized);


                    if (playerDist < catchingDistance)// && angleToPlayer < fieldOfViewDegrees / 2)
                    {


                        if (playerDist < catchingDistance / 2)
                        {
                            StopMoving();
                        }


                        //Below is only temporarily disabled so that we can add alternative behavior with a timer until we implement the really interesting game over mechanics!
                        //currentState = BabysitterAIState.PICKUP;

                        if (deathTimer > 0)
                        {
                            Debug.Log("GOTCHA GOTCHA GOTCHA, " + playerWeAreCurrentlyChasing.name + "!");


                            deathTimer -= Time.deltaTime;



                        }
                        else
                        {

                            //deathTimer = deathCountdownMaxTime;

                            Debug.Log("GAME OVER, BROOOOOOOOOOOOOO!!!");


                            //ARNAV, ADD THE SCENE CHANGE CODE HERE!

                            //GameManager.instance.GameOver(false);


                            //LUKAS AND ARNAV, THIS IS WHERE WE START UN-COMMENTING THINGS FOR THE BABYSITTER UPGRADE.
                            //The above code should hopefully be obsolete soon, because we're going to have the babysitter grab the child and run off with them!
                            //Their new task will be to run right off to the crib.

                            //currentState = BabysitterAIState.PICKUP;


                            currentState = BabysitterAIState.PREPICKUP;
                            photonView.RPC("changeState", RpcTarget.AllBuffered, (int)BabysitterAIState.PREPICKUP);



                        }







                    }
                    else
                    {
                        nav.SetDestination(playerWeAreCurrentlyChasing.position);

                        deathTimer = deathCountdownMaxTime;

                    }




                }


                break;

            case BabysitterAIState.PREPICKUP:


                anim.SetBool("prepickup", true);

                currentState = BabysitterAIState.PICKUP;
                photonView.RPC("changeState", RpcTarget.AllBuffered, (int)BabysitterAIState.PICKUP);

                break;

            case BabysitterAIState.PICKUP:


                //UNCOMMENT ALL THIS STUFF TOO!!!

                //               PlayerCatching player = playerWeAreCurrentlyChasing.GetComponent<PlayerCatching>();
                //               player.GetCaught();

                //And then I need to enable "the loaf"
                //The loaf is our name for the cylinder that the babysitter will hold that represents the baby that just got caught.
                //It will be yellow or red depending on the color of the baby.


                //And then after that we ought to have the babysitter run off to the crib, and deposit their prisoner.
                //PathfindToPos( GameObject.FindGameObjectWithTag("Crib").transform.position );



                //holdingBaby = true;

                anim.SetBool("patrol", false);
                anim.SetBool("chasing", false);
                anim.SetBool("prepickup", false);

                // Togle on pickup animation
                anim.SetBool("pickup", true);

                photonView.RPC("pickUp", RpcTarget.AllBuffered);


                //currentState = BabysitterAIState.PATHFIND;

                break;

            case BabysitterAIState.PATHFIND:

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


                            Debug.Log("Path has been found!");

                            StopMoving();
                            currentState = BabysitterAIState.PREIDLE;
                            photonView.RPC("changeState", RpcTarget.AllBuffered, (int)BabysitterAIState.PREIDLE);
               

                            if (holdingBaby)
                            {
                                //AND THEN THIS IS WHERE WE DROP THE BABY!!!
                                Debug.Log("found crib");

                                holdingBaby = false;

                                //UNCOMMENT ALL THIS STUFF TOO!!!

                                //               PlayerCatching player = playerWeAreCurrentlyChasing.GetComponent<PlayerCatching>();
                                //               player.Roomed();

                                GameManager.instance.crib.babyBedded(playerWeAreCurrentlyChasing.gameObject.GetComponent<PlayerControllerr>().colorId);
                                anim.SetBool("pickup", false);
                                photonView.RPC("drop", RpcTarget.AllBuffered);



                            }





                        }
                    }
                }






                break;





        }







    }

    public void PathfindToPos(Vector3 destination) {

        bool compatibleState = false;

        for (int i = 0; i < statesIntoAnsync.Length; i++)
        {
            BabysitterAIState state = statesIntoAnsync[i];

            if (state == currentState)
            {

                compatibleState = true;

            }

        }

        if (compatibleState)
        {
            nav.SetDestination(destination);

            
            photonView.RPC("changeState", RpcTarget.AllBuffered, (int)BabysitterAIState.PATHFIND);
            currentState = BabysitterAIState.PATHFIND;

        }









    }



    public void StopMoving()
    {

        nav.SetDestination(transform.position);


        //nav.isStopped = true;

        //nav.ResetPath();


        anim.SetBool("chasing", false);
    }


    void RandomAmountOfPointsToPatrol()
    {
 
        float randomPercent = UnityEngine.Random.Range(patrolPercentEachPatrolMinAndMax.x, patrolPercentEachPatrolMinAndMax.y);


        float roundedPercent = Mathf.Ceil(randomPercent * patrolPoints.Count);


        calculatedPatrolPointsToVisit = (int)(roundedPercent);

        Debug.Log("The babysitter will patrol " + calculatedPatrolPointsToVisit + " points!");


    }


    Transform ScanForPlayers()
    {

        if (holdingBaby)
        {
            return null;
        }

        Transform playerReturnVariable = null;


        GameObject[] players = GameObject.FindGameObjectsWithTag(tagToChase);


        GameObject closestPlayer = null;
        float closestDist = viewingDistance;


        for (int i = 0; i < players.Length; i++)
        {
            GameObject currentPlayer = players[i];

            if (currentPlayer.GetComponent<PlayerCatching>().grabbable)
            {
                Vector3 lookDirection = currentPlayer.transform.position - transform.position;

                float currentDist = Vector3.Magnitude(lookDirection);

                float angleDifference = Vector3.Angle(transform.forward, lookDirection);



                RaycastHit hit;


                if (Mathf.Abs(angleDifference) <= fieldOfViewDegrees / 2f)
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


        }




        return playerReturnVariable;
    }





    private void OnDrawGizmos()
    {

        Vector3 rightNormalFOV = Quaternion.Euler(0, fieldOfViewDegrees / 2f, 0) * transform.forward;
        Vector3 leftNormalFOV = Quaternion.Euler(0, -fieldOfViewDegrees / 2f, 0) * transform.forward;

        Vector3 sweepingNormalFOV = Quaternion.Euler(0, Mathf.Sin(Time.realtimeSinceStartup) * fieldOfViewDegrees / 2f, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + rightNormalFOV * viewingDistance);
        Gizmos.DrawLine(transform.position, transform.position + leftNormalFOV * viewingDistance);
        Gizmos.DrawLine(transform.position, transform.position + sweepingNormalFOV * viewingDistance);

        int arcSteps = 8;

        for (int i = 0; i < arcSteps; i++)
        {
            Vector3 from = Quaternion.Euler(0, (float)i / (float)arcSteps * fieldOfViewDegrees, 0) * leftNormalFOV;
            Vector3 to = Quaternion.Euler(0, (float)(i + 1) / (float)arcSteps * fieldOfViewDegrees, 0) * leftNormalFOV;


            Gizmos.DrawLine(transform.position + from * viewingDistance, transform.position + to * viewingDistance);


            //Gizmos.DrawLine(transform.position + from * catchingDistance, transform.position + to * catchingDistance);






        }


        for (int i = 0; i < patrolPoints.Count; i++)
        {

            Transform currentPoint = patrolPoints[i];

            Transform nextPoint = patrolPoints[(i + 1) % patrolPoints.Count];

            Gizmos.DrawSphere(currentPoint.position, .5f);

            Gizmos.DrawLine(currentPoint.position, nextPoint.position);



        }



        //Now we'll make a little visual but for the babysitter's escape range.
        //I want to do a striped circle around the babysitter, to represent the escape range.


        arcSteps = 32;
        for (int i = 0; i < arcSteps; i++)
        {

            Gizmos.color = Color.red;

            Vector3 from = Quaternion.Euler(0, (float)i / (float)arcSteps * 360 + Time.realtimeSinceStartup * 2, 0) * Vector3.forward;
            Vector3 to = Quaternion.Euler(0, (float)(i + 1) / (float)arcSteps * 360 + Time.realtimeSinceStartup * 2, 0) * Vector3.forward;


            Gizmos.DrawLine(transform.position + from * escapeDistance, transform.position + to * viewingDistance);

            Gizmos.DrawLine(transform.position + from * escapeDistance, transform.position + to * escapeDistance);


            Gizmos.color = Color.white;

            Gizmos.DrawLine(transform.position + from * catchingDistance, transform.position + to * catchingDistance);





        }



    }

    void babyOrbActive(bool on)
    {
        if (on )
        {
            int colorID = playerWeAreCurrentlyChasing.GetComponent<PlayerControllerr>().colorId;

            babyOrb.GetComponent<MeshRenderer>().material = playerMats[colorID];
            babyOrb.SetActive(true);
        }
        else
        {
            babyOrb.SetActive(false);
        }

    }

    [PunRPC]
    void pickUp()
    {
        // Toggle off all previous animations
       
        playerWeAreCurrentlyChasing.gameObject.GetComponent<PlayerCatching>().changeState(PlayerCatching.playerCatchState.caught);
        GameManager.instance.playerCaught = playerWeAreCurrentlyChasing.gameObject;
        StartCoroutine(displayOrb());
        //PathfindToPos(GameObject.FindGameObjectWithTag("Crib").transform.position);
        PathfindToPos(GameManager.instance.crib.placePos.position);
        Debug.Log("Crib:" + GameManager.instance.crib.transform.parent.position);
        holdingBaby = true;
    }
   

    private IEnumerator displayOrb()
    {
        yield return new WaitForSeconds(1.5f);
        if(currentState==BabysitterAIState.PICKUP)
        babyOrbActive(true);
    }

    [PunRPC]
    void drop()
    {
        
        playerWeAreCurrentlyChasing.gameObject.GetComponent<PlayerCatching>().changeState(PlayerCatching.playerCatchState.roomed);
        babyOrbActive(false);
    }

    [PunRPC]
    void changeState(int state)
    {
        
        currentState = (BabysitterAIState)state;

    }

    [PunRPC]
    public void SetTargetPlayer(int viewID)
    {
        
        if (viewID != -1)
        {
            PhotonView targetView = PhotonView.Find(viewID);
            playerWeAreCurrentlyChasing = targetView.transform;
        }
        else
        {
            playerWeAreCurrentlyChasing = null;
        }
    }
}
