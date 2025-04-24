using UnityEngine;

public class ImageScale : MonoBehaviour
{
    public float pulseSpeed = 1f;        // Speed of pulsing
    public float scaleAmount = 0.1f;     // Max scale increase (e.g., 0.1 = 10% bigger)

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * scaleAmount;
        transform.localScale = originalScale * scale;
    }
}
