Shader "Unlit/SVMap"
{
    Properties
    {
        _Hue ("Hue", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags {"Queue"="Overlay"}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _Hue;  // H（色相）を外部から設定

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

            // HSV → RGB 変換
            fixed3 HsvToRgb(float h, float s, float v)
            {
                float3 p = abs(fmod(h * 6.0 + float3(0, 4, 2), 6) - 3);
                return v * (s * clamp(p - 1, 0, 1) + (1 - s));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // UV座標を 0~1 の範囲に正規化
                float2 uv = i.uv;
                
                // RawImage のアスペクト比を考慮して UV座標をスケーリング
                uv.x = uv.x * 1.05f; // 横方向のスケーリング（必要に応じて調整）
                uv.y = uv.y * 1.0f; // 縦方向のスケーリング（必要に応じて調整）

                // HSV → RGB 変換
                float s = uv.x;  // X軸 = 彩度 (S)
                float v = uv.y;  // Y軸 = 明度 (V)（上が白、下が黒）
                fixed3 color = HsvToRgb(_Hue, s, v);
                return fixed4(color, 1);
            }
            ENDCG
        }
    }
}
