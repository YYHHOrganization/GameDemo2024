Shader "HPostProcessing/HGaussianBlur"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Spread ("Spread", Range(0, 1)) = 0.5
        _GridSize ("Grid Size", Integer) = 1
    }
    SubShader
    {
        Tags { 
            "RenderType"="Opaque" 
            "RenderPipeline" = "UniversalPipeline"
        }
        
        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #define E 2.71828f

        sampler2D _MainTex;
        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_TexelSize;
            uint _GridSize;
            float _Spread;
        CBUFFER_END

        float gaussian(int x)
        {
            float sigmaSqu = _Spread * _Spread;
            return (1 / (sqrt(TWO_PI * sigmaSqu))) * pow(E, -((x * x) / (2 * sigmaSqu)));
        }

        struct appdata
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        v2f vert(appdata v)
        {
            v2f o;
            o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
            o.uv = v.uv;
            return o;
        }
        
        ENDHLSL

        Pass
        {
            Name "Horizontal"
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag_horizontal

            float4 frag_horizontal(v2f i) : SV_Target
            {
                float3 col = float3(0, 0, 0);
                float gridSum = 0.0f;

                int upper = ((_GridSize - 1) / 2);
                int lower = -upper;
                for(int x = lower;x <= upper;++x)
                {
                    float gauss = gaussian(x);
                    gridSum += gauss;
                    float2 uv = i.uv + float2(x * _MainTex_TexelSize.x, 0);
                    col += gauss * tex2D(_MainTex, uv).xyz;
                }

                col /= gridSum;
                return float4(col, 1.0);
            }
            
            ENDHLSL
        }
        
        Pass
        {
            Name "Vertical"
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag_vertical

            float4 frag_vertical(v2f i) : SV_Target
            {
                float3 col = float3(0, 0, 0);
                float gridSum = 0.0f;

                int upper = ((_GridSize - 1) / 2);
                int lower = -upper;
                for(int y = lower;y <= upper;++y)
                {
                    float gauss = gaussian(y);
                    gridSum += gauss;
                    float2 uv = i.uv + float2(0, y * _MainTex_TexelSize.y);
                    col += gauss * tex2D(_MainTex, uv).xyz;
                }

                col /= gridSum;
                return float4(col, 1.0);
            }
            
            ENDHLSL
        }
    }
}
