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
    }

    // Update is called once per frame
   public static void changeCursor(bool isRed)
    {
        //change cursor here
        var cursor = new UnityEngine.UIElements.Cursor();
        if (isRed) { 
            cursor.texture = instance.red; 
        } else {  
            cursor.texture = instance.yellow; 
        }

        cursor.hotspot = new Vector2(0, cursor.texture.height);

        //for software (web)
        UnityEngine.Cursor.SetCursor(cursor.texture, cursor.hotspot, CursorMode.Auto);

        //for windows, mac, linux
        PlayerSettings.defaultCursor = cursor.texture;
    }
}
