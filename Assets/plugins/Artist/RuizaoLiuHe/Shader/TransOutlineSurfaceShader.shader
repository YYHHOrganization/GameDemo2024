Shader "ToonLit/OutlineTrans"
{
    Properties
    {
        _BaseMap ("BaseMap", 2D) = "white" {}
        [HDR]_BaseColor("BaseColor", Color) = (1,1,1,1)
        [Toggle]_ENABLE_ALPHA_TEST("Enable AlphaTest",float)=0
        _Cutoff("Cutoff", Range(0,1.1)) = 0.5
        [Toggle]_OLWVWD("OutlineWidth Varies With Distance?", float) = 0
        _OutlineWidth("OutlineWidth", Range(0, 30)) = 4
        [HDR]_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #pragma shader_feature _ENABLE_ALPHA_TEST_ON
        #pragma shader_feature _OLWVWD_ON
        TEXTURE2D(_BaseMap);        SAMPLER(sampler_BaseMap);
        
        CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            float _Cutoff;
            float4 _OutlineColor;
            float _OutlineWidth;
            float4 _BaseColor;
        CBUFFER_END
        
        struct Attributes{
            float4 positionOS : POSITION;
            float4 texcoord : TEXCOORD;
            float3 normalOS: NORMAL;
        };
        struct Varyings{
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD1;
        };
        ENDHLSL

        Pass
        {
            Tags{"LightMode" = "UniversalForward"}    
            //Cull back
            Cull Back  //note:使用Cull Off的话有些面能够更好的显示出来,比如《原神》中公子的披风
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
			HLSLPROGRAM
			#pragma target 3.0
            #pragma vertex vertex
            #pragma fragment frag
            Varyings vertex(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.uv = input.texcoord.xy;
                return output;
            }
            float4 frag(Varyings input):SV_Target
            {
                float4 tex = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,input.uv);
                float4 finalColor = tex * _BaseColor;
                #if _ENABLE_ALPHA_TEST_ON
                    clip(tex.a-_Cutoff);
                #endif
                return finalColor;
            }
            ENDHLSL
        }
//        Pass
//        {
//            Name "OutLine"
//			Tags{ "LightMode" = "SRPDefaultUnlit" }
//			Cull front
//			HLSLPROGRAM
//			#pragma vertex vert  
//			#pragma fragment frag
//			
//			Varyings vert(Attributes input)
//			{
//                float4 scaledScreenParams = GetScaledScreenParams();
//                float ScaleX = abs(scaledScreenParams.x / scaledScreenParams.y);//求得X因屏幕比例缩放的倍数
//				Varyings output;
//				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
//                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
//                float3 normalCS = TransformWorldToHClipDir(normalInput.normalWS);//法线转换到裁剪空间
//                float2 extendDis = normalize(normalCS.xy) *(_OutlineWidth*0.01);//根据法线和线宽计算偏移量
//                extendDis.x /=ScaleX ;//由于屏幕比例可能不是1:1，所以偏移量会被拉伸显示，根据屏幕比例把x进行修正
//                output.positionCS = vertexInput.positionCS;
//                #if _OLWVWD_ON
//                    //屏幕下描边宽度会变
//                    output.positionCS.xy += extendDis;
//               #else
//                    //下面这句是为了控制当相机离角色过远时，由于描边粗细控制不变导致画面很脏（角色会变得黑乎乎一片，因为包括了描边）的问题
//                    float ctrl = clamp(1/output.positionCS.w,0,1);  //当相机距离过远时，ctrl会接近于0，导致描边很细，不会出现”脏“的现象
//                    //屏幕下描边宽度不变，则需要顶点偏移的距离在NDC坐标下为固定值
//                    //因为后续会转换成NDC坐标，会除w进行缩放，所以先乘一个w，那么该偏移的距离就不会在NDC下有变换
//                    output.positionCS.xy += extendDis * output.positionCS.w * ctrl;
//                #endif
//				return output;
//			}
//			float4 frag(Varyings input) : SV_Target {
//				return float4(_OutlineColor.rgb, 1);
//			}
//            
//            ENDHLSL
//        }
    }
    Fallback "Universal Render Pipeline/Lit"
}