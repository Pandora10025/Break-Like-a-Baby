using UnityEngine;
using TMPro;
public class CaughtPlayerOverlay : MonoBehaviour
{
    [SerializeField]
    GameObject overlay;
    [SerializeField]
    TextMeshProUGUI t;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        turnOn(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void overlayOn(float time, string playerName)
    {
        t.text = playerName;
        turnOn(true);
        
        Invoke("turnOff", time);
    }
    void turnOff()
    {
        turnOn(false);
    }
    void turnOn(bool on)
    {
        if (on)
        {
            overlay.SetActive(true);
        }
        else
        {
            overlay.SetActive(false);
        }
    }
}
