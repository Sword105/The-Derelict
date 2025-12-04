Shader "Nikita/Builtin/DebugRaymarching" 
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define MAX_STEPS 100
            #define MAX_DIST 100
            #define SURF_DIST .001

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 rO : TEXCOORD1;
                float hitPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.rO = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
                o.hitPos = v.vertex;
                return o;
            }

            float Map(float3 p)
            {
                float d = length(p) - 0.5;
                return d;
            }

            float3 getNormal(float3 p)
            {
                float2 e = float2(0.01, 0.0);
                float n = Map(p) - float3(
                    Map(p-e.xyy),
                    Map(p-e.yxy),
                    Map(p-e.yyx)
                );
                return normalize(n);
            }
                
            

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5;
                //float3 rO = i.rO;
                //float3 rD = normalize(i.hitPos - i.rO);
                float3 rO = float3(0, 0, -3);
                float rD = normalize(float3(uv.x, uv.y, 1.));

                fixed4 col = 0;

                float dO = 0;
                float dS;
                for (int i = 0; i < MAX_STEPS; i++)
                {
                    float3 p = rO + dO * rD;
                    dS = Map(p);
                    dO += dS;

                    if (dS < SURF_DIST || dO > MAX_DIST) break;
                }

                //float d = Raymarch(rO, rD);
                if (dO < MAX_DIST) {
                    float3 p = rO + rD * dO;
                    float n = getNormal(p);
                    col.rgb = n;
                }
                return col;
            }
            ENDCG
        }
    }
}
