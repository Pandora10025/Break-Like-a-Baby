using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class CutsceneAudio : MonoBehaviour
{
    [Header("Add new sound clips here!")]
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    [SerializeField] private AudioSource aud;
    private int pickedClip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pickedClip = UnityEngine.Random.Range(0, clips.Count);
        aud.PlayOneShot(clips[pickedClip]);
        Debug.Log("picked clip: " + pickedClip);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
