
#ifndef VOLUMETRIC_FOG_LITE_CGINC
#define VOLUMETRIC_FOG_LITE_CGINC

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

/// \brief //HenyeyGreenstein 函数用于计算 Henyey-Greenstein 相函数值，该函数在常用于模拟参与介质（如雾或烟雾）中的光散射。
/// \param cosTheta 光方向和视线方向之间角度的余弦值
/// \param g2 向异性因子的两倍
/// \param gPow2 各向异性因子的平方
/// \return 
float HenyeyGreenstein(float cosTheta, float g2, float gPow2)
{
    float denominator = (1.0 + gPow2) - (g2 * cosTheta);
    return (1.0 - gPow2) / (4.0 * PI * pow(denominator, 1.5));
}

float CalculateMainLightScattering(float cosTheta, float costThetaPow2, float g2, float gPow2)
{
    return HenyeyGreenstein(cosTheta, g2, gPow2);
}

// https://www.shadertoy.com/view/WsfBDf
// Shadertoy author comment: "this noise, including the 5.58... scrolling constant are from Jorge Jimenez".

// NOTE: Random.hlsl also has InterleavedGradientNoise() but it has some banding with high noise strength.

// 生成交错梯度噪声的函数
float ignoise(float2 position, int frame)
{
    // 对位置进行变换，加入帧数的影响
    position += (float(frame) * 5.588238f);
    // 计算并返回噪声值
    return frac(52.9829189f * frac(0.06711056f * float(position.x) + 0.00583715f * float(position.y)));
}

#define MAX_RAYMARCH_STEPS 128

void VolumetricFog_float(

        float4 colour,

        float3 position,
        float2 screenPosition,

        uint raymarchSteps,
        float raymarchNoise,

        float raymarchDistance,
        float raymarchDistanceBias,

        float3 raymarchDirection,

        float raymarchMaxDepth,

        float mainLightStrength,

        float mainLightAnisotropy,
        float mainLightAnisotropyBlend,

        float mainLightShadowStrength,

        out float4 output)
{
    
    //discard;
    // 如果在Shader Graph预览中，丢弃。
#ifdef SHADERGRAPH_PREVIEW
    
    discard;
    
#endif
    // 生成随机噪声
    float random = frac((sin(dot(screenPosition, float2(12.9898, 78.233))) * 43758.55) + _Time.y);
    float interleavedGradientNoise = ignoise(screenPosition, random * 9999.0);  

    //将 raymarchDistance 限制在 raymarchMaxDepth（shaderGraph中取Screen深度） 和 raymarchDistance 加上一个偏差值之间的最小值。
    raymarchDistance = min(raymarchDistance, raymarchMaxDepth + (raymarchDistance * raymarchDistanceBias));
    //lerp函数返回值在raymarchDistance和raymarchDistance * interleavedGradientNoise之间。raymarchNoise作为插值因子，值在0-1之间。
    raymarchDistance = lerp(raymarchDistance, raymarchDistance * interleavedGradientNoise, raymarchNoise);    

    //rayStep计算每一步光线行进的步长。raymarchSteps是光线行进的总步数。
    float rayStep = raymarchDistance / raymarchSteps;
    //raymarchDirection  shaderGraph中取目光方向。
    float3 rayDirectionStep = raymarchDirection * rayStep;
    
    float density = 0.0;    
    float depth = 0.0;
    
    float mainLightShading = 0.0;
        
    float raymarchStepsMinusOne = raymarchSteps - 1.0;
    
// #define _MAIN_LIGHT_ENABLED
#ifdef _MAIN_LIGHT_ENABLED

    Light mainLight = GetMainLight();
    
    // Scattering pre-calculations. // 散射预计算
    
    float g = mainLightAnisotropy;// 获取主光源的各向异性因子
    
    float g2 = g * 2.0;// 计算各向异性因子的两倍
    float gSquared = g * g;// 计算各向异性因子的平方
        
    float cosTheta = dot(mainLight.direction, raymarchDirection);// 计算主光源方向和光线行进方向的余弦值
    float cosThetaSquared = cosTheta * cosTheta;

#endif
    
    float3 raymarchStartPosition = position;
    
    // Raymarch.
    
    bool breakLoop;
    
    for (int i = 0; i < MAX_RAYMARCH_STEPS; i++)
    {
        // 如果当前步数超过了光线行进的总步数，跳出循环
        if (i >= raymarchSteps)
        {
            break;
        }

        // 计算当前进度，表示形式是0-1之间的浮点数，即比例
        float progress = i / raymarchStepsMinusOne;

        //如果是最后一次迭代，调整步长以确保光线行进到最大深度
        // If last iteration, go all the way to the back :) 
        if (i == raymarchSteps - 1)
        {
            rayStep = raymarchMaxDepth - depth;
            rayDirectionStep = raymarchDirection * rayStep;
        }
                
        // March position forward by ray step.
        // 按步长前进位置
        position += rayDirectionStep;
        depth += rayStep;
        
        // Main light shadows and scattering.
        // 主光源阴影和散射
        float mainLightShadow = 1.0;        
        float mainLightScattering = 1.0;
        
#define _MAIN_LIGHT_SHADOWS_ENABLED
        
#ifdef _MAIN_LIGHT_ENABLED 
#if defined(_MAIN_LIGHT_SHADOWS_ENABLED)
        // 计算主光源阴影 把模型的世界空间顶点坐标输入，得到阴影坐标，用于在shadowmap下进行比较。 
        float4 shadowCoord = TransformWorldToShadowCoord(position);
        // 计算带阴影衰减的主光源。https://www.bilibili.com/read/cv6436088/ 
        mainLightShadow = MainLightRealtimeShadow(shadowCoord);//返回在指定坐标处从主阴影图中获取的阴影值
        //mainLightShadow = MainLightShadow(shadowCoord, position, half4(1.0, 1.0, 1.0, 1.0), _MainLightOcclusionProbes);
        
        mainLightShadow = lerp(1.0, mainLightShadow, mainLightShadowStrength);
    
#endif
        
        // Scattering.
        // 计算主光源散射
        mainLightScattering = CalculateMainLightScattering(cosTheta, cosThetaSquared, g2, gSquared);
        mainLightScattering = lerp(1.0, mainLightScattering, mainLightAnisotropyBlend);

#endif
        // 累加主光源阴影和散射的结果
        mainLightShading += mainLightShadow * mainLightScattering;
        // 增加密度
        density++;
                
        // Depth test.
        // 如果深度超过最大深度，跳出循环
        if (depth > raymarchMaxDepth)
        {
            break;
        }       
    }
    
    density /= raymarchSteps;
    colour.a *= density;
    
#ifdef _MAIN_LIGHT_ENABLED
    
    mainLightShading /= raymarchSteps;
    mainLightShading *= mainLightStrength;
    
    float3 mainLightColour = mainLight.color;    
    
    mainLightColour *= mainLightShading;    
    colour.rgb += mainLightColour;
    
#endif    
    
    output = colour;
}

#endif