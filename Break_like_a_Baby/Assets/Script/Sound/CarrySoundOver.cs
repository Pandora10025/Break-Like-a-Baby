using Unity.VisualScripting;
using UnityEngine;

public class CarrySoundOver : MonoBehaviour
{
    public static CarrySoundOver instance;
    [SerializeField] private static int numOfInstance = 0;
    private AudioSource aud;
    private void Awake()
    {
        Debug.Log(numOfInstance);
        instance = this;
        aud = GetComponent<AudioSource>();
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(this);
        
    }

    private void Start()
    {
        if (numOfInstance == 1)
        {
            Destroy(this.gameObject);
            Destroy(this);
        }
        numOfInstance++;
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
