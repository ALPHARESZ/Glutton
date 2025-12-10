Shader "Custom/NeonTriangleLit"
{
    Properties
    {
        [Header(Colors)]
        [HDR] _CoreColor ("Core Color (Inside)", Color) = (1, 1, 1, 1)    // Putih Terang
        [HDR] _GlowColor ("Glow Color (Outside)", Color) = (0, 1, 1, 1)   // Cyan Neon
        
        [Header(Shape Settings)]
        _TriangleSize ("Size", Range(0.1, 0.8)) = 0.3
        _Rotation ("Rotation (Degrees)", Range(0, 360)) = 180 // Default 180 agar menghadap atas
        _OffsetY ("Vertical Offset", Range(-0.5, 0.5)) = 0.05
        
        [Header(Glow Settings)]
        _GlowIntensity ("Glow Power", Range(1, 10)) = 3.0
        _CoreSoftness ("Edge Softness", Range(0.001, 0.1)) = 0.01
        
        [Header(3D Shading)]
        _LightDir ("Light Direction", Vector) = (-0.5, 0.5, -1.0, 0)
        _Bevel ("Bevel Strength", Range(0.0, 1.0)) = 0.3
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            float4 _CoreColor;
            float4 _GlowColor;
            float _GlowIntensity;
            float _CoreSoftness;
            float _TriangleSize;
            float4 _LightDir;
            float _Bevel;
            float _Rotation;
            float _OffsetY;

            // Fungsi Rotasi
            float2 rotate2d(float2 v, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);
                return float2(v.x * c - v.y * s, v.x * s + v.y * c);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2.0 - 1.0; 
                o.color = v.color;
                return o;
            }

            // Fungsi SDF Segitiga
            float sdEquilateralTriangle(float2 p, float r)
            {
                const float k = sqrt(3.0);
                p.x = abs(p.x) - r;
                p.y = p.y + r/k;
                if(p.x + k*p.y > 0.0) p = float2(p.x - k*p.y, -k*p.x - p.y)/2.0;
                p.x -= clamp( p.x, -2.0*r, 0.0 );
                return -length(p)*sign(p.y);
            }

            // Normal untuk efek 3D
            float3 calcNormal(float2 p, float size)
            {
                float2 e = float2(0.001, 0.0);
                float dx = sdEquilateralTriangle(p + e.xy, size) - sdEquilateralTriangle(p - e.xy, size);
                float dy = sdEquilateralTriangle(p + e.yx, size) - sdEquilateralTriangle(p - e.yx, size);
                return normalize(float3(dx, dy, 0.1 + (1.0 - _Bevel))); 
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pos = i.uv;

                // 1. ATUR POSISI & ROTASI
                pos.y -= _OffsetY; 
                
                // Konversi rotasi agar 0/360 = Atas (secara visual)
                // Kita tambah offset 180 derajat karena rumus aslinya menghadap bawah
                float rad = (_Rotation + 180) * 0.0174533; 
                pos = rotate2d(pos, rad);

                // 2. HITUNG JARAK (SDF)
                float dist = sdEquilateralTriangle(pos, _TriangleSize);
                
                float3 finalColor = float3(0,0,0);
                float alpha = 0.0;

                // --- BAGIAN LUAR (GLOW CYAN) ---
                if (dist > 0.0)
                {
                    // Gunakan _GlowColor (Cyan)
                    float glowFactor = exp(-dist * 12.0) * _GlowIntensity;
                    finalColor = _GlowColor.rgb * glowFactor;
                    alpha = glowFactor;
                }
                // --- BAGIAN DALAM (INTI PUTIH) ---
                else
                {
                    // Hitung Shading 3D
                    float3 normal = calcNormal(pos, _TriangleSize);
                    float3 lightDir = normalize(_LightDir.xyz);
                    
                    // Diffuse & Specular
                    float diff = max(0.0, dot(normal, lightDir));
                    float3 viewDir = float3(0,0,-1);
                    float3 reflectDir = reflect(-lightDir, normal);
                    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0);

                    // Dasar warna putih (_CoreColor) dicampur sedikit dengan Cyan (_GlowColor)
                    // Agar terlihat menyatu, shading gelapnya diberi tint Cyan.
                    float3 ambient = _GlowColor.rgb * 0.2; 
                    
                    // Warna Inti = Putih * Cahaya
                    float3 lighting = (_CoreColor.rgb * (ambient + diff * 0.8)) + (spec * 0.5);
                    
                    finalColor = lighting * 1.5; // Overbright biar silau
                    alpha = 1.0 - smoothstep(0.0, _CoreSoftness, dist);
                }

                return float4(finalColor, saturate(alpha) * i.color.a);
            }
            ENDCG
        }
    }
}