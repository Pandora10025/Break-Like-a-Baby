using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;



public class OutlineRenderPass : ScriptableRenderPass
{
    
    private Material material;

    private RenderTextureDescriptor outlineTextureDescriptor;

    private const string k_OutlineTextureName = "_OutlineTexture";
    private const string k_OutlinePassName = "OutlineRenderPass";

    //private static readonly int brightnessUpperThresholdId = Shader.PropertyToID("brightnessUpperThreshold");
    //private static readonly int brightnessLowerThresholdId = Shader.PropertyToID("brightnessLowerThreshold");
    //private static readonly int resolutionId = Shader.PropertyToID("resolution");

    private static readonly int outlineGlobalSizeMultiplier = Shader.PropertyToID("outlineGlobalSizeMultiplier");
    private static readonly int normalDefaultThreshold = Shader.PropertyToID("normalDefaultThreshold");
    private static readonly int normalFarThreshold = Shader.PropertyToID("normalFarThreshold");
    private static readonly int normalAdjustNearDepth = Shader.PropertyToID("normalAdjustNearDepth");
    private static readonly int normalAdjustFarDepth = Shader.PropertyToID("normalAdjustFarDepth");
    private static readonly int depthDefaultThreshold = Shader.PropertyToID("depthDefaultThreshold");
    private static readonly int acuteDepthDefaultThreshold = Shader.PropertyToID("depthDefaultThreshold");
    private static readonly int acuteAngleStartDotDefault = Shader.PropertyToID("acuteAngleStartDot");
    private static readonly int globalTextureID = Shader.PropertyToID("_OutlineTexture");



    //public TextureHandle dst;




    public OutlineRenderPass(Material material)
    {
        this.material = material;
        //this.defaultSettings = defaultSettings;


        outlineTextureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);

        //Shader.SetGlobalTexture(Shader.PropertyToID(k_OutlinePassName), dst);


    }


    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        TextureHandle srcCamColor = resourceData.activeColorTexture;
        TextureHandle dst = UniversalRenderer.CreateRenderGraphTexture(renderGraph, outlineTextureDescriptor, k_OutlineTextureName, false);


        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();


        //The following line ensures that the render pass doesn't blit
        // from the back buffer

        if (resourceData.isActiveTargetBackBuffer)
            return;



        //Set the blur texture size to be the same as the camera target size.
        outlineTextureDescriptor.width = cameraData.cameraTargetDescriptor.width;
        outlineTextureDescriptor.height = cameraData.cameraTargetDescriptor.height;
        outlineTextureDescriptor.depthBufferBits = 0;

        UpdateOutlineSettings();

        if (!srcCamColor.IsValid() || !dst.IsValid())
            return;


        RenderGraphUtils.BlitMaterialParameters paraHorizontal = new(srcCamColor, dst , material, 0);
        paraHorizontal.sourceTexturePropertyID = globalTextureID;
        renderGraph.AddBlitPass(paraHorizontal, k_OutlinePassName);


        //// Allocate a global shader texture called _GlobalTexture
        //private int globalTextureID = Shader.PropertyToID(k_OutlinePassName);


        

        // Get the RenderGraph builder
        using (var builder = renderGraph.AddRasterRenderPass<PassData>("MyRenderPass", out var passData))
        {


            //// Create a RenderGraph texture handle for the global texture
            //// (If you are using a render texture, create it here, otherwise use the asset directly)
            ////TextureHandle globalTextureHandle;
            //if (true)//globalTexture != null)
            //{
            //    //globalTextureHandle = UniversalRenderer.CreateRenderGraphTexture(renderGraph, new RenderTextureDescriptor(globalTexture.width, globalTexture.height, RenderTextureFormat.ARGB32, 0), "GlobalTexture");
            //    builder.UseTexture(dst, AccessFlags.Read);
            //}
            //else
            //{
            //    // if no texture is selected, fallback to a black texture.
            //    // you can add logic here if needed
            //    //globalTextureHandle = UniversalRenderer.CreateRenderGraphTexture(renderGraph, new RenderTextureDescriptor(1, 1, RenderTextureFormat.ARGB32, 0), "Default Black Texture");
            //    //builder.UseTexture(globalTextureHandle, AccessFlags.Read);
            //}

            //// set the global texture to be used by the shader, the last param is the identifier for the texture
            //builder.SetGlobalTextureAfterPass( dst, globalTextureID);

            //builder.SetRenderFunc((PassData data, RasterGraphContext context) => { });



            //UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            //UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            //passData.source = resourceData.activeColorTexture;
            //TextureHandle destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph, m_BlurTextureDescriptor, DistortionRenderPassName, false);

            builder.SetRenderAttachment(dst, 0);
            builder.AllowPassCulling(false); // means it does this every frame without skipping

            // the blit that was copied stays around after the pass instead of getting cleared
            builder.SetGlobalTextureAfterPass(dst, globalTextureID);
            builder.SetRenderFunc((PassData data, RasterGraphContext context) => { });


        }




        ////The AddBlitPass method adds a vertical blur render graph that blits from the source texture (camera color in this case)
        //RenderGraphUtils.BlitMaterialParameters paraVertical = new(srcCamColor, dst, material, 0);
        //renderGraph.AddBlitPass(paraVertical, k_VerticalPassName);


        //ORIGINAL WORKING!!!
        //RenderGraphUtils.BlitMaterialParameters paraHorizontal = new(dst, srcCamColor,  material, 0);
        //renderGraph.AddBlitPass(paraHorizontal, k_OutlinePassName);




    }


    private void UpdateOutlineSettings()
    {
        if (material == null) return;

        // Use the Volume settings or the default settings if no Volume is set.
        var volumeComponent =
            VolumeManager.instance.stack.GetComponent<CustomVolumeComponent>();

        //float horizontalBlur = volumeComponent.horizontalBlur.overrideState ?
        //    volumeComponent.horizontalBlur.value : defaultSettings.horizontalBlur;
        //float verticalBlur = volumeComponent.verticalBlur.overrideState ?
        //    volumeComponent.verticalBlur.value : defaultSettings.verticalBlur;
        //material.SetFloat(horizontalBlurId, horizontalBlur);
        //material.SetFloat(verticalBlurId, verticalBlur);


        float outlineGlobalSizeMultiplierValue = volumeComponent.outlineGlobalSizeMultiplier.overrideState ? volumeComponent.outlineGlobalSizeMultiplier.value : .5f;
        material.SetFloat(outlineGlobalSizeMultiplier, outlineGlobalSizeMultiplierValue);
        
        
        float normalValue = volumeComponent.normalThreshold.overrideState ? volumeComponent.normalThreshold.value : .5f;
        material.SetFloat(normalDefaultThreshold, normalValue);
        
        float normalFarThresholdValue = volumeComponent.normalFarThreshold.overrideState ? volumeComponent.normalFarThreshold.value : .5f;
        material.SetFloat(normalFarThreshold, normalFarThresholdValue);
        
        float normalAdjustNearDepthValue = volumeComponent.normalAdjustNearDepth.overrideState ? volumeComponent.normalAdjustNearDepth.value : .5f;
        material.SetFloat(normalAdjustNearDepth, normalAdjustNearDepthValue);
        
        float normalAdjustFarDepthValue = volumeComponent.normalAdjustFarDepth.overrideState ? volumeComponent.normalAdjustFarDepth.value : .5f;
        material.SetFloat(normalAdjustFarDepth, normalAdjustFarDepthValue);







        float depthValue = volumeComponent.depthThreshold.overrideState ? volumeComponent.depthThreshold.value : 0.005f;  
        material.SetFloat (depthDefaultThreshold, depthValue);
        
        float acuteDepthDefaultThresholdValue = volumeComponent.acuteDepthDefaultThreshold.overrideState ? volumeComponent.acuteDepthDefaultThreshold.value : 0.005f;  
        material.SetFloat (acuteDepthDefaultThreshold, acuteDepthDefaultThresholdValue);
        
        
        float acuteAngleStartDotValue = volumeComponent.acuteAngleStartDot.overrideState ? volumeComponent.acuteAngleStartDot.value : 0.005f;  
        material.SetFloat (acuteAngleStartDotDefault, acuteAngleStartDotValue);






    }






}

internal class PassData
{



}