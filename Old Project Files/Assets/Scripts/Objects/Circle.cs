using UnityEngine;
using UnityEngine.UI;

public class Circle : MonoBehaviour
{
    public bool Activation = false;
    public float Count = 0;
    public float maxCount = 100; // Threshold to activate
    public float decreaseRate = 10f; // Rate at which Count decreases
    public Slider energyBar; // UI Slider for tracking Count
    public Vector3 newSize = new Vector3(1, 1, 1);
    public GameObject UIbar;

    private AudioManager audioSearch;
    private GameManager itemManager;

    private void Start()
    {
        energyBar.maxValue = maxCount;
        energyBar.value = Count;

        audioSearch = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        itemManager = FindAnyObjectByType<GameManager>();  
    }

    private void Update()
    {
        if (!Activation)
        {
            // Decrease Count over time, ensuring it doesn't drop below 0
            Count = Mathf.Max(0, Count - decreaseRate * Time.deltaTime * 2);
            energyBar.value = Count;
            // Check if Count reaches or exceeds maxCount and activate the Circle
             if (Count >= 99)
            {
                //transform.localScale = newSize;
                gameObject.SetActive(false);
                UIbar.SetActive(false);
                Activation = true;
                Debug.Log("Activated");
                itemManager.UpdateActivationCount();
            }
            
        }

        if (Count >= maxCount)
        {
            Activation = false;
        }
    }

    public void IncreaseCount()
    {
        if (!Activation)
        {
            Count += 10; // Amount increased per Space press
            Count = Mathf.Min(Count, maxCount);
            energyBar.value = Count;
            Debug.Log("Count:" + Count);

        }
    }
}
