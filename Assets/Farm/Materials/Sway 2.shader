Shader "UI/AnimateSprite"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _SwayFlip ("Sway Flip", Range(0,1)) = 0
        _SwayAmount ("Sway Amount", Range(0,0.2)) = 0.1
        _SwaySpeed ("Sway Speed", Range(0,10)) = 2.0
        [Toggle] _EnableFlash ("Enable Flash", Float) = 0
        _Flash ("Flash Intensity", Range(0,1)) = 0
        _LineWidth ("Line Width", Range(0.001,0.1)) = 0.02
        _LineAngle ("Line Angle", Range(0,360)) = 45
        _LineSpeed ("Line Speed", Range(0,10)) = 1.0
        
        // UI Specific Properties
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _SwayAmount;
            float _SwaySpeed;
            float _Flash;
            float _LineWidth;
            float _LineAngle;
            float _LineSpeed;
            float _EnableFlash;
            float _SwayFlip;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                
                // Apply sway effect in world space
                float time = _Time.y * _SwaySpeed;
                float sway = sin(time + worldPos.x * 5.0) * _SwayAmount * 100.0; // Scale up for UI
                float swayMask = saturate((v.texcoord.y-0.4)*2.0); // Simple mask: more sway at top (y=1), less at bottom (y=0)
                
                if (_SwayFlip == 1)
                {
                    worldPos.x += sway * swayMask;
                }
                else
                {
                    worldPos.x -= sway * swayMask;
                }
                
                OUT.vertex = mul(UNITY_MATRIX_VP, worldPos);
                OUT.worldPosition = worldPos;
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                
                // Calculate line with angle
                float angleRad = radians(_LineAngle);
                float2 dir = float2(cos(angleRad), sin(angleRad));
                float proj = dot(IN.texcoord, dir);
                
                // Animate line position
                float linePos = frac(_Time.y * _LineSpeed);
                
                // Create line effect
                float lineEffect = saturate((1.0 - abs(proj - linePos) / _LineWidth) * 10.0);
                
                // Add flashing line to the sprite
                color.rgb += _EnableFlash * lineEffect * _Flash;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
        ENDCG
        }
    }
}
