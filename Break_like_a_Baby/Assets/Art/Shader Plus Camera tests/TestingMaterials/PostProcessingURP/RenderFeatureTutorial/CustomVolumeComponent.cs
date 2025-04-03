using System;
using UnityEngine.Rendering;


[Serializable]
public class CustomVolumeComponent : VolumeComponent 
{
    public ClampedFloatParameter horizontalBlur = new ClampedFloatParameter(0.05f, 0, 0.5f);

    public ClampedFloatParameter verticalBlur = new ClampedFloatParameter(0.05f, 0, 0.5f);



    public ClampedFloatParameter noiseBrightnessUpperThreshold = new ClampedFloatParameter( .5f, -1f , 1f);
    public FloatParameter noiseBrightnessShadowWieght = new FloatParameter(1f);
    public FloatParameter noiseResolution = new FloatParameter(512f);


    public FloatParameter outlineGlobalSizeMultiplier = new FloatParameter(1.5f);
    public FloatParameter normalThreshold = new FloatParameter(.5f);
    public FloatParameter normalFarThreshold = new FloatParameter(.5f);
    public FloatParameter normalAdjustNearDepth = new FloatParameter(.5f);
    public FloatParameter normalAdjustFarDepth = new FloatParameter(.5f);
    public FloatParameter depthThreshold = new FloatParameter(.005f);
    public FloatParameter acuteDepthDefaultThreshold = new FloatParameter(.005f);
    public FloatParameter acuteAngleStartDot = new FloatParameter(.005f);






    

}


