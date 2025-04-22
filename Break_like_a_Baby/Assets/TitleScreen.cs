using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] GameObject screenText;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject button, buttonB;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screenText.SetActive(false);
        rb.isKinematic = true;
        buttonB.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pressedScreen()
    {
        
        buttonB.SetActive(true);
        Invoke("show", 0.25f);
        button.SetActive(false);

    }

    void show()
    {
        screenText.SetActive(true);
        rb.isKinematic = false;
        gameObject.SetActive(false);
    }

}
