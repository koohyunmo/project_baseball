Shader "Custom/LambertWithOutline" {
    Properties{
        _Color("Main Color", Color) = (.5,.5,.5,1)
        _Outline("Outline color", Color) = (0,0,0,1)
        _OutlineWidth("Outline width", Range(.002, 0.03)) = .005
        _MainTex("Base (RGB)", 2D) = "white" { }
    }

        CGINCLUDE
#include "UnityCG.cginc"

        struct appdata {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
    };

    struct v2f {
        float4 pos : POSITION;
        float4 color : COLOR;
    };

    uniform float _Outline;
    uniform float4 _OutlineColor;

    v2f vert(appdata v) {
        // just make a copy of incoming vertex data but scaled according to normal direction
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);

        float3 norm = mul((float3x3) UNITY_MATRIX_IT_MV, v.normal);
        float2 offset = TransformViewToProjection(norm.xy);

        o.pos.xy += offset * o.pos.z * _Outline;
        o.color = _OutlineColor;
        return o;
    }
    ENDCG

        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass {
                Name "OUTLINE"
                Tags { "LightMode" = "Always" }
                Cull Front
                Offset 15,15

                CGPROGRAM
                #pragma surface surf Lambert
                ENDCG

                SetTexture[_MainTex] {
                    combine primary
                }
            }

            CGPROGRAM
            #pragma surface surf Lambert

            sampler2D _MainTex;
            fixed4 _Color;

            struct Input {
                float2 uv_MainTex;
            };

            void surf(Input IN, inout SurfaceOutput o) {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }
            ENDCG
    }
        FallBack "Diffuse"
}
