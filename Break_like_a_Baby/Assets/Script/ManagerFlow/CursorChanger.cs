using UnityEditor;
using UnityEngine;

public class CursorChanger : MonoBehaviour
{
    public static CursorChanger instance;
    [SerializeField] private Texture2D red, yellow;
    [SerializeField]  Texture2D[] redS, yellowS;

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

        int i = 0;

        if (Screen.width < 1000) i = 0;
        else if (Screen.width < 2000) i = 1;
        else i = 2;
        Debug.Log(i);

        if (isRed)
        {
            cur = instance.redS[i];
        }
        else
        {
            cur = instance.yellowS[i];
        }

        Cursor.SetCursor(cur, Vector2.zero, CursorMode.ForceSoftware);
        Cursor.visible = true; 
        Cursor.lockState = CursorLockMode.None;
    }
}
