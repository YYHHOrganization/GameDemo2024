Shader "Transparent/TestSeele"
{
    Properties
    {
        [MainTexture] _BaseMap1("Texture", 2D) = "white" {}
        [HDR] _BaseColor1("Color", Color) = (1, 1, 1, 1)
        _Cutoff1("AlphaCutout", Range(0.0, 1.0)) = 0.5

        // BlendMode
        _Surface("__surface", Float) = 0.0
        _Blend("__mode", Float) = 0.0
        _Cull("__cull", Float) = 2.0
        [ToggleUI] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _BlendOp("__blendop", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _SrcBlendAlpha("__srcA", Float) = 1.0
        [HideInInspector] _DstBlendAlpha("__dstA", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _AlphaToMask("__alphaToMask", Float) = 0.0

        // Editmode props
        _QueueOffset("Queue offset", Float) = 0.0

        // ObsoleteProperties
        [HideInInspector] _MainTex("BaseMap", 2D) = "white" {}
        [HideInInspector] _Color("Base Color", Color) = (0.5, 0.5, 0.5, 1)
        [HideInInspector] _SampleGI("SampleGI", float) = 0.0 // needed from bakedlit
        [HDR]_FresnelColor("Fresnel Color", Color) = (1, 1, 1, 1)
        _FresnelPower("Fresnel Power", Range(0, 20)) = 1.0
        [Toggle] _IsFace("Is Face", Float) = 0.0
        _TimeMultiplier("Time Multiplier", Float) = 1.0
        _GlitchIntensity("Glitch Intensity", Float) = 0.1
        _Offset("Color Offset", Float) = 0.02
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "IgnoreProjector" = "True"
            "UniversalMaterialType" = "Unlit"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }
        LOD 100

        // -------------------------------------
        // Render State Commands
        Blend [_SrcBlend][_DstBlend], [_SrcBlendAlpha][_DstBlendAlpha]
        ZWrite [_ZWrite]
        Cull [_Cull]

        Pass
        {
            Name "Unlit"

            // -------------------------------------
            // Render State Commands
            AlphaToMask[_AlphaToMask]

            HLSLPROGRAM
            #pragma target 2.0

            struct Attributes1
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                //UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings1
            {
                float2 uv : TEXCOORD0;
                float fogCoord : TEXCOORD1;
                float4 positionCS : SV_POSITION;
                
                float3 positionWS : TEXCOORD2;
                float3 normalWS : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
                
            };
    
            #pragma vertex UnlitPassVertex1
            #pragma fragment UnlitPassFragment1
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor1;
                float _Cutoff1;
                float4 _BaseMap1_ST;
                float4 _FresnelColor;
                float _FresnelPower;
                float _IsFace;
                float _TimeMultiplier;
                float _GlitchIntensity;
                float _Offset;
            CBUFFER_END
            TEXTURE2D(_BaseMap1);
            SAMPLER(sampler_BaseMap1);
            // float random(float2 uv)
            // {
            //     return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            // }
            //
            // float4 applyGlitch(float2 uv, float time)
            // {
            //     // Horizontal line displacement (simulating TV signal glitches)
            //     float glitchStrength = random(uv + time) * _GlitchIntensity;
            //     uv.y += glitchStrength * 0.05;
            //
            //     // RGB offset for chromatic aberration
            //     float2 uvR = uv + float2(_Offset, 0.0);
            //     float2 uvG = uv;
            //     float2 uvB = uv + float2(-_Offset, 0.0);
            //
            //     float4 color = float4(0.0, 0.0, 0.0, 1.0);
            //     color.r = SAMPLE_TEXTURE2D(_BaseMap1, sampler_BaseMap1, uvR).r;
            //     color.g = SAMPLE_TEXTURE2D(_BaseMap1, sampler_BaseMap1, uvG).g;
            //     color.b = SAMPLE_TEXTURE2D(_BaseMap1, sampler_BaseMap1, uvB).b;
            //
            //     return color;
            // }
            
            Varyings1 UnlitPassVertex1(Attributes1 input)
            {
                Varyings1 output = (Varyings1)0;
                //float random_value = Unity_RandomRange_float(_Time, -1.0, 1.0);
                //float simpleNoise = Unity_SimpleNoise_float(input.uv, 500);
                //float noise = simpleNoise * random_value * _GlitchIntensity;
                //隔一会儿闪一下
                //noise = step(frac(_Time.y * _GlitchSpeed), 0.5) * noise;
                // input.positionOS.x += noise;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

                output.positionCS = vertexInput.positionCS;
                //output.positionCS.x += noise;
                //output.uv = TRANSFORM_TEX(input.uv, _BaseMap1);
                output.uv = input.uv;
                
                // normalWS and tangentWS already normalize.
                // this is required to avoid skewing the direction during interpolation
                // also required for per-vertex lighting and SH evaluation
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);

                // already normalized from normal transform to WS.
                output.positionWS = vertexInput.positionWS;
                output.normalWS = normalInput.normalWS;
                output.viewDirWS = viewDirWS;

                return output;
            }

            half4 UnlitPassFragment1(Varyings1 input):SV_Target
            {
                half2 uv = input.uv;
                //float4 texColor = applyGlitch(uv, _Time.y);
                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap1, sampler_BaseMap1, uv);
                half3 color = texColor.rgb * _BaseColor1.rgb;
                half alpha = texColor.a * _BaseColor1.a;
                float3 viewDirWS = normalize(input.viewDirWS);
                float3 normalWS = normalize(input.normalWS);
                float fresnel = pow((1.0 - saturate(dot(viewDirWS, normalWS))), _FresnelPower) * (1.0 - _IsFace);
                
                half4 finalColor = lerp(half4(color, alpha), half4(_FresnelColor.rgb, alpha), fresnel);

                return finalColor;
            }
            // -------------------------------------

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
            //#pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAMODULATE_ON

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            // -------------------------------------
            // Includes
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitForwardPass.hlsl"
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    //CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.UnlitShader"
}
