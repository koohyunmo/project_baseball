Shader "Custom/Blink"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MaskTex ("Mask",2D) = "white" {}
        _RampTex ("Ramp",2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert


        sampler2D _MainTex;
        sampler2D _MaskTex;
        sampler2D _RampTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MaskTex;
            float2 uv_RampTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) ;
            fixed4 m = tex2D (_MaskTex, IN.uv_MaskTex) ;

            float intensity = lerp(0.5, 5, (sin(_Time.y * 1) + 1) * 0.5); // lerp를 사용하여 발광 강도를 부드럽게 전환
            fixed4 ramp = tex2D (_RampTex, float2(_Time.y,0.5)) ;
            o.Albedo = c.rgb;
            o.Emission = c.rgb * m.g * ramp.r * intensity;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
