using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] GameObject screenText;
    [SerializeField] Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screenText.SetActive(false);
        rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pressedScreen()
    {
        screenText.SetActive(true);
        rb.isKinematic = false;
        gameObject.SetActive(false);
    }

}
