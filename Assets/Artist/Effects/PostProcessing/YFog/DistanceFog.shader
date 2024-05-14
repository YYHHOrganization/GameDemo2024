Shader "Unlit/DistanceFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor("FogColor",color) = (1,1,1,1)
        _FogIntensity("FogIntensity",float) = 1
        _FogDistance("FogDistance",float) = 1
        
        //噪声
        _NoiseTexture("NoiseTexture",2D) = "white" {}
        _FogXSpeed("FogXSpeed",float) = 0.1
        _FogYSpeed("FogYSpeed",float) = 0.1
        _NoiseAmount("NoiseAmount",float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
        LOD 100

        Pass
        {
            Tags{"LightMode"="UniversalForward"}
            ZTest Always
            ZWrite Off
            Cull Off

            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/core.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float4 ssTexcoord : TEXCOORD1;
            };
            
            CBUFFER_START(UnityPerMaterial)
                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);
                TEXTURE2D(_CameraDepthTexture);
                SAMPLER(sampler_CameraDepthTexture);
                float4 _MainTex_ST;
                float4 _FogColor;
                float _FogIntensity;
                float _FogDistance;

                //噪声
                TEXTURE2D(_NoiseTexture);
                SAMPLER(sampler_NoiseTexture);
                float _FogXSpeed;
                float _FogYSpeed;
                float _NoiseAmount;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.ssTexcoord = ComputeScreenPos(o.positionCS);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 ssuv = i.ssTexcoord.xy/i.ssTexcoord.w; //uv
                float depth = SAMPLE_TEXTURE2D(_CameraDepthTexture,sampler_CameraDepthTexture,ssuv);//深度图采样
                float ssdepth = LinearEyeDepth(depth,_ZBufferParams);//线性深度 ssdepth范围是0-1？

                //噪声
                float2 speed = _Time.y*float2(_FogXSpeed,_FogYSpeed);
                float noise = SAMPLE_TEXTURE2D(_NoiseTexture,sampler_NoiseTexture,i.uv+speed).r-0.5;//噪声
                noise = noise*_NoiseAmount;//噪声

                //以下这个是对的！！
                // half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,i.uv) + pow(ssdepth,_FogDistance) * _FogIntensity * _FogColor;
                //增加噪声
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,i.uv) + pow(ssdepth,_FogDistance) * _FogIntensity * _FogColor * (1+noise);

                

                

                //half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,ssuv) ;
                //step(_FogDistance,ssdepth) 表示ssdepth大于_FogDistance的部分为1，小于的部分为0
                // half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,i.uv) + step(_FogDistance,ssdepth) * _FogIntensity* ssdepth * _FogColor;
                //让其smooth

                
                //想制造出越远的地方，雾越重
                // float fogDensity = 1-ssdepth;
                // fogDensity = saturate(fogDensity);
                // fogDensity = pow(fogDensity, _FogIntensity);
                // //加上噪声
                // fogDensity = fogDensity*(1+noise);
                
                
                // float fogDensity = _FogDistance - ssdepth;//
                // /
                // fogDensity = 
                // fogDensity = fogDensity * _FogIntensity*(1+noise);
                
                
                // float fogDensity = pow(ssdepth,_FogDistance) * _FogIntensity;
                // fogDensity = fogDensity*(1+noise);
                
                // half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,i.uv) + pow(ssdepth,_FogDistance) * _FogIntensity * _FogColor;
                // half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,i.uv) + fogDensity * _FogColor;

                return col;
            }
            ENDHLSL
        }
    }
}
