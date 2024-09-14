Shader "Scene/SandSystem/RutPaint"
{
    Properties
    {
        [Header(Texture)]
        [NoScaleOffset] _MainTex("轨迹渲染纹理", 2D) = "bump" {}//渲染纹理命名为_MainTex，保证Graphics.Blit传递正确
        [NoScaleOffset] _BrushTex ("笔刷法线高度图", 2D) = "bump" {}

        [Space(20)]
        [Header(Shape)]
        _BrushPosTS_Offset ("笔刷UV坐标", vector) = (0.5, 0.5, 0.0, 0.0)
        _BrushRadius ("笔刷半径", range(0, 0.5)) = 0.005
        _BrushInt ("笔刷强度", range(0, 2)) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalRenderPipeline"
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
                //贴图
                sampler2D _MainTex;
                sampler2D _BrushTex;
                sampler2D _BrushTexMask;
                //形状
                float4 _BrushPosTS_Offset;
                float _BrushRadius;
                float _BrushInt;
                float3 _playerDirection;
            CBUFFER_END

            //重映射01（把input依据min和max的范围remap到0~1）
            float Remap(float min, float max, float input)
            {
                float k = 1.0 / (max - min);
                float b = -min * k;
                return k * input + b;
            }

            struct a2v
            {
                float4 posOS	: POSITION;
                float2 uv0      : TEXCOORD0;
            };

            struct v2f
            {
                float4 posCS	    : SV_POSITION;
                float2 uv0      : TEXCOORD0;
                float2 uv_Main      : TEXCOORD1;
                float2 uv_Brush      : TEXCOORD2;
            };

            v2f vert(a2v i)
            {
                v2f o;

                //坐标
                o.posCS = TransformObjectToHClip(i.posOS.xyz);

                //UV
                o.uv0 = i.uv0;
                o.uv_Main = i.uv0 + _BrushPosTS_Offset.zw;

                //笔刷局部UV计算
                float2 uv00 = _BrushPosTS_Offset.xy - _BrushRadius;
                float2 uv11 = _BrushPosTS_Offset.xy + _BrushRadius;
                o.uv_Brush = float2(Remap(uv00.x, uv11.x, i.uv0.x), Remap(uv00.y, uv11.y, i.uv0.y));

                return o;
            }

            static float4 _Zero = float4(0.5, 0.5, 1.0, 0.5);
            float4 frag(v2f i) : SV_Target
            {
                //轨迹
                float4 rutParams = tex2D(_MainTex, i.uv_Main);

                //旋转UV，用于让脚印与玩家前进方向一致
                float angle = atan2(_playerDirection.x, _playerDirection.z);
                //angle + 90度
                angle -= 1.57079632679;
                float cosAngle = cos(angle);
                float sinAngle = sin(angle);

                // 创建旋转矩阵
                float2x2 rotationMatrix = float2x2(cosAngle, -sinAngle, sinAngle, cosAngle);

                // 旋转UV坐标
                float2 rotatedUV_Brush = mul(rotationMatrix, i.uv_Brush * 2.0 - 1.0) * 0.5 + 0.5;
                i.uv_Brush = rotatedUV_Brush;
                //笔刷
                float4 var_BrushTex = tex2D(_BrushTex, i.uv_Brush);
                float4 var_BrushTexMask = tex2D(_BrushTexMask, i.uv_Brush);
                
                float brushMask = step(0.0, i.uv_Brush.x) * step(i.uv_Brush.x, 1.0) * step(0.0, i.uv_Brush.y) * step(i.uv_Brush.y, 1.0);
                
                //法线混合
                float3 nDirTS_OLD = 2.0 * rutParams.xyz - 1.0;
                var_BrushTex.z *= var_BrushTexMask.a;
                float3 nDirTS_Brush = 2.0 * var_BrushTex.xyz - 1.0; //本来法线贴图（0.5，0.5，1）
                nDirTS_Brush.xy *= _BrushInt * brushMask * var_BrushTexMask.a;
                float3 nDirTS_NEW = float3(nDirTS_OLD.xy / nDirTS_OLD.z + nDirTS_Brush.xy / nDirTS_Brush.z, 1.0);
                nDirTS_NEW = 0.5 * normalize(nDirTS_NEW) + 0.5;

                //高度混合
                float h_OLD = 2.0 * rutParams.a - 1.0;
                float h_Brush = 2.0 * var_BrushTex.a - 1.0;
                h_Brush *= _BrushInt * brushMask * var_BrushTexMask.a;
                float h_NEW = saturate(0.5 * (h_OLD + h_Brush) + 0.5);

                //边缘遮罩 
                float edgeMask = saturate(Remap(0.5, 0.4, length(i.uv0 - float2(0.5, 0.5))));

                //混合
                float4 finalRGBA = edgeMask * float4(nDirTS_NEW, h_NEW) + (1.0 - edgeMask) * _Zero;
                return finalRGBA;
            }
            ENDHLSL
        }
    }
}