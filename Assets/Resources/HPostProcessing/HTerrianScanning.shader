Shader "HPostProcessing/HTerrianScanning"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanDepth("ScanDepth",Range(0, 1))=0
		_ScanWidth("ScanWidth",Range(0.01, 100))=1
		_CamFar("CamFar",float)=500
    }
    SubShader
    {
        Tags { 
            "RenderType"="Opaque" 
            "RenderPipeline" = "UniversalPipeline"
        }
        
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        
        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_TexelSize;
            float _ScanDepth;
            float _ScanWidth;
            float _CamFar;
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
            float2 uv_depth : TEXCOORD1;
        };

        v2f vert(appdata v)
        {
            v2f o;
            o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
            o.uv = v.uv;
            o.uv_depth = v.uv.xy;
            return o;
        }

        // camera depth texture
        TEXTURE2D_X_FLOAT(_CameraDepthTexture);
        SAMPLER(sampler_CameraDepthTexture);

        ENDHLSL

        Pass
        {
            Name "TerrianScanning"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 frag (v2f i) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                //计算深度
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv_depth).r;
                float linearDepth = Linear01Depth(depth, _ZBufferParams);
                //return float4(linearDepth, linearDepth, linearDepth, 1) * 10;

                linearDepth *= 30;
                
                // if(linearDepth < _ScanDepth)
                // {
                //     //近处比较大，远处比较小，再远处进不了下面的if判断
                //     float value = _ScanDepth - linearDepth;
                //     float4 displayColor = float4(1,1,0,1);
                //     float4 lerpColor;
                //     if((linearDepth>(_ScanDepth-_ScanWidth/_CamFar)) && linearDepth<1)
                //     {
                //         float scanPercent = 1 - (_ScanDepth-linearDepth)/(_ScanWidth/_CamFar);
                //         lerpColor = lerp(col,float4(1,1,0,1),scanPercent);
                //     }
                //     col = lerp(lerpColor, displayColor, value);
                // }
                // return col;
                
                //这个会让场景以条带方式向外扫，但我们需要的效果是整个场景都有扫描效果
                if(linearDepth < _ScanDepth && (linearDepth>(_ScanDepth-_ScanWidth/_CamFar)) && linearDepth<1) //最后linearDepth<1是为了防止到天空盒的位置
                {
                    //return lerp(col, float4(1,1,0,1), _ScanDepth);
                    //做一个渐变效果
					float scanPercent = 1 - (_ScanDepth-linearDepth)/(_ScanWidth/_CamFar);
                    float4 lerpColor = lerp(col,float4(1,1,0,1) * 15.0f,scanPercent);
					return lerpColor;
                }

                return col;
                //return float4(1,1,1,1);
            }
            ENDHLSL
        }
    }
}
