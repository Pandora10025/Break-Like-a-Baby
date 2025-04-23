using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;


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
        stateLastFrame = (int) babysitter.GetComponent<BabySitterAI>().currentState;

    }

    private void Update()
    {
        stateThisFrame = (int)babysitter.GetComponent<BabySitterAI>().currentState;

        //if the babysitter's state is different from last frame then...
        if (stateThisFrame != stateLastFrame)
        {
            if (stateThisFrame == (int)BabySitterAI.BabysitterAIState.CHASE)
            {//enums are default static
                AudioSource.PlayClipAtPoint(seenSFX, (Vector3)babysitter.transform.position);
                Debug.Log("BAM!");
            }
        }


        stateLastFrame = (int)babysitter.GetComponent<BabySitterAI>().currentState;
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

    public void PauseMusic()
    {
        music.Stop();
    }
}
