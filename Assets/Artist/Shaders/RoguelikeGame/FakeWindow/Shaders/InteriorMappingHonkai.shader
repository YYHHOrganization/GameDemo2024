Shader "MyShaders/InteriorMapping_2D_URP_Honkai"
{
    Properties
    {
        _RoomTex("Room Atlas RGB (A - back wall fraction)", 2D) = "white" {}
        _RoomFarTex("Room Texture Far", 2D) = "white" {}
        _RoomMiddleTex("Room Texture Middle", 2D) = "white" {}
        _RoomNearTex("Room Texture Near", 2D) = "white" {}
        _RoomSurfaceTex("Room Texture Surface", 2D) = "white" {}
        
        _Rooms("Room Atlas Rows&Cols (XY)", Vector) = (1,1,0,0)
        _RoomDepth("Room Depth",range(0.001,0.999)) = 0.5
        _RoomNearDepth("Room Near Depth",range(0.001,0.999)) = 0.5
        _RoomFarDepth("Room Far Depth",range(0.001,0.999)) = 0.5
        _RoomMiddleDepth("Room Middle Depth",range(0.001,0.999)) = 0.5
        
        _RoomSurfaceNormalMap("Room Surface Normal Map", 2D) = "bump" {}
        _NormalStrength("Normal Strength", Range(0, 0.5)) = 0.03
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" } // Queue用来保证此时不透明物体的渲染已经完成
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };

            struct Varyings
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 tangentViewDir : TEXCOORD1;
                float2 uvFar : TEXCOORD2;
                float2 uvNear : TEXCOORD3;
                float2 uvMiddle : TEXCOORD4;
                float2 uvNormal : TEXCOORD5;
                //float4 scrPos : TEXCOORD5;
            };

            TEXTURE2D(_RoomTex);
            SAMPLER(sampler_RoomTex);
            TEXTURE2D(_RoomFarTex);
            SAMPLER(sampler_RoomFarTex);
            TEXTURE2D(_RoomMiddleTex);
            SAMPLER(sampler_RoomMiddleTex);
            TEXTURE2D(_RoomNearTex);
            SAMPLER(sampler_RoomNearTex);
            TEXTURE2D(_RoomSurfaceTex);
            SAMPLER(sampler_RoomSurfaceTex);
            TEXTURE2D(_RoomSurfaceNormalMap);
            SAMPLER(sampler_RoomSurfaceNormalMap);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _RoomTex_ST;
                float4 _RoomFarTex_ST;
                float4 _RoomMiddleTex_ST;
                float4 _RoomNearTex_ST;
                float4 _Rooms;
                float _RoomDepth;
                float _RoomNearDepth;
                float _RoomFarDepth;
                float _RoomMiddleDepth;
                float4 _CameraOpaqueTexture_TexelSize;
                float4 _RoomSurfaceNormalMap_ST;
                float _NormalStrength;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.pos = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _RoomTex);
                o.uvFar = TRANSFORM_TEX(v.uv, _RoomFarTex);
                o.uvNear = TRANSFORM_TEX(v.uv, _RoomNearTex);
                o.uvMiddle = TRANSFORM_TEX(v.uv, _RoomMiddleTex);
                o.uvNormal = TRANSFORM_TEX(v.uv, _RoomSurfaceNormalMap);

                // get tangent space camera vector
                float4 objCam = mul(UNITY_MATRIX_I_M, float4(_WorldSpaceCameraPos, 1.0));
                float3 viewDir = v.positionOS.xyz - objCam.xyz;
                float tangentSign = v.tangentOS.w * unity_WorldTransformParams.w;
                float3 bitangent = cross(v.normalOS.xyz, v.tangentOS.xyz) * tangentSign;
                o.tangentViewDir = float3(
                    dot(viewDir, v.tangentOS.xyz),
                    dot(viewDir, bitangent),
                    dot(viewDir, v.normalOS)
                    );
                
                return o;
            }

            float2 getRoomUV(float farFrac, Varyings i, float2 roomUV)
            {
                float depthScale = 1.0 / (1.0 - farFrac) - 1.0;

                // raytrace box from view dir
                // normalized box space's ray start pos is on triangle surface, where z = -1
                float3 pos = float3(roomUV * 2 - 1, -1);
                // transform input ray dir from tangent space to normalized box space
                i.tangentViewDir.z *= -depthScale;

                // 预先处理倒数  t=(1-p)/view=1/view-p/view
                float3 id = 1.0 / i.tangentViewDir;
                float3 k = abs(id) - pos * id;
                float kMin = min(min(k.x, k.y), k.z);
                pos += kMin * i.tangentViewDir;

                // remap from [-1,1] to [0,1] room depth
                float interp = pos.z * 0.5 + 0.5;

                // account for perspective in "room" textures
                // assumes camera with an fov of 53.13 degrees (atan(0.5))
                // visual result = transform nonlinear depth back to linear
                float realZ = saturate(interp) / depthScale + 1;
                interp = 1.0 - (1.0 / realZ);
                interp *= depthScale + 1.0;
                
                // iterpolate from wall back to near wall
                float2 interiorUV = pos.xy * lerp(1.0, farFrac, interp);

                interiorUV = interiorUV * 0.5 + 0.5;
                float4 bump = SAMPLE_TEXTURE2D(_RoomSurfaceNormalMap, sampler_RoomSurfaceNormalMap, i.uvNormal);
                float2 distortion = UnpackNormal(bump).rg;
                interiorUV += distortion * _NormalStrength; // 根据需要调整偏移强度
                
                return interiorUV;
            }
            
            // psuedo random
            float2 rand2(float co) {
                return frac(sin(co * float2(12.9898,78.233)) * 43758.5453);
            }

            float4 frag(Varyings i) : SV_Target
            {
                // room uvs
                float2 roomUV = frac(i.uv);
                float2 roomUVFar = frac(i.uvFar);
                float2 roomUVNear = frac(i.uvNear);
                float2 roomUVMiddle = frac(i.uvMiddle);

                // Specify depth manually
                float farFrac = _RoomDepth;
                float farDepth = _RoomFarDepth;
                float nearDepth = _RoomNearDepth;
                float middleDepth = _RoomMiddleDepth;
                
                float2 interiorUV = getRoomUV(farFrac, i, roomUV);
                float2 farUV = getRoomUV(farDepth, i, roomUVFar);
                float2 nearUV = getRoomUV(nearDepth, i, roomUVNear);
                float2 middleUV = getRoomUV(middleDepth, i, roomUVMiddle);
                
                float4 room = SAMPLE_TEXTURE2D(_RoomTex, sampler_RoomTex,  interiorUV.xy);
                float4 far = SAMPLE_TEXTURE2D(_RoomFarTex, sampler_RoomFarTex, farUV.xy);
                float4 near = SAMPLE_TEXTURE2D(_RoomNearTex, sampler_RoomNearTex, nearUV.xy);
                float4 middle = SAMPLE_TEXTURE2D(_RoomMiddleTex, sampler_RoomMiddleTex, middleUV.xy);
                float4 surface = SAMPLE_TEXTURE2D(_RoomSurfaceTex, sampler_RoomSurfaceTex, roomUV);
                float4 lerp1 =  lerp(room, far, far.a);
                float4 lerp2 =  lerp(lerp1, middle, middle.a);
                float4 lerp3 =  lerp(lerp2, near, near.a);
                float4 lerp4 =  lerp(lerp3, surface, surface.a);

                // //尝试来点玻璃偏移,对lerp4进行偏移
                // float4 bump = SAMPLE_TEXTURE2D(_RoomSurfaceNormalMap, sampler_RoomSurfaceNormalMap, roomUV);
                // float2 distortion = UnpackNormal(bump).rg;
                // float2 screenUV = GetNormalizedScreenSpaceUV(i.pos);
                // screenUV += distortion * 0.01;
                // float3 screenCol = SampleSceneColor(screenUV);
                // return float4(screenCol, 1.0);
                return lerp4;
                
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}