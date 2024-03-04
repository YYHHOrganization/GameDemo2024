Shader "HPostProcessing/HRadialBlur"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BlurRadius ("Blur Radius", Range(0, 15)) = 1
        _BlurCenterX ("Blur Center X", Float) = 0.5
        _BlurCenterY ("Blur Center X", Float) = 0.5
        _BlurIterations("Blur Iterations", Range(1, 10)) = 1
        _BufferRadius("Buffer Radius", Range(0, 15)) = 1
    }
    SubShader
    {
        Tags { 
            "RenderType"="Opaque" 
            "RenderPipeline" = "UniversalPipeline"
        }
        
        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_TexelSize;
            float _BlurRadius;
            float _BlurCenterX;
            float _BlurCenterY;
            int _BlurIterations;
            float _BufferRadius;
        CBUFFER_END
        

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
            Name "RadialBlur"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragRadial

            float4 fragRadial(v2f i) : SV_Target
            {
                float2 _BlurCenter = float2(_BlurCenterX,_BlurCenterY);
                float2 blurVector = (_BlurCenter - i.uv) * _BlurRadius * 0.01f;
                half4 acumulateColor = half4(0, 0, 0, 0);

                float blurParams = saturate(distance(i.uv, _BlurCenter) / _BufferRadius);   
                //[unroll(30)] //暗示Unity把循环进行展开
                for (int j = 0; j < _BlurIterations; j++)
                {
                    acumulateColor += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + blurVector * j * blurParams);
                }

                //return float4(0,0,0,0);
                return acumulateColor / _BlurIterations;
            }
            
            ENDHLSL
        }
        
    }
}
