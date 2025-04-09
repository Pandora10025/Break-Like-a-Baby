Shader "Custom/RockingURPToon"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _ShadowColor("ShadowColor", Color) = (1,1,1,1)
        _shadingBands ("ShadingBandsNumber", int) = 3
        _GradientSize ("GradientSize", Range(0,1)) = 0.5
        _TestingOffset("Testing Offset", float) = 0
        _ShadowSmoothingSize("ShadowSmoothness", float) = 0

        _rotY ("HitRotation", Range(-1,1)) = 0
        _UpAxis ("UpAxis", Vector) = (0,1,0)
        _ForwardAxis ("RotationAxis", Vector) = (0,1,0)
        _Center ("RotationCenter", Vector) = (0,0,0)

        _OutlineColor ("Outline Color", Color) = (0,0,0)
        _OutlineOpacity( "Outline Opacity", float ) = 1
        _OutlineSizeMultiplier( " Outline Size Multiplier", Range(0, 10) ) = 1



    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _FORWARD_PLUS
            #pragma shader_feature_fragment _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS




            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            #define TAU 6.28318530718

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv: TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 uv: TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 shadowCoords : TEXCOORD2;
                float shadowDarkness : TEXCOORD3;
                float3 positionWS : TEXCOORD4;
                float4 positionSC : TEXCOORD5;



                


            };

            TEXTURE2D(_OutlineTexture); 
            SAMPLER(sampler_OutlineTexture);

            float4 _OutlineTexture_TexelSize;



            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap); 

            CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float4 _ShadowColor;
            float4 _BaseMap_ST;
            int _shadingBands;
            float _GradientSize;
            float _TestingOffset;
            float _ShadowSmoothingSize;

            float _rotY;
            //_UpAxis ("UpAxis", Vector) = (0,1,0)
            float3 _ForwardAxis;
            float3 _Center;


            float3 _OutlineColor;
            float _OutlineOpacity;
    
            float _OutlineSizeMultiplier;


            CBUFFER_END

            float3x3 boxBlurKernel = float3x3 (
                    // box
                    0.11, 0.11, 0.11,
                    0.11, 0.11, 0.11,
                    0.11, 0.11, 0.11
                );

            float3x3 gaussianBlurKernel = float3x3 (
                // gaussian
                0.0625, 0.125, 0.0625,
                0.1250, 0.250, 0.1250,
                0.0625, 0.125, 0.0625
            );


            
            float GetOutlineValue ( float2 screenUV ){
                
                float2 ts = _OutlineTexture_TexelSize.xy ;

                float baseValue = SAMPLE_TEXTURE2D( _OutlineTexture, sampler_PointClamp , screenUV );
                
                float p1 = SAMPLE_TEXTURE2D( _OutlineTexture, sampler_PointClamp , screenUV + float2(1,0)*ts*_OutlineSizeMultiplier );
                float p2 = SAMPLE_TEXTURE2D( _OutlineTexture, sampler_PointClamp , screenUV + float2(1,1) * .707 *ts*_OutlineSizeMultiplier );
                float p3 = SAMPLE_TEXTURE2D( _OutlineTexture, sampler_PointClamp , screenUV + float2(0,1)  *ts*_OutlineSizeMultiplier );
                float p4 = SAMPLE_TEXTURE2D( _OutlineTexture, sampler_PointClamp , screenUV + float2(-1,1) * .707 *ts*_OutlineSizeMultiplier );
                float p5 = SAMPLE_TEXTURE2D( _OutlineTexture, sampler_PointClamp , screenUV + float2(-1,0)*ts*_OutlineSizeMultiplier );
                float p6 = SAMPLE_TEXTURE2D( _OutlineTexture, sampler_PointClamp , screenUV + float2(-1,-1) * .707 *ts*_OutlineSizeMultiplier );
                float p7 = SAMPLE_TEXTURE2D( _OutlineTexture, sampler_PointClamp , screenUV + float2(0,-1)*ts*_OutlineSizeMultiplier );
                float p8 = SAMPLE_TEXTURE2D( _OutlineTexture, sampler_PointClamp , screenUV + float2(1,-1) * .707 *ts*_OutlineSizeMultiplier );             

                float outlineValue = saturate(baseValue + p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8);


                //We'll try a horizontal and vertical sweep!

                // float p1 = outlineRaycast( float2(1,0), screenUV );
                // float p3 = outlineRaycast( float2(-1,0), screenUV );
                // float p5 = outlineRaycast( float2(0,1), screenUV );
                // float p7 = outlineRaycast( float2(0,-1), screenUV );
                
                // float outlineValue = saturate(baseValue + p1 + p3 + p5 + p7);

                






                return outlineValue;
            
            
            }


            float shadowConvolution(float3 positionWS, float3 normalWS, float3x3 kernel){
                    int steps = 1;

                    float3 camPos = GetCameraPositionWS();

                    float3 viewdirection = normalize(camPos - positionWS);

                    //Real quick, we need to get the tangent and bitangent that we want to move along, to sample the shadow coords.

                    float3 bitangentVector = cross( normalWS, viewdirection);
                    float3 tangentVector = cross( normalWS, bitangentVector);
                    

                   

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

                    float p1 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(-1 *tangentVector + 1 * bitangentVector) * _ShadowSmoothingSize ) );
                    float p2 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(0 *tangentVector + 1 * bitangentVector) * _ShadowSmoothingSize  ));
                    float p3 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(1 *tangentVector + 1 * bitangentVector) * _ShadowSmoothingSize ) );
                    float p4 =  MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(-1 *tangentVector + 0 * bitangentVector) * _ShadowSmoothingSize ) );
                    float p5 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS));
                    float p6 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(1 *tangentVector + 0 * bitangentVector) * _ShadowSmoothingSize ) );
                    float p7 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(-1 *tangentVector + -1 * bitangentVector) * _ShadowSmoothingSize ) );
                    float p8 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(0 *tangentVector + -1 * bitangentVector) * _ShadowSmoothingSize  ));
                    float p9 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(1 *tangentVector + -1 * bitangentVector) * _ShadowSmoothingSize ) );

                    //result = abs( (p1+ (2*p2)+p3)-(p7+(2*p8)+p9) )+ abs( (p3+ (2*p6) +p9 )-(p1+ (2*p4) + p7) );
                    result = (p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9)/9;


                    return result;
            }


            float shadowGaussianConvolution(float3 positionWS, float3 normalWS, float3x3 kernel){
                    int steps = 1;

                    float3 camPos = GetCameraPositionWS();

                    Light mainLight = GetMainLight();

                    float3 lightDirection = mainLight.direction;


                    float3 viewDirection = normalize(camPos - positionWS);

                    //Real quick, we need to get the tangent and bitangent that we want to move along, to sample the shadow coords.

                    float3 bitangentVector = cross( normalWS, lightDirection);
                    float3 tangentVector = cross( normalWS, bitangentVector);
                    

                   

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

                    
                    
            //         float3x3 gaussianBlurKernel = float3x3 (
            //     // gaussian
            //     0.0625, 0.125, 0.0625,
            //     0.1250, 0.250, 0.1250,
            //     0.0625, 0.125, 0.0625
            // );


                    float p1 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(-1 *tangentVector + 1 * bitangentVector) * _ShadowSmoothingSize ) ) * 0.0625;
                    float p2 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(0 *tangentVector + 1 * bitangentVector) * _ShadowSmoothingSize  )) * 0.1250;
                    float p3 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(1 *tangentVector + 1 * bitangentVector) * _ShadowSmoothingSize ) ) * 0.0625;
                    float p4 =  MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(-1 *tangentVector + 0 * bitangentVector) * _ShadowSmoothingSize ) ) * 0.1250 ;
                    float p5 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS)) * 0.250;
                    float p6 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(1 *tangentVector + 0 * bitangentVector) * _ShadowSmoothingSize ) ) * 0.1250;
                    float p7 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(-1 *tangentVector + -1 * bitangentVector) * _ShadowSmoothingSize ) ) * 0.0625;
                    float p8 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(0 *tangentVector + -1 * bitangentVector) * _ShadowSmoothingSize  ) ) * 0.1250;
                    float p9 = MainLightRealtimeShadow(TransformWorldToShadowCoord( positionWS + normalize(1 *tangentVector + -1 * bitangentVector) * _ShadowSmoothingSize ) ) * 0.0625;

                    //result = abs( (p1+ (2*p2)+p3)-(p7+(2*p8)+p9) )+ abs( (p3+ (2*p6) +p9 )-(p1+ (2*p4) + p7) );
                    result = (p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9);


                    return result;
            }


            float4x4 rotation_matrix (float3 axis, float angle) {
                axis = normalize(axis);
                float s = sin(angle);
                float c = cos(angle);
                float oc = 1.0 - c;
                
                return float4x4(
                    oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,  0.0,
                    oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,  0.0,
                    oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c,           0.0,
                    0.0,                                0.0,                                0.0,                                1.0);
            }


            Varyings vert(Attributes IN)
            {
                Varyings OUT;


                

                
                
                


                //IN.positionOS += float4(1,1,1,0);


                //float4x4 x = rotation_matrix(float3(1, 0 ,0), _rotX  * TAU  );
                float4x4 y = rotation_matrix(float3 (0, 1 ,0), _rotY * TAU );
                //float4x4 y = rotation_matrix(_Axis.xyz, _rotY * TAU );
                //float4x4 z = rotation_matrix(float3(0, 0 ,1), _rotZ * TAU  );

                //float4x4 rotation = mul( mul(x , y), z );


                //mul(IN.normal.xyz, (float3x3)unity_WorldToObject)



                float3 worldPos = mul( (float3x3)unity_ObjectToWorld, IN.positionOS);

                float3 offsetPos = worldPos - _Center;

                float3 offsetRotatedPos = mul( offsetPos, rotation_matrix( _ForwardAxis.xyz, -_rotY * TAU ) ); 

                float3 finalPos = offsetRotatedPos + _Center;

                IN.positionOS =  float4( mul( (float3x3)unity_WorldToObject, finalPos) ,0);
                

                
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                // Get the VertexPositionInputs for the vertex position  
                VertexPositionInputs positions = GetVertexPositionInputs(IN.positionOS.xyz);

                // Convert the vertex position to a position on the shadow map
                float4 shadowCoordinates = TransformWorldToShadowCoord(positions.positionWS.xyz);

                // Pass the shadow coordinates to the fragment shader
                OUT.shadowCoords = shadowCoordinates;

                

                OUT.positionWS = positions.positionWS.xyz;
                

                OUT.positionSC = ComputeScreenPos( TransformObjectToHClip(IN.positionOS.xyz)  );


                //OUT.normal = UnityObjectToWorldNormal(IN.normal);
                OUT.normal = normalize(mul(IN.normal.xyz, (float3x3)unity_WorldToObject));
                //OUT.normal = mul(unity_ObjectToWorld, IN.normal) - IN.positionOS ; //UnityObjectToWorldNormal(IN.normal);
                //OUT.normal = normalize(OUT.normal);


                OUT.shadowDarkness = shadowConvolution(positions.positionWS.xyz, OUT.normal, gaussianBlurKernel);


                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 texel = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                // return texel * _Color;

                float2 screenUV = IN.positionSC.xy / IN.positionSC.w;


                float3 color;

                float3 normal = normalize(IN.normal);
                
                Light mainLight = GetMainLight();

                
               // half3 LightingLambert(half3 lightColor, half3 lightDirection, half3 surfaceNormal);

                



                float3 lightdirection = mainLight.direction;
                float3 lightcolor = mainLight.color; // includes intensity

               // float3 viewdirection = normalize(_worldspacecamerapos.xyz - in.posworld);
                //float3 halfdirection = normalize(viewdirection + lightdirection);

                //half shadowAmount = MainLightRealtimeShadow(IN.shadowCoords);
                half shadowAmount = shadowGaussianConvolution( IN.positionWS, IN.normal, boxBlurKernel);


                float ndotl = (dot(normal, lightdirection)+1)/2;
                
                ndotl = min(shadowAmount, ndotl);


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


                float3 diffuse =  lerp(  _ShadowColor , _Color , falloffPlusGradients) * lightcolor * texel.rgb; // * _surfacecolor;
                //float3 specular = specularfalloff * lightcolor;

                color = diffuse;// + specular + _ambientcolor;







                //half ndotl = (dot(IN.normal, lightdir)+1)/2;
                // //half ndotl = (dot(s.normal, lightdir));
                // //ndotl = //tex2d(_ramptex, fixed2(ndotl, 0.5));
                // ndotl = clamp(ndotl, 0, 1);
        
                // half4 c;

                // float shadeamount = lerp( ndotl, ndotl * atten , ndotl );

                // //c.rgb =  lerp( ndotl, ndotl * atten , ndotl ); //s.albedo * _lightcolor0.rgb * ndotl * atten;
                // //c.rgb = (ndotl * atten);

                // float currentcell = round( shadeamount * _shadingbands )/_shadingbands;

                // float3 calculatedcolor = lerp( _shadowcolor, _color, currentcell );

                // c.rgb = calculatedcolor * _lightcolor0 * s.albedo * 1000; 
                // //c.rgb = s.albedo * 1000;

        
                // c.a = s.alpha;
                // return c;
                
                

                //return float4(1,1,1, 1);
                //return float4(falloffPlusGradients.rrr, 1);
                //return float4(gradientMask.rrr, 1);
                //return float4(gradientFalloff.rrr * gradientMask, 1);
                //return float4(percentFromNearestEdge.rrr * gradientMask, 1);
                //return float4(distanceFromNearestEdge.rrr, 1);
                //return float4(diffusefalloff.rrr, 1);
                //return float4(diffusefalloffOffset.rrr, 1);
                //return float4(color, 1);
                //return float4( shadowConvolution( IN.positionWS, IN.normal, boxBlurKernel).rrr, 1);


                 float3 rawOutline = SAMPLE_TEXTURE2D( _OutlineTexture, sampler_PointClamp , screenUV );
                
                float outlineMask = GetOutlineValue(screenUV); // SAMPLE_TEXTURE2D( _OutlineTexture, sampler_PointClamp , screenUV );

              

                //float4 outline = SAMPLE_BLIT( screenUV.xy );

                //float4 outline = tex2D( _OutlineTexture, screenUV.xy );
                //float3 coloredOutlines = outlineMask * _OutlineColor;

                float3 coloredOutlines = outlineMask *  lerp( color, _OutlineColor, _OutlineOpacity) ;


                //return float4( coloredOutlines.xyz , 1);
                return float4( coloredOutlines + color * (1-outlineMask) , 1);


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

        float4x4 rotation_matrix (float3 axis, float angle) {
                axis = normalize(axis);
                float s = sin(angle);
                float c = cos(angle);
                float oc = 1.0 - c;
                
                return float4x4(
                    oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,  0.0,
                    oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,  0.0,
                    oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c,           0.0,
                    0.0,                                0.0,                                0.0,                                1.0);
            }

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
            float4x4 y = rotation_matrix(float3 (0, 1 ,0), _rotY * TAU );
            //float4x4 y = rotation_matrix(_Axis.xyz, _rotY * TAU );
            //float4x4 z = rotation_matrix(float3(0, 0 ,1), _rotZ * TAU  );

            //float4x4 rotation = mul( mul(x , y), z );


            //mul(input.normal.xyz, (float3x3)unity_WorldToObject)



            float3 worldPos = mul( (float3x3)unity_ObjectToWorld, input.positionOS);

            float3 offsetPos = worldPos - _Center;

            float3 offsetRotatedPos = mul( offsetPos, rotation_matrix( _ForwardAxis.xyz, -_rotY * TAU ) ); 

            float3 finalPos = offsetRotatedPos + _Center;

            input.positionOS =  float4( mul( (float3x3)unity_WorldToObject, finalPos) ,0);
                
            //HERES THE CRAZY PART WHERE I MODIFIED THE SOURCE CODE LOL






            output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

            VertexNormalInputs normalInput = GetVertexNormalInputs(input.normal, input.tangentOS);
            output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);

            return output;
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

            #define TAU 6.28318530718


            struct Attributes{

                float3 positionLS : POSITION;
                float3 normalLS : NORMAL;
            };


            struct Varyings{
                float4 positionCS : SV_POSITION;
            };


            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _ShadowColor;
                float4 _BaseMap_ST;
                int _shadingBands;
                float _GradientSize;
                float _TestingOffset;
                float _ShadowSmoothingSize;

                float _rotY;
                //_UpAxis ("UpAxis", Vector) = (0,1,0)
                float3 _ForwardAxis;
                float3 _Center;

                float3 _OutlineColor;
                float _OutlineOpacity;


            CBUFFER_END

            float4 GetShadowPositionHClip(Attributes input)
            {
                float3 positionWS = TransformObjectToWorld(input.positionLS.xyz);
                float3 normalWS = TransformObjectToWorldDir(input.normalLS);

                float invNdotL = 1.0 - saturate(dot(_LightDirection, normalWS));
                float scale = invNdotL * _ShadowBias.y;

                // normal bias is negative since we want to apply an inset normal offset
                positionWS = _LightDirection * _ShadowBias.xxx + positionWS;
                positionWS = normalWS * scale.xxx + positionWS;
                float4 positionCS = TransformWorldToHClip(positionWS);

                #if UNITY_REVERSED_Z
                    positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #else
                    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #endif

                return positionCS;
            }
            float4x4 rotation_matrix (float3 axis, float angle) {
                    axis = normalize(axis);
                    float s = sin(angle);
                    float c = cos(angle);
                    float oc = 1.0 - c;
                
                    return float4x4(
                        oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,  0.0,
                        oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,  0.0,
                        oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c,           0.0,
                        0.0,                                0.0,                                0.0,                                1.0);
                }


            Varyings Vertex(Attributes IN){
            
                Varyings output;



                //float4x4 x = rotation_matrix(float3(1, 0 ,0), _rotX  * TAU  );
                float4x4 y = rotation_matrix(float3 (0, 1 ,0), _rotY * TAU );
                //float4x4 y = rotation_matrix(_Axis.xyz, _rotY * TAU );
                //float4x4 z = rotation_matrix(float3(0, 0 ,1), _rotZ * TAU  );

                //float4x4 rotation = mul( mul(x , y), z );


                //mul(IN.normal.xyz, (float3x3)unity_WorldToObject)



                float3 worldPos = mul( (float3x3)unity_ObjectToWorld, IN.positionLS);

                float3 offsetPos = worldPos - _Center;

                float3 offsetRotatedPos = mul( offsetPos, rotation_matrix( _ForwardAxis.xyz, -_rotY * TAU ) ); 

                float3 finalPos = offsetRotatedPos + _Center;

                IN.positionLS =  float4( mul( (float3x3)unity_WorldToObject, finalPos) ,0);
                




                output.positionCS = GetShadowPositionHClip(IN);



                return output;
        
            }


            half4 Fragment(Varyings v) : SV_Target {
        
            
                return 0;
            }




            ENDHLSL

     
        }
        


        




    }

    


}
