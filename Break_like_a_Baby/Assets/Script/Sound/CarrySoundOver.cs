using Unity.VisualScripting;
using UnityEngine;

public class CarrySoundOver : MonoBehaviour
{
    public static CarrySoundOver instance;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    public void stopMusic()
    {
        Destroy(this);
    }

    

}
