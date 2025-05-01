Shader "Custom/DustRing"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
       
        _Color("Color", Color) = (1,1,1,1)
        _ShadowColor("ShadowColor", Color) = (1,1,1,1)
        _shadingBands ("ShadingBandsNumber", int) = 2
        _GradientSize ("GradientSize", Range(0,1)) = 0.5
        _shadowBias("ShadowBias", float) = 0
      
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "Lit"
            "IgnoreProjector" = "True"
         }
        
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        //Cull Off
        //ZWrite [_ZWrite]
        //ZTest Off

        Pass
        {

            
            Tags{
                
            "Queue" = "2000"
            }

            HLSLPROGRAM



            
        

            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _FORWARD_PLUS
            #pragma shader_feature_fragment _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS




            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"


            

             // custom interpolator pre-include
            /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
            
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
          
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"


            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv: TEXCOORD0;
                float3 normal : NORMAL;
                float4 color        : COLOR;
            };



            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 uv: TEXCOORD0;
                float4 color        : COLOR;
                float3 normal : TEXCOORD1;
                float4 shadowCoords : TEXCOORD2;
                float shadowDarkness : TEXCOORD3;
                float3 positionWS : TEXCOORD4;
                float4 positionSC : TEXCOORD5;

            };

            TEXTURE2D(_OutlineTexture); 
            SAMPLER(sampler_OutlineTexture);

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap); 
            //TEXTURE2D(_CameraDepthTexture);
            //SAMPLER(sampler_DepthMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(normTex);

            float4 _OutlineTexture_TexelSize;

            // Identifier same as the RenderPass
            #define SAMPLE_BLIT(uv) SAMPLE_TEXTURE2D( _OutlineTexture, sampler_LinearClamp, uv )
        
            

            //sampler2D _CameraDepthTexture;


            void Unity_SceneDepth_Raw_float(float4 UV, out float Out)
            {
                Out = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy);
            }


            CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float4 _ShadowColor;
            float4 _BaseMap_ST;
            int _shadingBands;
            float _GradientSize;
            
            float _shadowBias;
            
            CBUFFER_END

            


            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                //So in this next part we're going to try getting the rightward direction
                //of the object, so that we can have it ripple in that direction to create a trippy spiral!
                
                float3 worldPos = mul( (float3x3)unity_ObjectToWorld, ( IN.positionOS ) ) ;

                float deformAmount = lerp( 0, .05, sin( (( IN.positionOS.z*8  + _Time.y*3) )+1)/2  );

                float3 worldPosDeformed = worldPos + IN.normal * deformAmount;



                IN.positionOS = mul( (float4x4)unity_WorldToObject, float4( worldPosDeformed.xyz, 0 ) ) ;





                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.color = IN.color;


                


                // Get the VertexPositionInputs for the vertex position  
                VertexPositionInputs positions = GetVertexPositionInputs(IN.positionOS.xyz);

                // Convert the vertex position to a position on the shadow map
                float4 shadowCoordinates = TransformWorldToShadowCoord(positions.positionWS.xyz);

                // Pass the shadow coordinates to the fragment shader
                OUT.shadowCoords = shadowCoordinates;

                

                OUT.positionWS = positions.positionWS.xyz;
                
                //OUT.normal = UnityObjectToWorldNormal(IN.normal);
                OUT.normal = normalize(mul(IN.normal.xyz, (float3x3)unity_WorldToObject));
                //OUT.normal = mul(unity_ObjectToWorld, IN.normal) - IN.positionOS ; //UnityObjectToWorldNormal(IN.normal);
                //OUT.normal = normalize(OUT.normal);

               
                OUT.positionSC = ComputeScreenPos( TransformObjectToHClip(IN.positionOS.xyz)  );


                

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 texel = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                // return texel * _Color;

                


                float3 color;

                float3 normal = normalize(IN.normal);
                
                Light mainLight = GetMainLight();

                
               // half3 LightingLambert(half3 lightColor, half3 lightDirection, half3 surfaceNormal);

                



                float3 lightdirection = mainLight.direction;
                float3 lightcolor = mainLight.color; // includes intensity

               // float3 viewdirection = normalize(_worldspacecamerapos.xyz - in.posworld);
                //float3 halfdirection = normalize(viewdirection + lightdirection);

                float shadowAmount = MainLightRealtimeShadow(TransformWorldToShadowCoord( IN.positionWS));
                    

                float ndotl = (dot(normal, lightdirection)+1)/2;
                
                ndotl = min(shadowAmount, ndotl);

                ndotl = pow( ndotl, _shadowBias );



                //float diffusefalloff = round( ndotl* (_shadingBands-1) )/(_shadingBands-1);
                float diffusefalloff = floor( ndotl* (_shadingBands) )/(_shadingBands-1);

                //float distanceFromNearestEdge = abs((ndotl-diffusefalloff));
               
                //float amountToOffset = 

                //float diffusefalloffOffset = round( (ndotl + .5  )  * (_shadingBands-1) )/(_shadingBands-1);
                float diffusefalloffOffset = floor( ndotl* (_shadingBands) + .5 )/(_shadingBands-1);



                //float distanceFromNearestEdge = ((ndotl+.5-diffusefalloffOffset) * (_shadingBands-1)+.5);
                float distanceFromNearestEdge = (ndotl*(_shadingBands)-floor( ndotl*(_shadingBands) +.5 ) +.5 );  //(x*4-floor(x*4+1/2)+1/2)


                float percentFromNearestEdge = smoothstep( .5 - _GradientSize/2, .5 + _GradientSize/2 , distanceFromNearestEdge);

                float gradientMask = 1-step( _GradientSize/2, abs(distanceFromNearestEdge -.5) );

                float minGradientFalloff = saturate( floor( ndotl* (_shadingBands) - .5)/(_shadingBands-1));

                float maxGradientFalloff = saturate(floor( ndotl* (_shadingBands) + .5)/(_shadingBands-1));

                float gradientFalloff = lerp( minGradientFalloff , maxGradientFalloff  , percentFromNearestEdge );
                 

                //float3 cellColor = 
                
                float falloffPlusGradients = (1-gradientMask) * diffusefalloff + gradientMask * gradientFalloff;
                //float falloffPlusGradients =  gradientMask * gradientFalloff;


                //float specularfalloff = max(0, dot(normal, halfdirection));
                //specularfalloff = pow(specularfalloff, _gloss * max_specular_power/ _extraspecularmultiplier  + 0.0001) * _gloss;

                //specularfalloff = floor(specularfalloff * _specularlightsteps)/_specularlightsteps;


                float4 diffuse =  lerp(  _ShadowColor , _Color , falloffPlusGradients) * float4(lightcolor,1 ) * texel.rgba; // * _surfacecolor;
                //float3 specular = specularfalloff * lightcolor;

                color = diffuse;// + specular + _ambientcolor;

                return float4(color.rgb, diffuse.a * IN.color.a);


            }
            ENDHLSL

        }

         Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"

                "Queue" = "1900"
            
            }

            //-------------------------------------
            //Render State Commands
            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 3.0

            #define TAU 6.28318530718



        // -------------------------------------
        //Shader Stages
        #pragma vertex DepthNormalsVertexModded
        #pragma fragment DepthNormalsFragment

        //-------------------------------------
        //Material Keywords
        #pragma shader_feature_local _NORMALMAP
        #pragma shader_feature_local _PARALLAXMAP
        #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
        #pragma shader_feature_local _ALPHATEST_ON
        #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A


        //-------------------------------------
        // Unity defined keywords
        #pragma multi_compile _ LOD_FADE_CROSSFADE

        // -------------------------------------
        // Universal Pipeline keywords
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"

        // --------------------------------------
        // GPU Instancing
        #pragma multi_compile_instancing
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

        //-------------------------------------
        //Includes
        #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/LitDepthNormalsPass.hlsl"

        #define TAU 6.28318530718

        CBUFFER_START(UnityPerMaterial)
                
               
            float _rotY;
            //_UpAxis ("UpAxis", Vector) = (0,1,0)
            float3 _ForwardAxis;
            float3 _Center;

                


        CBUFFER_END

        

        Varyings DepthNormalsVertexModded(Attributes input)
        {
            Varyings output = (Varyings)0;
            UNITY_SETUP_INSTANCE_ID(input);
            UNITY_TRANSFER_INSTANCE_ID(input, output);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

            #if defined(_ALPHATEST_ON)
                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
            #endif

                
                


            //HERES THE CRAZY PART WHERE I MODIFIED THE SOURCE CODE LOL


            //float4x4 x = rotation_matrix(float3(1, 0 ,0), _rotX  * TAU  );
            //float4x4 y = rotation_matrix(float3 (0, 1 ,0), _rotY * TAU );
            //float4x4 y = rotation_matrix(_Axis.xyz, _rotY * TAU );
            //float4x4 z = rotation_matrix(float3(0, 0 ,1), _rotZ * TAU  );

            //float4x4 rotation = mul( mul(x , y), z );


            //mul(input.normal.xyz, (float3x3)unity_WorldToObject)



            float3 worldPos = mul( (float3x3)unity_ObjectToWorld, ( input.positionOS ) ) ;

            float deformAmount = lerp( 0, .05, sin( (( input.positionOS.z*8  + _Time.y*3) )+1)/2  );

            float3 worldPosDeformed = worldPos + input.normal * deformAmount;



            input.positionOS = mul( (float4x4)unity_WorldToObject, float4( worldPosDeformed.xyz, 0 ) ) ;
   
            //HERES THE CRAZY PART WHERE I MODIFIED THE SOURCE CODE LOL






            output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

            VertexNormalInputs normalInput = GetVertexNormalInputs(input.normal, input.tangentOS);
            output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);

            return output;
        }


        ENDHLSL
    }

    }

   


}
