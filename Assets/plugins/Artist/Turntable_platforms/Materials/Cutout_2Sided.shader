Shader "Custom/Cutout_2Sided" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_Intensity ("Light Intensity", Range(0,5)) = 0.5
}

SubShader 
{
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	Cull Off
	LOD 200

 CGPROGRAM
 #pragma surface surf Lambert alphatest:_Cutoff 
 
	   
	sampler2D _MainTex;
	fixed4 _Color;
	float _Intensity;
            
            
	struct Input 
	{
		float2 uv_MainTex;
	};

     

    void surf (Input IN, inout SurfaceOutput o) 
     {
 	   fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;  

        o.Emission = c.rgb * _Intensity;
        o.Albedo = c.rgb;
        o.Alpha = c.a;
         
     }
     
 ENDCG
}

Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}
