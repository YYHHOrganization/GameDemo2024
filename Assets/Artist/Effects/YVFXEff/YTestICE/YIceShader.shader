Shader "Custom/YIceShader"
{
    Properties
    {
        [Header(Texture)]
        _FaceTex ("表层贴图", 2D) = "black" {}
        _InnerTex ("里层贴图", 2D) = "white" {}
        _NormalMap ("法线图", 2D) = "bump" {}
        _SurfaceAttMap ("冰面属性图", 2D) = "white" {}

        [Space(20)]
        [Header(Color)]
        [HDR]_FaceCol ("表层色调", Color) = (1.0, 1.0, 1.0, 1.0)
        [HDR]_InnerCol ("里层色调", color) = (1.0, 1.0, 1.0, 1.0)
        [HDR] _SpecularCol ("高光色", color) = (1.0, 1.0, 1.0, 1.0)
        _AmbCol ("环境色", color) = (1.0, 1.0, 1.0, 1.0)

        [Space(20)]
        [Header(Material)]
        _NormalInt ("法线强度", range(0, 10)) = 1.0
        _Rough ("粗糙度", range(0.001, 1)) = 0.1
        _FresnelPow ("菲涅尔次幂", range(1, 10)) = 5.0
        _Terrace ("阶梯化次数", range(1, 5)) = 3

        [Space(20)]
        [Header(Refract)]
        _RefractRatio ("折射率", range(0, 1)) = 0.5
        _Thick ("层厚度", range(0, 5)) = 0.1
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Geometry"
        }

        Pass
        {
            Cull back
            ZWrite on

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                //纹理
                sampler2D _FaceTex; float4 _FaceTex_ST;
                sampler2D _InnerTex; float4 _InnerTex_ST;
                sampler2D _NormalMap; float4 _NormalMap_ST;
                sampler2D _SurfaceAttMap; float4 _SurfaceAttMap_ST;

                //颜色
                half4 _FaceCol;
                half3 _InnerCol;
                half3 _SpecularCol;
                half3 _AmbCol;

                //材质
                half _NormalInt;
                half _Rough;
                half _FresnelPow;
                uint _Terrace;

                //折射
                half _RefractRatio;
                half _Thick;
            CBUFFER_END

            //计算任意平面交点
            float3 GetPosAnyPlaneCrossDir(float3 posPlane, float3 posRay, float3 nDirPlane, float3 nDirRay)
            {
                float3 deltaPos = posPlane - posRay;
                float temp = dot(nDirPlane, deltaPos) / dot(nDirPlane, nDirRay);
                return temp * nDirRay + posRay;
            }

            struct a2v
            {
                half4 posOS    : POSITION;
                float2 uv0   : TEXCOORD0;
                half3 nDirOS  : NORMAL;
                half4 tDirOS    : TANGENT;
            };

            struct v2f
            {
                half4 posCS    : SV_POSITION;
                float2 uv_Front     : TEXCOORD0;
                float2 uv_Normal    : TEXCOORD1;
                float2 uv_Back      : TEXCOORD6;
                half3 nDirWS        : TEXCOORD2;
                half3 tDirWS        : TEXCOORD3;
                half3 bDirWS        : TEXCOORD4;
                half3 vDirWS        : TEXCOORD5;
            };

            v2f vert (a2v i)
            {
                v2f o;

                //坐标
                o.posCS = TransformObjectToHClip(i.posOS.xyz);

                //向量
                o.nDirWS = TransformObjectToWorldNormal(i.nDirOS);
                o.tDirWS = TransformObjectToWorldDir(i.tDirOS.xyz);
                o.bDirWS = cross(o.nDirWS, o.tDirWS) * i.tDirOS.w;
                o.vDirWS = GetCameraPositionWS() - TransformObjectToWorld(i.posOS);

                //uv
                o.uv_Front = TRANSFORM_TEX(i.uv0, _FaceTex);
                o.uv_Normal = TRANSFORM_TEX(i.uv0, _NormalMap);

                //里层UV
                half3x3 TBN = half3x3(normalize(o.tDirWS), normalize(o.bDirWS), normalize(o.nDirWS));
                half3 vDirTS = TransformWorldToTangent(o.vDirWS, TBN);
                o.uv_Back = GetPosAnyPlaneCrossDir(half3(0, 0, -_Thick), half3(i.uv0, 0), half3(0,0,1), -vDirTS);
                o.uv_Back = _InnerTex_ST.xy * o.uv_Back + _InnerTex_ST.zw;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                //法线转换
                half3 nDirTS = UnpackNormal(tex2D(_NormalMap, i.uv_Normal));
                float2 warpUV = nDirTS.xy;
                nDirTS.xy *= _NormalInt;
                half3x3 TBN = half3x3(normalize(i.tDirWS), normalize(i.bDirWS), normalize(i.nDirWS));

                //向量准备
                Light light = GetMainLight();
                half3 nDirWS = normalize(mul(nDirTS, TBN));
                half3 lDirWS = light.direction;
                half3 vDirWS = normalize(i.vDirWS);
                half3 hDirWS = normalize(lDirWS + vDirWS);

                //光照
                half lambert = saturate(dot(nDirWS, lDirWS));
                half blinn = lambert * pow(saturate(dot(nDirWS, hDirWS)), 1.0 / (_Rough*_Rough));
                //blinn = floor((_Terrace + 1) * blinn) / (_Terrace + 1);//色阶阶梯化
                half3 specularCol = blinn * _SpecularCol;

                //折射
                float2 uv_Back = i.uv_Back + _RefractRatio * warpUV;
                half3 refractCol = tex2D(_InnerTex, uv_Back).rgb * _InnerCol;

                //取出表层属性图的g作为a通道
                half4 surfaceAtt = tex2D(_SurfaceAttMap, i.uv_Front);
                half4 surfaceAtt2 = tex2D(_SurfaceAttMap, i.uv_Front);
                half faceOpacity1 = surfaceAtt.g;
                
                //表层采样
                half4 var_FaceTex = tex2D(_FaceTex, i.uv_Front);
                half3 faceCol = var_FaceTex.rgb * _FaceCol.rgb;
                half faceOpacity = var_FaceTex.a * _FaceCol.a * faceOpacity1;

                //环境光
                //half fresnel = pow(1.0 - saturate(dot(nDirWS, vDirWS)), _FresnelPow);
                //half3 ambCol = fresnel * _AmbCol;

                //混合
                half3 finalCol = faceCol * faceOpacity + (1.0 - faceOpacity) * refractCol;
                // finalCol += specularCol + ambCol;

                half3 surfaceDetail = surfaceAtt2.b * 0.2;
                
                finalCol += specularCol;
                //把surfaceDetail加到表面上
                //finalCol = lerp(finalCol, surfaceDetail, surfaceDetail);

                //将surfaceAtt的b通道 提取出来，并且将其clip掉 有值的部分保留
                //clip(surfaceAtt2.b - 0.5);
                finalCol = finalCol+surfaceDetail;
                
                return half4(finalCol , 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
} 