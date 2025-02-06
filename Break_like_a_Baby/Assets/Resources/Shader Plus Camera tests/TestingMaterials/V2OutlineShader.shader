Shader "Unlit/V2OutlineShader"
{
    Properties
    {


        _MainTex ("Texture", 2D) = "white" {}

        _ColorContrast("Color Sensitivity", float) = 1
        _DepthContrast("Depth Sensitivity", float) = 1
        _DepthThreshold("Depth Threshold", float) = 1
        _NormalContrast("DepthNormalScale", float) = 1
        _NormalThreshold("Normal Threshold", float) = 1

        _ModifiedOutlineHSV("ModifiedOutlineHSV", Color) = (0,1,1,0)
        
        _OutlineColor( "Outline Color" , Color ) = (0,1,0,1)
    }
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        
        Pass
        {
            Tags { "RenderType"="Opaque" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            
            sampler2D _CameraDepthNormalsTexture;
            sampler2D _CameraDepthTexture;

            float4x4 _ClipToView;

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _ColorContrast;
            float _DepthContrast;
            float _DepthThreshold;
            float _NormalContrast;
            float _NormalThreshold;
            float4 _ModifiedOutlineHSV;
            float4 _OutlineColor;


            


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 viewSpaceDir : TEXCOORD2;
            };

            


            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                
                //o.vertex = float4(v.vertex.xy, 0.0, 1.0);
				//o.uv = TransformTriangleVertexToUV(v.vertex.xy);
                
                o.viewSpaceDir = mul(_ClipToView, o.vertex).xyz;
                
                
                
                o.screenPos = ComputeScreenPos(o.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 convolution (float2 uv, float3x3 kernel) {
                float2 ts = _MainTex_TexelSize.xy;
                float3 result = 0;
                
                for(int x = -1; x <= 1; x++) {
                    for(int y = -1; y <= 1; y++) {
                        float2 offset = float2(x, y) * ts;
                        float3 sample = tex2D(_MainTex, uv + offset);
                        result += sample * kernel[x+1][y+1];
                    }
                }

                return result;
            }

            float getIntensity(float3 color){
                float3 weights = float3(.299, 0.587, 0.114);
                float grayscale = dot(color, weights);
                
                return grayscale;
            
            
            }

            float3 sobelConvolution (float2 uv) {
                float2 ts = _MainTex_TexelSize.xy;
                float3 result = 0;
                

                
                float3 p1 = getIntensity( tex2D(_MainTex, uv + float2(-1, 1) * ts) );
                float3 p2 = getIntensity( tex2D(_MainTex, uv + float2(0, 1) * ts) );
                float3 p3 = getIntensity( tex2D(_MainTex, uv + float2(1, 1) * ts) );
                float3 p4 = getIntensity( tex2D(_MainTex, uv + float2(-1, 0) * ts) );
                float3 p5 = getIntensity( tex2D(_MainTex, uv + float2(0, 0) * ts) );
                float3 p6 = getIntensity( tex2D(_MainTex, uv + float2(1, 0) * ts) );
                float3 p7 = getIntensity( tex2D(_MainTex, uv + float2(-1, -1) * ts) );
                float3 p8 = getIntensity( tex2D(_MainTex, uv + float2(0, -1) * ts) );
                float3 p9 = getIntensity( tex2D(_MainTex, uv + float2(1, -1) * ts) );

                result = abs( (p1+ (2*p2)+p3)-(p7+(2*p8)+p9) )+ abs( (p3+ (2*p6) +p9 )-(p1+ (2*p4) + p7) );


                return result;
            }



            float4 getScaledDepthNormals(float2 uv){
                fixed4 depthNormals = tex2D(_CameraDepthNormalsTexture, uv);
                half3 viewNormal;
                float depth;
                DecodeDepthNormal(depthNormals, depth, viewNormal);

                

                //float2 screenUV = i.screenPos.xy / i.screenPos.w;
                //color = Linear01Depth( tex2D(_CameraDepthTexture, screenUV ) );

               float scaledDepth = Linear01Depth(tex2D(_CameraDepthTexture, uv));

               //scaledDepth /= _ProjectionParams.w;


               return float4( viewNormal, scaledDepth);
               //return float4( float3(depthNormals.rg, 0) * (1-scaledDepth ), scaledDepth);
            
            }


            float3 sobelDepthConvolution (float2 uv, float2 screenPos){
                float2 ts = _MainTex_TexelSize.xy;
                float3 result = 0;
                
                float4 sampleDepthNormals = tex2D(_CameraDepthNormalsTexture, uv );

                float4 scaledNormal = (sampleDepthNormals) *2-1;

                //float2 screenUV = i.screenPos.xy / i.screenPos.w;
                //color = Linear01Depth( tex2D(_CameraDepthTexture, screenUV ) );

                //float4 scaledDepth = Linear01Depth(sampleDepthNormals.ba);



                
                float3 p1 = getScaledDepthNormals(uv + float2(-1, 1) * ts).aaa;
                float3 p2 = getScaledDepthNormals( uv + float2(0, 1) * ts ).aaa;
                float3 p3 = getScaledDepthNormals( uv + float2(1, 1) * ts ).aaa;
                float3 p4 = getScaledDepthNormals( uv + float2(-1, 0) * ts ).aaa;
                float3 p5 = getScaledDepthNormals( uv + float2(0, 0) * ts ).aaa;
                float3 p6 = getScaledDepthNormals( uv + float2(1, 0) * ts ).aaa;
                float3 p7 = getScaledDepthNormals( uv + float2(-1, -1) * ts ).aaa;
                float3 p8 = getScaledDepthNormals( uv + float2(0, -1) * ts ).aaa;
                float3 p9 = getScaledDepthNormals( uv + float2(1, -1) * ts ).aaa;

                result = abs( (p1+ (2*p2)+p3)-(p7+(2*p8)+p9) )+ abs( (p3+ (2*p6) +p9 )-(p1+ (2*p4) + p7) );


                return result;            
            }

            float3 sobelNormalConvolution (float2 uv){
                float2 ts = _MainTex_TexelSize.xy;
                float3 result = 0;
                
                float4 sampleDepthNormals = tex2D(_CameraDepthNormalsTexture, uv );

                float4 scaledNormal = (sampleDepthNormals) *2-1;

                //float2 screenUV = i.screenPos.xy / i.screenPos.w;
                //color = Linear01Depth( tex2D(_CameraDepthTexture, screenUV ) );

                float4 scaledDepth = Linear01Depth(sampleDepthNormals.ab);



                
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

            

            fixed4 frag (v2f i) : SV_Target
            {
             
                
				//return float4(i.viewSpaceDir, 1);


                //float3 viewNormal = normal0 * 2 - 1;
                float3 viewNormal = (getScaledDepthNormals(i.uv).rgb);
                //float NdotV =1- (dot(viewNormal, -i.viewSpaceDir)+.5f)*.5f;
                float NdotV =  1-dot(viewNormal, -i.viewSpaceDir);

                float normalScale01 = saturate((NdotV - _NormalContrast) / (1 - _NormalContrast));
                float scaledNormalThreshold = normalScale01 * _NormalContrast + 1;

                float scaledDepthThreshold = _DepthThreshold * scaledNormalThreshold * (getScaledDepthNormals(i.uv).a);
                //float scaledDepthThreshold =  (getScaledDepthNormals(i.uv).a) * _NormalContrast ;//* NdotV;

                

                //return float4(normalThreshold01.xxx,0);
                //return float4((NdotV.xxx),0);
                //return float4((normalScale01.xxx),0);
                //return float4( (scaledNormalThreshold.xxx),0);
               




                ////float normalThreshold01 = saturate((NdotV - _DepthNormalThreshold) / (1 - _DepthNormalThreshold));
				// Scale the threshold, and add 1 so that it is in the range of 1..._NormalThresholdScale + 1.
				////float normalThreshold = normalThreshold01 * _DepthNormalThresholdScale + 1;

				// Modulate the threshold by the existing depth value;
				// pixels further from the screen will require smaller differences
				// to draw an edge.
				////float depthThreshold = _DepthThreshold * depth0 * normalThreshold;






                //float3 decodedDepthNormals = DecodeDepthNormal( _CameraDepthNormalsTexture.r, _CameraDepthNormalsTexture.g, _CameraDepthNormalsTexture.b );

                //float4 decoded = DecodeDepthNormal( tex2D(_CameraDepthNormalsTexture, i.uv ));
                
                //return(i.viewSpaceDir,0);
                
                //float4 depthNormal = sampleDepthNormals(i.uv, 0);

                //float4 decodedDepthNormal

                float3 sample = tex2D( _MainTex,i.uv);
                
                
                float3 color = 0;
               
                
                float3 colorMagnitude = sobelConvolution(i.uv);

                //color = step(_Threshold, magnitude);
                //color = smoothstep(_Threshold, 1, magnitude);
                //colorMagnitude = pow(colorMagnitude, _ColorContrast);
                colorMagnitude = 0;

                //color = magnitude;

                float depth = Linear01Depth( tex2D(_CameraDepthTexture, i.uv ) );



                float3 depthMagnitude = sobelDepthConvolution(i.uv, i.screenPos);
                //depthMagnitude = pow(depthMagnitude, _DepthContrast);

                depthMagnitude = step(_DepthThreshold * depth, depthMagnitude);

                //depthMagnitude = depthMagnitude * step( _DepthThreshold ,depthMagnitude  );

                
                
                float3 normalMagnitude = sobelNormalConvolution(i.uv);
                //normalMagnitude = pow(normalMagnitude, _NormalContrast);

                normalMagnitude = getIntensity(normalMagnitude).xxx;
                
                //normalMagnitude = normalMagnitude * step( _NormalThreshold ,normalMagnitude  );
                normalMagnitude = step( _NormalThreshold ,normalMagnitude  );
                //normalMagnitude = step( _NormalThreshold - (scaledDepthThreshold) ,normalMagnitude  );




                float fullMagnitude = max(  max( colorMagnitude , normalMagnitude ) , depthMagnitude    );


                float3 sampleAsHSV = ConvertToHSV(sample);

                //float newV = 1;

                sampleAsHSV.z = 0;

                //sampleAsHSV.yz = _ModifiedOutlineHSV.yz *step(.1, sampleAsHSV.y)  + sampleAsHSV.yz *    (1-step(.1, sampleAsHSV.y));
                //sampleAsHSV.yz = _ModifiedOutlineHSV.yz *step(.1, sampleAsHSV.y)  + float3(1,0,1).yz *    (1-step(.1, sampleAsHSV.y));

                float3 boostedSample = ConvertFromHSV(sampleAsHSV);


                color = lerp( tex2D( _MainTex,i.uv) , boostedSample, fullMagnitude);



                

                //return  Linear01Depth( tex2D(_CameraDepthTexture, i.uv ) );

                //return float4(depthMagnitude,1);

                //return float4( color, 0);
                //return float4( getScaledDepthNormals(i.uv).rgb, 0);
                ///return float4( i.vertex.xyz, 0);

                return float4(color,1);

            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
