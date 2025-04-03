Shader "Custom/NoisePostProcessingShader"
{

    // Properties
    // {
    //     _NoiseScale("Noise Scale", float) = 3 
  
    // }

    

    


    HLSLINCLUDE


        
    
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        // The Blit.hlsl file provides the vertex shader (Vert),
        // the input structure (Attributes), and the output structure (Varyings)
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        float _VerticalBlur;

        float brightnessUpperThreshold;
        float brightnessLowerThreshold;
        float resolution;


        
        float rand (float2 uv) {
                return frac(sin(dot(uv.xy, float2(12.98908, 78.23003))) * 43758.545354123);
        }

        float value_noise (float2 uv) {
            float2 ipos = floor(uv);
            float2 fpos = frac(uv); 
                
            float o  = rand(ipos);
            float x  = rand(ipos + float2(1, 0));
            float y  = rand(ipos + float2(0, 1));
            float xy = rand(ipos + float2(1, 1));

            float2 smooth = smoothstep(0, 1, fpos);
            return lerp( lerp(o,  x, smooth.x), 
                            lerp(y, xy, smooth.x), smooth.y);
        }

        float fractal_noise (float2 uv) {
            float n = 0;

            n  = (1 / 2.0)  * value_noise( uv * 1);
            n += (1 / 4.0)  * value_noise( uv * 2); 
            n += (1 / 8.0)  * value_noise( uv * 4); 
            n += (1 / 16.0) * value_noise( uv * 8);
                
            return n;
        }


        float3 ConvertToHSV(float3 In ){
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 P = lerp(float4(In.bg, K.wz), float4(In.gb, K.xy), step(In.b, In.g));
                float4 Q = lerp(float4(P.xyw, In.r), float4(In.r, P.yzx), step(P.x, In.r));
                float D = Q.x - min(Q.w, Q.y);
                float E = 1e-10;
                float3 HSV = float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
            
                return HSV;
            }


            float3 ConvertFromHSV(float3 hsv){
                
                float4 K2 = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
                return hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);
            }


        float4 NoiseFilter (Varyings input) : SV_Target
        {
            //const float BLUR_SAMPLES = 64;
            //const float BLUR_SAMPLES_RANGE = BLUR_SAMPLES / 2;
            
            //float3 color = 0;
            //float blurPixels = _VerticalBlur * _ScreenParams.y;
            
            //for(float i = -BLUR_SAMPLES_RANGE; i <= BLUR_SAMPLES_RANGE; i++)
            //{
                //float2 sampleOffset = float2 (0, (blurPixels / _BlitTexture_TexelSize.w) * (i / BLUR_SAMPLES_RANGE));
                //color += SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord + sampleOffset).rgb;
            //}
            
            //return float4(color.rgb / (BLUR_SAMPLES + 1), 1);




            float3 sample = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).rgb;


            //float2 uv = input.texcoord + rand( frac( ceil( _Time.y * 8 )/8 ));
            
            

            float3 weights = float3(0.299, 0.587, 0.114);
            float3 grayscale = saturate( dot(sample, weights) );
            

            float aspect = _ScreenParams.x / _ScreenParams.y;

            float2 pixelUV = input.texcoord;

            float2 randomPixelUV = rand( floor( _Time.y * 5 )/5);



            pixelUV.x *= aspect;

            randomPixelUV.x *= aspect;



            pixelUV = floor(pixelUV * resolution)/resolution;

            randomPixelUV = floor(randomPixelUV * resolution)/resolution;
            



            float noiseValuepixelUV = value_noise(  (pixelUV + randomPixelUV )* resolution );

            float secondNoiseValue = fractal_noise( (pixelUV - randomPixelUV )* 100 );


           
            pixelUV.x /= aspect;

            


            float3 pixelColor =  SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp,  pixelUV ).rgb;

            float3 pixelGrayscale = saturate( dot(pixelColor, weights) );
                
            float pixelDitherAmount = 1-step(noiseValuepixelUV , pow( pixelGrayscale.x + brightnessUpperThreshold, brightnessLowerThreshold  ));

            //Now we're going to try using the pixelated version to do some dithering! 

            float noiseToAdd = ( (secondNoiseValue )*2 -.7)*.05 ;

            float3 samplePlusNoise =  ConvertFromHSV(   ConvertToHSV(pixelColor) + pixelDitherAmount* float3( 0, 0, noiseToAdd) );

            float3 pixelHSV = ConvertToHSV(pixelColor);

            float3 moddedPixelHSV = float3(   pixelHSV.x , pixelHSV.y , clamp(  pixelHSV.z + pixelDitherAmount * noiseToAdd , 0, 1 )    );




            
            return float4( ConvertFromHSV(moddedPixelHSV)  , 1 );


        }

        
    
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "NoisePass"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment NoiseFilter
            
            ENDHLSL
        }
        
        
    }
}
