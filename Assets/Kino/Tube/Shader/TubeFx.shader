// KinoTube - Composite video/old TV artifacts simulation
// https://github.com/keijiro/KinoTube

Shader "Hidden/Kino/Tube"
{
    Properties
    {
        _MainTex("", 2D) = ""{}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    uint _BleedTaps;
    float _BleedDelta;
    float _Scanline;

    half3 RGB2YIQ(fixed3 rgb)
    {
        return mul(half3x3(0.299,  0.587,  0.114,
                           0.596, -0.274, -0.322,
                           0.211, -0.523,  0.313), rgb);
    }

    fixed3 YIQ2RGB(half3 yiq)
    {
        return mul(half3x3(1,  0.956,  0.621,
                           1, -0.272, -0.647,
                           1, -1.106,  1.703), yiq);
    }

    half3 SampleYIQ(float2 uv, float du)
    {
        uv.x += du;
        fixed3 rgb = saturate(tex2D(_MainTex, uv).rgb);
        return RGB2YIQ(LinearToGammaSpace(rgb));
    }

    fixed4 frag(v2f_img input) : SV_Target
    {
        float2 uv = input.uv;
        half3 yiq = SampleYIQ(uv, 0);

        half total = _BleedTaps * 2;
        yiq.yz *= total;

        for (uint i = 0; i < _BleedTaps; i++)
        {
            half w = _BleedTaps * 2 - i;
            yiq.yz += SampleYIQ(uv, +_BleedDelta * i).yz * w;
            yiq.yz += SampleYIQ(uv, -_BleedDelta * i).yz * w;
            total += w * 2;
        }

        yiq.yz /= total;

        half scan = sin(uv.y * 500 * UNITY_PI);
        scan = lerp(1, (scan + 1) / 2, _Scanline);

        return fixed4(GammaToLinearSpace(YIQ2RGB(yiq * scan)), 1);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
}
