using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    public List<AudioClip> breakSfxs;
    //data structures for holding audio clips and assigning
    private static Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
    private enum materialType
    {
        glass,
        wood,
        metal,
        porcelain,
        soft
    }


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

        List<GameObject> bObjects = ObjectManager.getBObjNames();
        //set key/value pair based off of material set on value set in scene
        foreach(GameObject g in bObjects)
        {
            sfxDict[g.name] = breakSfxs[g.GetComponent<BreakableObject>().getMatType()];
        }

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

    /// <summary>
    /// Method <c>PlaySFX</c> is an overload of <c>PlaySFX(AudioClip clip)</c> 
    /// As per inbuilt Unity method <c>PlayClipAtPoint</c>, <br/> it will create a 
    /// temporary AudioClip and place it at <paramref name="position"/>. The sound
    /// will be based off of the inputted <paramref name="name"/>.
    /// </summary>
    /// if game is lagging, this might be the culprit. it's quite costly.
    public void PlaySFX(string name, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(sfxDict[name], position);
    }

    public void PauseMusic()
    {
        music.Stop();
    }

}
