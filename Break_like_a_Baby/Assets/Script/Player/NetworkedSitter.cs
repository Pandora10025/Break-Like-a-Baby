using UnityEngine;
using Photon.Pun;

public class NetworkedSitter : MonoBehaviourPun, IPunObservable
{
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    public float positionLerpSpeed = 10f;
    public float rotationLerpSpeed = 20f;

    //public Animator anim;
    //public bool networkedChasing;
    //public bool networkedPickup;
    //public bool networkedPrePickup;
    //public bool networkedPatrol;
    //public bool networkedPrePatrol;
    //public bool networkedPreIdle;


    void Awake()
    {
        //anim = GetComponent<Animator>();
        PhotonNetwork.SendRate = 100;          // Default is 50, lower reduces network lag
        PhotonNetwork.SerializationRate = 100;
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            // Smooth position and rotation for remote clients
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.fixedDeltaTime * positionLerpSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.fixedDeltaTime * rotationLerpSpeed);

            //anim.SetBool("chasing", networkedChasing);
            //anim.SetBool("patrol", networkedPatrol);
            //anim.SetBool("prepatrol", networkedPrePatrol);
            //anim.SetBool("preidle", networkedPreIdle);
            //anim.SetBool("pickup", networkedPickup);
            //anim.SetBool("prepickup", networkedPrePickup);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            //stream.SendNext(anim.GetBool("chasing"));
            //stream.SendNext(anim.GetBool("patrol"));
            //stream.SendNext(anim.GetBool("prepatrol"));
            //stream.SendNext(anim.GetBool("preidle"));
            //stream.SendNext(anim.GetBool("pickup"));
            //stream.SendNext(anim.GetBool("prepickup"));

        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            //networkedChasing = (bool)stream.ReceiveNext();
            //networkedPatrol= (bool)stream.ReceiveNext();
            //networkedPrePatrol = (bool)stream.ReceiveNext();
            //networkedPreIdle = (bool)stream.ReceiveNext();
            //networkedPickup = (bool)stream.ReceiveNext();
            //networkedPrePickup = (bool)stream.ReceiveNext();
        }
    }
}