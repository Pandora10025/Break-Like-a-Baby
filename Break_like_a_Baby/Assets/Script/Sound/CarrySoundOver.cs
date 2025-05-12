using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarrySoundOver : MonoBehaviour
{
    public static CarrySoundOver instance;
    [SerializeField] private static int numOfInstance = 0;
    private AudioSource aud;
    private void Awake()
    {
      
        
    }

    private void Start()
    {
        name = name + numOfInstance.ToString();

        if (numOfInstance >= 1)
        {

            //numOfInstance -= 1;
            Destroy(this.gameObject);
            Destroy(this);

        }
        else {
            Debug.Log(numOfInstance);
            instance = this;
            aud = GetComponent<AudioSource>();
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(this);


            numOfInstance++;

            SceneManager.sceneLoaded += OnLevelFinishedLoading;


        }



    }

    public void PlayMusic()
    {
        if (aud.isPlaying) return;
        aud.Play();
    }

    public void StopMusic() 
    {
        if(aud)
        aud.Stop();
    }





    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Level Loaded");
        Debug.Log(scene.name);
        Debug.Log(mode);

        if (scene.name == "Arnav_Implement")
        {
            Debug.Log("STOP THE MUSIC");

            StopMusic();

        }
        else { 
            PlayMusic();
        }




    }




}
