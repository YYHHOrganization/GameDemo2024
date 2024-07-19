Shader "Custom/YOriIceShader"
{
    Properties
    {
        [Header(Texture)]
        _FaceTex ("表层贴图", 2D) = "black" {}
        _InnerTex ("里层贴图", 2D) = "white" {}
        _NormalMap ("法线图", 2D) = "bump" {}

        [Space(20)]
        [Header(Color)]
        _FaceCol ("表层色调", Color) = (1.0, 1.0, 1.0, 1.0)
        _InnerCol ("里层色调", color) = (1.0, 1.0, 1.0, 1.0)
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
            "RenderType" = "Opaque"
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
                float2 uv0          : TEXCOORD6;
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
                o.uv0 = i.uv0;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                //法线转换
                half3 nDirTS = UnpackNormal(tex2D(_NormalMap, i.uv_Normal));
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
                blinn = floor((_Terrace + 1) * blinn) / (_Terrace + 1);//色阶阶梯化
                half3 specularCol = blinn * _SpecularCol;

                //折射
                half3 refDirTS = TransformWorldToTangent(refract(-vDirWS, nDirWS, _RefractRatio), TBN);//视向量反方向经折射后的切空间方向
                float2 uv_Back = GetPosAnyPlaneCrossDir(half3(0, 0, -_Thick), half3(i.uv0, 0), half3(0,0,1), refDirTS);
                half3 refractCol = tex2D(_InnerTex, _InnerTex_ST.xy * uv_Back + _InnerTex_ST.zw).rgb * _InnerCol;

                //表层采样
                half4 var_FaceTex = tex2D(_FaceTex, i.uv_Front);
                half3 faceCol = var_FaceTex.rgb * _FaceCol.rgb;
                half faceOpacity = var_FaceTex.a * _FaceCol.a;

                //环境光
                half fresnel = pow(1.0 - saturate(dot(nDirWS, vDirWS)), _FresnelPow);
                half3 ambCol = fresnel * _AmbCol;

                //混合
                half3 finalCol = faceCol * faceOpacity + (1.0 - faceOpacity) * refractCol;
                finalCol += specularCol + ambCol;

                return half4(finalCol, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
} 