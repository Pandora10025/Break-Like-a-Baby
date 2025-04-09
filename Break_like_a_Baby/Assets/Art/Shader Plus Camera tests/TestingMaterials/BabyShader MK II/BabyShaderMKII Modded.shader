Shader "Custom/BabyShaderMKII Modded"
{
    Properties{
        _MainTex("Sprite", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)    
        _TestingPower ("Testing Power", float) = 1    
    
    }
    SubShader{
     Tags{
         "RenderPipeline" = "UniversalPipeline"
         "RenderType" = "Opaque"
         "Queue" = "Geometry"
     
     }


     Pass{
        Name "ForwardPass"    
        Tags {"LightMode" = "UniversalForward"}

        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest always
        ZWrite Off

        HLSLPROGRAM
        #define _SPECULAR_COLOR_SPECULAR_COLOR
        #pragma vertex Vertex
        #pragma fragment Fragment
        #pragma shader_feature _FORWARD_PLUS
        #pragma shader_feature_fragment _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
          
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"


        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"

        #define REQUIRE_DEPTH_TEXTURE

        CBUFFER_START(UnityPerMaterial)
        half4 _Color;
        float _TestingPower;
        float4 _MainTex_ST;
        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        float4 _MainTex_TexelSize;


        struct Attributes{
            float3 positionLS : POSITION;
            float2 uv: TEXCOORD0;
            float3 normalLS : NORMAL;
        };


        struct Varyings{
            float4 positionCS : SV_POSITION;
            float3 normalWS : TEXCOORD0;
            float3 positionWS : TEXCOORD1;
            float2 uv: TEXCOORD2;
            float surfZ : TEXCOORD3;

        };


        float alphaSobelOperator( float2 uv ){
            
            float outlineScanSizeMultiplier = .000001;


            float2 ts = _MainTex_ST * outlineScanSizeMultiplier;

            
            //float3 camPos = GetCameraPositionWS();

            //float3 viewdirection = normalize(camPos - positionWS);

            //Real quick, we need to get the tangent and bitangent that we want to move along, to sample the shadow coords.

            //float3 bitangentVector = cross( normalWS, viewdirection);
            //float3 tangentVector = cross( normalWS, bitangentVector);
                  
            
            float aspect = _MainTex_TexelSize.z/_MainTex_TexelSize.w;
            
            float2 resolution = 50;

            resolution.x *= aspect;
            
            float2 pixelUV = uv;

            // float2 randomPixelUV = rand( floor( _Time.y * 5 )/5
            //pixelUV.x *= 1;
            // randomPixelUV.x *= aspect;

            //pixelUV = floor(pixelUV * resolution)/resolution;

            
            
            //float2 CondensedEquationPixelUV = ;
            

                   

            //Down here we attempt convolution, but with shadows!

            //float2 ts = _MainTex_TexelSize.xy;
            float result = 0;
                
            // for(int x = -1; x <= 1; x++) {
            //     for(int y = -1; y <= 1; y++) {
            //         //float2 offset = float2(x, y) * ts;

            //         float3 offsetPos = x*tangentVector*_ShadowSmoothingSize + y*bitangentVector * _ShadowSmoothingSize;

            //         //float3 sample = tex2D(_MainTex, uv + offset);
            //         float sample = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS  )); //tex2D(_MainTex, uv + offset);
            //         //result += sample * kernel[x+1][y+1];
            //         result += sample * kernel[x+1][y+1];
            //     }
            // }







            float2 p1Pos = floor( (pixelUV + float2(-1,1)*ts ) * resolution)/resolution;
            float2 p2Pos = floor( (pixelUV + float2(0,1)*ts ) * resolution)/resolution;
            float2 p3Pos = floor( (pixelUV + float2(1,1)*ts ) * resolution)/resolution;
            float2 p4Pos = floor( (pixelUV + float2(-1,0)*ts ) * resolution)/resolution;
            float2 p5Pos = floor(pixelUV * resolution)/resolution;
            float2 p6Pos = floor( (pixelUV + float2(1,0)*ts ) * resolution)/resolution;
            float2 p7Pos = floor( (pixelUV + float2(-1,-1)*ts ) * resolution)/resolution;
            float2 p8Pos = floor( (pixelUV + float2(0, -1)*ts ) * resolution)/resolution;
            float2 p9Pos = floor( (pixelUV + float2(1, -1)*ts ) * resolution)/resolution;


            // float3 p1 = getScaledDepthNormals(uv + float2(-1, 1) * ts).rgb;
                // float3 p2 = getScaledDepthNormals( uv + float2(0, 1) * ts ).rgb;
                // float3 p3 = getScaledDepthNormals( uv + float2(1, 1) * ts ).rgb;
                // float3 p4 = getScaledDepthNormals( uv + float2(-1, 0) * ts ).rgb;
                // float3 p5 = getScaledDepthNormals( uv + float2(0, 0) * ts ).rgb;
                // float3 p6 = getScaledDepthNormals( uv + float2(1, 0) * ts ).rgb;
                // float3 p7 = getScaledDepthNormals( uv + float2(-1, -1) * ts ).rgb;
                // float3 p8 = getScaledDepthNormals( uv + float2(0, -1) * ts ).rgb;
                // float3 p9 = getScaledDepthNormals( uv + float2(1, -1) * ts ).rgb;
                



            float p1 = SAMPLE_TEXTURE2D( _MainTex ,sampler_MainTex, p1Pos  ).a;
            float p2 = SAMPLE_TEXTURE2D( _MainTex ,sampler_MainTex, p2Pos  ).a;
            float p3 = SAMPLE_TEXTURE2D( _MainTex ,sampler_MainTex, p3Pos  ).a;
            float p4 = SAMPLE_TEXTURE2D( _MainTex ,sampler_MainTex, p4Pos  ).a;
            float p5 = SAMPLE_TEXTURE2D( _MainTex ,sampler_MainTex, p5Pos).a;
            float p6 = SAMPLE_TEXTURE2D( _MainTex ,sampler_MainTex, p6Pos  );
            float p7 = SAMPLE_TEXTURE2D( _MainTex ,sampler_MainTex, p7Pos  ).a;
            float p8 = SAMPLE_TEXTURE2D( _MainTex ,sampler_MainTex, p8Pos  ).a;
            float p9 = SAMPLE_TEXTURE2D( _MainTex ,sampler_MainTex, p9Pos  ).a;




            result = abs( (p1+ (2*p2)+p3)-(p7+(2*p8)+p9) )+ abs( (p3+ (2*p6) +p9 )-(p1+ (2*p4) + p7) );


            return result;
        
        
        
        }






        Varyings Vertex(Attributes input){
            
            Varyings output;

            output.positionCS = TransformObjectToHClip(input.positionLS);
            output.normalWS = TransformObjectToWorldNormal(input.normalLS);

            output.uv = TRANSFORM_TEX(input.uv, _MainTex);
            
            VertexPositionInputs positions = GetVertexPositionInputs(input.positionLS);

            // Set positionWS to the screen space position of the vertex
            output.positionWS = positions.positionWS.xyz;

            //output.surfZ = (-UnityObjectToViewPos(input.vertex)).z;
            output.surfZ = (-TransformWorldToView(output.positionWS)).z;


            return output;
        
        }




        half4 Fragment(Varyings v) : SV_Target {
            Light mainLight = GetMainLight(TransformWorldToShadowCoord(v.positionWS));
            
            float4 texel = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, v.uv);

            float2 normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(v.positionCS.xy);

            
            float depth = Linear01Depth(SampleSceneDepth(   normalizedScreenSpaceUV  ), _ZBufferParams);
            depth /= _ProjectionParams.w;


            //float difference = abs( depth - v.surfZ);
            float difference = (v.surfZ- depth );



            float alphaSobel = alphaSobelOperator(v.uv);




            float calculatedTransparency = texel.a;



            float aspect = _MainTex_TexelSize.z/_MainTex_TexelSize.w;
            
            float2 resolution = 10;

            resolution.x *= aspect;
            
            float2 pixelUV = v.uv;

            // float2 randomPixelUV = rand( floor( _Time.y * 5 )/5



            //pixelUV.x *= 1;

            // randomPixelUV.x *= aspect;



            pixelUV = floor(pixelUV * resolution)/resolution;

            // randomPixelUV = floor(randomPixelUV * resolution)/resolution;
            

            float4 pixelColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, pixelUV);



            InputData lighting = (InputData)0;
            lighting.positionWS = v.positionWS;
            lighting.normalWS = normalize(v.normalWS);
            lighting.viewDirectionWS = GetWorldSpaceViewDir(v.positionWS);
            lighting.shadowCoord = TransformWorldToShadowCoord(v.positionWS);


            SurfaceData surface = (SurfaceData)0;
            surface.albedo = difference;//texel.rgb;
            surface.alpha = 1 ;//calculatedTransparency;
            surface.smoothness = .9;
            surface.specular = .9;

            float4 calculatedBlinnPhong = UniversalFragmentBlinnPhong(lighting, surface);


            
            //return UniversalFragmentBlinnPhong(lighting, surface) * calculatedTransparency;// + unity_AmbientSky;
            return float4( alphaSobelOperator(v.uv).rrr, 1 );// + unity_AmbientSky;
            //return MainLightRealtimeShadow(lighting.shadowCoord);// * (1-GetMainLightShadowFade(v.positionWS));
            //return float4( (MainLightRealtimeShadow(lighting.shadowCoord)).rrr, 1);
            //return float4( pow((mainLight.shadowAttenuation), _TestingPower).rrr, 1);
           




        }




        ENDHLSL


     }

     Pass{
        Name "ShadowCaster"
        Tags{"LightMode" = "ShadowCaster"  }
     
        ColorMask 0

        HLSLPROGRAM
        #pragma vertex Vertex
        #pragma fragment Fragment
        
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
    
        float3 _LightDirection;

        struct Attributes{
            float3 positionLS : POSITION;
            float3 normalLS : NORMAL;
        };


        struct Varyings{
            float4 positionCS : SV_POSITION;
        };


        float4 GetShadowPositionHClip(Attributes input){
            
            VertexPositionInputs positions = GetVertexPositionInputs(input.positionLS);
            VertexNormalInputs normals = GetVertexNormalInputs(input.normalLS);


            float4 positionCS = TransformWorldToHClip( ApplyShadowBias( positions.positionWS , normals.normalWS, _LightDirection) );

            #if UNITY_REVERSED_Z
                positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
            #else
                positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
            #endif


            positionCS = ApplyShadowClamping(positionCS);
            
            return positionCS;
        
        }

        Varyings Vertex(Attributes input){
            
            Varyings output;

            output.positionCS = GetShadowPositionHClip(input);

            return output;
        
        }


        half4 Fragment(Varyings v) : SV_Target {
        
            
            return 0;
        }




        ENDHLSL

     
     }

        


    }


}
