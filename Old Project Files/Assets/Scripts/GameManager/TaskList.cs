using UnityEngine;

public class TaskList : MonoBehaviour
{
    public bool isOpen;
    public bool isOpened;
    public GameObject notifications;
    public GameObject Icons;
    public GameObject backDrop;

    private Animator anim;
    private AudioManager audioSearch;
    void Start()
    {
        anim = GetComponent<Animator>();
        isOpened = false;
        //notifications.SetActive(true);
        //Icons.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen && !isOpened)
        {
            anim.SetTrigger("Open");
            isOpened = true;
            notifications.SetActive(false);
            Icons.SetActive(false);
            backDrop.SetActive(true);
            AudioManager.instance.ApplyLowPassFilter(true, 500f);
        }

        if (!isOpen && isOpened)
        {
            anim.SetTrigger("Close");
            isOpened = false;
            notifications.SetActive(true);
            Icons.SetActive(true);
            backDrop?.SetActive(false);
            AudioManager.instance.ApplyLowPassFilter(false);
        }
    }

    public void ToggleTaskList()
    {
        isOpen = !isOpen;
    }
}
