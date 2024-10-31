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

float GetShadow(Varyings input, half3 lightDirection, half aoFactor)
{
    float NDotL = dot(input.normalWS, lightDirection); //[-1,1]
    float halfLambert = 0.5 * NDotL + 0.5; //[0,1]
    float shadow = saturate(2.0 * aoFactor * halfLambert); //halfLambert:[0,1]
    return shadow; 
}   

half GetFaceShadow(Varyings input, half3 lightDirection)
{
    _FaceDirection = half4(TransformObjectToWorldDir(float3(0, 0, 1)), 1.0);
    half3 F = SafeNormalize(half3(_FaceDirection.x, 0.0, _FaceDirection.z));
    half3 L = SafeNormalize(half3(lightDirection.x, 0.0, lightDirection.z));
    half FDotL = dot(F, L); //这一项<0说明光在角色后面，>0说明光在角色前面
    half FCrossL = cross(F, L).y; //>0说明在光在角色右边，<0说明光在角色左边
    
    half2 shadowUV = input.uv;
    shadowUV.x = lerp(shadowUV.x, 1.0 - shadowUV.x, step(0.0, FCrossL));
    half faceShadowMap = SAMPLE_TEXTURE2D(_FaceLightMap, sampler_FaceLightMap, shadowUV).r; //faceLightmnap就是那张SDF脸部阴影阈值图
    half faceShadow = step(-0.5 * FDotL + 0.5 + _FaceShadowOffset, faceShadowMap);
    
    half faceMask = SAMPLE_TEXTURE2D(_FaceShadow, sampler_FaceShadow, input.uv).a; //FaceShadow是一张脸部阴影遮罩图
    half maskedFaceShadow = lerp(faceShadow, 1.0, faceMask); //faceMask是1的地方，对应比如眼睛的位置，就是1.0，常亮
    return maskedFaceShadow;
}

half3 GetShadowColor(half shadow, half material, half day) //正常来说，material传入的是lightmap.a的值，决定漫反射是什么材质类型
{
    int index = 4; //index表示是那一条条带（从下往上数）
    /*- 灰度1.0 ： 皮肤类质感
    - 灰度0.7： 丝绸/丝袜
    - 灰度0.5 ： 金属/金属投影
    - 灰度0.3 ： 软的物体
    - 灰度0.0 ：硬的物体
    */
    index = lerp(index, 1, step(0.2, material)); //如果>0.2,index=1，否则为4
    index = lerp(index, 2, step(0.4, material)); //如果>0.4，index=2，否则为1
    index = lerp(index, 0, step(0.6, material)); //如果>0.6，index=0，否则为2
    index = lerp(index, 3, step(0.8, material)); //如果>0.8，index=3，否则为0
    //index：[0,0.2]:硬的物体，条带4,  (0.2,0.4]:软的物体，条带1, (0.4,0.6]:金属，条带2, (0.6,0.8]:丝绸，条带0, >0.8:皮肤类质感，条带3
    half rangeMin = 0.5 + _ShadowOffset - _ShadowSmoothness;
    half rangeMax = 0.5 + _ShadowOffset;
    //这些灰度采样的条带不同，具体如下：
    half2 rampUV = half2(smoothstep(rangeMin, rangeMax, shadow), index / 10.0 + 0.5 * day + 0.03); //index/10.0，会取到0，0.1，0.2，0.3，0.4
    half3 shadowRamp = SAMPLE_TEXTURE2D(_ShadowRamp, sampler_ShadowRamp, rampUV);
    half3 shadowColor = shadowRamp * lerp(_ShadowColor, 1.0, smoothstep(0.9, 1.0, rampUV.x)); //rampUV.x会在0.9和1.0之间产生一个从shadowRamp*_ShadowColor到ShadowRamp的过渡
    shadowColor = lerp(shadowColor, 1.0, step(rangeMax, shadow)); //超过rangeMax就是1，否则就是shadowColor

    return shadowColor;
}

half3 GetSpecular(Varyings input, half3 lightDirection, half3 albedo, half3 lightMap)
{
    half3 V = GetWorldSpaceNormalizeViewDir(input.positionWS);
    half3 H = SafeNormalize(lightDirection + V);
    half NDotH = dot(input.normalWS, H);
    half blinnPhong = pow(saturate(NDotH), _SpecularSmoothness); //这里用_SpecularSmoothness来充当高光系数

    half3 normalVS = TransformWorldToViewNormal(input.normalWS, true);
    half2 matcapUV = 0.5 * normalVS.xy + 0.5;
    half3 metalMap = SAMPLE_TEXTURE2D(_MetalMap, sampler_MetalMap, matcapUV);

    half3 nonMetallic = step(1.1, lightMap.b + blinnPhong) * lightMap.r * _NonmetallicIntensity;  //非金属用BlinnPhong来做，lightmap.b叠加到高光项之上，能体现出不同非金属之间的高光差异
    half3 metallic = blinnPhong * lightMap.b * albedo * metalMap * _MetallicIntensity;  //金属用Matcap来做
    half3 specular = lerp(nonMetallic, metallic, step(0.9, lightMap.r)); //直接限制死了，lightMap.r>0.9就是金属（用Matcap），否则就是非金属（用BlinnPhong）

    return specular;
}

half GetRim(Varyings input)
{
    half3 normalVS = TransformWorldToViewNormal(input.normalWS, true);
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

    //output.positionCS.xy += _ScreenOffset.xy * output.positionCS.w;  //todo：这个应该是说后面Unity会自动/w，不过存疑，先放着
    return output;
}

half4 ForwardPassFragment(Varyings input, FRONT_FACE_TYPE facing : FRONT_FACE_SEMANTIC) : SV_TARGET  //https://zhuanlan.zhihu.com/p/573996843,Unity提供了是否为背面的判断
{
    #if _DOUBLE_SIDED
    //是正面，用正面的UV，是背面则用背面的UV
     input.uv = lerp(input.uv, input.backUV, IS_FRONT_VFACE(facing, 0.0, 1.0)); // https://blog.csdn.net/wodownload2/article/details/99673897 
    #endif

    half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
    half3 albedo = baseMap.rgb * _BaseColor.rgb;
    half alpha = baseMap.a;  //脸部的baseMap.a控制脸红，所以用下面的lerp逻辑和_FaceBlushStrength参数可以控制角色是否脸红；
    //如果是身体的baseColor.a，可以控制如神之眼部分的自发光现象

    #if _IS_FACE
    albedo = lerp(albedo, _FaceBlushColor.rgb, _FaceBlushStrength * alpha);  
    #endif

    #if _NORMAL_MAP  //比较新的原神角色引入了法线贴图，这里也考虑一下法线
    half3x3 tangentToWorld = half3x3(input.tangentWS, input.bitangentWS, input.normalWS);
    half4 normalMap = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv);
    half3 normalTS = UnpackNormal(normalMap);
    half3 normalWS = TransformTangentToWorld(normalTS, tangentToWorld, true); //切线空间转到世界空间
    input.normalWS = normalWS; //更新使用了法线贴图之后的法线
    #endif

    Light mainLight = GetMainLight();
    //half3 lightDirection = SafeNormalize(mainLight.direction * _LightDirectionMultiplier); //_LightDirectionMultiplier可以用来调整光照方向
    half3 lightDirection = normalize(mainLight.direction);
    
    half4 lightMap = SAMPLE_TEXTURE2D(_LightMap, sampler_LightMap, input.uv);
    //half material = lerp(lightMap.a, _CustomMaterialType, _UseCustomMaterialType); //如果使用自定义，则为1，否则使用Lightmap.a通道的值，来决定材质类型（漫反射）
    half material = lightMap.a;
    
    
    #if _IS_FACE
    half shadow = GetFaceShadow(input, lightDirection); //脸部阴影的值，还没有乘颜色
    #else
        //half aoFactor = lightMap.g * input.color.r; //input.color.r 脖子是黑色的，也就是说脖子会一直处于阴影中，其他地方白色
        half aoFactor = lightMap.g;
        half shadow = GetShadow(input, lightDirection, aoFactor);
    #endif
        //test shadow
        //return float4(input.normalWS, 1.0); 
        //return float4(shadow,shadow,shadow,1.0);

        half3 shadowColor = GetShadowColor(shadow, material, _IsDay);
    //return half4(shadowColor, 1.0);
    
    half3 specular = 0.0;
    #if _SPECULAR
    specular = GetSpecular(input, lightDirection, albedo, lightMap.rgb);
    #endif

    half3 emission = 0.0;
    #if _EMISSION
    emission = albedo * _EmissionIntensity * alpha * abs((frac(_Time.y * 0.5) - 0.5) * 2);
    #endif

    half3 rim = 0.0;
    #if _RIM
    rim = albedo * GetRim(input);
    #endif
    //return float4(shadowColor, 1.0);
    half3 finalColor = albedo * shadowColor + specular + rim + emission;
    half finalAlpha = 1.0;

    return half4(finalColor, finalAlpha);
    
}

