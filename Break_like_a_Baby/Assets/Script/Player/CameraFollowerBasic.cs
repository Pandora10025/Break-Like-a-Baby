using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraFollowerBasic : MonoBehaviour
{
    public float camDist = 10f;

    public Vector3 offsetDirection = Vector3.up;

   public Transform followTarget;

    

    // Start is called before the first frame update
    void Start()
    {

        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (followTarget != null)
        {
            transform.position = followTarget.position + offsetDirection.normalized * camDist;

            transform.LookAt(followTarget.position);

        }
        else
        {

            followTarget = FindLocalPlayer().transform;
        }
    }

    GameObject FindLocalPlayer()
    {
        GameObject[] players =GameObject.FindGameObjectsWithTag("Player");
        GameObject localPlayer = null;
        foreach(GameObject player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv!=null && pv.IsMine)
            {
                localPlayer = player;
                break;
            }
        }
        return localPlayer;

    }
}
