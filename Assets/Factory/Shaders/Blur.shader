Shader "Unlit/Blur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0,10)) = 1
        _BlurSamples ("Blur Samples", Range(1,20)) = 10
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _BlurSize;
            float _BlurSamples;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 texelSize = _MainTex_TexelSize.xy;
                fixed4 col = fixed4(0,0,0,0);
                float weightSum = 0;
                
                // Gaussian blur
                for(float x = -_BlurSamples; x <= _BlurSamples; x++)
                {
                    for(float y = -_BlurSamples; y <= _BlurSamples; y++)
                    {
                        float2 offset = float2(x,y) * texelSize * _BlurSize;
                        float weight = exp(-(x*x + y*y)/(2.0 * _BlurSamples));
                        col += tex2D(_MainTex, i.uv + offset) * weight;
                        weightSum += weight;
                    }
                }
                col /= weightSum;
                
                // Apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
