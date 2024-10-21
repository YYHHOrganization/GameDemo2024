Shader "Test"
{
 Properties
 {
     [MainTexture] _BaseMap("Texture", 2D) = "white" {}
     [MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)
 }

 SubShader
 {
     Tags
     {
         "RenderType"="Transparent"
         "UniversalMaterialType" = "Unlit"
         "RenderPipeline" = "UniversalPipeline"
     }
     LOD 100

     Blend SrcAlpha OneMinusSrcAlpha
     ZWrite Off  //这个是无所谓的，因为我们已经开了propass：https://www.xuanyusong.com/archives/4759
     Cull Off

     Pass
     {
         HLSLPROGRAM
         #pragma target 2.0
         #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
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
         CBUFFER_START(UnityPerMaterial)
             float4 _BaseColor;
             float _Cutoff;
             float4 _BaseMap_ST;
         CBUFFER_END

         TEXTURE2D(_BaseMap);
         SAMPLER(sampler_BaseMap);

         Varyings1 UnlitPassVertex1(Attributes1 input)
         {
             Varyings1 output = (Varyings1)0;

             VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

             output.positionCS = vertexInput.positionCS;
             output.uv = TRANSFORM_TEX(input.uv, _BaseMap);

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

         half4 UnlitPassFragment1(Varyings1 input):SV_TARGET
         {

             half2 uv = input.uv;
             half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
             half3 color = texColor.rgb * _BaseColor.rgb;
             half alpha = texColor.a * _BaseColor.a;

             half4 finalColor = half4(color, alpha);

             return finalColor;
         }
         ENDHLSL
     }
 }

 FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
