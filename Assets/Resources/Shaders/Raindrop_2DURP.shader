Shader "Custom/Raindrop2DURP"
{
    Properties
    {
        _BaseColor ("基础颜色", Color) = (0.8, 0.8, 1.0, 1.0)
        _RainSpeed ("雨滴下落速度", Range(0, 1)) = 0.5
        _DynamicDrops ("动态雨滴数量", Range(2, 5)) = 3
        _StaticDrops ("静态雨滴数量", Range(0.5, 5)) = 2
        _RefractionIntensity ("折射强度", Range(0, 0.1)) = 0.01
        _RaindropOpacity ("雨滴不透明度", Range(0, 1)) = 0.7
        [Space(20)]
        [KeywordEnum(Cheap, Expensive)] _NormalType ("法线类型", Float) = 0
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" "Queue" = "Transparent" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            // 修改为适合2D渲染的LightMode
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _NORMALTYPE_CHEAP _NORMALTYPE_EXPENSIVE

            // 使用更适合2D项目的包含文件
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _RainSpeed;
                float _DynamicDrops;
                float _StaticDrops;
                float _RefractionIntensity;
                float _RaindropOpacity;
            CBUFFER_END

            // Utility functions from original shader
            #define S(a, b, t) smoothstep(a, b, t)

            float3 N13(float p)
            {
                float3 p3 = frac(float3(p, p, p) * float3(0.1031, 0.11369, 0.13787));
                p3 += dot(p3, p3.yzx + 19.19);
                return frac(float3((p3.x + p3.y) * p3.z, (p3.x + p3.z) * p3.y, (p3.y + p3.z) * p3.x));
            }

            float N(float t)
            {
                return frac(sin(t * 12345.564) * 7658.76);
            }

            float Saw(float b, float t)
            {
                return S(0.0, b, t) * S(1.0, b, t);
            }

            float2 DropLayer2(float2 uv, float t)
            {
                float2 UV = uv;
                uv.y += t * 0.75;
                
                float2 a = float2(6.0, 1.0);
                float2 grid = a * 2.0;
                float2 id = floor(uv * grid);
                
                float colShift = N(id.x);
                uv.y += colShift;
                
                id = floor(uv * grid);
                float3 n = N13(id.x * 35.2 + id.y * 2376.1);
                float2 st = frac(uv * grid) - float2(0.5, 0.0);
                
                float x = n.x - 0.5;
                float y = UV.y * 20.0;
                float wiggle = sin(y + sin(y));
                x += wiggle * (0.5 - abs(x)) * (n.z - 0.5);
                x *= 0.7;
                
                float ti = frac(t + n.z);
                y = (Saw(0.85, ti) - 0.5) * 0.9 + 0.5;
                float2 p = float2(x, y);
                
                float d = length((st - p) * a.yx);
                float mainDrop = S(0.4, 0.0, d);
                
                float r = sqrt(S(1.0, y, st.y));
                float cd = abs(st.x - x);
                float trail = S(0.23 * r, 0.15 * r * r, cd);
                float trailFront = S(-0.02, 0.02, st.y - y);
                trail *= trailFront * r * r;
                
                y = UV.y;
                float trail2 = S(0.2 * r, 0.0, cd);
                float droplets = max(0.0, (sin(y * (1.0 - y) * 120.0) - st.y)) * trail2 * trailFront * n.z;
                
                y = frac(y * 10.0) + (st.y - 0.5);
                float dd = length(st - float2(x, y));
                droplets = S(0.3, 0.0, dd);
                float m = mainDrop + droplets * r * trailFront;
                
                return float2(m, trail);
            }

            float StaticDrops(float2 uv, float t)
            {
                uv *= 40.0;
                float2 id = floor(uv);
                uv = frac(uv) - 0.5;
                
                float3 n = N13(id.x * 107.45 + id.y * 3543.654);
                float2 p = (n.xy - 0.5) * 0.7;
                float d = length(uv - p);
                
                float fade = Saw(0.025, frac(t + n.z));
                float c = S(0.3, 0.0, d) * frac(n.z * 10.0) * fade;
                return c;
            }

            float2 Drops(float2 uv, float t, float l0, float l1, float l2)
            {
                float s = StaticDrops(uv, t) * l0;
                float2 m1 = DropLayer2(uv, t) * l1;
                float2 m2 = DropLayer2(uv * 1.85, t) * l2;
                
                float c = s + m1.x + m2.x;
                c = S(0.3, 1.0, c);
                
                return float2(c, max(m1.y * l0, m2.y * l1));
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Calculate time with speed adjustment
                float T = _Time.y * _RainSpeed;
                
                // Calculate rain amount based on parameters
                float rainAmount = _DynamicDrops * 0.2;
                
                // Calculate drop layers
                float staticDrops = S(-0.5, 1.0, rainAmount) * _StaticDrops;
                float layer1 = S(0.25, 0.75, rainAmount);
                float layer2 = S(0.0, 0.5, rainAmount);
                
                // Calculate drops
                float2 c = Drops(IN.uv, T, staticDrops, layer1, layer2);
                
                // Calculate normals
                float2 n;
                #if defined(_NORMALTYPE_CHEAP)
                    // Cheap normals (3x cheaper)
                    n = float2(ddx(c.x), ddy(c.x));
                #else
                    // Expensive normals (better quality)
                    float2 e = float2(0.001, 0.0);
                    float cx = Drops(IN.uv + e, T, staticDrops, layer1, layer2).x;
                    float cy = Drops(IN.uv + e.yx, T, staticDrops, layer1, layer2).x;
                    n = float2(cx - c.x, cy - c.x);
                #endif
                
                // Base transparency - mostly transparent except where there are raindrops
                float baseAlpha = 0.1; // Very transparent base
                
                // Raindrops add opacity
                float raindropAlpha = c.x * _RaindropOpacity;
                
                // Combine base and raindrop alpha
                float finalAlpha = saturate(baseAlpha + raindropAlpha);
                
                // Color - slight tint on raindrops
                half3 finalColor = _BaseColor.rgb;
                
                // Add some visual enhancement to make raindrops more visible
                finalColor = lerp(finalColor, finalColor * 1.2, c.x);
                
                // Apply refraction effect through UV distortion
                float2 distortedUV = IN.uv + n * _RefractionIntensity;
                
                // For a more visible effect, we adjust color based on normal direction
                finalColor += float3(n.x, n.y, 0) * 0.1 * c.x;
                
                return half4(finalColor, finalAlpha);
            }
            ENDHLSL
        }
    }
    
    FallBack "Universal Render Pipeline/Simple Lit"
}