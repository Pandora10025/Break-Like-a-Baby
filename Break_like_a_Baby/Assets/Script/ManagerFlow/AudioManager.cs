using UnityEngine;

public class AudioManager : MonoBehaviour
{
   public static AudioManager instance;

    [Header("Audio Sources")]

    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource ambience;
    [SerializeField] private AudioSource footsteps;
    [SerializeField] private AudioSource SFX;

    [Space]

    public AudioClip backgroundMusic;
    public AudioClip gameOverMusic;

    public AudioClip ambienceSFX;
    public AudioClip gameOverSFX;
    public AudioClip startGameSfx;
    public AudioClip hitSFX;
    public AudioClip caughtSFX;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        PlayAmbience();
        PlayMusic();



    }

    
    public void PlayMusic()
    {
        if (!music.isPlaying)
        {
            music.clip = backgroundMusic;
            music.Play();
        }
    }

    public void PlayAmbience()
    {
        if(!ambience.isPlaying)
        {
            ambience.clip = ambienceSFX;
            ambience.Play();

        }
    }

    public void PlaySFX(AudioClip clip)
    {
        SFX.PlayOneShot(clip);
    }

    public void PauseMusic()
    {
        music.Stop();
    }
}
