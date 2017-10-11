Shader "Hidden/Room/LightSphere"
{
    Properties
    {
        [HDR] _Color("", Color) = (1, 1, 1, 1)
    }

    CGINCLUDE

    #include "UnityCG.cginc"
    #include "SimplexNoise3D.hlsl"

    half4 _Color;
    float _Radius;
    half _NoiseAmp;
    half _NoiseFreq;
    float3 _NoiseOffs;

    float4 Vertex(float4 position : POSITION) : POSITION
    {
        float n = snoise(position.xyz * _NoiseFreq + _NoiseOffs);
        position.xyz *= _Radius * (1 + n * _NoiseAmp);
        return UnityObjectToClipPos(position);
    }

    half4 Fragment(float4 position : SV_POSITION) : SV_Target
    {
        return _Color;
    }

    ENDCG

    SubShader
    {
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }
}
