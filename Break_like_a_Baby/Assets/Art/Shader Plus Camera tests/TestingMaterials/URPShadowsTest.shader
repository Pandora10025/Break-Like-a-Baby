Shader "Unlit/URPShadowsTest"
{
    Properties{
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


        HLSLPROGRAM
        #define _SPECULAR_COLOR_SPECULAR_COLOR
        #pragma vertex Vertex
        #pragma fragment Fragment
        #pragma shader_feature _FORWARD_PLUS
        #pragma shader_feature_fragment _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        CBUFFER_START(UnityPerMaterial)
        half4 _Color;
        float _TestingPower;
        CBUFFER_END



        struct Attributes{
            float3 positionLS : POSITION;
            float3 normalLS : NORMAL;
        };


        struct Varyings{
            float4 positionCS : SV_POSITION;
            float3 normalWS : TEXCOORD0;
            float3 positionWS : TEXCOORD1;
        };


        Varyings Vertex(Attributes input){
            
            Varyings output;

            output.positionCS = TransformObjectToHClip(input.positionLS);
            output.normalWS = TransformObjectToWorldNormal(input.normalLS);
            
            VertexPositionInputs positions = GetVertexPositionInputs(input.positionLS);

            // Set positionWS to the screen space position of the vertex
            output.positionWS = positions.positionWS.xyz;


            return output;
        
        }


        half4 Fragment(Varyings v) : SV_Target {
            Light mainLight = GetMainLight(TransformWorldToShadowCoord(v.positionWS));
            
            
            InputData lighting = (InputData)0;
            lighting.positionWS = v.positionWS;
            lighting.normalWS = normalize(v.normalWS);
            lighting.viewDirectionWS = GetWorldSpaceViewDir(v.positionWS);
            lighting.shadowCoord = TransformWorldToShadowCoord(v.positionWS);


            SurfaceData surface = (SurfaceData)0;
            surface.albedo = _Color.rgb;
            surface.alpha = 1;
            surface.smoothness = .9;
            surface.specular = .9;

            float4 calculatedBlinnPhong = UniversalFragmentBlinnPhong(lighting, surface);


            return UniversalFragmentBlinnPhong(lighting, surface);// + unity_AmbientSky;
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
