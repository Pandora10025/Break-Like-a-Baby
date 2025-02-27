
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;



[Serializable, VolumeComponentMenuForRenderPipeline("Custom/CustomEffectcomponent", typeof(UniversalRenderPipeline))]
public class CustomEffectComponent : VolumeComponent, IPostProcessComponent {

    // For example, an intensity paramter that goes from 0 to 1
    public ClampedFloatParameter intensity = new ClampedFloatParameter(value: 0, min: 0, max: 1, overrideState: true);
    // A color that is constant even when the weight changes
    public NoInterpColorParameter overlayColor = new NoInterpColorParameter(Color.cyan);

    // Other 'Parameter' variables you might have

    // Tells when our effect should be rendered
    public bool IsActive() => intensity.value > 0;

    public bool IsTileCompatible() => true;





}
