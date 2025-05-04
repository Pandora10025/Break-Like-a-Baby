using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class vigControl : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private Vector3 babysitter, player;
    private Vignette vig;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        if(distanceCheck() < 3f)
        {
            if (volume.profile.TryGet(out Vignette vig)) // for e.g set vignette intensity to .4f
            {
                vig.intensity.value = .4f;
            }
        }
        else
        {
            if (volume.profile.TryGet(out Vignette vig)) // for e.g set vignette intensity to .4f
            {
                vig.intensity.value = 0.0f;
            }
        }
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
