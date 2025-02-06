Shader "Custom/ToonShadingShadows"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _shadingBands ("ShadingBandsNumber", int) = 3
        _shadowAbsorption ("Metallic", Range(0,1))=0



    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM


        

        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Toon fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _ShadowColor;
        int _shadingBands;


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        

        


        // void surf (Input IN, inout SurfaceOutputStandard o)
        // {
        //     // Albedo comes from a texture tinted by color
        //     fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
        //     o.Albedo = c.rgb;
        //     // Metallic and smoothness come from slider variables
        //     o.Metallic = _Metallic;
        //     o.Smoothness = _Glossiness;
        //     o.Alpha = c.a;
        // }

        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * .001;
            //o.Emission = 0;

            //o.Albedo = IN.uv_MainTex;

            //o.Alpha = 1;

            
            
        }


        fixed4 LightingToon (SurfaceOutput s, fixed3 lightDir,
        fixed atten)
        {
        half NdotL = (dot(s.Normal, lightDir)+1)/2;
        //half NdotL = (dot(s.Normal, lightDir));
        //NdotL = //tex2D(_RampTex, fixed2(NdotL, 0.5));
        NdotL = clamp(NdotL, 0, 1);
        
        fixed4 c;

        float shadeAmount = lerp( NdotL, NdotL * atten , NdotL );

        //c.rgb =  lerp( NdotL, NdotL * atten , NdotL ); //s.Albedo * _LightColor0.rgb * NdotL * atten;
        //c.rgb = (NdotL * atten);

        float currentCell = round( shadeAmount * _shadingBands )/_shadingBands;

        float3 calculatedColor = lerp( _ShadowColor, _Color, currentCell );

        c.rgb = calculatedColor * _LightColor0 * s.Albedo * 1000; 
        //c.rgb = s.Albedo * 1000;

        
        c.a = s.Alpha;
        return c;
        }


        ENDCG
    }
    FallBack "Diffuse"
}
