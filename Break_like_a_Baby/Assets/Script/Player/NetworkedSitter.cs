using UnityEngine;
using Photon.Pun;

public class NetworkedSitter : MonoBehaviourPun, IPunObservable
{
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    public float positionLerpSpeed = 10f;
    public float rotationLerpSpeed = 20f;

    public Animator anim;
    public bool networkedChasing;

    void Awake()
    {
        anim = GetComponent<Animator>();

        PhotonNetwork.SendRate = 100;          // Default is 50
        PhotonNetwork.SerializationRate = 100;
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            // Smooth position and rotation for remote clients
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.fixedDeltaTime * positionLerpSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.fixedDeltaTime * rotationLerpSpeed);

            anim.SetBool("chasing", networkedChasing);
        }
    }

    [PunRPC]
    void SyncAnimation(float velocity)
    {
        anim.SetFloat("Velocity", velocity);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(anim.GetBool("chasing"));
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            networkedChasing = (bool)stream.ReceiveNext();
        }
    }
}