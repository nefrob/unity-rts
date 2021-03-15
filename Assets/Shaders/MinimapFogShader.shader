Shader "Custom/MinimapFogShader"
{
    Properties {
        _FogColor ("Fog Color", Color) = (1,1,1,1)
        _VisionColor ("Vision Color", Color) = (1,1,1,1)
        _MinimapTex ("Minimap", 2D) = "white" {}
        _FogTex ("Fog", 2D) = "white" {} 
        _VisionTex ("Vision", 2D) = "white" {}
        _FogBlend ("Fog Blend", Range(0,1)) = 1
        _VisionBlend ("Vision Blend", Range(0,1)) = .95
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags { "Queue"="Transparent+100"  }
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MinimapTex;
        sampler2D _FogTex;
        sampler2D _VisionTex;

        struct Input {
            float2 uv_MinimapTex;
            float2 uv_FogTex;
            float2 uv_VisionTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _FogColor;
        fixed4 _VisionColor;
        float _FogBlend;
        float _VisionBlend;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D(_MinimapTex, IN.uv_MinimapTex);

            if (tex2D(_FogTex, IN.uv_FogTex).a == 0) {
                c = lerp(c, _FogColor, _FogBlend);
            }

            if (tex2D(_VisionTex, IN.uv_VisionTex).a == 0) {
                c = lerp(c, _VisionColor, _VisionBlend);
            }
            

            // fixed isFog = round(1 - tex2D(_FogTex, IN.uv_FogTex).a);
            // c = c * isFog * (1 - _FogBlend) + _FogColor * isFog * _FogBlend;

            // fixed isVision = round(1 - tex2D(_VisionTex, IN.uv_VisionTex).a);
            // fixed4 visionBlend = c * (1 - _VisionColor.a) + _VisionColor * _VisionColor.a;
            // c = c * (1 - isVision) + visionBlend * isVision;

            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
