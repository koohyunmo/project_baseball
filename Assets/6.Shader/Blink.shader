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

            float intensity = lerp(0.5, 5, (sin(_Time.y * 1) + 1) * 1.2); // lerp를 사용하여 발광 강도를 부드럽게 전환

            //fixed4 ramp = tex2D (_RampTex, float2(_Time.y,0.5)) ;
                       // Assuming white color in mask texture is the area you want to keep color unchanged
            if (m.r > 0.1 && m.g > 0.1 && m.b > 0.1)
            {
                // Keep the color unchanged in the masked area
                o.Albedo = m.rgb;
            }
            else
            {
                // Change color in non-masked area (you can modify this part to achieve desired color change)
                o.Albedo = c.rgb;
            }




            o.Emission = c.rgb * m.g * intensity;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
