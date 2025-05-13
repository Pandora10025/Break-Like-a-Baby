using UnityEditor;
using UnityEngine;

public class CursorChanger : MonoBehaviour
{
    public static CursorChanger instance;
    [SerializeField] private Texture2D red, yellow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        changeCursor(false);
    }

    // Update is called once per frame
    public static void changeCursor(bool isRed)
    {
       
        Texture2D cur;

        if (isRed)
        {
            cur = instance.red;
        }
        else
        {
            cur = instance.yellow;
        }

        Cursor.SetCursor(cur, Vector2.zero, CursorMode.ForceSoftware);
        Cursor.visible = true; 
        Cursor.lockState = CursorLockMode.None;
    }
}
