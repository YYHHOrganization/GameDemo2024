Shader "Unlit/HighFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogColor("FogColor",color) = (1,1,1,1)
        _FogIntensity("FogIntensity",float) = 1
        _FogHigh("FogHigh",float) = 1
        
         //噪声
        _NoiseTexture("NoiseTexture",2D) = "white" {}
        _FogXSpeed("FogXSpeed",float) = 0.1
        _FogYSpeed("FogYSpeed",float) = 0.1
        _NoiseAmount("NoiseAmount",float) = 1
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
                float _FogHigh;

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
                float ssdepth = LinearEyeDepth(depth,_ZBufferParams);//线性深度
                float3 postionWS = ComputeWorldSpacePosition(ssuv,depth,unity_MatrixInvVP);//世界坐标重建

                ////下方有雾，越低雾越多
                // float fogDensity = saturate((postionWS.y+_FogHigh));//雾密度
                float fogDensity = saturate((_FogHigh-postionWS.y));//雾密度

                //噪声
                float2 speed = _Time.y*float2(_FogXSpeed,_FogYSpeed);
                float noise = SAMPLE_TEXTURE2D(_NoiseTexture,sampler_NoiseTexture,i.uv+speed).r-0.5;//噪声
                noise = noise*_NoiseAmount;//噪声
                //(tex2D(_NoiseTexture,i.uv+speed).r-0.5)*_NoiseAmount;//噪声

                fogDensity = saturate(fogDensity*_FogIntensity*(1+noise));//雾密度

                float4 finalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,i.uv);//纹理采样
                //finalColor.rgb = lerp(finalColor.rgb,_FogColor,fogDensity);//颜色插值
                ////下方有雾，越低雾越多
                finalColor.rgb = lerp(finalColor.rgb,_FogColor,fogDensity);//颜色插值
                return finalColor;
                
                //没有平滑的雾 
                // half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,i.uv) + step(_FogHigh,postionWS.y) * _FogIntensity * _FogColor * ssdepth;
                
                // 使用smoothstep函数替换step函数
                //上方有雾，越高雾越多
                // half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_M
                // ainTex,i.uv) + smoothstep(0.0, _FogHigh, postionWS.y) * _FogIntensity * _FogColor * ssdepth;
                //下方有雾，越低雾越多
                //half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,i.uv) + smoothstep( _FogHigh, 0.0,postionWS.y) * _FogIntensity * _FogColor * ssdepth;
                // return col;
            }
            ENDHLSL
        }
    }
}
