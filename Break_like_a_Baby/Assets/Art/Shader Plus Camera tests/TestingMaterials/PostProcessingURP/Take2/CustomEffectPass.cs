using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class CustomEffectPass : ScriptableRenderPass {
    Material effectMaterial;
    RTHandle source;
    RTHandle tempTexture;

    public CustomEffectPass(Material material) {
        effectMaterial = material;
        // initialize a handle for a temporary texture
        tempTexture = RTHandles.Alloc("_TempColorTexture", name: "_TempColorTexture");
    }

    //public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
    //    // this sets 'source' to the current camera color target
    //    source = renderingData.cameraData.renderer.cameraColorTargetHandle;
    //}

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        base.RecordRenderGraph(renderGraph, frameData);
    }



    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
        if (effectMaterial == null)
            return;

        CommandBuffer cmd = CommandBufferPool.Get("CustomEffect");

        // acquire a temporary render texture
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        cmd.GetTemporaryRT( Shader.PropertyToID(tempTexture.name) , descriptor);

        // first blit: source -> temp with the custom material
        //Blit(cmd, source, tempTexture, effectMaterial, 0);
        Blitter.BlitCameraTexture(cmd, source, tempTexture, effectMaterial, 0);
        // second blit: temp -> source (apply the effect back into the camera target)
        //Blit(cmd, tempTexture.nameID, source);

        Blitter.BlitCameraTexture(cmd, tempTexture, source);

        


        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd) {
        cmd.ReleaseTemporaryRT(Shader.PropertyToID(tempTexture.name));
    }
}
