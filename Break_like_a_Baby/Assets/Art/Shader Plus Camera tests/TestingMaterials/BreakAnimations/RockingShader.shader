Shader "Custom/RockingShader"
{
   Properties
    {
        _rotX ("x rotation", Range(-1,1)) = 0
        _rotY ("y rotation", Range(-1,1)) = 0
        _rotZ ("z rotation", Range(-1,1)) = 0

        _UpAxis ("UpAxis", Vector) = (0,1,0)
        _ForwardAxis ("ForwardAxis", Vector) = (0,1,0)
        _Center ("FirstCenter", Vector) = (0,0,0)


    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define TAU 6.28318530718

            float _rotX;
            float _rotY;
            float _rotZ;
            float3 _UpAxis;
            float3 _ForwardAxis;
            float3 _Center;


            struct MeshData
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

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

            
           


            Interpolators vert (MeshData v)
            {
                Interpolators o;

                // set vertex color
                o.color = v.color;
                

                //float3 up = normalize(_Axis); 
                //float3 right = -normalize(cross(up, float3(0,1,0)));
                
                //float3 forward = -normalize(cross( up, right));
                
                //float3x3 rotationMatrix = float3x3(right, up, forward);
                
                
                
                
                // float3 forward = normalize(forwardVec[instanceID]); // or pre-normalize it in c#
                // float3 right = normalize(cross(forward, float3(0,1,0)));
                // float3 up = cross(right, forward); // does not need to be normalized
                // float3x3 rotationMatrix = float3x3(right, up, forward);


                float4x4 x = rotation_matrix(float3(1, 0 ,0), _rotX  * TAU  );
                float4x4 y = rotation_matrix(float3 (0, 1 ,0), _rotY * TAU );
                //float4x4 y = rotation_matrix(_Axis.xyz, _rotY * TAU );
                float4x4 z = rotation_matrix(float3(0, 0 ,1), _rotZ * TAU  );

                float4x4 rotation = mul( mul(x , y), z );


                //mul(IN.normal.xyz, (float3x3)unity_WorldToObject)

                float3 worldPos = mul( (float3x3)unity_ObjectToWorld, v.vertex);

                float3 offsetPos = worldPos - _Center;

                float3 offsetRotatedPos = mul( rotation_matrix(_ForwardAxis.xyz, _rotY * TAU ) , offsetPos ); 

                float3 finalPos = offsetRotatedPos + _Center;

                v.vertex = float4( mul( (float3x3)unity_WorldToObject, finalPos) ,0);




                //float3 objectSpacePivot =  mul( (float3x3)unity_WorldToObject, _Center);

                //float3 offsetObjectPos = v.vertex - objectSpacePivot;

                //float3 offsetRotatedObjectPos = mul( rotation , offsetObjectPos ); 
                //float3 offsetRotatedObjectPos = mul( rotation_matrix(_ForwardAxis.xyz, _rotY * TAU ) , offsetObjectPos ); 

                //float3 finalObjectPos = offsetRotatedObjectPos + objectSpacePivot;

                //v.vertex = float4( finalObjectPos.xyz ,0);


                //v.vertex = float4( mul( v.vertex.xyz, rotationMatrix ),0 );


                //v.vertex = mul( rotation, v.vertex );




                o.vertex = UnityObjectToClipPos(v.vertex);
                
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                return float4(i.color.rgb, 1.0);
            }
            ENDCG
        }
    }
}
