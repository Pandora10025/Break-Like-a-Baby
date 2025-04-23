using Unity.VisualScripting;
using UnityEngine;

public class CarrySoundOver : MonoBehaviour
{
    public static CarrySoundOver instance;
    private AudioSource aud;
    private void Awake()
    {
        instance = this;
        aud = GetComponent<AudioSource>();
        DontDestroyOnLoad(this);
    }

    public void PlayMusic()
    {
        if (aud.isPlaying) return;
        aud.Play();
    }

    public void StopMusic() 
    {
        aud.Stop();
    }





}
