using UnityEngine;

public class CreditScreen : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toggleOn(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggleOn(bool b)
    {
        transform.GetChild(0).gameObject.SetActive(b);
    }
}
