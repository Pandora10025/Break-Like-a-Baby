using System.Collections.Generic;
using UnityEngine;

public class BabySitterFootsteps : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource footstepSource;

    [Header("Footsteps")]
    [SerializeField] private List<AudioClip> tileFX;
    [SerializeField] private List<AudioClip> woodFX;

    [Header("Audio Clip")]
    [SerializeField] private List<AudioClip> footStepSFX = new List<AudioClip>();
    private int lastDrawnNum = 0, currDrawnNum = 0;

    enum Terrain
    {
        Snow, Wood, Empty
    }


    private void Update()
    {
        /*footstepSource.volume = Random.Range(0.2f, 0.8f);
        footstepSource.pitch = Random.Range(0.8f, 1.3f);*/
    }

    public void PlayFootStep()
    {
        do
        {
            currDrawnNum = (int)Random.Range(0, footStepSFX.Count);
        } while (currDrawnNum == lastDrawnNum);

        footstepSource.PlayOneShot(footStepSFX[currDrawnNum]);
        lastDrawnNum = currDrawnNum;
    }
}
