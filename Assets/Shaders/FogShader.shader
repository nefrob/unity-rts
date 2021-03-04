// Ref: https://pastebin.com/dYYYckCi
Shader "Custom/FogShader"
{
    Properties
    {
        _CurTex ("Current Texture", 2D) = "white" {}
        _PrevTex ("Previous Texture", 2D) = "white" {}
		_Color ("Color", Color) = (0, 0, 0, 0)
		_Blend("Blend", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent+100" } // to cover other transparent non-z-writes

        Pass
        {
            ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Equal

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4x4 unity_Projector;
			sampler2D _CurTex;
			sampler2D _PrevTex;
			fixed4 _Color;
			float _Blend;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = mul(unity_Projector, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float aPrev = tex2Dproj(_PrevTex, i.uv).a;
                float aCur = tex2Dproj(_CurTex, i.uv).a;

                fixed a = lerp(aPrev, aCur, _Blend);
				// Keep alpha positive
				_Color.a = max(0, _Color.a - a);

				return _Color;
            }
            ENDCG
        }
    }
}
