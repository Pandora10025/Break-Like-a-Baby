using UnityEngine;
using System.Collections;

public class SlowMotion : MonoBehaviour
{
    public float slowMotionFactor = 0.2f;  // The factor to slow down time
    public float slowMotionDuration = 5f;  // Duration of the slow-motion effect
    private float normalTimeScale = 1f;

    public bool slowMo;

    private void Start()
    {
        normalTimeScale = Time.timeScale;  // Store the normal time scale
    }


    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) // Trigger slow-motion on pressing 'S'
        {
            StartCoroutine(ActivateSlowMotion());
        }
    }
    */
    public IEnumerator ActivateSlowMotion()
    {
        slowMo = true;
        Time.timeScale = slowMotionFactor;  // Slow down time
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Adjust physics to match the time scale
        yield return new WaitForSeconds(slowMotionDuration); // Wait for the slow-motion duration
        Time.timeScale = normalTimeScale;  // Reset time scale to normal
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Reset physics delta time
    }
    public void StartSlowMotion()
    {
        StartCoroutine(ActivateSlowMotion());
    }
    public void StopSlowMotion()
    {
        if (slowMo)
        {
            slowMo = false;
            Time.timeScale = normalTimeScale;  // Reset time scale to normal
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // Reset physics delta time
        }
    }
}
