Shader "ToonLit/Outline"
{
    Properties
    {
        _MainTex ("BaseMap", 2D) = "white" {}
        [HDR]_BaseColor1("BaseColor", Color) = (1,1,1,1)
        [Toggle]_ENABLE_ALPHA_TEST("Enable AlphaTest",float)=0
        _Cutoff1("Cutoff", Range(0,1.1)) = 0.5
        [Toggle]_OLWVWD("OutlineWidth Varies With Distance?", float) = 0
        _OutlineWidth("OutlineWidth", Range(0, 30)) = 4
        [HDR]_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "Queue" = "Geometry-100" "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "False"}
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #pragma shader_feature _ENABLE_ALPHA_TEST_ON
        #pragma shader_feature _OLWVWD_ON
        TEXTURE2D(_MainTex);        SAMPLER(sampler_MainTex);
        
        CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float _Cutoff1;
            float4 _OutlineColor;
            float _OutlineWidth;
            float4 _BaseColor1;
        CBUFFER_END
        
        struct Attributes1{
            float4 positionOS : POSITION;
            float4 texcoord : TEXCOORD;
            float3 normalOS: NORMAL;
        };
        struct Varyings1{
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD1;
        };
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            //Cull back
            Cull Off  //note:使用Cull Off的话有些面能够更好的显示出来,比如《原神》中公子的披风
            ZWrite On
            //Blend One Zero
//            ZWrite Off
//            Blend SrcAlpha OneMinusSrcAlpha
			HLSLPROGRAM
			
			#pragma target 3.0
            #pragma vertex vertex
            #pragma fragment frag
            Varyings1 vertex(Attributes1 input)
            {
                Varyings1 output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.uv = input.texcoord.xy;
                return output;
            }
            float4 frag(Varyings1 input):SV_Target
            {
                float4 tex = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,input.uv);
                float4 finalColor = tex * _BaseColor1;
                #if _ENABLE_ALPHA_TEST_ON
                    clip(tex.a-_Cutoff1);
                #endif
                //finalColor = float4(1.0,1.0,1.0,1.0);
                return finalColor;
            }
            ENDHLSL
        }
        Pass
        {
            Name "OutLine"
			Tags{ "LightMode" = "SRPDefaultUnlit" }
			Cull front
			HLSLPROGRAM
			#pragma vertex vert  
			#pragma fragment frag
			
			Varyings1 vert(Attributes1 input)
			{
                float4 scaledScreenParams = GetScaledScreenParams();
                float ScaleX = abs(scaledScreenParams.x / scaledScreenParams.y);//求得X因屏幕比例缩放的倍数
				Varyings1 output;
				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
                float3 normalCS = TransformWorldToHClipDir(normalInput.normalWS);//法线转换到裁剪空间
                float2 extendDis = normalize(normalCS.xy) *(_OutlineWidth*0.01);//根据法线和线宽计算偏移量
                extendDis.x /=ScaleX ;//由于屏幕比例可能不是1:1，所以偏移量会被拉伸显示，根据屏幕比例把x进行修正
                output.positionCS = vertexInput.positionCS;
                #if _OLWVWD_ON
                    //屏幕下描边宽度会变
                    output.positionCS.xy += extendDis;
               #else
                    //下面这句是为了控制当相机离角色过远时，由于描边粗细控制不变导致画面很脏（角色会变得黑乎乎一片，因为包括了描边）的问题
                    float ctrl = clamp(1/output.positionCS.w,0,1);  //当相机距离过远时，ctrl会接近于0，导致描边很细，不会出现”脏“的现象
                    //屏幕下描边宽度不变，则需要顶点偏移的距离在NDC坐标下为固定值
                    //因为后续会转换成NDC坐标，会除w进行缩放，所以先乘一个w，那么该偏移的距离就不会在NDC下有变换
                    output.positionCS.xy += extendDis * output.positionCS.w * ctrl;
                #endif
				return output;
			}
			float4 frag(Varyings1 input) : SV_Target {
				return float4(_OutlineColor.rgb, 1);
			}
            
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On
            ColorMask R
            Cull Back

            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Shader Stages
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            // -------------------------------------
            // Includes
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

        // This pass is used when drawing to a _CameraNormalsTexture texture
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Shader Stages
            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _PARALLAXMAP
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE

            // -------------------------------------
            // Universal Pipeline keywords
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            // -------------------------------------
            // Includes
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitDepthNormalsPass.hlsl"
            ENDHLSL
        }
    }
    Fallback "Universal Render Pipeline/Lit"
}