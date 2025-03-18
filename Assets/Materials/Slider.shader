Shader "Unlit/HueSlider"
{
    SubShader
    {
        Tags {"Queue"="Overlay"}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            // HSV Å® RGB ïœä∑
            fixed3 HsvToRgb(float h)
            {
                float3 p = abs(fmod(h * 6.0 + float3(0, 4, 2), 6) - 3);
                return clamp(p - 1, 0, 1);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float hue = i.uv.x;  // Xé≤ = êFëä (H)
                fixed3 color = HsvToRgb(hue);
                return fixed4(color, 1);
            }
            ENDCG
        }
    }
}
