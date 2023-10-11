Shader "Custom/XrayShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

        _RimPower ("Rim Power", Range(0,8)) = 3

        _BumpMap ("Normal Map",2D) = "white" {}

        _Metallic ("Metallic",Range(0,1)) = 0
        _Smoothness ("Smoothness",Range(0,1)) = 0

        _XColor ("Xray Color", Color) = (1,1,1,1)

        _MaskTex ("Detail Mask", 2D) = "white" {}

        _LineColor ("Line Color", Color) = (1,0,0,1)

    }
    SubShader
    {
        Tags { "RenderType"="Overlay"}
        LOD 200
        //NORMAL
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
         sampler2D _BumpMap;
         sampler2D _MaskTex;


        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_MaskTex;
            float4 screenPos;
            float3 viewDir;
           
        };

        fixed4 _Color;
        fixed4 _LineColor;
     
        float _Metallic;
        float _Smoothness;
        float _RimPower;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 m = tex2D (_MaskTex, IN.uv_MaskTex);
            o.Normal = UnpackNormal(tex2D(_BumpMap,IN.uv_BumpMap)) * 2;
            //o.Albedo = c.rgb;
            o.Albedo = c.rgb;
            
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;

            // Assuming white color in mask texture is the area you want to keep color unchanged
            if (m.r > 0.9 && m.g > 0.9 && m.b > 0.9)
            {
                // Keep the color unchanged in the masked area
                o.Albedo = m.rgb * _LineColor.rgb;
            }
            else
            {
                // Change color in non-masked area (you can modify this part to achieve desired color change)
                o.Albedo = c.rgb;
            }

            float rim = saturate(dot(o.Normal, IN.viewDir));
            o.Emission = pow(1-rim, _RimPower) * _Color.rgb;
            o.Alpha = _Color.a;
        }
        ENDCG

        //Xray
        ZTEST GREATER

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        
        sampler2D _MainTex;
        float4 _XColor;
        float _RimPower;

        struct Input
        {
            float2 uv_MainTex;
             float3 viewDir;
        };

        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            //o.Albedo = c.rgb;
            //o.Alpha = c.a;
            float rim = saturate(dot(o.Normal, IN.viewDir));
            o.Emission = pow(1-rim, _RimPower) * _XColor.rgb;
            o.Alpha = 1;
        }
        ENDCG
        
    }
    FallBack "Diffuse"
}
