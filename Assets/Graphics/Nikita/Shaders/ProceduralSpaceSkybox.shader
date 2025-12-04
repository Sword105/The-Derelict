Shader "Nikita/Builtin/Skybox_SpaceFull"
{
    Properties
    {
        _BgColor      ("Background", Color) = (0.01,0.01,0.02,1)
        _Exposure     ("Exposure", Range(0.1,5)) = 1.6

        _CellScale    ("Stars: Cell Scale", Range(40,600)) = 220
        _Density      ("Stars: Probability", Range(0,0.05)) = 0.012
        _SizePow      ("Stars: Sharpness", Range(1,8)) = 4
        _Intensity    ("Stars: Intensity", Range(0,5)) = 2.2
        _TwinkleSpeed ("Stars: Twinkle Speed", Range(0,5)) = 0.6
        _TwinkleAmt   ("Stars: Twinkle Amount", Range(0,1)) = 0.25

        _NebulaColorA ("Nebula Color A", Color) = (0.30, 0.55, 1.00, 1)
        _NebulaColorB ("Nebula Color B", Color) = (1.00, 0.35, 0.70, 1)
        _NebulaBlend  ("Nebula A↔B", Range(0,1)) = 0.5
        _NebulaIntensity ("Nebula Intensity", Range(0,2)) = 0.8
        _NebulaScale  ("Nebula Scale", Range(0.1,8)) = 1.6
        _NebulaContrast("Nebula Contrast", Range(0.1,3)) = 1.2
        _NebulaCut    ("Nebula Cutoff", Range(0,1)) = 0.22

        _BandColor    ("Band Color", Color) = (1.0,0.9,0.8,1)
        _BandIntensity("Band Intensity", Range(0,3)) = 0.8
        _BandSharpness("Band Sharpness", Range(0.5,8)) = 2.8
        _BandNoise    ("Band Noise", Range(0,1)) = 0.35
        _BandYaw      ("Band Yaw (deg)", Range(0,360)) = 0
        _BandPitch    ("Band Pitch (deg)", Range(-90,90)) = 12

        _PlanetTex        ("Planet Albedo (equirect)", 2D) = "gray" {}
        _PlanetNormalMap  ("Planet Normal Map", 2D) = "bump" {}
        _PlanetUseNormal  ("Use Normal Map (0/1)", Range(0,1)) = 0
        _PlanetDir        ("Planet Direction (xyz)", Vector) = (0,0,1,0)
        _PlanetRadiusDeg  ("Planet Radius (deg)", Range(0.1,70)) = 70
        _PlanetSoftDeg    ("Edge Softness (deg)", Range(0.0,10)) = 0.8
        _PlanetGlowDeg    ("Atmos Glow Radius (deg)", Range(0.1,60)) = 14
        _PlanetGlowInt    ("Atmos Glow Intensity", Range(0,5)) = 1.2
        _PlanetRotDeg     ("Texture Rotation (deg)", Range(0,360)) = 0

        _PlanetSpinSpeed  ("Spin Speed Z (deg/sec)", Range(-180,180)) = 3
        _PlanetSpinSpeedX ("Spin Speed X (deg/sec)", Range(-180,180)) = 0
        _PlanetSpinSpeedY ("Spin Speed Y (deg/sec)", Range(-180,180)) = 0

        _PlanetTiling     ("Texture Tiling (u,v)", Vector) = (1,1,0,0)

        _PlanetUVMode      ("Planet UV Mode (0=Equirect, 1=Triplanar)", Range(0,1)) = 0
        _TriplanarSharpness("Triplanar Sharpness", Range(1,8)) = 4

        _LightDir         ("Light Direction (xyz)", Vector) = (0.3, 0.4, 0.85, 0)
        _LightIntensity   ("Light Intensity", Range(0,4)) = 1.2
        _Ambient          ("Ambient", Range(0,1)) = 0.15
        _SpecularInt      ("Specular Intensity", Range(0,2)) = 0.25
        _SpecularPower    ("Specular Power", Range(1,128)) = 32

        _Rotation     ("Sky Rotation (deg)", Range(0,360)) = 0
    }

    SubShader
    {
        Tags{ "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off
        ZWrite Off
        ZTest LEqual

        Pass
        {
            CGPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnityCG.cginc"

            fixed4 _BgColor; float _Exposure;
            float _CellScale,_Density,_SizePow,_Intensity,_TwinkleSpeed,_TwinkleAmt;
            float4 _NebulaColorA,_NebulaColorB;
            float _NebulaBlend,_NebulaIntensity,_NebulaScale,_NebulaContrast,_NebulaCut;
            float4 _BandColor; float _BandIntensity,_BandSharpness,_BandNoise,_BandYaw,_BandPitch;

            sampler2D _PlanetTex;
            sampler2D _PlanetNormalMap;
            float _PlanetUseNormal;
            float3 _PlanetDir;
            float _PlanetRadiusDeg, _PlanetSoftDeg, _PlanetGlowDeg, _PlanetGlowInt;
            float _PlanetRotDeg, _PlanetSpinSpeed, _PlanetSpinSpeedX, _PlanetSpinSpeedY;
            float4 _PlanetTiling;
            float _PlanetUVMode, _TriplanarSharpness;
            float3 _LightDir; float _LightIntensity, _Ambient, _SpecularInt, _SpecularPower;
            float _Rotation;

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; float3 dir : TEXCOORD0; };

            float3 rotY(float3 v, float deg){ float a=radians(deg); float s=sin(a), c=cos(a); return float3(c*v.x+s*v.z, v.y, -s*v.x+c*v.z); }
            float3 rotX(float3 v, float deg){ float a=radians(deg); float s=sin(a), c=cos(a); return float3(v.x, c*v.y - s*v.z, s*v.y + c*v.z); }

            float hash31(float3 p){ return frac(sin(dot(p, float3(12.9898,78.233,37.719))) * 43758.5453); }
            float noise3D(float3 p){
                float3 i=floor(p), f=frac(p), u=f*f*(3-2*f);
                float n000=hash31(i+float3(0,0,0)), n100=hash31(i+float3(1,0,0));
                float n010=hash31(i+float3(0,1,0)), n110=hash31(i+float3(1,1,0));
                float n001=hash31(i+float3(0,0,1)), n101=hash31(i+float3(1,0,1));
                float n011=hash31(i+float3(0,1,1)), n111=hash31(i+float3(1,1,1));
                float nx00=lerp(n000,n100,u.x), nx10=lerp(n010,n110,u.x);
                float nx01=lerp(n001,n101,u.x), nx11=lerp(n011,n111,u.x);
                float nxy0=lerp(nx00,nx10,u.y), nxy1=lerp(nx01,nx11,u.y);
                return lerp(nxy0,nxy1,u.z);
            }
            float fbm(float3 p){ float a=0.5, f=0; [unroll] for(int i=0;i<5;i++){ f+=a*noise3D(p); p*=2.02; a*=0.5; } return f; }

            float starsSparse(float3 dir){
                float3 cell = floor(dir * _CellScale);
                float h = hash31(cell);
                float m = step(1.0 - _Density, h);
                float star = m * pow(max(h, 1e-4), _SizePow);
                float tw = sin(dot(cell, float3(0.7,1.3,2.1)) + _Time.y * _TwinkleSpeed) * 0.5 + 0.5;
                star *= lerp(1.0, 1.0 + _TwinkleAmt, tw);
                return star * _Intensity;
            }
            float3 nebula(float3 dir){
                float3 p = dir * _NebulaScale * 4.0;
                float w = fbm(p + 3.7);
                float n = fbm(p + w * 2.0);
                n = pow(saturate(n * _NebulaContrast - _NebulaCut), 1.35);
                float3 colA = _NebulaColorA.rgb, colB = _NebulaColorB.rgb;
                float t = saturate(0.5 + 0.5*dir.x);
                return lerp(colA, colB, lerp(t, 1.0 - t, _NebulaBlend)) * n * _NebulaIntensity;
            }
            float3 galaxyBand(float3 dir){
                float3 d = rotX(rotY(dir, _BandYaw), _BandPitch);
                float m = pow(saturate(1.0 - abs(d.y)), _BandSharpness);
                m = saturate(m + (noise3D(dir * 20.0) * _BandNoise) - 0.1);
                return _BandColor.rgb * m * _BandIntensity;
            }

            float3 rotateAroundAxis(float3 v, float3 n, float ang){
                float s=sin(ang), c=cos(ang);
                return v*c + cross(n,v)*s + n*dot(n,v)*(1.0-c);
            }
            void basisFromDir(in float3 n, out float3 T, out float3 B){
                float3 up = (abs(n.y) < 0.999) ? float3(0,1,0) : float3(1,0,0);
                T = normalize(cross(up, n));
                B = cross(n, T);
            }

            struct PlanetSample { float3 color; float disk; float glow; };

            PlanetSample samplePlanet(float3 dir)
            {
                PlanetSample ps; ps.color=0; ps.disk=0; ps.glow=0;

                float3 Nbase = normalize(rotY(_PlanetDir, _Rotation));

                float dDotBase = dot(normalize(dir), Nbase);
                float r = radians(_PlanetRadiusDeg);
                float s = radians(max(_PlanetSoftDeg, 1e-4));
                float cIn  = cos(max(r - s, 0.0));
                float cOut = cos(r + s);
                float disk = smoothstep(cOut, cIn, dDotBase);

                float rg   = radians(max(_PlanetGlowDeg, _PlanetRadiusDeg + _PlanetSoftDeg + 0.01));
                float cgIn = cos(rg);
                float cgOut= cos(rg + max(s*0.75, 0.01));
                float glow = saturate(smoothstep(cgOut, cgIn, dDotBase)) * _PlanetGlowInt;

                if (disk <= 0.0) { ps.glow = glow; return ps; }

                float3 T,B; basisFromDir(Nbase, T, B);

                float lx = dot(dir, T);
                float ly = dot(dir, B);
                float lz = dot(dir, Nbase);
                float3 L = normalize(float3(lx, ly, lz));

                float timeSec = _Time.y;

                float angX = radians(_PlanetSpinSpeedX) * timeSec;
                float angY = radians(_PlanetSpinSpeedY) * timeSec;
                float angZ = radians(_PlanetSpinSpeed)  * timeSec + radians(_PlanetRotDeg);

                L = rotateAroundAxis(L, T, angX);
                L = rotateAroundAxis(L, B, angY);
                L = rotateAroundAxis(L, Nbase, angZ);

                float rlx = L.x;
                float rly = L.y;
                float rlz = L.z;

                float3 albedo;

                if (_PlanetUVMode < 0.5)
                {
                    float theta = atan2(rlx, rlz);
                    float phi   = asin(clamp(rly, -1.0, 1.0));
                    float2 uv;
                    uv.x = theta * (1.0/6.2831853) + 0.5;
                    uv.y = phi   * (1.0/3.1415927) + 0.5;
                    uv *= _PlanetTiling.xy; uv = frac(uv);
                    albedo = tex2D(_PlanetTex, uv).rgb;
                }
                else
                {
                    float3 w = pow(abs(L), _TriplanarSharpness); w /= max(w.x+w.y+w.z,1e-4);
                    float2 scale = _PlanetTiling.xy * 0.5;

                    float2 uvN = frac(float2( rlx,  rly) * scale + 0.5);
                    float2 uvT = frac(float2( rlz,  rly) * scale + 0.5);
                    float2 uvB = frac(float2( rlx,  rlz) * scale + 0.5);

                    float3 cN = tex2D(_PlanetTex, uvN).rgb;
                    float3 cT = tex2D(_PlanetTex, uvT).rgb;
                    float3 cB = tex2D(_PlanetTex, uvB).rgb;

                    albedo = cT * w.x + cB * w.y + cN * w.z;
                }

                float3 Ldir = normalize(_LightDir);
                float3 V = normalize(-dir);
                float3 H = normalize(Ldir + V);
                float lambert = max(dot(L, Ldir), 0.0);
                float spec = pow(saturate(dot(L, H)), _SpecularPower) * _SpecularInt;
                float3 lit = albedo * (lambert * _LightIntensity + _Ambient) + spec;
                float limb = saturate((dDotBase - cos(r)) / max(1e-4, (1 - cos(r))));
                lit += pow(1.0 - limb, 1.5) * 0.08;
                ps.color = saturate(lit);

                ps.disk = disk;
                ps.glow = glow * (1.0 - disk);
                return ps;
            }

            v2f Vert(appdata v){
                v2f o; o.pos = UnityObjectToClipPos(v.vertex);
                float3 dir = normalize(v.vertex.xyz);
                dir = rotY(dir, _Rotation);
                o.dir = dir;
                return o;
            }

            fixed4 Frag(v2f i) : SV_Target
            {
                float3 dir = normalize(i.dir);
                float3 col = _BgColor.rgb;
                col += nebula(dir);
                col += galaxyBand(dir);
                col += starsSparse(dir);

                PlanetSample ps = samplePlanet(dir);
                col = lerp(col, ps.color, ps.disk);
                col += ps.glow * float3(0.6,0.7,1.0) * 0.5;

                col *= _Exposure;
                return float4(saturate(col), 1);
            }
            ENDCG
        }
    }
    Fallback Off
}