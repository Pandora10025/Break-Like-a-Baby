using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;


[ExecuteInEditMode, ImageEffectAllowedInSceneView]

public class V2OutlinePostProcessing : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {

        cam = GetComponent<Camera>();
         


        cam.depthTextureMode |= DepthTextureMode.DepthNormals;

    }


    public Material effectMaterial;

    [Range(0, 10)] public int iterations = 1;
    [Range(0, 5)] public int downRes;


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int width = source.width >> downRes;
        int height = source.height >> downRes;

        RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);

        Matrix4x4 clipToView = GL.GetGPUProjectionMatrix(cam.projectionMatrix, true).inverse;
        effectMaterial.SetMatrix("_ClipToView", clipToView);

        Graphics.Blit(source, temp);

        for (int i = 0; i < iterations; i++)
        {
            RenderTexture temp2 = RenderTexture.GetTemporary(width, height);

            Graphics.Blit(temp, temp2, effectMaterial);

            RenderTexture.ReleaseTemporary(temp);

            temp = temp2;

        }


        Graphics.Blit(temp, destination);


        //Graphics.Blit(source, destination, effectMaterial);

        RenderTexture.ReleaseTemporary(temp);
    }
}
