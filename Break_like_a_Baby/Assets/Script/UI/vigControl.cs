using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Photon.Pun;


public class vigControl : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private Vector3 player;
    Transform babysitter;
    private Vignette vig;
    float speed = 8f;
     public bool on=true;
    public bool vigOn = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        on = true;
        GameObject g = GameObject.Find("Global Volume");
        if (g != null)
        {
            volume = g.GetComponent<Volume>();
        }
        volume.profile.components.ForEach(c => Debug.Log(c.GetType().Name)); // displays the volumes components name, for e.g: Fog, HDRISKY, Bloom
        if (!volume.profile.TryGet(out vig)) // for e.g set vignette intensity to .4f
            vigOn = false;

        babysitter = GameObject.FindGameObjectWithTag("Babysitter").transform;
        if (!babysitter) vigOn = false;
       

    }

    // Update is called once per frame
    void Update()
    {
        if (!vigOn)
        {
            return;
        }
        float targetIntensity = ((distanceCheck() < 2f) && GetComponent<PhotonView>().IsMine && on) ? 0.4f : 0f;
        vig.intensity.value = Mathf.Lerp(vig.intensity.value, targetIntensity, Time.deltaTime * speed);
    }

    /// <summary>
    /// Method <c>distanceCheck</c> returns the distance between two vector3s as a float
    /// </summary>
    /// <returns></returns>
    private float distanceCheck()
    {
        Vector3 babysitter = this.babysitter.position;
        player = this.transform.position;

        return Vector3.Distance(babysitter, player); 
    }
}
