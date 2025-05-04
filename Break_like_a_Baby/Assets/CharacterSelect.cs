using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public GameObject[] characterScreens;
    public GameObject screen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setScreen(0);
        setOverlay(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setScreen(int b)
    {
        for(int i=0;i<characterScreens.Length;i++)
        {

            characterScreens[i].SetActive(i == b);
            
        }
    }

    public void setOverlay(bool on)
    {
        screen.SetActive(on);
    }
}
