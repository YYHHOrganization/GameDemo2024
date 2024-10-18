Shader "Unlit/PrePass"
{
    SubShader
    {
        Pass {
			Tags{"LightMode"="UniversalForward"}
			ZWrite On
            ColorMask 0
            HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			struct v2f 
			{
				float4 vertex : POSITION;
			};
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(vertex);	
				return o;
			}
            float4 frag (v2f i) : SV_Target
			{
                return 0;
			}
			ENDHLSL
		}
    }
}