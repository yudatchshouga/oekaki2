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

            float _Hue;  // H�i�F���j���O������ݒ�

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

            // HSV �� RGB �ϊ�
            fixed3 HsvToRgb(float h, float s, float v)
            {
                float3 p = abs(fmod(h * 6.0 + float3(0, 4, 2), 6) - 3);
                return v * (s * clamp(p - 1, 0, 1) + (1 - s));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // UV���W�� 0~1 �͈̔͂ɐ��K��
                float2 uv = i.uv;
                
                // RawImage �̃A�X�y�N�g����l������ UV���W���X�P�[�����O
                uv.x = uv.x * 1.05f; // �������̃X�P�[�����O�i�K�v�ɉ����Ē����j
                uv.y = uv.y * 1.0f; // �c�����̃X�P�[�����O�i�K�v�ɉ����Ē����j

                // HSV �� RGB �ϊ�
                float s = uv.x;  // X�� = �ʓx (S)
                float v = uv.y;  // Y�� = ���x (V)�i�オ���A�������j
                fixed3 color = HsvToRgb(_Hue, s, v);
                return fixed4(color, 1);
            }
            ENDCG
        }
    }
}
