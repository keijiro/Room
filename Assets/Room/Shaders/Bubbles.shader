Shader "Room/Bubbles"
{
    Properties
    {
        _Color("Albedo", Color) = (0.5, 0.5, 0.5)
        _Glossiness("Smoothness", Range(0, 1)) = 0
        _Metallic("Metallic", Range(0, 1)) = 0

        [Space]
        [HDR] _Emission("Emission Color", Color) = (1, 0, 0)
        _Intensity1("R to Intensity", Float) = 1
        _Intensity2("G to Intensity", Float) = 0
        _Intensity3("B to Intensity", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma surface surf Standard addshadow
        #pragma target 3.0

        struct Input
        {
            fixed3 color : COLOR;
        };

        half3 _Color;
        half _Glossiness;
        half _Metallic;

        half3 _Emission;
        half _Intensity1;
        half _Intensity2;
        half _Intensity3;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _Color;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            half3 c = floor(IN.color + 0.1);
            c *= float3(_Intensity1, _Intensity2, _Intensity3);
            o.Emission = _Emission * max(max(c.r, c.g), c.b);
        }

        ENDCG
    }
    FallBack "Diffuse"
}
