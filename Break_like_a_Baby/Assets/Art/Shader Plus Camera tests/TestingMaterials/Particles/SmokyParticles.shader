Shader "Unlit/SmokyParticles"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _UpperTex ("Upper Texture", 2D) = "black" {}

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        
        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
            #if defined(DEBUG_DISPLAY)
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/InputData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"
            #endif

            

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            #pragma multi_compile_fragment _ DEBUG_DISPLAY

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_SKINNED_VERTEX_INPUTS
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS      : SV_POSITION;
                float4  color           : COLOR;
                float2  uv              : TEXCOORD0;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS      : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            UNITY_TEXTURE_STREAMING_DEBUG_VARS_FOR_TEX(_MainTex);

            //TEXTURE2D_X(_MotionVectorTexture);
            //SAMPLER(sampler_MotionVectorTexture);


            // NOTE: Do not ifdef the properties here as SRP batcher can not handle different layouts.
            CBUFFER_START( UnityPerMaterial )
                half4 _Color;
            CBUFFER_END

            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_SKINNED_VERTEX_COMPUTE(attributes);

                attributes.positionOS = UnityFlipSprite(attributes.positionOS, unity_SpriteProps.xy);
                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(attributes.positionOS);
                #endif
                o.uv = attributes.uv;
                o.color = attributes.color * _Color * unity_SpriteColor;
                return o;
            }

            float4 UnlitFragment(Varyings i) : SV_Target
            {
                //float4 motionSample = SAMPLE_TEXTURE2D_X(_MotionVectorTexture, sampler_MotionVectorTexture, i.uv);


                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                #if defined(DEBUG_DISPLAY)
                SurfaceData2D surfaceData;
                InputData2D inputData;
                half4 debugColor = 0;

                InitializeSurfaceData(mainTex.rgb, mainTex.a, surfaceData);
                InitializeInputData(i.uv, inputData);
                SETUP_DEBUG_TEXTURE_DATA_2D_NO_TS(inputData, i.positionWS, i.positionCS, _MainTex);

                if(CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                {
                    return debugColor;
                }
                #endif

                return mainTex;
                //return  float4(  normalize(motionSample.rgb) , 1) ;
            }
            ENDHLSL
        }


        Pass
        {
            Tags { "LightMode" = "Universal2D"  }


            //Rendering Layer 1! For the bottom layer of smoke.
            HLSLPROGRAM

            Tags{"Queue" = "Transparent-100"}

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
            #if defined(DEBUG_DISPLAY)
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/InputData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"
            #endif

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            #pragma multi_compile _ DEBUG_DISPLAY SKINNED_SPRITE

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_SKINNED_VERTEX_INPUTS
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                half4   color       : COLOR;
                float2  uv          : TEXCOORD0;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS  : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            UNITY_TEXTURE_STREAMING_DEBUG_VARS_FOR_TEX(_MainTex);

            
            TEXTURE2D_X(_MotionVectorTexture);
            SAMPLER(sampler_MotionVectorTexture);

            // NOTE: Do not ifdef the properties here as SRP batcher can not handle different layouts.
            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
            CBUFFER_END

            // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
            // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
            // #pragma instancing_options assumeuniformscaling
            UNITY_INSTANCING_BUFFER_START(Props)
                // put more per-instance properties here
            UNITY_INSTANCING_BUFFER_END(Props)


            Varyings UnlitVertex(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_SKINNED_VERTEX_COMPUTE(v);

                v.positionOS = UnityFlipSprite(v.positionOS, unity_SpriteProps.xy);
                o.positionCS = TransformObjectToHClip(v.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(v.positionOS);
                #endif
                o.uv = v.uv;
                o.color = v.color * _Color * unity_SpriteColor;
                return o;
            }

            half4 UnlitFragment(Varyings i) : SV_Target
            {

                //FRAGCOLOR

               // float4 motionSample = SAMPLE_TEXTURE2D_X(_MotionVectorTexture, sampler_MotionVectorTexture, i.uv);

                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                #if defined(DEBUG_DISPLAY)
                SurfaceData2D surfaceData;
                InputData2D inputData;
                half4 debugColor = 0;

                InitializeSurfaceData(mainTex.rgb, mainTex.a, surfaceData);
                InitializeInputData(i.uv, inputData);
                SETUP_DEBUG_TEXTURE_DATA_2D_NO_TS(inputData, i.positionWS, i.positionCS, _MainTex);

                if(CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                {
                    return debugColor;
                }
                #endif

                return mainTex;
                //return float4( 0,0,0,1 );
            }
            ENDHLSL



            //Rendering layer 2! For the top layer of the smoke
            HLSLPROGRAM

            

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
            #if defined(DEBUG_DISPLAY)
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/InputData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"
            #endif

            //Tags{ "Queue" = "Transparent" }
                                                                                                               
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            #pragma multi_compile _ DEBUG_DISPLAY SKINNED_SPRITE

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_SKINNED_VERTEX_INPUTS
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                half4   color       : COLOR;
                float2  uv          : TEXCOORD0;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS  : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            UNITY_TEXTURE_STREAMING_DEBUG_VARS_FOR_TEX(_MainTex);


            TEXTURE2D(_UpperTex);
            SAMPLER(sampler_UpperTex);
            UNITY_TEXTURE_STREAMING_DEBUG_VARS_FOR_TEX(_UpperTex);

            
            TEXTURE2D_X(_MotionVectorTexture);
            SAMPLER(sampler_MotionVectorTexture);

            // NOTE: Do not ifdef the properties here as SRP batcher can not handle different layouts.
            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
            CBUFFER_END

            Varyings UnlitVertex(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_SKINNED_VERTEX_COMPUTE(v);

                v.positionOS = UnityFlipSprite(v.positionOS, unity_SpriteProps.xy);
                o.positionCS = TransformObjectToHClip(v.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(v.positionOS);
                #endif
                o.uv = v.uv;
                o.color = v.color * _Color * unity_SpriteColor;
                return o;
            }

            half4 UnlitFragment(Varyings i) : SV_Target
            {
                //FRAGCOLOR

                //float4 motionSample = SAMPLE_TEXTURE2D_X(_MotionVectorTexture, sampler_MotionVectorTexture, i.uv);

                //float scaleDivisor = 2;

                //float2 rescaledUV = scaleDivisor*(-.5)+.5;

                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_UpperTex, sampler_UpperTex, i.uv);

                #if defined(DEBUG_DISPLAY)
                SurfaceData2D surfaceData;
                InputData2D inputData;
                half4 debugColor = 0;

                InitializeSurfaceData(mainTex.rgb, mainTex.a, surfaceData);
                InitializeInputData(i.uv, inputData);
                SETUP_DEBUG_TEXTURE_DATA_2D_NO_TS(inputData, i.positionWS, i.positionCS, _MainTex);

                if(CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                {
                    return debugColor;
                }
                #endif

                return mainTex;
                //return float4( 0,0,0,1 );
            }
            ENDHLSL



        }


        //   Pass
        // {
        //     Name "Camera Motion Vectors"

        //     Tags{ "Queue"="2000" }

        //     Cull Off
        //     ZWrite On

        //     HLSLPROGRAM
        //     #pragma target 3.5

        //     #pragma vertex vert
        //     #pragma fragment frag

        //     // -------------------------------------
        //     // Includes
        //     #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        //     #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        //     #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityInput.hlsl"
        //     #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        //     #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"

        //     struct Attributes
        //     {
        //         uint vertexID : SV_VertexID;
        //         UNITY_VERTEX_INPUT_INSTANCE_ID
        //     };

        //     struct Varyings
        //     {
        //         float4 positionCS : SV_POSITION;
        //         float2 texcoord   : TEXCOORD0;
        //         UNITY_VERTEX_OUTPUT_STEREO
        //     };

        //     // -------------------------------------
        //     // Vertex
        //     Varyings vert(Attributes input)
        //     {
        //         Varyings output;
        //         UNITY_SETUP_INSTANCE_ID(input);
        //         UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        //         float4 pos = GetFullScreenTriangleVertexPosition(input.vertexID);
        //         float2 uv  = GetFullScreenTriangleTexCoord(input.vertexID);

        //         output.positionCS = pos;
        //         output.texcoord   = uv;

        //         return output;
        //     }

        //     // -------------------------------------
        //     // Fragment
        //     half4 frag(Varyings input, out float outDepth : SV_Depth) : SV_Target
        //     {
        //         UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        //         float2 uv = input.texcoord;
        //         float depth = LoadSceneDepth(uv * _CameraDepthTexture_TexelSize.zw);
        //         outDepth = depth; // Write depth out unmodified

        //     #if !UNITY_REVERSED_Z
        //         depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(uv).x);
        //     #endif

        //     #if defined(SUPPORTS_FOVEATED_RENDERING_NON_UNIFORM_RASTER)
        //         UNITY_BRANCH if (_FOVEATED_RENDERING_NON_UNIFORM_RASTER)
        //         {
        //             // Get the UVs from non-unifrom space to linear space to determine the right world-space position
        //             uv = RemapFoveatedRenderingNonUniformToLinear(uv);
        //         }
        //     #endif

        //         // Reconstruct world position
        //         float3 posWS = ComputeWorldSpacePosition(uv, depth, UNITY_MATRIX_I_VP);

        //         // Multiply with current and previous non-jittered view projection
        //         float4 posCS = mul(_NonJitteredViewProjMatrix, float4(posWS.xyz, 1.0));
        //         float4 prevPosCS = mul(_PrevViewProjMatrix, float4(posWS.xyz, 1.0));

        //         // Non-uniform raster needs to keep the posNDC values in float to avoid additional conversions
        //         // since uv remap functions use floats
        //         float2 posNDC = posCS.xy * rcp(posCS.w);
        //         float2 prevPosNDC = prevPosCS.xy * rcp(prevPosCS.w);

        //         float2 velocity;
        //         #if defined(SUPPORTS_FOVEATED_RENDERING_NON_UNIFORM_RASTER)
        //         UNITY_BRANCH if (_FOVEATED_RENDERING_NON_UNIFORM_RASTER)
        //         {
        //             // Convert velocity from NDC space (-1..1) to screen UV 0..1 space since FoveatedRendering remap needs that range.
        //             // Also return both position in non-uniform UV space to get the right velocity vector

        //             float2 posUV = RemapFoveatedRenderingResolve(posNDC * 0.5f + 0.5f);
        //             float2 prevPosUV = RemapFoveatedRenderingPrevFrameLinearToNonUniform(prevPosNDC * 0.5f + 0.5f);

        //             // Calculate forward velocity
        //             velocity = (posUV - prevPosUV);
        //             #if UNITY_UV_STARTS_AT_TOP
        //                 velocity.y = -velocity.y;
        //             #endif
        //         }
        //         else
        //         #endif
        //         {
        //             // Calculate forward velocity
        //             velocity = (posNDC - prevPosNDC);

        //             // TODO: test that velocity.y is correct
        //             #if UNITY_UV_STARTS_AT_TOP
        //                 velocity.y = -velocity.y;
        //             #endif

        //             // Convert velocity from NDC space (-1..1) to screen UV 0..1 space
        //             // Note: It doesn't mean we don't have negative values, we store negative or positive offset in the UV space.
        //             // Note: ((posNDC * 0.5 + 0.5) - (prevPosNDC * 0.5 + 0.5)) = (velocity * 0.5)
        //             velocity.xy *= 0.5;
        //         }

        //         return float4(velocity, 0, 0);
        //     }

        //     ENDHLSL
        // }


    }

    Fallback "Sprites/Default"
}
