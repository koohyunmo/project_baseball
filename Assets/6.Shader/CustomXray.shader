Shader "Custom/XrayShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

         _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0,8)) = 3

        _BumpMap ("Normal Map",2D) = "white" {}

        _Metallic ("Metallic",Range(0,1)) = 0

        _XColor ("Xray Color", Color) = (1,1,1,1)
        _XTex ("Xray Tex", 2D) = "white" {}

    }
    SubShader
    {
        Tags { "RenderType"="Overlay" }
        LOD 200
        //NORMAL
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
         sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float4 screenPos;
           
        };

        fixed4 _Color;
        fixed4 _RimColor;
     
        float _Metallic;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Normal = UnpackNormal(tex2D(_BumpMap,IN.uv_BumpMap));
            o.Albedo = c.rgb;
            
            o.Metallic = _Metallic;

            //float rim = 1.0 - saturate(dot(normalize(IN.screenPos.xyz), o.Normal));
            //o.Emission = _RimColor.rgb * pow(rim, _RimPower);
            o.Alpha = c.a;
        }
        ENDCG

        //Xray
        ZTEST GREATER

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _XTex;
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
