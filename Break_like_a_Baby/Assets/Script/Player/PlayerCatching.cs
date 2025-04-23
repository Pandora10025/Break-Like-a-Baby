using UnityEngine;
using Photon.Pun;

public class PlayerCatching : MonoBehaviourPun
{
    public enum playerCatchState
    {
        free,
        caught,
        roomed
    }
    public playerCatchState catchState;

    public PlayerControllerr playerC;

    int ogSpriteOrder;
    [SerializeField] int pushedSpriteOrder = -10;

    SpriteRenderer mySpr;

    public bool grabbable = true;

    public int catchCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        catchState = playerCatchState.free;
        playerC = GetComponent<PlayerControllerr>();
        mySpr = GetComponent<SpriteRenderer>();
        ogSpriteOrder = mySpr.sortingOrder;
        grabbable = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(catchState);
        }
        if (catchState == playerCatchState.caught)
        {
            Debug.Log("aaaaa");
            Vector3 babySitterPosition = GameManager.instance.babySitter.transform.position;
            transform.position = new Vector3(babySitterPosition.x, transform.position.y, babySitterPosition.z);
        }
    }

    public void changeState(playerCatchState pc)
    {
        catchState = pc;
        switch (pc)
        {

            case playerCatchState.free:
                playerC.enableMove = true;
                GetComponent<Collider>().enabled = true;
                transform.position = new Vector3(GameManager.instance.respawnPos.position.x, transform.position.y, GameManager.instance.respawnPos.position.z);
                mySpr.enabled = true;
                grabbable = true;
                GetComponent<PlayerBreak>().canBreak = true;
                break;
            case playerCatchState.caught:
                //UI Change to be added
                Debug.Log("Catch");
                playerC.enableMove = false;
                GetComponent<Collider>().enabled = false;
                //mySpr.sortingOrder = pushedSpriteOrder;
                mySpr.enabled = false;
                GameManager.instance.caughtPlayerOverlay(photonView.ViewID);
                catchCount++;
                break;
            case playerCatchState.roomed:
                GetComponent<PlayerBreak>().canBreak = false;
                playerC.enableMove = false;
                GetComponent<Collider>().enabled = false;
                //mySpr.sortingOrder = pushedSpriteOrder;
                GameManager.instance.playerRoomed();
                mySpr.enabled = false;
                grabbable = false;
                GetComponent<Rigidbody>().linearVelocity = new Vector3(0f, 0f, 0f);
                Vector3 cribPos = GameManager.instance.crib.transform.position;
                transform.position = new Vector3(cribPos.x, transform.position.y, cribPos.z);
                break;
        }

    }
}
