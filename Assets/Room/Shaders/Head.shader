Shader "Hidden/Room/Head"
{
    Properties
    {
        _MainTex("Face Texture", 2D) = "gray" {}
        [HDR] _Color("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma surface Surface Standard vertex:Vertex addshadow nolightmap
        #pragma target 3.0

        #include "UnityCG.cginc"
        #include "SimplexNoise3D.hlsl"

        // Hue to RGB convertion
        half3 HueToRGB(half h)
        {
            h = frac(h);
            half r = abs(h * 6 - 3) - 1;
            half g = 2 - abs(h * 6 - 2);
            half b = 2 - abs(h * 6 - 4);
            half3 rgb = saturate(half3(r, g, b));
            return GammaToLinearSpace(rgb);
        }

        struct Input
        {
            float2 uv_MainTex;
            float3 localCoord;
        };

        sampler2D _MainTex;
        half4 _Color;
        half _GradFreq;
        float _GradOffs;

        void Vertex(inout appdata_full input, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);
            data.localCoord = input.vertex.xyz;
        }

        void Surface(Input IN, inout SurfaceOutputStandard o)
        {
            float3 tex = tex2D(_MainTex, IN.uv_MainTex).rgb;
            float hue = snoise((IN.localCoord.xyz + _GradOffs) * _GradFreq);
            o.Albedo = HueToRGB(hue) * _Color.rgb * (1 - _Color.a);
            o.Emission = tex * _Color.rgb * _Color.a;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
