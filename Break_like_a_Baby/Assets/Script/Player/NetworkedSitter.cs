using UnityEngine;
using Photon.Pun;

public class NetworkedSitter : MonoBehaviourPun, IPunObservable
{
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private Vector3 lastReceivedPosition;
    private Quaternion lastReceivedRotation;

    private float distance;
    private float angle;
    private float lag;

    private float interpolationTime = 0f;
    private float timeSinceLastUpdate = 0f;

    public float positionLerpSpeed = 10f;
    public float rotationLerpSpeed = 20f;

    void Awake()
    {
        PhotonNetwork.SendRate = 100;          // Default is 50, lower reduces network lag
        PhotonNetwork.SerializationRate = 100;
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            timeSinceLastUpdate += Time.fixedDeltaTime;

            float lerpFactor = Mathf.Clamp01(timeSinceLastUpdate / interpolationTime);

            transform.position = Vector3.Lerp(transform.position, networkPosition, lerpFactor * positionLerpSpeed * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, lerpFactor * rotationLerpSpeed * Time.fixedDeltaTime);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            lastReceivedPosition = networkPosition;
            lastReceivedRotation = networkRotation;

            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();

            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            interpolationTime = lag;

            timeSinceLastUpdate = 0f;

            // Optional: Predict slightly forward based on velocity (if needed)
            // networkPosition += velocity * lag;
        }
    }
}