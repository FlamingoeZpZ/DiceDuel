Shader "Custom/TMP_SDFShader"
{
    Properties
    {
        _MainTex ("Font Atlas", 2D) = "white" {}
        _FaceColor ("Face Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 0.2)) = 0.05
        _GlowColor ("Glow Color", Color) = (1,1,1,0)
        _GlowOffset ("Glow Offset", Range(0, 1)) = 0.0
        _GlowPower ("Glow Power", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags {"Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Lighting Off
        ZWrite Off
        Cull Off
        Fog { Mode Off }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _FaceColor;
            float4 _OutlineColor;
            float _OutlineWidth;
            float4 _GlowColor;
            float _GlowOffset;
            float _GlowPower;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the main texture (SDF Font Atlas)
                float4 col = tex2D(_MainTex, i.uv);
                
                // SDF edge threshold
                float distance = col.a; // SDF alpha channel stores the distance field
                float width = fwidth(distance);
                float alpha = smoothstep(0.5 - _OutlineWidth - width, 0.5 + _OutlineWidth + width, distance);
                
                // Apply face color and outline
                float4 face = _FaceColor * alpha;
                float4 outline = _OutlineColor * (1 - alpha);
                
                // Add glow effect
                float glow = smoothstep(_GlowOffset, _GlowPower, distance);
                float4 glowEffect = _GlowColor * glow;

                return face + outline + glowEffect;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
