using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.XR.XRDisplaySubsystem;

public class OutlineRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader shader;
    [SerializeField] private Material outlineMaterial;
    private OutlineRenderPass outlineRenderPass;






    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

        if (outlineRenderPass == null)
        {
            return;
        }


        if ( renderingData.cameraData.cameraType == CameraType.Game)
        {
            outlineRenderPass.ConfigureInput(ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Motion);

           
            renderer.EnqueuePass(outlineRenderPass);

            
        }






    }

    public override void Create()
    {
        if (shader == null)
        {
            return;
        }
        //outlineMaterial = new Material(shader);
        outlineRenderPass = new OutlineRenderPass(outlineMaterial);

        outlineRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingGbuffer;
    }




    //protected override void Dispose(bool disposing)
    //{
    //    if (Application.isPlaying)
    //    {
    //        Destroy(outlineMaterial);
    //    }
    //    else
    //    {

    //        DestroyImmediate(outlineMaterial);

    //    }


    //}
}
