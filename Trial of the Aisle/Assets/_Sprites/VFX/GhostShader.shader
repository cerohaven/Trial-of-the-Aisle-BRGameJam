Shader "Custom/GhostShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,0.5)
        _UseSingleColor ("Use Single Color", Float) = 0
        _PlayerPos ("Player Position", Vector) = (0,0,0)
        _MaxDistance ("Max Distance", Float) = 10.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _UseSingleColor;
            float3 _PlayerPos;
            float _MaxDistance;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                float dist = distance(i.worldPos, _PlayerPos);
                float fadeFactor = saturate(1.0 - (dist / _MaxDistance));
                fixed4 finalColor;

                if (_UseSingleColor == 1)
                {
                    finalColor = fixed4(_Color.rgb, texColor.a * fadeFactor);
                }
                else
                {
                    finalColor = texColor * _Color;
                    finalColor.a *= fadeFactor;
                }

                return finalColor;
            }
            ENDCG
        }
    }
}
