Shader "Nikita/Builtin/SmokyFog"
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
                //UNITY_FOG_COORDS(1);
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float Map(float3 p)
            {
                float d = length(p) - 0.5;
                return d;
            }

            float3 getNormal(float3 p)
            {
                float2 d = float2(0.01, 0.0);
                float gx = Map(p + d.xyy) - Map(p - d.xyy);
                float gy = Map(p + d.yxy) - Map(p - d.yxy);
                float gz = Map(p + d.yyx) - Map(p - d.yxx);
                float3 normal = float3(gx, gy, gz);
                return normalize(normal);
            }
                
            float Raymarch(float3 rO, float3 rD)
            {
                float dO = 0;
                float dS;
                for (int i = 0; i < MAX_STEPS; i++)
                {
                    float3 p = rO + dO * rD;
                    dS = Map(p);
                    dO += dS;

                    // Lighting
                    float3 normal = getNormal(p);
                    float3 lightColor = float3(1., 1., 1.);
                    float3 lightSource = float3(2.5, 2.5, 1.);
                    float diffuseStrength = max(0.0, dot(normalize(lightSource), 
                    normal));
                    float3 diffuse = lightColor * diffuseStrength;
        
                    float3 viewSource = normalize(rO);
                    float3 reflectSource = normalize(reflect(-lightSource, normal));
                    float specularStrength = max(0.0, dot(viewSource, reflectSource));
                    specularStrength = pow(specularStrength, 64.);
                    float3 specular = specularStrength * lightColor;
        
                    float3 lighting = diffuse * 0.75 + specular * 0.25;
                    dO = lighting.rgb;


                    if (dS < SURF_DIST || dO > MAX_DIST) break;
                }
                return dO;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5;
                float3 rO = float3(0, 0, -3);
                float3 rD = normalize(float3(uv.x, uv.y, 1));

                fixed4 col = 0;

                float dO = 0;
                float dS;
                for (int i = 0; i < MAX_STEPS; i++)
                {
                    float3 p = rO + dO * rD;
                    dS = Map(p);
                    dO += dS;

                    // Lighting
                    float3 normal = getNormal(p);
                    float3 lightColor = float3(1., 1., 1.);
                    float3 lightSource = float3(sin(_Time.y) * 2., 2.5, sin(_Time.y) * 3.);
                    float diffuseStrength = max(0.0, dot(normalize(lightSource), 
                    normal));
                    float3 diffuse = lightColor * diffuseStrength;
        
                    float3 viewSource = normalize(rO);
                    float3 reflectSource = normalize(reflect(-lightSource, normal));
                    float specularStrength = max(0.0, dot(viewSource, reflectSource));
                    specularStrength = pow(specularStrength, 64.);
                    float3 specular = specularStrength * lightColor;
        
                    float3 lighting = diffuse * 0.75 + specular * 0.25;
                    col.rgb = lighting.rgb;


                    if (dS < SURF_DIST || dO > MAX_DIST) break;
                }

                //float d = Raymarch(rO, rD);

                //col.rgb = Raymarch(rO, rD);
                return col;
            }
            ENDCG
        }
    }
}
