Shader "Scene/SandSystem/RutFade"
{
    Properties
    {
        [Header(Texture)]
        [NoScaleOffset] _MainTex("轨迹渲染纹理", 2D) = "black" {}
        _AttenTime ("衰减时间", float) = 10
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 

            CBUFFER_START(UnityPerMaterial)
                sampler2D _MainTex;
                float _AttenTime;
            CBUFFER_END

            struct a2v
            {
                float4 posOS	: POSITION;
                float2 uv0      : TEXCOORD0;
            };

            struct v2f
            {
                float4 posCS	    : SV_POSITION;
                float2 uv0      : TEXCOORD0;
            };

            v2f vert(a2v i)
            {
                v2f o;

                //坐标
                o.posCS = TransformObjectToHClip(i.posOS.xyz);

                //UV
                o.uv0 = i.uv0;

                return o;
            }

            static float4 _Zero = float4(0.5, 0.5, 1.0, 0.5);
            float4 frag(v2f i) : SV_Target
            {
                //轨迹采样
                float4 rutParams = tex2D(_MainTex, i.uv0);
                float dt = unity_DeltaTime.x / _AttenTime;

                //高度衰减
                float height = _Zero.w;
                if (rutParams.w != _Zero.w)
                {
                    height = 2.0 * rutParams.w - 1.0;
                    float _sign = sign(height);
                    height -= _sign * dt;
                    height = (sign(height) == _sign) ? (0.5 * height + 0.5) : _Zero.w;
                }

                //法线衰减
                float3 nDirTS = _Zero.xyz;
                if (rutParams.z != _Zero.z)
                {
                    nDirTS = 2.0 * rutParams.xyz - 1.0;
                    float2 nDirTS_XY_OLD = nDirTS.xy;
                    nDirTS.xy -= normalize(nDirTS.xy) * dt;
                    nDirTS = (dot(nDirTS_XY_OLD, nDirTS.xy) > 0) ? (0.5 * normalize(nDirTS) + 0.5) : _Zero.xyz;
                }

                return float4(nDirTS, height);
            }
            ENDHLSL
        }
    }
}