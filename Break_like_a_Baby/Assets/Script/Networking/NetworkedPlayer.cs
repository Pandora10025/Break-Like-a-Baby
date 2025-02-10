using UnityEngine;
using Photon.Pun;

public class NetworkedPlayer : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;
    private Vector3 networkVelocity;
    private Vector3 networkPosition;
    public float positionLerpSpeed = 10f;  // Smooth position updates
    public float rotationLerpSpeed = 20f; // Adjust for smoothness
    Quaternion networkRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        PhotonNetwork.SendRate = 100;          // Default is 50, lower reduces network lag
        PhotonNetwork.SerializationRate = 100;

    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            // Smooth movement for remote players
            //rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, networkVelocity, Time.deltaTime * positionLerpSpeed);

            rb.position = Vector3.Lerp(rb.position, networkPosition, Time.deltaTime * positionLerpSpeed);

            networkRotation = Quaternion.Normalize(networkRotation);
            rb.rotation = Quaternion.Slerp(rb.rotation, networkRotation, Time.deltaTime * rotationLerpSpeed);


        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            
            //stream.SendNext(rb.linearVelocity);
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
        }
        else
        {
            //networkVelocity = (Vector3)stream.ReceiveNext();
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
