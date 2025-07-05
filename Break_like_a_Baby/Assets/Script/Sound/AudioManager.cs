using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class VolMan
{
    private float currVolume { get; set; }
    private float minVolume {  get; set; }
    private float maxVolume { get; set; }
    private bool changing {  get; set; }

    public VolMan() { }
    public VolMan(float _cv, float _minV, float _maxV)
    {
        currVolume = _cv;
        minVolume = _cv;
        maxVolume = _cv;
        changing = false;
    }

    
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Background Music Sources")]

    [SerializeField] private AudioSource[] musics;
    private Dictionary<AudioSource, VolMan> volDict = new Dictionary<AudioSource, VolMan>();
    
    [Space]
    [Header("Other Sources")]

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
    public AudioClip seenSFX;

    public List<AudioClip> breakSfxs;
    //data structures for holding audio clips and assigning
    private static Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();
    public bool populated = false;

    [Header("Babysitter")]
    [SerializeField] private GameObject babysitter;
    private int stateLastFrame = 0;
    private int stateThisFrame = 0;

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

    //for the music sources
    private enum instrument
    {
        melody,
        middle,
        drums,
        quiet
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
    }
    private void Start()
    {
        
        PlayAmbience();
        //PlayMusic();
        stateLastFrame = (int) babysitter.GetComponent<BabySitterAI>().currentState;

    }

    private void Update()
    {
        stateThisFrame = (int)babysitter.GetComponent<BabySitterAI>().currentState;

        //if the babysitter's state is different from last frame then...
        if (stateThisFrame != stateLastFrame)
        { 
            if(stateThisFrame == (int)BabySitterAI.BabysitterAIState.CHASE)
            {
                AudioSource.PlayClipAtPoint(seenSFX, (Vector3)babysitter.transform.position);
            }
            else if(stateThisFrame == (int)BabySitterAI.BabysitterAIState.PICKUP)
            {
                AudioSource.PlayClipAtPoint(caughtSFX, (Vector3)babysitter.transform.position);
            }
        }

        if (stateThisFrame == (int)BabySitterAI.BabysitterAIState.CHASE)
        {//enums are default static
            //fade this in later
            musics[(int)instrument.drums].volume = 1;
            musics[(int)instrument.middle].volume = 1;
        }
        else
        {
            musics[(int)instrument.drums].volume = 0;
            musics[(int)instrument.middle].volume = 0.5f;
        }

        stateLastFrame = (int)babysitter.GetComponent<BabySitterAI>().currentState;

        //and if shift is being pressed than play the melody
        
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


    /// <summary>
    /// Method <c>Populate</c> adds all active objects to the list of sounds available,
    /// and sets the type of sounds accordingly.
    /// </summary>
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

    public void PlayAmbience()
    {
        if (!ambience.isPlaying)
        {
            ambience.clip = ambienceSFX;
            ambience.Play();

        }
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
    public void PlaySFX(AudioClip clip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(clip, position);
    }

    /*public void PauseMusic()
    {
        music.Stop();
    }*/

    /// <summary>
    /// Overload of the static definition PlayCLipAtPoint belonging to teh class AudioClip.
    /// This version should be used exclusively for pitch shifting on the PlayerBreak script
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="position"></param>
    /// <param name="volume"></param>
    public static void PlayClipAtPoint(AudioClip clip, Vector3 position, [UnityEngine.Internal.DefaultValue("1.0F")] float volume, float pitch)
    {
        GameObject gameObject = new GameObject("One shot audio");
        gameObject.transform.position = position;
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();
        Object.Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }

}
