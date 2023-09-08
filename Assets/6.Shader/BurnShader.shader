Shader"Custom/ShaderToyToUnity"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _SecondTex ("Second Texture", 2D) = "white" {}
    }

    SubShader
    {
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

sampler2D _MainTex;
sampler2D _SecondTex;

float3 _Resolution;
float _CustomTime;

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

            // snoise function here...

float4 frag(v2f i) : SV_Target
{
    float4 resultColor = float4(0, 0, 0, 1); // Initialize with black, full opacity

    // ... (mainImage function contents)

    resultColor.rgb = float3(0.8,0.2,0.0);
    resultColor.a = 1;

    return resultColor;
}

            ENDCG
        }
    }
}
