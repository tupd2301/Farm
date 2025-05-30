Shader "Unlit/AnimateSprite"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _SwayAmount ("Sway Amount", Range(0,0.2)) = 0.05
        _SwaySpeed ("Sway Speed", Range(0,10)) = 2.0
        [Toggle] _EnableFlash ("Enable Flash", Float) = 0
        _Flash ("Flash Intensity", Range(0,1)) = 0
        _LineWidth ("Line Width", Range(0.001,0.1)) = 0.02
        _LineAngle ("Line Angle", Range(0,360)) = 45
        _LineSpeed ("Line Speed", Range(0,10)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
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
            float4 _Color;
            float _SwayAmount;
            float _SwaySpeed;
            float _Flash;
            float _LineWidth;
            float _LineAngle;
            float _LineSpeed;
            float _EnableFlash;

            v2f vert (appdata v)
            {
                v2f o;
                float sway = sin(_Time.y * _SwaySpeed + v.vertex.y * 2.0) * _SwayAmount;
                float4 pos = v.vertex;
                float swayMask = saturate((v.uv.y - 0.5) * 2.0); // 0 for uv.y <= 0.5, 1 for uv.y == 1
                pos.x += sway * swayMask; // Changed from pos.x to pos.y for vertical sway
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // Calculate line with angle
                float angleRad = radians(_LineAngle);
                float2 dir = float2(cos(angleRad), sin(angleRad));
                float proj = dot(i.uv, dir);
                
                // Animate line position
                float linePos = frac(_Time.y * _LineSpeed);
                
                // Create line effect
                float lineEffect = saturate((1.0 - abs(proj - linePos) / _LineWidth) * 10.0);
                
                // Add flashing line to the sprite
                col.rgb += _EnableFlash * lineEffect * _Flash;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                // output with alpha
                return col;
            }
            ENDCG
        }
    }
}
