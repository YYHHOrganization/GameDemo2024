#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float4 tangentOS    : TANGENT;
    float4 color        : COLOR;
    float2 uv           : TEXCOORD0;
    float2 backUV       : TEXCOORD1;
};

struct Varyings
{
    float2 uv           : TEXCOORD0;
    float2 backUV       : TEXCOORD1;
    float3 positionWS   : TEXCOORD2;
    half3 tangentWS     : TEXCOORD3;
    half3 bitangentWS   : TEXCOORD4;
    half3 normalWS      : TEXCOORD5;
    float4 positionNDC  : TEXCOORD6;
    half4 color         : COLOR;
    float4 positionCS   : SV_POSITION;
};

half GetShadow(Varyings input, half3 lightDirection, half aoFactor)
{
    half NDotL = dot(input.normalWS, lightDirection);
    half halfLambert = 0.5 * NDotL + 0.5;
    half shadow = saturate(2.0 * halfLambert * aoFactor);
    return lerp(shadow, 1.0, step(0.9, aoFactor));
}

half GetFaceShadow(Varyings input, half3 lightDirection)
{
    half3 F = SafeNormalize(half3(_FaceDirection.x, 0.0, _FaceDirection.z));
    half3 L = SafeNormalize(half3(lightDirection.x, 0.0, lightDirection.z));
    half FDotL = dot(F, L);
    half FCrossL = cross(F, L).y;
    
    half2 shadowUV = input.uv;
    shadowUV.x = lerp(shadowUV.x, 1.0 - shadowUV.x, step(0.0, FCrossL));
    half faceShadowMap = SAMPLE_TEXTURE2D(_FaceLightMap, sampler_FaceLightMap, shadowUV).r;
    half faceShadow = step(-0.5 * FDotL + 0.5 + _FaceShadowOffset, faceShadowMap);

    half faceMask = SAMPLE_TEXTURE2D(_FaceShadow, sampler_FaceShadow, input.uv).a;
    half maskedFaceShadow = lerp(faceShadow, 1.0, faceMask);

    return maskedFaceShadow;
}

half3 GetShadowColor(half shadow, half material, half day)
{
    int index = 4;
    index = lerp(index, 1, step(0.2, material));
    index = lerp(index, 2, step(0.4, material));
    index = lerp(index, 0, step(0.6, material));
    index = lerp(index, 3, step(0.8, material));

    half rangeMin = 0.5 + _ShadowOffset - _ShadowSmoothness;
    half rangeMax = 0.5 + _ShadowOffset;
    half2 rampUV = half2(smoothstep(rangeMin, rangeMax, shadow), index / 10.0 + 0.5 * day + 0.05);
    half3 shadowRamp = SAMPLE_TEXTURE2D(_ShadowRamp, sampler_ShadowRamp, rampUV);

    half3 shadowColor = shadowRamp * lerp(_ShadowColor, 1.0, smoothstep(0.9, 1.0, rampUV.x));
    shadowColor = lerp(shadowColor, 1.0, step(rangeMax, shadow));

    return shadowColor;
}

half3 GetSpecular(Varyings input, half3 lightDirection, half3 albedo, half3 lightMap)
{
    half3 V = GetWorldSpaceNormalizeViewDir(input.positionWS);
    half3 H = SafeNormalize(lightDirection + V);
    half NDotH = dot(input.normalWS, H);
    half blinnPhong = pow(saturate(NDotH), _SpecularSmoothness);

    half3 normalVS = TransformWorldToViewNormal(input.normalWS, true);
    half2 matcapUV = 0.5 * normalVS.xy + 0.5;
    half3 metalMap = SAMPLE_TEXTURE2D(_MetalMap, sampler_MetalMap, matcapUV);

    half3 nonMetallic = step(1.1, lightMap.b + blinnPhong) * lightMap.r * _NonmetallicIntensity;
    half3 metallic = blinnPhong * lightMap.b * albedo * metalMap * _MetallicIntensity;
    half3 specular = lerp(nonMetallic, metallic, step(0.9, lightMap.r));

    return specular;
}

half GetRim(Varyings input)
{
    half3 normalVS = TransformWorldToViewNormal(input.normalWS, true);
    float2 uv = input.positionNDC.xy / input.positionNDC.w;
    float2 offset = float2(_RimOffset * normalVS.x / _ScreenParams.x, 0.0);

    float depth = LinearEyeDepth(SampleSceneDepth(uv), _ZBufferParams);
    float offsetDepth = LinearEyeDepth(SampleSceneDepth(uv + offset), _ZBufferParams);
    half rim = smoothstep(0.0, _RimThreshold, offsetDepth - depth) * _RimIntensity;

    half3 V = GetWorldSpaceNormalizeViewDir(input.positionWS);
    half NDotV = dot(input.normalWS, V);
    half fresnel = pow(saturate(1.0 - NDotV), 5.0);

    return rim * fresnel;
}

half GetRimNew(Varyings input)
{
    //half3 normalVS = TransformWorldToViewNormal(input.normalWS, true);
    half3 normalVS = TransformWorldToHClipDir(input.normalWS, true);  //做了约定，这里的normalVS是在HClip空间下的，这样空间比较统一？
    float2 uv = input.positionNDC.xy / input.positionNDC.w; //转换为非齐次坐标,除以w，因为NDC的xy的范围是[0,w]，详见GetVertexPositionInputs函数
    
    //要计算用于采样深度缓冲区的 UV 坐标，请将像素位置除以渲染目标分辨率 _ScaledScreenParams。_ScaledScreenParams.xy 属性会考虑渲染目标的任何缩放，例如动态分辨率。
    //这一步参考URP官方文档:https://docs.unity3d.com/cn/Packages/com.unity.render-pipelines.universal@12.1/manual/writing-shaders-urp-reconstruct-world-position.html
    float2 offset = float2(_RimOffset * normalVS.x / _ScreenParams.x, 0.0); //只沿着X方向有Rim效果
    float depth = LinearEyeDepth(SampleSceneDepth(uv), _ZBufferParams);
    float offsetDepth = LinearEyeDepth(SampleSceneDepth(uv + offset), _ZBufferParams);
    half rim = smoothstep(0.0, _RimThreshold, offsetDepth - depth) * _RimIntensity;

    half3 V = GetWorldSpaceNormalizeViewDir(input.positionWS);
    half NDotV = dot(input.normalWS, V);
    half fresnel = pow(saturate(1.0 - NDotV), 5.0); //边缘Fresnel项的强度
    return rim * fresnel;
}

Varyings ForwardPassVertex(Attributes input)
{
    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

    Varyings output = (Varyings)0;
    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
    output.backUV = TRANSFORM_TEX(input.backUV, _BaseMap);
    output.positionWS = vertexInput.positionWS;
    output.tangentWS = normalInput.tangentWS;
    output.bitangentWS = normalInput.bitangentWS;
    output.normalWS = normalInput.normalWS;
    output.color = input.color;
    output.positionNDC = vertexInput.positionNDC;
    output.positionCS = vertexInput.positionCS;

    output.positionCS.xy += _ScreenOffset.xy * output.positionCS.w;

    return output;
}

half4 ForwardPassFragment(Varyings input, FRONT_FACE_TYPE facing : FRONT_FACE_SEMANTIC) : SV_TARGET
{
#if _DOUBLE_SIDED
    input.uv = lerp(input.uv, input.backUV, IS_FRONT_VFACE(facing, 0.0, 1.0));
#endif

    half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
    half3 albedo = baseMap.rgb * _BaseColor.rgb;
    half alpha = baseMap.a;

#if _IS_FACE
    albedo = lerp(albedo, _FaceBlushColor.rgb, _FaceBlushStrength * alpha);
#endif

#if _NORMAL_MAP
    half3x3 tangentToWorld = half3x3(input.tangentWS, input.bitangentWS, input.normalWS);
    half4 normalMap = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv);
    half3 normalTS = UnpackNormal(normalMap);
    half3 normalWS = TransformTangentToWorld(normalTS, tangentToWorld, true);
    input.normalWS = normalWS;
#endif

    Light mainLight = GetMainLight();
    half3 lightDirection = SafeNormalize(mainLight.direction * _LightDirectionMultiplier);

    half4 lightMap = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, input.uv);
    half material = lerp(lightMap.a, _CustomMaterialType, _UseCustomMaterialType);
#if _IS_FACE
        half shadow = GetFaceShadow(input, lightDirection);
#else
        half aoFactor = lightMap.g * input.color.r;
        half shadow = GetShadow(input, lightDirection, aoFactor);
#endif
    //test shadow
    //return half4(shadow, shadow, shadow, 1.0);
    half3 shadowColor = GetShadowColor(shadow, material, _IsDay);
    //return float4(shadowColor, 1.0);

    half3 specular = 0.0;
#if _SPECULAR
    specular = GetSpecular(input, lightDirection, albedo, lightMap.rgb);
    //return half4(specular, 1.0);
#endif

    half3 emission = 0.0;
#if _EMISSION
    emission = albedo * _EmissionIntensity * alpha; //可以再乘一项* abs((frac(_Time.y * 0.5) - 0.5) * 2)
#endif

    half3 rim = 0.0;
#if _RIM
    rim = albedo * GetRimNew(input);
    //return half4(rim, 1.0);
#endif

    half3 finalColor = albedo * shadowColor + specular + rim + emission;
    half finalAlpha = 1.0;

    return half4(finalColor, finalAlpha);
}
