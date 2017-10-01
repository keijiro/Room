Shader "Room/Triplanar"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _Smoothness("Smoothness", Range(0, 1)) = 0
        [Gamma] _Metallic("Metallic", Range(0, 1)) = 0
        _NormalMap("Normal Map", 2D) = "bump" {}
        _NormalMapScale("Normal Map Scale", Range(0, 2)) = 1
        _TextureScale("Texture Scale", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma surface Surface Standard vertex:Vertex addshadow fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float3 localCoord;
            float3 localNormal;
        };

        half3 _Color;
        half _Smoothness;
        half _Metallic;
        sampler2D _NormalMap;
        half _NormalMapScale;
        half _TextureScale;

        void Vertex(inout appdata_full v, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);
            data.localCoord = v.vertex.xyz;
            data.localNormal = v.normal.xyz;
        }

        void Surface(Input IN, inout SurfaceOutputStandard o)
        {
            // Blend factor of triplanar mapping
            float3 bf = abs(IN.localNormal);
            bf /= dot(bf, 1);

            // Normal map
            float3 tc = IN.localCoord * _TextureScale;
            half4 nml = tex2D(_NormalMap, tc.yz) * bf.x +
                        tex2D(_NormalMap, tc.zx) * bf.y +
                        tex2D(_NormalMap, tc.xy) * bf.z;
            o.Normal = UnpackScaleNormal(nml, _NormalMapScale);

            // Etc.
            o.Albedo = _Color;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
        }

        ENDCG
    }

    FallBack "Diffuse"
    CustomEditor "Room.TriplanarMaterialInspector"
}
