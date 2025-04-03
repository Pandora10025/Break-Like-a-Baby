using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class NoiseRenderFeature : ScriptableRendererFeature
{

    [SerializeField] private Shader shader;
    [SerializeField] private Material noiseMaterial;
    private NoiseRenderPass noiseRenderPass;






    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

        if (noiseRenderPass == null)
        {
            return;
        }


        if ( renderingData.cameraData.cameraType == CameraType.Game )
        {
            renderer.EnqueuePass(noiseRenderPass);

            noiseRenderPass.ConfigureInput(ScriptableRenderPassInput.Normal);
        }


        



    }

    public override void Create()
    {
        if (shader == null)
        {
            return;
        }
        //noiseMaterial = new Material(shader);
        noiseRenderPass = new NoiseRenderPass(noiseMaterial);

        noiseRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingSkybox;
    }




    //protected override void Dispose(bool disposing)
    //{
    //    if (Application.isPlaying)
    //    {
    //        Destroy(noiseMaterial);
    //    }
    //    else
    //    {

    //        DestroyImmediate(noiseMaterial);

    //    }


    //}

}
