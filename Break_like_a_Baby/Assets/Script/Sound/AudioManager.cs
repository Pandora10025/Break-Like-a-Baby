using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Audio Sources")]

    [SerializeField] private AudioSource[] musics;
    [SerializeField] private AudioSource ambience;
    [SerializeField] private AudioSource footsteps;
    [SerializeField] private AudioSource SFX;
    [SerializeField] private AudioMixer mixer;

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
    public bool populated = false;

    //for my reference only
    //sfx zone
    private enum materialType
    {
        glass,
        wood,
        metal,
        porcelain,
        soft
    }

    //bgm zone
    private int _bgmState = (int) bgmState.intro;
    public enum bgmState
    {
        intro,
        mid,
        end
    }

    /// <summary>
    /// Method <c>switchState</c> changes local bgmState and music to inputted <paramref name="state"/>. <br/>
    /// Call with AudioManager.instance.bgmState.x where x is intro, mid, or end.
    /// </summary>
    /// <param name="state"></param> use AudioManager.instance.bgmState.x; x = intro, mid, or end
    public void switchState(bgmState state)
    {
        _bgmState = (int)state;
    }

    private void Start()
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
        //PlayMusic();
        Populate();
    }

    public void Populate()
    {
        List<GameObject> bObjects = ObjectManager.instance.getBObjs();

        //set key/value pair based off of material set on value set in scene
        foreach (GameObject g in bObjects)
        {
            sfxDict[g.name] = breakSfxs[g.transform.GetChild(0).GetComponent<BreakableObject>().getMatType()];
            Debug.Log("inside dict: " + g.name);
        }
        Debug.Log("pop = true");
        populated = true;
    }

    //make a coroutine that does a transition every measure. i think separate instruments still.

    
    /*public void PlayMusic()
    {
        if (!music.isPlaying)
        {
            music.clip = backgroundMusic;
            music.Play();
        }
    }*/

    public void PlayAmbience()
    {
        if (!ambience.isPlaying)
        {
            ambience.clip = ambienceSFX;
            ambience.Play();

        }
    }

    /// <summary>
    /// Method <c>PlaySFX</c> deprecated. Do not call.
    /// </summary>
    /// <param name="clip"></param> audio clip to be played
    public void PlaySFX(AudioClip clip)
    {
        SFX.PlayOneShot(clip);
    }

    /// <summary>
    /// Method <c>PlaySFX</c> is an overload of <c>PlaySFX(AudioClip clip)</c> 
    /// As per inbuilt Unity method <c>PlayClipAtPoint</c>, <br/> it will create a 
    /// temporary AudioClip and place it at <paramref name="position"/>. The sound
    /// will be based off of the inputted <paramref name="name"/>. <b>Used for objects.</b>
    /// </summary>
    /// if game is lagging, this might be the culprit. it's quite costly.
    public void PlaySFX(string name, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(sfxDict[name], position);
    }

    /// <summary>
    /// Method <c>PlaySFX</c> is an overload that takes in <paramref name="position"/> of the <b>Player</b> and <paramref name="aud"/> on the <b>Player</b>. <br/>
    /// It will create a temporary AudioClip and place it at the <b>Player's</b> location.
    /// </summary>
    /// <param name="position"></param> Current position of <b>Player</b>
    public void PlaySFX(AudioSource aud, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(aud.clip, position);
    }

    /*public void PauseMusic()
    {
        music.Stop();
    }*/

}
