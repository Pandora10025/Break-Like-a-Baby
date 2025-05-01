Shader "Custom/OutlinesV4"
{
    HLSLINCLUDE


        
    
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        // The Blit.hlsl file provides the vertex shader (Vert),
        // the input structure (Attributes), and the output structure (Varyings)
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        //These two need to  be included so that unity knows to get the depth and normal texture sready for use in this shader
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl" 



        float _VerticalBlur;

        float outlineGlobalSizeMultiplier;
        float normalDefaultThreshold;
        float normalFarThreshold;
        float normalAdjustNearDepth;
        float normalAdjustFarDepth;
        float depthDefaultThreshold;
        float acuteDepthDefaultThreshold;
        float acuteAngleStartDot;
        float _OutlineResolution;




        //sampler2D _CameraDepthTexture;


        
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


        
            float4 getScaledDepthNormals(float2 uv){
                float4 depthNormals = float4( SampleSceneNormals(uv), SampleSceneDepth(uv) );
               // half3 viewNormal;
               // float depth;
                //DecodeDepthNormal(depthNormals, depth, viewNormal);


                #if !UNITY_REVERSED_Z
                     depthNormals.w = 1.0-depthNormals.w;
                #endif
                

                //float2 screenUV = i.screenPos.xy / i.screenPos.w;
                //color = Linear01Depth( tex2D(_CameraDepthTexture, screenUV ) );

               //float scaledDepth = Linear01Depth(tex2D(_CameraDepthTexture, uv));

               //scaledDepth /= _ProjectionParams.w;


               return depthNormals;
               //return float4( float3(depthNormals.rg, 0) * (1-scaledDepth ), scaledDepth);
            
            }


            float3 sobelDepthConvolution (float2 uv, out float verticality){
                float aspect =  _ScreenParams.x / _ScreenParams.y;
            
                
                float2 ts = _BlitTexture_TexelSize.xy * outlineGlobalSizeMultiplier * aspect;
                float3 result = 0;
                
                
                float2 resolution = _OutlineResolution;

                resolution.x *= aspect;
            
                float2 pixelUV = uv;

        
                


                float2 p1Pos = floor( (pixelUV + float2(-1,1)*ts * .707 ) * resolution)/resolution;
                float2 p2Pos = floor( (pixelUV + float2(0,1)*ts ) * resolution)/resolution;
                float2 p3Pos = floor( (pixelUV + float2(1,1)*ts * .707 ) * resolution)/resolution;
                float2 p4Pos = floor( (pixelUV + float2(-1,0)*ts ) * resolution)/resolution;
                float2 p5Pos = floor(pixelUV * resolution)/resolution;
                float2 p6Pos = floor( (pixelUV + float2(1,0)*ts ) * resolution)/resolution;
                float2 p7Pos = floor( (pixelUV + float2(-1,-1)*ts * .707 ) * resolution)/resolution;
                float2 p8Pos = floor( (pixelUV + float2(0, -1)*ts ) * resolution)/resolution;
                float2 p9Pos = floor( (pixelUV + float2(1, -1)*ts * .707 ) * resolution)/resolution;



                
                float3 p1 = getScaledDepthNormals(p1Pos).aaa;
                float3 p2 = getScaledDepthNormals(p2Pos).aaa;
                float3 p3 = getScaledDepthNormals(p3Pos).aaa;
                float3 p4 = getScaledDepthNormals(p4Pos).aaa;
                float3 p5 = getScaledDepthNormals(p5Pos).aaa;
                float3 p6 = getScaledDepthNormals(p6Pos).aaa;
                float3 p7 = getScaledDepthNormals(p7Pos).aaa;
                float3 p8 = getScaledDepthNormals(p8Pos).aaa;
                float3 p9 = getScaledDepthNormals(p9Pos).aaa;

                float d = getScaledDepthNormals(uv).w;

                verticality =  (d-p1)+(d-p2)+(d-p3)+(d-p4)+(d-p5)+(d-p6)+(d-p7)+(d-p8)+(d-p9);

                result = abs( (p1+ (2*p2)+p3)-(p7+(2*p8)+p9) )+ abs( (p3+ (2*p6) +p9 )-(p1+ (2*p4) + p7) );


                return result;            
            }

            float3 sobelNormalConvolution (float2 uv){
                float aspect =  _ScreenParams.x / _ScreenParams.y;
            
                
                float2 ts = _BlitTexture_TexelSize.xy * outlineGlobalSizeMultiplier * aspect;
                float3 result = 0;
                
                
                float2 resolution = _OutlineResolution;

                resolution.x *= aspect;
            
                float2 pixelUV = uv;

        
                


                float2 p1Pos = floor( (pixelUV + float2(-1,1)*ts * .707) * resolution)/resolution;
                float2 p2Pos = floor( (pixelUV + float2(0,1)*ts ) * resolution)/resolution;
                float2 p3Pos = floor( (pixelUV + float2(1,1)*ts * .707) * resolution)/resolution;
                float2 p4Pos = floor( (pixelUV + float2(-1,0)*ts ) * resolution)/resolution;
                float2 p5Pos = floor(pixelUV * resolution)/resolution;
                float2 p6Pos = floor( (pixelUV + float2(1,0)*ts ) * resolution)/resolution;
                float2 p7Pos = floor( (pixelUV + float2(-1,-1)*ts * .707) * resolution)/resolution;
                float2 p8Pos = floor( (pixelUV + float2(0, -1)*ts ) * resolution)/resolution;
                float2 p9Pos = floor( (pixelUV + float2(1, -1)*ts * .707) * resolution)/resolution;

                
                float3 p1 = getScaledDepthNormals(p1Pos).rgb;
                float3 p2 = getScaledDepthNormals(p2Pos).rgb;
                float3 p3 = getScaledDepthNormals(p3Pos).rgb;
                float3 p4 = getScaledDepthNormals(p4Pos).rgb;
                float3 p5 = getScaledDepthNormals(p5Pos).rgb;
                float3 p6 = getScaledDepthNormals(p6Pos).rgb;
                float3 p7 = getScaledDepthNormals(p7Pos).rgb;
                float3 p8 = getScaledDepthNormals(p8Pos).rgb;
                float3 p9 = getScaledDepthNormals(p9Pos).rgb;

                result = abs( (p1+ (2*p2)+p3)-(p7+(2*p8)+p9) )+ abs( (p3+ (2*p6) +p9 )-(p1+ (2*p4) + p7) );


                return result;            
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







        float4 OutlineFilter (Varyings input) : SV_Target
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
            
            

            // float3 weights = float3(0.299, 0.587, 0.114);
            // float3 grayscale = saturate( dot(sample, weights) );
            

            // float aspect = _ScreenParams.x / _ScreenParams.y;

            // float2 pixelUV = input.texcoord;

            // float2 randomPixelUV = rand( floor( _Time.y * 5 )/5);



            // pixelUV.x *= aspect;

            // randomPixelUV.x *= aspect;



            // pixelUV = floor(pixelUV * resolution)/resolution;

            // randomPixelUV = floor(randomPixelUV * resolution)/resolution;
            



            // float noiseValuepixelUV = value_noise(  (pixelUV + randomPixelUV )* 100 );

            // float secondNoiseValue = fractal_noise( (pixelUV - randomPixelUV )* 100 );


           
            // pixelUV.x /= aspect;

            


            // float3 pixelColor =  SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp,  pixelUV ).rgb;

            // float3 pixelGrayscale = saturate( dot(pixelColor, weights) );
                
            // float pixelDitherAmount = 1-step(noiseValuepixelUV , pow( pixelGrayscale.x + brightnessUpperThreshold, brightnessLowerThreshold  ));

            // //Now we're going to try using the pixelated version to do some dithering! 

            // float noiseToAdd = ( (secondNoiseValue )*2 -.7)*.05 ;

            // float3 samplePlusNoise =  ConvertFromHSV(   ConvertToHSV(pixelColor) + pixelDitherAmount* float3( 0, 0, noiseToAdd) );

            // float3 pixelHSV = ConvertToHSV(pixelColor);

            // float3 moddedPixelHSV = float3(   pixelHSV.x , pixelHSV.y , clamp(  pixelHSV.z + pixelDitherAmount * noiseToAdd , 0, 1 )    );





            //I'm keeping the stuff from above for now, just so that I have a good source of code to reference going forward.
            //FYI, this was for coding that static effect. 

            //It took a long time to clue this together without proper documentation...
            //... peeking the source code, and all kinds of other insane things...
            //...but the line below is how we get the world normal values!

            float3 normalTest = SampleSceneNormals(input.texcoord);

            //float3 depthTest = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, _WorldSpaceCameraPos.xy / _WorldSpaceCameraPos.z);

            float3 depthTest = SampleSceneDepth(input.texcoord);


            float4 customDepthNormals = getScaledDepthNormals(input.texcoord);

            float rawDepth = (1-depthTest).rrr * _ProjectionParams.z;


            float3 viewSpaceNormals = mul( (float3x3)UNITY_MATRIX_V  ,  customDepthNormals.xyz);


            float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);

            float3 nedNumber = -normalize(float3((input.texcoord*2-1)/ p11_22,-1));



            //float3 improvisedFresnel = 1-dot( float3( 0, 0, 1 ) , viewSpaceNormals );
            
            
            float3 improvisedFresnel = 1-dot( viewSpaceNormals, nedNumber );



            

            float3 normalSobel = sobelNormalConvolution(input.texcoord);

            float calculatedNormalThreshold = lerp(   normalDefaultThreshold   ,   normalFarThreshold       ,   smoothstep( normalAdjustNearDepth , normalAdjustFarDepth , customDepthNormals.w ) );

            float thresholdedNormal = step( calculatedNormalThreshold ,  ConvertToHSV(normalSobel).zzz  );
            //float thresholdedNormal = 0;





            float verticality;

            float3 depthSobel = sobelDepthConvolution(input.texcoord, verticality);

            float calculatedDepthThreshold = customDepthNormals.w * lerp(   depthDefaultThreshold,     acuteDepthDefaultThreshold,  smoothstep(  acuteAngleStartDot   , 1 , improvisedFresnel     )   );

            float thresholdedDepth = step( calculatedDepthThreshold , depthSobel  );


            float3 finalOutline = max( thresholdedDepth, thresholdedNormal );

            float remappedVerticality = (.5 + verticality*1000);

            float verticalityThreshold = .1;

            float thresholdedVerticality = step(verticalityThreshold, remappedVerticality );
            
            //finalOutline *= lerp( float3(1,0,0), float3(0,1,0), saturate( thresholdedVerticality) );

            
            return float4( finalOutline, 1 );
            //return float4(  customDepthNormals.www , 1 );


        }

        
    
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "OutlinePass"

            HLSLPROGRAM
            
            #pragma vertex Vert
            #pragma fragment OutlineFilter
            
            ENDHLSL
        }
        
        
    }
}
