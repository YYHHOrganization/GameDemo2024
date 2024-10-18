Shader "Transparent/TestSeele"
{
    Properties
    {
        [MainTexture] _BaseMap1("Texture", 2D) = "white" {}
        [MainColor] _BaseColor1("Color", Color) = (1, 1, 1, 1)
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
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "IgnoreProjector" = "True"
            "UniversalMaterialType" = "Unlit"
            "RenderPipeline" = "UniversalPipeline"
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
            CBUFFER_END
            TEXTURE2D(_BaseMap1);
            SAMPLER(sampler_BaseMap1);
            
            Varyings1 UnlitPassVertex1(Attributes1 input)
            {
                Varyings1 output = (Varyings1)0;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

                output.positionCS = vertexInput.positionCS;
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
                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap1, sampler_BaseMap1, uv);
                half3 color = texColor.rgb * _BaseColor1.rgb;
                half alpha = texColor.a * _BaseColor1.a;
                
                half4 finalColor = half4(color, alpha);

                return finalColor;
            }
            // -------------------------------------

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
            #pragma shader_feature_local_fragment _ALPHATEST_ON
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

        // Fill GBuffer data to prevent "holes", just in case someone wants to reuse GBuffer for non-lighting effects.
        // Deferred lighting is stenciled out.
//        Pass
//        {
//            Name "GBuffer"
//            Tags
//            {
//                "LightMode" = "UniversalGBuffer"
//            }
//
//            HLSLPROGRAM
//            #pragma target 4.5
//
//            // Deferred Rendering Path does not support the OpenGL-based graphics API:
//            // Desktop OpenGL, OpenGL ES 3.0, WebGL 2.0.
//            #pragma exclude_renderers gles3 glcore
//
//            // -------------------------------------
//            // Shader Stages
//            #pragma vertex UnlitPassVertex
//            #pragma fragment UnlitPassFragment
//
//            // -------------------------------------
//            // Material Keywords
//            #pragma shader_feature_local_fragment _ALPHATEST_ON
//            #pragma shader_feature_local_fragment _ALPHAMODULATE_ON
//
//            // -------------------------------------
//            // Unity defined keywords
//            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
//            #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
//            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
//            #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
//            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
//
//            //--------------------------------------
//            // GPU Instancing
//            #pragma multi_compile_instancing
//            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
//
//            // -------------------------------------
//            // Includes
//            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
//            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitGBufferPass.hlsl"
//            ENDHLSL
//        }

//        Pass
//        {
//            Name "DepthOnly"
//            Tags
//            {
//                "LightMode" = "DepthOnly"
//            }
//
//            // -------------------------------------
//            // Render State Commands
//            ZWrite On
//            ColorMask R
//
//            HLSLPROGRAM
//            #pragma target 2.0
//
//            // -------------------------------------
//            // Shader Stages
//            #pragma vertex DepthOnlyVertex
//            #pragma fragment DepthOnlyFragment
//
//            // -------------------------------------
//            // Material Keywords
//            #pragma shader_feature_local_fragment _ALPHATEST_ON
//
//            // -------------------------------------
//            // Unity defined keywords
//            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
//
//            //--------------------------------------
//            // GPU Instancing
//            #pragma multi_compile_instancing
//            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
//
//            // -------------------------------------
//            // Includes
//            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
//            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
//            ENDHLSL
//        }
//
//        Pass
//        {
//            Name "DepthNormalsOnly"
//            Tags
//            {
//                "LightMode" = "DepthNormalsOnly"
//            }
//
//            // -------------------------------------
//            // Render State Commands
//            ZWrite On
//
//            HLSLPROGRAM
//            #pragma target 2.0
//
//            // -------------------------------------
//            // Shader Stages
//            #pragma vertex DepthNormalsVertex
//            #pragma fragment DepthNormalsFragment
//
//            // -------------------------------------
//            // Material Keywords
//            #pragma shader_feature_local_fragment _ALPHATEST_ON
//
//            // -------------------------------------
//            // Universal Pipeline keywords
//            #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT // forward-only variant
//            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
//            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
//
//            //--------------------------------------
//            // GPU Instancing
//            #pragma multi_compile_instancing
//            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
//
//            // -------------------------------------
//            // Includes
//            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
//            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitDepthNormalsPass.hlsl"
//            ENDHLSL
//        }
//
//        // This pass it not used during regular rendering, only for lightmap baking.
//        Pass
//        {
//            Name "Meta"
//            Tags
//            {
//                "LightMode" = "Meta"
//            }
//
//            // -------------------------------------
//            // Render State Commands
//            Cull Off
//
//            HLSLPROGRAM
//            #pragma target 2.0
//
//            // -------------------------------------
//            // Shader Stages
//            #pragma vertex UniversalVertexMeta
//            #pragma fragment UniversalFragmentMetaUnlit
//
//            // -------------------------------------
//            // Unity defined keywords
//            #pragma shader_feature EDITOR_VISUALIZATION
//
//            // -------------------------------------
//            // Includes
//            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
//            #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitMetaPass.hlsl"
//            ENDHLSL
//        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.UnlitShader"
}
