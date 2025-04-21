using UnityEngine;

public class mouseInput : MonoBehaviour
{
    public float parallaxAmount = 0.5f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2;

        Vector3 offset = new Vector3(mouseX, mouseY, 0) * parallaxAmount;
        transform.position = startPos + offset;
    }
}
