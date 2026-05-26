Shader "Custom/MonoMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _MaskTex ("Mask Texture", 2D) = "white" {}
        _MonoStrength ("Monochrome Strength", Range(0, 1)) = 1
        _Color ("Color Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float _MonoStrength;
            fixed4 _Color;
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 mask = tex2D(_MaskTex, i.uv);

                if (mask.r < 0.1 && mask.g < 0.1 && mask.b < 0.1){
                    // モノクロ変換
                    float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                    col.rgb = lerp(col.rgb, float3(gray, gray, gray) * _Color.rgb, _MonoStrength);
                }
                else{
                }

                return col;
            }
            ENDCG
        }
    }
}
