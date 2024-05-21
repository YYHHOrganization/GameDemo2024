Shader "Custom/UI Shiny"
{

    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlossIntensity ("Gloss Intensity", Range(0, 1)) = 0.5
        _GlossColor ("Gloss Color", Color) = (1, 1, 1, 1)
        _GlossThickness ("Gloss Thickness", Range(0, 0.5)) = 0.1
        _SweepSpeed ("Sweep Speed", Range(0, 5)) = 0.5
        _TimeInterval ("Time Interval", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float _GlossIntensity;
            float4 _GlossColor;
            float _GlossThickness;
            float _SweepSpeed;
            float _TimeInterval;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                float glossValue = i.uv.x + _Time.y * _SweepSpeed - i.uv.y;
                float gloss = step(abs(sin(glossValue * _TimeInterval)), _GlossThickness * 0.5f);
                //float gloss = smoothstep(0.2 - _GlossThickness, 0.2 + _GlossThickness, abs(sin(glossValue)));
                tex.rgb += gloss * _GlossIntensity;
                return tex;
            }

            ENDCG
        }

    }

}