Shader "Custom/BabyShaderMKII Modded"
{
    Properties{
        _MainTex("Sprite", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)    
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)    
        _TestingPower ("Testing Power", float) = 1 
        _OutlineResolution("Outline Resolution", float) = 216
        _OutlineSizeMultiplier("Outline Size", float) = 1
    
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
        //Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Blend SrcAlpha OneMinusSrcAlpha
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
        float4 _OutlineColor;
        float _TestingPower;
        float _OutlineResolution;
        float _OutlineSizeMultiplier;
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
            
            float outlineScanSizeMultiplier = _OutlineSizeMultiplier;


            float2 ts = _MainTex_TexelSize.xy * outlineScanSizeMultiplier;

            
            //float3 camPos = GetCameraPositionWS();

            //float3 viewdirection = normalize(camPos - positionWS);

            //Real quick, we need to get the tangent and bitangent that we want to move along, to sample the shadow coords.

            //float3 bitangentVector = cross( normalWS, viewdirection);
            //float3 tangentVector = cross( normalWS, bitangentVector);
                  
            
            float aspect = _MainTex_TexelSize.z/_MainTex_TexelSize.w;
            
            float2 resolution = _OutlineResolution;

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
            float p6 = SAMPLE_TEXTURE2D( _MainTex ,sampler_MainTex, p6Pos  ).a;
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

            float steppedDifference = saturate(step( 0, difference ));



            float alphaSobel = alphaSobelOperator(v.uv);




            float calculatedTransparency =  texel.a ;



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
            surface.albedo = texel.rgb;
            surface.alpha = calculatedTransparency;
            surface.smoothness = .9;
            surface.specular = .9;

            float4 calculatedBlinnPhong = UniversalFragmentBlinnPhong(lighting, surface);

            float4 coloredOutline = alphaSobel * _OutlineColor;

            float4 calculatedColor = lerp( calculatedBlinnPhong   ,  coloredOutline  , steppedDifference   );


            
            //return UniversalFragmentBlinnPhong(lighting, surface) * calculatedTransparency;// + unity_AmbientSky;
            return float4( calculatedColor.rgba );// + unity_AmbientSky;
            //return MainLightRealtimeShadow(lighting.shadowCoord);// * (1-GetMainLightShadowFade(v.positionWS));
            //return float4( (MainLightRealtimeShadow(lighting.shadowCoord)).rrr, 1);
            //return float4( pow((mainLight.shadowAttenuation), _TestingPower).rrr, 1);
           




        }




        ENDHLSL


     }

     Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
        
        // Render State
        Cull Off
        ZTest LEqual
        ZWrite On
        ColorMask 0
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX_NORMAL_OUTPUT
        #define FEATURES_GRAPH_VERTEX_TANGENT_OUTPUT
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        #define _ALPHATEST_ON 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 normalWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Split_b35f8b8c85bb4973937dc261703aeac2_R_1_Float = IN.VertexColor[0];
            float _Split_b35f8b8c85bb4973937dc261703aeac2_G_2_Float = IN.VertexColor[1];
            float _Split_b35f8b8c85bb4973937dc261703aeac2_B_3_Float = IN.VertexColor[2];
            float _Split_b35f8b8c85bb4973937dc261703aeac2_A_4_Float = IN.VertexColor[3];
            UnityTexture2D _Property_6d51d03cd663484b860c7174bb75d79d_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_3a67302bb363410e96211dcf5511a88d_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_6d51d03cd663484b860c7174bb75d79d_Out_0_Texture2D.tex, _Property_6d51d03cd663484b860c7174bb75d79d_Out_0_Texture2D.samplerstate, _Property_6d51d03cd663484b860c7174bb75d79d_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_3a67302bb363410e96211dcf5511a88d_R_4_Float = _SampleTexture2D_3a67302bb363410e96211dcf5511a88d_RGBA_0_Vector4.r;
            float _SampleTexture2D_3a67302bb363410e96211dcf5511a88d_G_5_Float = _SampleTexture2D_3a67302bb363410e96211dcf5511a88d_RGBA_0_Vector4.g;
            float _SampleTexture2D_3a67302bb363410e96211dcf5511a88d_B_6_Float = _SampleTexture2D_3a67302bb363410e96211dcf5511a88d_RGBA_0_Vector4.b;
            float _SampleTexture2D_3a67302bb363410e96211dcf5511a88d_A_7_Float = _SampleTexture2D_3a67302bb363410e96211dcf5511a88d_RGBA_0_Vector4.a;
            float _Multiply_64451f60605d4577a5a606a0c215e4cb_Out_2_Float;
            Unity_Multiply_float_float(_Split_b35f8b8c85bb4973937dc261703aeac2_A_4_Float, _SampleTexture2D_3a67302bb363410e96211dcf5511a88d_A_7_Float, _Multiply_64451f60605d4577a5a606a0c215e4cb_Out_2_Float);
            surface.Alpha = _Multiply_64451f60605d4577a5a606a0c215e4cb_Out_2_Float;
            surface.AlphaClipThreshold = float(0.1);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }

        


    }


}
