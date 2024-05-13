Shader "Custom/Outline/Stencil Outline Shading"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Outline ("Outline", Range(0,1)) = 0.1
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        [Toggle] _ENABLE_CLIPSPACE_OUTLINE("Enable ClipSpace Outline", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="SRPDefaultUnlit" }
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 texcoord: TEXCOORD0;
                float4 vertex: POSITION;
            };

            struct Varyings
            {
                float4 pos: SV_POSITION;
                float2 uv : TEXCOORD1;
            };

            // 此宏将 _BaseMap 声明为 Texture2D 对象。
            TEXTURE2D(_MainTex);
            // This macro declares the sampler for the _BaseMap texture.
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Outline;
                float4 _OutlineColor;
            CBUFFER_END

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                return col;
            }
            ENDHLSL
        }

        Pass
        {
            //在这个Pass当中延法线外扩，并设置Stencil Ref为1，Comp NotEqual
            Name "Outline"
            Tags {"LightMode"="UniversalForward"}

            Stencil
            {
                Ref 1
                Comp NotEqual
                Pass Keep
            }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #pragma shader_feature _ENABLE_CLIPSPACE_OUTLINE_ON  // 注意在pragma这里要把宏定义写成_ON，否则在材质面板上勾选的逻辑会有问题

            CBUFFER_START(UnityPerMaterial)
                float _Outline;
                float4 _OutlineColor;
            CBUFFER_END

            struct Attributes
            {
                float4 vertex: POSITION;
                float3 normal: NORMAL;
            };

            struct Varyings
            {
                float4 pos: SV_POSITION;
            };

            Varyings vert (Attributes v)
            {
                //将顶点法线外扩，不过是在视图空间下进行的
                Varyings o;

                //方案1：模型空间外扩，效果并不好
                float3 vertexWithOffset = v.vertex.xyz + v.normal * _Outline * 0.0005f;
                o.pos = TransformObjectToHClip(vertexWithOffset);

                //方案2：裁剪空间外扩，效果会好一些
                #if _ENABLE_CLIPSPACE_OUTLINE_ON
                
                VertexNormalInputs vertexNormalInputs = GetVertexNormalInputs(v.normal.xyz);
                float2 normalCS = TransformWorldToHClipDir(vertexNormalInputs.normalWS).xy;
                o.pos = TransformObjectToHClip(v.vertex);
                o.pos.xy += normalize(normalCS.xy) * _Outline * 0.05f;  //乘0.05是为了让扩张的线条更细一些,否则太粗了
                #endif
                return o;

                //下面这段代码原本是在视图空间去做顶点外扩的，但是效果并不理想，所以就不用了
                // float4 pos = mul(UNITY_MATRIX_MV, v.vertex); //将顶点从模型空间转换到视图空间，这样做扩散描边效果更好
                // float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal); //将法线从模型空间转换到视图空间,注意这里要用逆转置矩阵，见入门精要数学原理部分
                // //normal.z = -0.5; //尽可能避免背面扩张后的顶点挡住正面的面片
                // pos = pos + float4(normalize(normal),0) * _Outline; //将法线方向乘以扩张距离，然后加到顶点坐标上
                // o.pos = mul(UNITY_MATRIX_P, pos); //将顶点从视图空间转换到裁剪空间
                // return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return float4(_OutlineColor.rgb, 1);
            }
            
            ENDHLSL
        }
    }
}