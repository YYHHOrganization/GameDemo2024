Shader "HEnvironment/HBaseAirWall"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        _PlayerPos("Player Position", Vector) = (0,0,0,0)
        _EdgeRange("Edge Range", Vector) = (2,6,0,0)
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        
        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_TexelSize;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _PlayerPos;
            float4 _EdgeRange;
        CBUFFER_END
        

        struct appdata
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float4 positionCS : SV_POSITION;
            float4 positionWS : TEXCOORD1;
            float2 uv : TEXCOORD0;
        };

        v2f vert(appdata v)
        {
            v2f o;
            o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
            o.positionWS = mul(unity_ObjectToWorld, v.positionOS);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            return o;
        }
        
        ENDHLSL

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragment

            float4 fragment(v2f i) : SV_Target
            {
                float dist = abs(distance(i.positionWS, _PlayerPos));
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
    
                col.a *= (1 - smoothstep(_EdgeRange.x, _EdgeRange.y, dist));
                return col * _Color;
                
            }
            
            ENDHLSL
        }
        
    }
}
