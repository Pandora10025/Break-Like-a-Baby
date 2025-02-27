using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class CustomEffectSettings {
    public Material effectMaterial;
    // Add other settings if needed
}

public class CustomEffectFeature : ScriptableRendererFeature {
    public CustomEffectSettings[] effectSettings;
    CustomEffectPass[] passes;

    public override void Create() {
        passes = new CustomEffectPass[effectSettings.Length];
        
        for (int i = 0; i < passes.Length; i++) {
            // create the pass and choose when it runs (after post-processing, for example)
            passes[i] = new CustomEffectPass(effectSettings[i].effectMaterial) {
                renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
            };
        }
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        for (int i = 0; i < passes.Length; i++) {
            // add the pass in the order it appears in the array
            renderer.EnqueuePass(passes[i]);
        }
    }
}