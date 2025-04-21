using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreen : MonoBehaviour
{
  
    [SerializeField] List<GameObject> slides;

    
    [SerializeField] private float autoSwitchTime = 60f;

    private int currentSlideIndex = 0;
    private float timer;

    [SerializeField] RectTransform[] buttons;
    [SerializeField] RectTransform highlight;
    private void Start()
    {
        ShowSlide(currentSlideIndex);
        timer = autoSwitchTime;
    }

    private void Update()
    {
        HandleInput();

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            NextSlide();
            timer = autoSwitchTime;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            timer = autoSwitchTime;
            NextSlide();
            ResetTimer();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            timer = autoSwitchTime;
            PreviousSlide();
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        timer = autoSwitchTime;
    }

    public void ShowSlide(int index)
    {
        for (int i = 0; i < slides.Count; i++)
        {
           if(i == index)
            {
                slides[i].SetActive(true);
                highlight.position = buttons[i].position;
            }
            else
            {
                slides[i].SetActive(false);
            }
           
        }
    }

    public void NextSlide()
    {
        currentSlideIndex = (currentSlideIndex + 1) % slides.Count;
        ShowSlide(currentSlideIndex);
    }

    public void PreviousSlide()
    {
        currentSlideIndex = (currentSlideIndex - 1 + slides.Count) % slides.Count;
        ShowSlide(currentSlideIndex);
    }
}
