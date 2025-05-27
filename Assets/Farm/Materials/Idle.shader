Shader "Unlit/Idle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SwayAmount ("Sway Amount", Range(0,0.2)) = 0.05
        _SwaySpeed ("Sway Speed", Range(0,10)) = 2.0
        _SwayX ("Sway X Amount", Range(0,0.2)) = 0.02
        _PhaseOffset ("Phase Offset", Range(0,6.28)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _SwayAmount;
            float _SwaySpeed;
            float _SwayX;
            float _PhaseOffset;

            v2f vert (appdata v)
            {
                v2f o;
                float time = _Time.y * _SwaySpeed;
                // Phase offset by x position for more organic look
                float phase = time + v.vertex.x * _PhaseOffset;
                // Sway mask: smooth transition from bottom to top
                float swayMask = smoothstep(0.0, 1.0, v.uv.y);

                // Sway in Y and X
                float swayY = sin(phase) * _SwayAmount * swayMask;
                float swayX = cos(phase) * _SwayX * swayMask;

                float4 pos = v.vertex;
                pos.y += swayY;
                pos.x += swayX;

                o.vertex = UnityObjectToClipPos(pos);
                o.uv = v.uv; // For SpriteRenderer compatibility
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                // output with alpha
                return col;
            }
            ENDCG
        }
    }
}
