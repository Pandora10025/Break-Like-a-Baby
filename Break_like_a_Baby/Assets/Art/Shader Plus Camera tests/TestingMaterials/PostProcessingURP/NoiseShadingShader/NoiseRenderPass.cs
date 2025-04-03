using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class NoiseRenderPass : ScriptableRenderPass
{

    private Material material;
    private RenderTextureDescriptor noiseTextureDescriptor;

    private const string k_NoiseTextureName = "_NoiseTexture";
    private const string k_NoisePassName = "NoiseRenderPass";

    private static readonly int brightnessUpperThresholdId = Shader.PropertyToID("brightnessUpperThreshold");
    private static readonly int brightnessLowerThresholdId = Shader.PropertyToID("brightnessLowerThreshold");
    private static readonly int resolutionId = Shader.PropertyToID("resolution");




    public NoiseRenderPass( Material newMaterial ) { 
    
        this.material = newMaterial;

        noiseTextureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);



    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        TextureHandle srcCamColor = resourceData.activeColorTexture;
        TextureHandle dst = UniversalRenderer.CreateRenderGraphTexture(renderGraph, noiseTextureDescriptor, k_NoiseTextureName, false);


        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();


        //The following line ensures that the render pass doesn't blit
        // from the back buffer

        if (resourceData.isActiveTargetBackBuffer)
            return;



        //Set the blur texture size to be the same as the camera target size.
        noiseTextureDescriptor.width = cameraData.cameraTargetDescriptor.width;
        noiseTextureDescriptor.height = cameraData.cameraTargetDescriptor.height;
        noiseTextureDescriptor.depthBufferBits = 0;


        UpdateNoiseSettings();

        if (!srcCamColor.IsValid() || !dst.IsValid())
            return;


        //The AddBlitPass method adds a vertical blur render graph that blits from the source texture (camera color in this case)
        //RenderGraphUtils.BlitMaterialParameters paraVertical = new(srcCamColor, dst, material, 0);
        //renderGraph.AddBlitPass(paraVertical, k_NoisePassName);

        RenderGraphUtils.BlitMaterialParameters paraHorizontal = new(dst, srcCamColor, material, 0);
        renderGraph.AddBlitPass(paraHorizontal, k_NoisePassName);



    }



    private void UpdateNoiseSettings()
    {
        if (material == null) return;

        // Use the Volume settings or the default settings if no Volume is set.
        var volumeComponent =
            VolumeManager.instance.stack.GetComponent<CustomVolumeComponent>();
        float brightnessUpperThreshold = volumeComponent.noiseBrightnessUpperThreshold.overrideState ?
            volumeComponent.noiseBrightnessUpperThreshold.value : .5f;
        material.SetFloat(brightnessUpperThresholdId, brightnessUpperThreshold  );

        float brightnessLowerThreshold = volumeComponent.noiseBrightnessShadowWieght.overrideState ?
            volumeComponent.noiseBrightnessShadowWieght.value : 1;
        material.SetFloat(brightnessLowerThresholdId, brightnessLowerThreshold  );

        float noiseResolution = volumeComponent.noiseResolution.overrideState ?
            volumeComponent.noiseResolution.value : 512;
        material.SetFloat(resolutionId, noiseResolution);



    }



}
