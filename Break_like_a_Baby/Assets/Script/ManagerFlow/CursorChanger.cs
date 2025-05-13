using UnityEditor;
using UnityEngine;

public class CursorChanger : MonoBehaviour
{
    public static CursorChanger instance;
    [SerializeField] private Texture2D red, yellow;
    [SerializeField] private float cursorScale = 1024;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }

    private void Update()
    {
    }
    // Update is called once per frame
    public static void changeCursor(bool isRed)
    {
        
        //change cursor here
        var cursor = new UnityEngine.UIElements.Cursor();
        if (isRed) { 
            cursor.texture = instance.changeSize(instance.red); 
        } else {  
            cursor.texture = instance.changeSize(instance.yellow); 
        }

        //cursor.hotspot = new Vector2(0, cursor.texture.height);

        //for software (web)
        UnityEngine.Cursor.SetCursor(cursor.texture, cursor.hotspot, CursorMode.ForceSoftware);

        //for windows, mac, linux
        PlayerSettings.defaultCursor = cursor.texture;
    }

    private Texture2D changeSize(Texture2D cursorTexture)
    {
        int newResolution = (int) ((Screen.currentResolution.width) * cursorScale);
        cursorTexture.Reinitialize(newResolution, newResolution);
        return cursorTexture;
    }
}
