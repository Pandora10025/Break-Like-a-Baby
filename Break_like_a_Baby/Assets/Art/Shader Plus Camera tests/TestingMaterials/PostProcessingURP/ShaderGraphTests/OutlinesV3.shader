Shader "Custom/OutlinesV3"
{
    Properties
    {
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

        _DepthCutoff( "Depth Cutoff", float) = .1
        _DepthCutoffNormalModifier( "Depth Cutoff Normal Modifier", float) = .1
        _DepthContrast( "Depth contrast", float) = 1
        _NormalCutoff( "Normal Cutoff Min", float ) = .5
        _NormalCutoffCap( "Normal Cutoff Max", float ) = 7
        _NormalCutoffDepthModifier( "Normal Cutoff Depth Modifier", float ) = 2
        _NormalContrast( "Normal Contrast", float) = 1



        // _ColorContrast("Color Sensitivity", float) = 1
        // _DepthContrast("Depth Sensitivity", float) = 1
        // _DepthThreshold("Depth Threshold", float) = 1
        // _NormalContrast("DepthNormalScale", float) = 1
        // _NormalThreshold("Normal Threshold", float) = 1

        // _ModifiedOutlineHSV("ModifiedOutlineHSV", Color) = (0,1,1,0)
        
        // _OutlineColor( "Outline Color" , Color ) = (0,1,0,1)




    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            // RenderType: <None>
            // Queue: <None>
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalFullscreenSubTarget"
        }
        Pass
        {
            Name "DrawProcedural"
        
        // Render State
        Cull Off
        Blend Off
        ZTest Off
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 3.0
        #pragma vertex vert
        #pragma fragment frag
        // #pragma enable_d3d11_debug_symbols
        
        /* WARNING: $splice Could not find named fragment 'DotsInstancingOptions' */
        /* WARNING: $splice Could not find named fragment 'HybridV1InjectedBuiltinProperties' */
        
        // Keywords
        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
        // GraphKeywords: <None>
        
        #define FULLSCREEN_SHADERGRAPH
        
        // Defines
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_VERTEXID
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        
        // Force depth texture because we need it for almost every nodes
        // TODO: dependency system that triggers this define from position or view direction usage
        #define REQUIRE_DEPTH_TEXTURE
        #define REQUIRE_NORMAL_TEXTURE
        
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DRAWPROCEDURAL
        #define REQUIRE_DEPTH_TEXTURE
        #define REQUIRE_OPAQUE_TEXTURE
        
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
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
             uint vertexID : VERTEXID_SEMANTIC;
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float2 NDCPosition;
             float2 PixelPosition;
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 texCoord1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };
        struct VertexDescriptionInputs
        {
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _DepthCutoff;
        float _DepthContrast;
        float _DepthCutoffNormalModifier;
        float _NormalCutoff;
        float _NormalCutoffCap;
        float _NormalContrast;
        float _NormalCutoffDepthModifier;

        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        float _FlipY;
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // Graph Functions
        
        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }
        
        void Unity_SceneDepth_Raw_float(float4 UV, out float Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy);
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        float3 Unity_Universal_SampleBuffer_NormalWorldSpace_float(float2 uv)
        {
            return SHADERGRAPH_SAMPLE_SCENE_NORMAL(uv);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        // GraphVertex: <None>
        
        // Custom interpolators, pre surface
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreSurface' */
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };





        //LUKAS COMMENT: This is where Im pasting A LOT of my own code into this

        float getIntensity(float3 color){
                float3 weights = float3(.299, 0.587, 0.114);
                float grayscale = dot(color, weights);
                
                return grayscale;
            
            
            }

            float3 sobelConvolution (float2 uv) {
                float2 ts = 1;// _MainTex_TexelSize.xy;
                float3 result = 0;
                
                

                
                float3 p1 = getIntensity( SHADERGRAPH_SAMPLE_SCENE_COLOR( uv + float2(-1, 1) * ts) );
                float3 p2 = getIntensity( SHADERGRAPH_SAMPLE_SCENE_COLOR( uv + float2(0, 1) * ts) );
                float3 p3 = getIntensity( SHADERGRAPH_SAMPLE_SCENE_COLOR( uv + float2(1, 1) * ts) );
                float3 p4 = getIntensity( SHADERGRAPH_SAMPLE_SCENE_COLOR( uv + float2(-1, 0) * ts) );
                float3 p5 = getIntensity( SHADERGRAPH_SAMPLE_SCENE_COLOR( uv + float2(0, 0) * ts) );
                float3 p6 = getIntensity( SHADERGRAPH_SAMPLE_SCENE_COLOR( uv + float2(1, 0) * ts) );
                float3 p7 = getIntensity( SHADERGRAPH_SAMPLE_SCENE_COLOR( uv + float2(-1, -1) * ts) );
                float3 p8 = getIntensity( SHADERGRAPH_SAMPLE_SCENE_COLOR( uv + float2(0, -1) * ts) );
                float3 p9 = getIntensity( SHADERGRAPH_SAMPLE_SCENE_COLOR( uv + float2(1, -1) * ts) );

                result = abs( (p1+ (2*p2)+p3)-(p7+(2*p8)+p9) )+ abs( (p3+ (2*p6) +p9 )-(p1+ (2*p4) + p7) );


                return result;
            }



            float4 getScaledDepthNormals(float2 uv){
                float4 depthNormals =  float4( SHADERGRAPH_SAMPLE_SCENE_NORMAL(uv)  , Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(uv), _ZBufferParams));
                half3 viewNormal;
                float depth;
                //DecodeDepthNormal(depthNormals, depth, viewNormal);

                

                //float2 screenUV = i.screenPos.xy / i.screenPos.w;
                //color = Linear01Depth( tex2D(_CameraDepthTexture, screenUV ) );

               //float scaledDepth = Linear01Depth(tex2D(_CameraDepthTexture, uv));

               //scaledDepth /= _ProjectionParams.w;


               return depthNormals;
               //return float4( viewNormal, scaledDepth);
               //return float4( float3(depthNormals.rg, 0) * (1-scaledDepth ), scaledDepth);
            
            }


            float sobelDepthConvolution (float2 uv){
                float2 ts = 0.001;
                float3 result = 0;
                
                //float4 sampleDepthNormals = tex2D(_CameraDepthNormalsTexture, uv );

                //float4 scaledNormal = (sampleDepthNormals) *2-1;

                //float2 screenUV = i.screenPos.xy / i.screenPos.w;
                //color = Linear01Depth( tex2D(_CameraDepthTexture, screenUV ) );

                //float4 scaledDepth = Linear01Depth(sampleDepthNormals.ba);



                
                float p1 = getScaledDepthNormals(uv + float2(-1, 1) * ts).a;
                float p2 = getScaledDepthNormals( uv + float2(0, 1) * ts ).a;
                float p3 = getScaledDepthNormals( uv + float2(1, 1) * ts ).a;
                float p4 = getScaledDepthNormals( uv + float2(-1, 0) * ts ).a;
                float p5 = getScaledDepthNormals( uv + float2(0, 0) * ts ).a;
                float p6 = getScaledDepthNormals( uv + float2(1, 0) * ts ).a;
                float p7 = getScaledDepthNormals( uv + float2(-1, -1) * ts ).a;
                float p8 = getScaledDepthNormals( uv + float2(0, -1) * ts ).a;
                float p9 = getScaledDepthNormals( uv + float2(1, -1) * ts ).a;

                result = abs( (p1+ (2*p2)+p3)-(p7+(2*p8)+p9) )+ abs( (p3+ (2*p6) +p9 )-(p1+ (2*p4) + p7) );


                return result;            
            }

          
            float3 sobelNormalConvolution (float2 uv){
                float2 ts = 0.0005;
                float3 result = 0;
                
                //float4 sampleDepthNormals = tex2D(_CameraDepthNormalsTexture, uv );

                //float4 scaledNormal = (sampleDepthNormals) *2-1;

                //float2 screenUV = i.screenPos.xy / i.screenPos.w;
                //color = Linear01Depth( tex2D(_CameraDepthTexture, screenUV ) );

                //float4 scaledDepth = Linear01Depth(sampleDepthNormals.ab);



                
                // float3 p1 = getScaledDepthNormals(uv + float2(-1, 1) * ts).rgb;
                // float3 p2 = getScaledDepthNormals( uv + float2(0, 1) * ts ).rgb;
                // float3 p3 = getScaledDepthNormals( uv + float2(1, 1) * ts ).rgb;
                // float3 p4 = getScaledDepthNormals( uv + float2(-1, 0) * ts ).rgb;
                // float3 p5 = getScaledDepthNormals( uv + float2(0, 0) * ts ).rgb;
                // float3 p6 = getScaledDepthNormals( uv + float2(1, 0) * ts ).rgb;
                // float3 p7 = getScaledDepthNormals( uv + float2(-1, -1) * ts ).rgb;
                // float3 p8 = getScaledDepthNormals( uv + float2(0, -1) * ts ).rgb;
                // float3 p9 = getScaledDepthNormals( uv + float2(1, -1) * ts ).rgb;
                
                
                float3 p1 = getScaledDepthNormals(uv + float2(-1, 1) * ts).rgb;
                float3 p2 = getScaledDepthNormals( uv + float2(0, 1) * ts ).rgb;
                float3 p3 = getScaledDepthNormals( uv + float2(1, 1) * ts ).rgb;
                float3 p4 = getScaledDepthNormals( uv + float2(-1, 0) * ts ).rgb;
                float3 p5 = getScaledDepthNormals( uv + float2(0, 0) * ts ).rgb;
                float3 p6 = getScaledDepthNormals( uv + float2(1, 0) * ts ).rgb;
                float3 p7 = getScaledDepthNormals( uv + float2(-1, -1) * ts ).rgb;
                float3 p8 = getScaledDepthNormals( uv + float2(0, -1) * ts ).rgb;
                float3 p9 = getScaledDepthNormals( uv + float2(1, -1) * ts ).rgb;

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



        




        //LUKAS COMMENT: This looks to be the equivalent of the frag/surf function. We output colors here!
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float3 _SceneColor_3729885ae3bb4bed90cbb9d71705b266_Out_1_Vector3;
            Unity_SceneColor_float(float4(IN.NDCPosition.xy, 0, 0), _SceneColor_3729885ae3bb4bed90cbb9d71705b266_Out_1_Vector3);
            float _SceneDepth_5c38b5611303420aa54f1bb37c46d9c8_Out_1_Float;
            Unity_SceneDepth_Raw_float(float4(IN.NDCPosition.xy, 0, 0), _SceneDepth_5c38b5611303420aa54f1bb37c46d9c8_Out_1_Float);
            float3 _Multiply_93b90905b9424767856c9e71d516433f_Out_2_Vector3;
            Unity_Multiply_float3_float3(_SceneColor_3729885ae3bb4bed90cbb9d71705b266_Out_1_Vector3, (_SceneDepth_5c38b5611303420aa54f1bb37c46d9c8_Out_1_Float.xxx), _Multiply_93b90905b9424767856c9e71d516433f_Out_2_Vector3);
            float3 _URPSampleBuffer_539074bbd1664536ba90f820be8e5d0a_Output_2_Vector3 = Unity_Universal_SampleBuffer_NormalWorldSpace_float(float4(IN.NDCPosition.xy, 0, 0).xy);
            float3 _Multiply_05339da72f5a43fd941b50c930711d40_Out_2_Vector3;
            Unity_Multiply_float3_float3(_Multiply_93b90905b9424767856c9e71d516433f_Out_2_Vector3, _URPSampleBuffer_539074bbd1664536ba90f820be8e5d0a_Output_2_Vector3, _Multiply_05339da72f5a43fd941b50c930711d40_Out_2_Vector3);
            surface.BaseColor = _Multiply_05339da72f5a43fd941b50c930711d40_Out_2_Vector3;
            surface.Alpha = float(1);
            






            float normalSobel = getIntensity(sobelNormalConvolution(IN.NDCPosition.xy));
            float cutoffNormal = normalSobel* step( _NormalCutoff  +  pow(Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(IN.NDCPosition.xy), _ZBufferParams), _NormalContrast)*_NormalCutoffDepthModifier, normalSobel) * (1-step(_NormalCutoffCap, normalSobel) );
            float exponentialNormal = pow( normalSobel, _NormalContrast); 





            float3 viewdirection = GetWorldSpaceViewDir(IN.WorldSpacePosition);

            float nDotv = 1-dot( getScaledDepthNormals(IN.NDCPosition.xy).xyz , normalize(viewdirection) );

            nDotv = saturate(nDotv);

            float depthSobel = sobelDepthConvolution(IN.NDCPosition.xy);
            
            float exponentialDepth = pow( depthSobel, _DepthContrast );
            
            //float cutoffDepth = step( _DepthCutoff - Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(IN.NDCPosition.xy), _ZBufferParams) * _DepthContrast , depthSobel);
            //float cutoffDepth = step( _DepthCutoff , depthSobel);
            
            float cutoffDepth = step( _DepthCutoff * Linear01Depth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(IN.NDCPosition.xy), _ZBufferParams) + nDotv * _DepthCutoffNormalModifier, depthSobel);



            float outlinesMask = max(cutoffNormal , cutoffDepth);


            float3 calculateOutlinesColor =  _SceneColor_3729885ae3bb4bed90cbb9d71705b266_Out_1_Vector3 * 0;

            float3 outlinesColored = outlinesMask * calculateOutlinesColor;

            float3 sceneColor = _SceneColor_3729885ae3bb4bed90cbb9d71705b266_Out_1_Vector3 * (1-outlinesMask) + outlinesColored;



            //surface.BaseColor = cutoffDepth;// + exponentialNormal;
            
            surface.BaseColor = sceneColor;
            
            //surface.BaseColor = cutoffDepth;
            //surface.BaseColor = nDotv.rrr;
            
            //surface.BaseColor = exponentialNormal;



            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            float3 normalWS = SHADERGRAPH_SAMPLE_SCENE_NORMAL(input.texCoord0.xy);
            float4 tangentWS = float4(0, 1, 0, 0); // We can't access the tangent in screen space
        
        
        
        
            float3 viewDirWS = normalize(input.texCoord1.xyz);
            float linearDepth = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(input.texCoord0.xy), _ZBufferParams);
            float3 cameraForward = -UNITY_MATRIX_V[2].xyz;
            float camearDistance = linearDepth / dot(viewDirWS, cameraForward);
            float3 positionWS = viewDirWS * camearDistance + GetCameraPositionWS();
        
        
            output.WorldSpacePosition = positionWS;
            output.ScreenPosition = float4(input.texCoord0.xy, 0, 1);
            output.NDCPosition = input.texCoord0.xy;
        
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
        
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenCommon.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenDrawProcedural.hlsl"
        
        ENDHLSL
        }
        Pass
        {
            Name "Blit"
        
        // Render State
        Cull Off
        Blend Off
        ZTest Off
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 3.0
        #pragma vertex vert
        #pragma fragment frag
        // #pragma enable_d3d11_debug_symbols
        
        /* WARNING: $splice Could not find named fragment 'DotsInstancingOptions' */
        /* WARNING: $splice Could not find named fragment 'HybridV1InjectedBuiltinProperties' */
        
        // Keywords
        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
        // GraphKeywords: <None>
        
        #define FULLSCREEN_SHADERGRAPH
        
        // Defines
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_VERTEXID
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        
        // Force depth texture because we need it for almost every nodes
        // TODO: dependency system that triggers this define from position or view direction usage
        #define REQUIRE_DEPTH_TEXTURE
        #define REQUIRE_NORMAL_TEXTURE
        
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_BLIT
        #define REQUIRE_DEPTH_TEXTURE
        #define REQUIRE_OPAQUE_TEXTURE
        
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
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
             uint vertexID : VERTEXID_SEMANTIC;
             float3 positionOS : POSITION;
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float2 NDCPosition;
             float2 PixelPosition;
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 texCoord1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };
        struct VertexDescriptionInputs
        {
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END
        
        
        // Object and Global properties
        float _FlipY;
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // Graph Functions
        
        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }
        
        void Unity_SceneDepth_Raw_float(float4 UV, out float Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy);
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        float3 Unity_Universal_SampleBuffer_NormalWorldSpace_float(float2 uv)
        {
            return SHADERGRAPH_SAMPLE_SCENE_NORMAL(uv);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        // GraphVertex: <None>
        
        // Custom interpolators, pre surface
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreSurface' */
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float3 _SceneColor_3729885ae3bb4bed90cbb9d71705b266_Out_1_Vector3;
            Unity_SceneColor_float(float4(IN.NDCPosition.xy, 0, 0), _SceneColor_3729885ae3bb4bed90cbb9d71705b266_Out_1_Vector3);
            float _SceneDepth_5c38b5611303420aa54f1bb37c46d9c8_Out_1_Float;
            Unity_SceneDepth_Raw_float(float4(IN.NDCPosition.xy, 0, 0), _SceneDepth_5c38b5611303420aa54f1bb37c46d9c8_Out_1_Float);
            float3 _Multiply_93b90905b9424767856c9e71d516433f_Out_2_Vector3;
            Unity_Multiply_float3_float3(_SceneColor_3729885ae3bb4bed90cbb9d71705b266_Out_1_Vector3, (_SceneDepth_5c38b5611303420aa54f1bb37c46d9c8_Out_1_Float.xxx), _Multiply_93b90905b9424767856c9e71d516433f_Out_2_Vector3);
            float3 _URPSampleBuffer_539074bbd1664536ba90f820be8e5d0a_Output_2_Vector3 = Unity_Universal_SampleBuffer_NormalWorldSpace_float(float4(IN.NDCPosition.xy, 0, 0).xy);
            float3 _Multiply_05339da72f5a43fd941b50c930711d40_Out_2_Vector3;
            Unity_Multiply_float3_float3(_Multiply_93b90905b9424767856c9e71d516433f_Out_2_Vector3, _URPSampleBuffer_539074bbd1664536ba90f820be8e5d0a_Output_2_Vector3, _Multiply_05339da72f5a43fd941b50c930711d40_Out_2_Vector3);
            surface.BaseColor = _Multiply_05339da72f5a43fd941b50c930711d40_Out_2_Vector3;
            surface.Alpha = float(1);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            float3 normalWS = SHADERGRAPH_SAMPLE_SCENE_NORMAL(input.texCoord0.xy);
            float4 tangentWS = float4(0, 1, 0, 0); // We can't access the tangent in screen space
        
        
        
        
            float3 viewDirWS = normalize(input.texCoord1.xyz);
            float linearDepth = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(input.texCoord0.xy), _ZBufferParams);
            float3 cameraForward = -UNITY_MATRIX_V[2].xyz;
            float camearDistance = linearDepth / dot(viewDirWS, cameraForward);
            float3 positionWS = viewDirWS * camearDistance + GetCameraPositionWS();
        
        
            output.WorldSpacePosition = positionWS;
            output.ScreenPosition = float4(input.texCoord0.xy, 0, 1);
            output.NDCPosition = input.texCoord0.xy;
        
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
        
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenCommon.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/Fullscreen/Includes/FullscreenBlit.hlsl"
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.Rendering.Fullscreen.ShaderGraph.FullscreenShaderGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}