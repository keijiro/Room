Shader "Room/Wall"
{
    Properties
    {
        // Render mode options
        [KeywordEnum(Default, Wave)] 
        _Mode("", Float) = 0

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
        Cull Off

        CGPROGRAM

        #pragma surface Surface Standard addshadow nolightmap exclude_path:forward
        #pragma multi_compile _MODE_DEFAULT _MODE_WAVE
        #pragma target 3.0

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

        half3 _PrimaryAxis;
        half3 _SecondaryAxis;

        float _LocalTime;
        float _Threshold;
        float _Param1;
        float _Param2;

        void Surface(Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv1 = IN.uv_NormalMap;
            float2 uv2 = uv1 * _DetailMapScale;

#if defined(_MODE_WAVE)
            float wx = dot(IN.worldPos, _PrimaryAxis);
            float wy = dot(IN.worldPos, _SecondaryAxis);
            float pt = frac(wx * _Param1 + sin(wy * _Param2 + _LocalTime));
            pt = smoothstep(1, 1.02, 2 * abs(pt - 0.5) + _Threshold);
            o.Albedo = lerp(_Color1, _Color2, pt);
#else
            o.Albedo = _Color1;
#endif
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;

            half4 n1 = tex2D(_NormalMap, uv1);
            half4 n2 = tex2D(_DetailNormalMap, uv2);
            o.Normal = BlendNormals(
                UnpackNormal(n1),
                UnpackScaleNormal(n2, _DetailNormalMapScale)
            );

            half occ = tex2D(_OcclusionMap, uv1).g;
            o.Occlusion = LerpOneTo(occ, _OcclusionMapStrength);
        }

        ENDCG
    }

    CustomEditor "Room.WallMaterialInspector"
    FallBack "Diffuse"
}
