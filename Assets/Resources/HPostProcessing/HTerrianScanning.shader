Shader "HPostProcessing/HTerrianScanning"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanDepth("ScanDepth",Range(0, 1))=0
		_ScanWidth("ScanWidth",Range(0.01, 100))=1
        [HDR]_LineColor("LineColor",Color)=(1,1,0,1)
		_CamFar("CamFar",float)=500
        _MainTex("Texture", 2D) = "white" {}
        _DetailTex("Texture", 2D) = "white" {}
        
        _MidColor("Mid Color", Color) = (1, 1, 1, 0)
        _TrailColor("Trail Color", Color) = (1, 1, 1, 0)
        _HBarColor("Horizontal Bar Color", Color) = (0.5, 0.5, 0.5, 0)

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
            float4 _LineColor;
            float _CamFar;
        
            float _LeadSharp;
            float4 _MidColor;
            float4 _TrailColor;
            float4 _HBarColor;
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
            float4 positionWS : TEXCOORD2;
        };

        v2f vert(appdata v)
        {
            v2f o;
            o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
            o.uv = v.uv;
            o.uv_depth = v.uv.xy;
            o.positionWS = float4(TransformObjectToWorld(v.positionOS.xyz), 1.0);//将物体坐标转换为世界坐标
            
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
            float4 horizBars(float2 p)
            {
                //*frac* 函数返回其参数的小数部分。
                //*round* 函数将其参数四舍五入到最接近的整数。
                //abs(frac(p.y * 100) * 2) 的结果是 p.y * 100 的小数部分乘以 2 的绝对值，这个值的范围是 [0, 2]。
                //然后 round 函数将这个值四舍五入到最接近的整数，所以结果只能是 0、1 或 2。
                return 1 - saturate(round(abs(frac(p.y * 100) * 2)));
            }
            float4 frag (v2f i) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                //计算深度
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv_depth).r;

                //linearDepth 是一个变量，它表示从摄像机到场景中某个像素的线性深度。
                //这个深度是通过从深度纹理 _CameraDepthTexture 中采样得到的，
                //然后通过 Linear01Depth 函数将其转换为线性深度。
                
                // 在计算机图形学中，深度通常被存储在一个叫做深度缓冲（Depth Buffer）或 Z 缓冲（Z-Buffer）的特殊图像中。
                // 这个深度值通常是非线性的，这是因为人眼对接近的物体比对远处的物体更敏感，所以计算机图形系统通常会给接近的物体分配更多的深度值。
                // Linear01Depth 函数的作用就是将这个非线性的深度值转换为线性的，范围在 0 到 1 之间。
                // 这样处理后，深度值就可以直接用于计算和比较了。
                float linearDepth = Linear01Depth(depth, _ZBufferParams);
                //return float4(linearDepth, linearDepth, linearDepth, 1) * 10;
                
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

                /*
                linearDepth *= 30;
                //这个会让场景以条带方式向外扫，但我们需要的效果是整个场景都有扫描效果
                //判断线性深度是否在扫描深度 _ScanDepth 和 _ScanDepth - _ScanWidth/_CamFar 之间
                if(linearDepth < _ScanDepth && (linearDepth>(_ScanDepth-_ScanWidth/_CamFar)) && linearDepth<1) //最后linearDepth<1是为了防止到天空盒的位置
                {
                    //return lerp(col, float4(1,1,0,1), _ScanDepth);
                    //做一个渐变效果
                    //在这个深度范围内，对颜色进行插值处理，使得颜色有一个从原始颜色到某个颜色的过渡效果
					float scanPercent = 1 - (_ScanDepth-linearDepth)/(_ScanWidth/_CamFar);
                    //float4 lerpColor = lerp(col,float4(1,1,0,1) * 15.0f,scanPercent);
                    float4 lerpColor = lerp(col,_LineColor,scanPercent);
					return lerpColor;
                }
                */
                _ScanWidth = _ScanWidth / 10000;
                _ScanDepth = _ScanDepth / 10;

               //float4 wsDir = linearDepth * i.positionWS;
                float4 wsDir = linearDepth * normalize(i.positionWS - float4(_WorldSpaceCameraPos,1.0));
                float3 wsPos = _WorldSpaceCameraPos + wsDir;
                float4 scannerCol = float4(0, 0, 0, 0);

                //测试
                _MidColor = float4(1, 1, 1, 1);//float4(1, 1, 1, 1)这是白色
                _TrailColor = float4(1,0, 0, 1)*2.0f;//float4(1, 0, 0, 1)这是红色

                float dist = distance(wsPos, _WorldSpaceCameraPos);
                //判断线性深度是否在扫描深度 _ScanDepth 和 _ScanDepth - _ScanWidth/_CamFar 之间
                if (dist < _ScanDepth && dist > _ScanDepth- _ScanWidth)
                {
                    //当前片元（fragment）距离扫描线的相对距离。
                    float diff = 1 - (_ScanDepth - dist) / (_ScanWidth);
                    
                    // 这行代码计算的是扫描线的颜色。
                    // lerp 是线性插值函数，它根据 diff 的值在 _MidColor（扫描线中心的颜色）和 _LineColor（扫描线边缘的颜色）之间进行插值。
                    // pow(diff, _LeadSharp) 是对 diff 进行了指数运算，
                    // _LeadSharp 是一个大于 0 的数，这样可以使得颜色从 _MidColor 到 _LineColor 的过渡更加尖锐
                    half4 edge = lerp(_MidColor, _LineColor, pow(diff, _LeadSharp));
                    
                    scannerCol = lerp(_TrailColor, edge, diff) + horizBars(i.uv) * _HBarColor;

                    //scannerCol *= diff; 这行代码是对最终的扫描线颜色进行了淡化处理。
                    //diff 的值越小，颜色就越淡，这样可以使得扫描线的边缘颜色更淡，从而产生一种扫描线从中心向外扩散的效果
                    scannerCol *= diff;
                }

                return col + scannerCol;
                
                //return float4(1,1,1,1);
            }
            
            ENDHLSL
        }


    }
}
