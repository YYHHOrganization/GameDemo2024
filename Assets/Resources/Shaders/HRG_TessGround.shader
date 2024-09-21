Shader "Scene/SandSystem/TessGround"
{
    Properties
    {
        [Header(Texture)]
        _MainTex ("主贴图", 2D) = "gray" {}
        [NoScaleOffset]_NormalMap ("法线图", 2D) = "bump" {}
        _FlashTex ("闪烁遮罩", 2D) = "black" {}
        [NoScaleOffset]_RutRTTex ("轨迹渲染纹理", 2D) = "bump" {}
        _PaintRect("轨迹范围", vector) = (0.0, 0.0, 1.0, 1.0)

        [Space(20)]
        [Header(Color)]
        [HDR]_BrightCol ("亮部色", color) = (1.0, 1.0, 1.0, 1.0)
        _DarkCol ("暗部色", color) = (0.1, 0.1, 0.1, 1.0)
        [HDR]_SpecularCol ("高光色", color) = (1.0, 1.0, 1.0, 1.0)
        _AmbCol ("环境色", color) = (1.0, 1.0, 1.0, 1.0)

        [Space(20)]
        [Header(Material)]
        _NormalInt ("法线强度", range(0, 10)) = 1.0
        _Rough ("粗糙度", range(0.001, 1)) = 0.5
        _FresnelPow ("菲涅尔次幂", range(1, 10)) = 5.0
        _F0 ("基础反射率", Range(0, 1)) = 0.05

        [Space(20)]
        [Header(Flash)]
        _FlashInt ("闪烁强度", float) = 10
        _FlashOffset ("闪烁偏移", float) = -0.1
        _FlashRange_Min ("闪烁最小衰减半径", float) = 5.0
        _FlashRange_Max ("闪烁最大衰减半径", float) = 10.0

        [Space(20)]
        [Header(Tessellation)]
        _TessStep ("最大细分段数", range(1, 64)) = 1
        _TessPow ("细分曲线", range(1, 10)) = 2.0

        [Space(20)]
        [Header(Rut)]
        _RutHeight ("轨迹高度", float) = 0.5
        _RutNormalInt ("轨迹法线强度", float) = 1.0
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
            Cull back
            ZWrite on

            HLSLPROGRAM

            #pragma vertex vert
            #pragma hull hs
            #pragma domain ds
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
			CBUFFER_START(UnityPerMaterial)
                //贴图
                sampler2D _MainTex; float4 _MainTex_ST;
                sampler2D _NormalMap;
                sampler2D _FlashTex; float4 _FlashTex_ST;
                sampler2D _RutRTTex;
                float4 _PaintRect;
                
                //颜色
                float3 _BrightCol;
                float3 _DarkCol;
                float3 _SpecularCol;
                float3 _AmbCol;
            
                //质感
                float _NormalInt;
                float _Rough;
                float _FresnelPow;
                float _F0;

                //闪烁
                float _FlashInt;
                float _FlashOffset;
                float _FlashRange_Min;
                float _FlashRange_Max;

                //细分参数
                uint _TessStep;
                float _TessPow;

                //痕迹
                float _RutHeight;
                float _RutNormalInt;
            CBUFFER_END
            
            //重映射01
            float Remap(float min, float max, float input)
            {
                float k = 1.0 / (max - min);
                float b = -min * k;
                return saturate(k * input + b);
            }

            //三角是否存在顶点包含在矩形内
            bool IsTriRectCross2D(float2 triVert[3], float4 rect)
            {
                for (uint idx = 0; idx < 3; idx++)
                {
                    if (triVert[idx].x >= rect.x && triVert[idx].x <= rect.z && triVert[idx].y >= rect.y && triVert[idx].y <= rect.w)
                    {
                        return true;
                    }
                }
                return false;
            }

            //计算任意平面交点
            float3 GetPosAnyPlaneCrossDir(float3 posPlane, float3 posRay, float3 nDirPlane, float3 nDirRay)
            {
                float3 deltaPos = posPlane - posRay;
                float temp = dot(nDirPlane, deltaPos) / dot(nDirPlane, nDirRay);
                return temp * nDirRay + posRay;
            }

			//顶点shader
            struct a2v
            {
                float4 posOS	: POSITION;
                float3 nDirOS : NORMAL;
                float4 tDirOS : TANGENT;
                float2 uv0  : TEXCOORD0;
            };
            struct v2t
            {
                float3 posWS	: TEXCOORD0;
                float3 nDirWS : TEXCOORD1;
                float3 tDirWS : TEXCOORD2;
                float3 bDirWS    : TEXCOORD3;
                float2 uv0  : TEXCOORD4;
                float2 uv_Rut : TEXCOORD5;
            };
            v2t vert(a2v i)
            {
                v2t o;

                //坐标
                o.posWS = TransformObjectToWorld(i.posOS.xyz);

                //向量
                o.nDirWS = TransformObjectToWorldNormal(i.nDirOS);
                o.tDirWS = TransformObjectToWorldDir(i.tDirOS.xyz);
                o.bDirWS = cross(o.nDirWS, o.tDirWS) * i.tDirOS.w;

                //UV
                o.uv0 = i.uv0;
                o.uv_Rut = float2(Remap(_PaintRect.x, _PaintRect.z, o.posWS.x), Remap(_PaintRect.y, _PaintRect.w, o.posWS.z));

                return o;
            }

			//对三角面或其他形式图元进行细分的配置
            struct TessParam
            {
                float EdgeTess[3]	: SV_TessFactor;//各边细分数
                float InsideTess	    : SV_InsideTessFactor;//内部点细分数
            };
            TessParam ConstantHS(InputPatch<v2t, 3> i, uint id : SV_PrimitiveID)
            {
                TessParam o;

                 //判断当前三角面与轨迹的矩形范围是否存在交集
                 float2 triVert[3] = { i[0].posWS.xz, i[1].posWS.xz, i[2].posWS.xz };
                 if (IsTriRectCross2D(triVert, _PaintRect))
                 {
                     //计算边与图元的中心
                     float2 edgeUV_Rut[3] = { 
                         0.5 * (i[1].uv_Rut + i[2].uv_Rut),
                         0.5 * (i[2].uv_Rut + i[0].uv_Rut),
                         0.5 * (i[0].uv_Rut + i[1].uv_Rut)
                     };
                     float2 centerUV_Rut = (i[0].uv_Rut + i[1].uv_Rut + i[2].uv_Rut) / 3.0;
                
                     //基于UV距离进行细分段数判断
                     for (uint idx = 0; idx < 3; idx++)
                     {
                         float lerpT = 2.0 * length(edgeUV_Rut[idx] - float2(0.5, 0.5));
                         lerpT = pow(saturate(lerpT), _TessPow);
                         o.EdgeTess[idx] = lerp(_TessStep, 1.0, lerpT);
                     }
                     float lerpT = 2.0 * length(centerUV_Rut - float2(0.5, 0.5));
                     lerpT = pow(saturate(lerpT), _TessPow);
                     o.InsideTess = lerp(_TessStep, 1.0, lerpT);
                 }
                 else
                 {
                     o.EdgeTess[0] = 1;
                     o.EdgeTess[1] = 1;
                     o.EdgeTess[2] = 1;
                     o.InsideTess = 1;
                 }

                return o;
            }
			
			//将原模型顶点属性按指定图元打包？
            struct TessOut
			{
				float3 posWS	: TEXCOORD0;
                float3 nDirWS : TEXCOORD1;
                float3 tDirWS : TEXCOORD2;
                float3 bDirWS    : TEXCOORD3;
                float2 uv0  : TEXCOORD4;
                float2 uv_Rut : TEXCOORD5;
			};
            [domain("tri")]//图元类型
            [partitioning("integer")]//曲面细分的过渡方式是整数还是小数
            [outputtopology("triangle_cw")]//三角面正方向是顺时针还是逆时针
            [outputcontrolpoints(3)]//输出的控制点数
            [patchconstantfunc("ConstantHS")]//对应之前的细分因子配置阶段的方法名
            [maxtessfactor(64.0)]//最大可能的细分段数
            TessOut hs(InputPatch<v2t, 3> i, uint idx : SV_OutputControlPointID)//在此处进行的操作是对原模型的操作，而非细分后
            {
				TessOut o;
				o.posWS = i[idx].posWS;
                o.nDirWS = i[idx].nDirWS;
                o.tDirWS = i[idx].tDirWS;
                o.bDirWS = i[idx].bDirWS;
                o.uv0 = i[idx].uv0;
                o.uv_Rut = i[idx].uv_Rut;
                return o;
            }
			
			//基于bary在上述打包的图元中进行插值权重混合(经过上面的细分处理形成了一组新的顶点属性组，这一阶段相当于处理这些新顶点的顶点shader)
            static float minError = 1.5 / 255;
            struct t2f
            {
                float4 posCS	       : SV_POSITION;
                float3 posWS            : TEXCOORD7;
                float3 nDirWS       : TEXCOORD0;
                float3 tDirWS       : TEXCOORD1;
                float3 bDirWS       : TEXCOORD2;
                float3 vDirWS       : TEXCOORD3;
                float2 uv_Main     : TEXCOORD4;
                float4 uv_Flash    : TEXCOORD5;
                float2 uv_Rut       : TEXCOORD6;
            };
            [domain("tri")]
            t2f ds(TessParam tessParam, float3 bary : SV_DomainLocation, const OutputPatch<TessOut, 3> i)
            {
                t2f o;   

                //线性转换
                o.posWS = i[0].posWS * bary.x + i[1].posWS * bary.y + i[2].posWS * bary.z;
                o.nDirWS = i[0].nDirWS * bary.x + i[1].nDirWS * bary.y + i[2].nDirWS * bary.z;
                o.tDirWS = i[0].tDirWS * bary.x + i[1].tDirWS * bary.y + i[2].tDirWS * bary.z;
                o.bDirWS = i[0].bDirWS * bary.x + i[1].bDirWS * bary.y + i[2].bDirWS * bary.z;
                float2 uv0 = i[0].uv0 * bary.x + i[1].uv0 * bary.y + i[2].uv0 * bary.z;
                o.uv_Rut = i[0].uv_Rut * bary.x + i[1].uv_Rut * bary.y + i[2].uv_Rut * bary.z;

                //痕迹变形
                float height = tex2Dlod(_RutRTTex, float4(o.uv_Rut, 0, 0)).a;
                height = abs(height - 0.5) < minError ? 0.5 : height;//误差截断，防止在平坦区域产生变形
                o.posWS += _RutHeight * (2.0 * height - 1.0) * o.nDirWS;

                //坐标
                o.posCS = TransformWorldToHClip(o.posWS);

                //向量
                o.vDirWS = GetCameraPositionWS() - o.posWS;

                //UV
                o.uv_Main = TRANSFORM_TEX(uv0, _MainTex);
                o.uv_Flash.xy = TRANSFORM_TEX(uv0, _FlashTex);

                //闪烁内层UV偏移
                float3x3 TBN = float3x3(normalize(o.tDirWS), normalize(o.bDirWS), normalize(o.nDirWS));
                float3 vDirTS = TransformWorldToTangent(o.vDirWS, TBN);
                o.uv_Flash.zw = GetPosAnyPlaneCrossDir(float3(0, 0, _FlashOffset), float3(o.uv_Flash.xy, 0), float3(0,0,1), vDirTS).xy;

                return o;
            }

			//像素shader
            float4 frag(t2f i) : SV_Target
            {
                //轨迹图采样
                float4 var_RutTex = tex2D(_RutRTTex, i.uv_Rut);
                var_RutTex.xyw = abs(var_RutTex.xyw - 0.5) < minError ? 0.5 : var_RutTex.xyw;//误差截断，防止在平坦区域应用法线贴图

                //地形法线
                float3 nDirTS = UnpackNormal(tex2D(_NormalMap, i.uv_Main));
                nDirTS.xy *= _NormalInt;

                //轨迹法线
                float3 nDirTS_Rut = (2.0 * var_RutTex.xyz - 1.0);
                nDirTS_Rut.xy *= _RutHeight * _RutNormalInt;

                //法线混合
                nDirTS = normalize(float3(nDirTS.xy / nDirTS.z + nDirTS_Rut.xy / nDirTS_Rut.z, 1.0));
                float3x3 TBN = float3x3(normalize(i.tDirWS), normalize(i.bDirWS), normalize(i.nDirWS));

                //todo:可以用更好的法线混合方案：https://www.gameres.com/896279.html

                //向量
                Light light = GetMainLight(TransformWorldToShadowCoord(i.posWS));
                float3 nDirWS = normalize(mul(nDirTS, TBN));
                float3 lDirWS = light.direction;
                float3 vDirWS = normalize(i.vDirWS);
                float3 hDirWS = normalize(lDirWS + vDirWS);
                
                //光照
                float lambert = saturate(dot(nDirWS, lDirWS));
                float blinn = lambert * pow(saturate(dot(nDirWS, hDirWS)), 1.0 / (_Rough*_Rough));
                float3 baseCol = tex2D(_MainTex, i.uv_Main).rgb;
                float3 diffuseCol = baseCol * lerp(_DarkCol, _BrightCol, lambert);
                float3 specularCol = _SpecularCol * blinn;

                //环境光
                //float nv = saturate(dot(nDirWS, vDirWS));
                //float fresnel = _F0 + (1.0-_F0) * pow(1.0 - nv, _FresnelPow);
                float fresnel = 0;
                float3 ambCol = fresnel * _AmbCol * baseCol;

                //闪烁
                float flashMask = Remap(_FlashRange_Max, _FlashRange_Min, length(i.vDirWS));
                float mask0 = tex2D(_FlashTex, i.uv_Flash.xy).r;
                float mask1 = tex2D(_FlashTex, i.uv_Flash.zw).r;
                float flashCol = _FlashInt * flashMask * mask0 * mask1;

                //混合
                float3 finalCol = (diffuseCol + specularCol + flashCol) * light.shadowAttenuation + ambCol;
                return float4(finalCol, 1.0);
            }            
            ENDHLSL
        }
    }
}