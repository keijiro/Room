Shader "Hidden/Room/Wiper"
{
    Properties
    {
        _MainTex("", 2D) = "white" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4 _Color;

    uint _Seed;
    float _LocalTime;

    // Hash function from H. Schechter & R. Bridson, goo.gl/RXiKaH
    uint Hash(uint s)
    {
        s ^= 2747636419u;
        s *= 2654435769u;
        s ^= s >> 16;
        s *= 2654435769u;
        s ^= s >> 16;
        s *= 2654435769u;
        return s;
    }

    float Random(uint seed)
    {
        return float(Hash(seed)) / 4294967295.0; // 2^32-1
    }

    // Wiping animation
    fixed Wiper(float2 uv, float time, uint seed)
    {
        uint wave = floor(time) * 100 + seed * 10000;
        float param = frac(time);

        float y1 = smoothstep(Random(wave + 0) / 2, Random(wave + 1) / 2 + 0.5, param);
        float y2 = smoothstep(Random(wave + 2) / 2, Random(wave + 3) / 2 + 0.5, param);
        float y3 = smoothstep(Random(wave + 4) / 2, Random(wave + 5) / 2 + 0.5, param);

        float thresh = lerp(lerp(y1, y2, saturate(uv.x * 2)), y3, saturate(uv.x * 2 - 1));
        return frac(time / 2) < 0.5 ? uv.y < thresh : uv.y > thresh;
    }

    fixed4 frag(v2f_img i) : SV_Target
    {
        fixed4 src = tex2D(_MainTex, i.uv);
        return lerp(src, _Color, Wiper(i.uv, _LocalTime, _Seed));
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
            #pragma target 3.0
            ENDCG
        }
    }
}
