using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Photon.Pun;


public class vigControl : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private Vector3 babysitter, player;
    private Vignette vig;
    float speed = 2f;
     public bool on=true;
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
        

        

    }

    // Update is called once per frame
    void Update()
    {
        if (!volume.profile.TryGet(out Vignette vig)) // for e.g set vignette intensity to .4f
            return;
        float targetIntensity = ((distanceCheck() < 1f) && GetComponent<PhotonView>().IsMine && on) ? 0.4f : 0f;
        vig.intensity.value = Mathf.Lerp(vig.intensity.value, targetIntensity, Time.deltaTime * speed);
    }

    /// <summary>
    /// Method <c>distanceCheck</c> returns the distance between two vector3s as a float
    /// </summary>
    /// <returns></returns>
    private float distanceCheck()
    {
        babysitter = GameObject.FindGameObjectWithTag("Babysitter").transform.position;
        player = this.transform.position;
        float xDiff = babysitter.x - player.x;
        float yDiff = babysitter.y - player.y;
        return Mathf.Sqrt(Mathf.Pow(xDiff, 2) + Mathf.Pow(yDiff, 2));
    }
}
