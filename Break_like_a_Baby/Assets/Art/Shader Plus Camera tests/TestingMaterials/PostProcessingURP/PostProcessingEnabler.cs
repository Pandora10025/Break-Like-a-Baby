using UnityEngine;

public class PostProcessingEnabler : MonoBehaviour
{
    private Camera cam;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {

        cam = GetComponent<Camera>();



        cam.depthTextureMode |= DepthTextureMode.DepthNormals;

    }


        // Update is called once per frame
        void Update()
    {
        
    }
}
