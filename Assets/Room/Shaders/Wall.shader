Shader "Room/Wall"
{
    Properties
    {
        // Base parameters
        _Color1("", Color) = (1, 1, 1, 1)
        _Color2("", Color) = (1, 1, 1, 1)
        [Gamma] _Metallic("", Range(0, 1)) = 0
        _Smoothness("", Range(0, 1)) = 0

        // Base maps
        _NormalMap("", 2D) = "bump" {}
        _OcclusionMap("", 2D) = "white" {}
        _OcclusionMapStrength("", Range(0, 1)) = 1

        // Detail maps
        _DetailNormalMap("", 2D) = "bump" {}
        _DetailNormalMapScale("", Range(0, 2)) = 1
        _DetailMapScale("", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma surface Surface Standard addshadow nolightmap nolppv
        #pragma multi_compile _MODE_DEFAULT _MODE_WAVE _MODE_STRIPE _MODE_SCROLL _MODE_RIPPLE _MODE_LIGHT _MODE_DOT
        #pragma target 3.0

        #include "Common.cginc"

        struct Input
        {
            float2 uv_NormalMap;
            float3 worldPos;
        };

        half3 _Color1;
        half3 _Color2;
        half _Metallic;
        half _Smoothness;

        sampler2D _NormalMap;
        sampler2D _OcclusionMap;
        half _OcclusionMapStrength;

        sampler2D _DetailNormalMap;
        half _DetailNormalMapScale;
        half _DetailMapScale;

        float4x4 _WorldToEffect;
        float _LocalTime;
        float _Threshold;
        float4 _Params;

    #if defined(_MODE_RIPPLE)

        float Ripple(float3 fx, float param, uint seed)
        {
            seed += floor(param) * 2;

            fx.x += lerp(-1.1, 1.1, Random(seed + 0));
            fx.y += lerp(-0.8, 0.8, Random(seed + 1));

            float t = frac(param);
            float d = distance(fx, 0) * 0.3;
            float p = saturate(d - t + 1) * saturate(1 - t) * (d - t < 0);
            return saturate(frac(d * 10) * p);
        }

    #elif defined(_MODE_LIGHT)

        #include "SimplexNoise2D.hlsl"

        float Light(float x, float offs)
        {
            float n = snoise(float2(x    , _LocalTime)) +
                      snoise(float2(x * 2, _LocalTime)) * 0.5;
            return abs(n) < _Threshold + offs;
        }

    #endif

        void Surface(Input IN, inout SurfaceOutputStandard o)
        {
            float3 fx = mul(_WorldToEffect, float4(IN.worldPos, 1)).xyz;

        #if defined(_MODE_WAVE)

            float p = frac(fx.z * _Params.x + sin(fx.y * _Params.y + _LocalTime));
            p = smoothstep(1, 1.01, 2 * abs(p - 0.5) + _Threshold);
            o.Albedo = lerp(_Color1, _Color2, p);

        #elif defined(_MODE_STRIPE)

            float p = frac(fx.z * _Params.x + _LocalTime);
            p = smoothstep(1, 1.01, 2 * abs(p - 0.5) + _Threshold);
            o.Albedo = lerp(_Color1, _Color2, p);

        #elif defined(_MODE_SCROLL)

            uint seed = floor(fx.y * _Params.x) + 10000;
            float offs = lerp(0.2, 1, Random(seed)) * (100 + _LocalTime);
            float p = frac(fx.x * _Params.y + offs);
            p = smoothstep(1, 1.05, 2 * abs(p - 0.5) + _Threshold);
            o.Albedo = lerp(_Color1, _Color2, p);

        #elif defined(_MODE_RIPPLE)

            float r1 = Ripple(fx, _Params.x, 18213);
            float r2 = Ripple(fx, _Params.y, 13284);
            float r3 = Ripple(fx, _Params.z, 11293);
            float p = max(r1, max(r2, r3));
            o.Albedo = lerp(_Color1, _Color2, abs(p - 0.5) < 0.25);

        #elif defined(_MODE_LIGHT)

            o.Albedo = _Color1;
            o.Emission = _Params.x * Light(fx.x     , _Params.z) +
                         _Params.y * Light(fx.x + 20, _Params.w) * _Color2;

        #elif defined(_MODE_DOT)

            float2 p = fx.xy * _Params.x + 1000;
            float r = (sin(dot(p * _Params.y, 1) + _LocalTime) + 1) / 4;
            float l = length(p - floor(p + 0.5));
            o.Albedo = lerp(_Color1, _Color2, l < r * _Params.z);

        #else // _MODE_DEFAULT

            o.Albedo = _Color1;

        #endif

            float2 uv1 = IN.uv_NormalMap;
            float2 uv2 = uv1 * _DetailMapScale;

            half4 n1 = tex2D(_NormalMap, uv1);
            half4 n2 = tex2D(_DetailNormalMap, uv2);
            o.Normal = BlendNormals(
                UnpackNormal(n1),
                UnpackScaleNormal(n2, _DetailNormalMapScale)
            );

            half occ = tex2D(_OcclusionMap, uv1).g;
            o.Occlusion = LerpOneTo(occ, _OcclusionMapStrength);

            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }

        ENDCG
    }

    CustomEditor "Room.WallMaterialInspector"
    FallBack "Diffuse"
}
