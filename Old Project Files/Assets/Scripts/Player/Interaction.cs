using UnityEngine;

public class Interaction : MonoBehaviour
{
    public Circle currentCircle;
  
   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Circle"))
        {
            currentCircle = other.GetComponent<Circle>();
            
        }
    }

        private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Circle"))
        {
            currentCircle = other.GetComponent<Circle>();
            currentCircle.UIbar.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Circle"))
        {
            currentCircle = null;
            currentCircle.UIbar.SetActive(false);
        }
    }
    /*
    private void Update()
    {

        if (currentCircle != null && Input.GetKeyDown(KeyCode.Space))
        {
            currentCircle.IncreaseCount();
        }

    }
    */

    public void TriggerBar()
    {
        currentCircle.IncreaseCount();
    }
}
