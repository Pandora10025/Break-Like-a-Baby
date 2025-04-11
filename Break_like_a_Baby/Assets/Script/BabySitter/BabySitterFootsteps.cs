using System.Collections.Generic;
using UnityEngine;

public class BabySitterFootsteps : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource footstepSource;

    [Header("Footsteps")]
    public List<AudioClip> tileFX;
    public List<AudioClip> woodFX;

    [Header("Audio Clip")]
    public AudioClip footStep1SFX;
    public AudioClip footStep2SFX;
    public AudioClip footStep3SFX;
    public AudioClip footStep4SFX;
    public AudioClip footStep5SFX;

    enum Terrain
    {
        Snow, Wood, Empty
    }


    private void Update()
    {
        footstepSource.volume = Random.Range(0.2f, 0.8f);
        footstepSource.pitch = Random.Range(0.8f, 1.3f);
    }

    public void PlayFootStep1()
    {
        footstepSource.PlayOneShot(footStep1SFX);
    }

    public void PlayFootStep2()
    {
        footstepSource.PlayOneShot(footStep2SFX);
    }

    public void PlayFootStep3()
    {
        footstepSource.PlayOneShot(footStep3SFX);
    }

    public void PlayFootStep4()
    {
        footstepSource.PlayOneShot(footStep4SFX);
    }
}
