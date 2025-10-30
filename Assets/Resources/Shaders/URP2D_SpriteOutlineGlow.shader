Shader "Universal Render Pipeline/2D/Sprite Outline Glow (Custom)"
{
    Properties
    {
        _MainTex("Diffuse", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}

        // Compatibility properties (match Sprite-Lit-Default)
        [HideInInspector] _Color("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0

        // Outline controls
        [HDR]_OutlineColor("Outline Color", Color) = (1,0.85,0.4,1)
        _OutlineThickness("Outline Thickness (px)", Range(0,8)) = 1
        _OutlineSoftness("Outline Softness", Range(0,1)) = 0.3
        _GlowStrength("Glow Strength", Range(0,5)) = 1
        _AlphaClip("Alpha Clip Threshold", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragmentOutline

            // GPU Instancing / 2D light variants
            #pragma multi_compile_instancing
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __
            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                half4 color         : COLOR;
                float2 uv           : TEXCOORD0;
                half2 lightingUV    : TEXCOORD1;
#if defined(DEBUG_DISPLAY)
                float3 positionWS   : TEXCOORD2;
#endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/InputData2D.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

#if defined(ETC1_EXTERNAL_ALPHA)
            TEXTURE2D(_AlphaTex);
            SAMPLER(sampler_AlphaTex);
#endif

            half4 _MainTex_ST;
            float4 _Color;
            half4 _RendererColor;
            float _EnableExternalAlpha;
            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineThickness;
                float _OutlineSoftness;
                float _GlowStrength;
                float _AlphaClip;
            CBUFFER_END

#if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
#endif
#if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
#endif
#if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
#endif
#if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
#endif

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

#ifdef UNITY_INSTANCING_ENABLED
                v.positionOS = UnityFlipSprite(v.positionOS, unity_SpriteFlip);
#endif
                o.positionCS = TransformObjectToHClip(v.positionOS);
#if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(v.positionOS);
#endif
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);

                o.color = v.color * _Color * _RendererColor;
#ifdef UNITY_INSTANCING_ENABLED
                o.color *= unity_SpriteColor;
#endif
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            float ExternalAlpha(float2 uv)
            {
#if defined(ETC1_EXTERNAL_ALPHA)
                return SAMPLE_TEXTURE2D(_AlphaTex, sampler_AlphaTex, uv).r;
#else
                return 1.0;
#endif
            }

            float SampleSpriteAlpha(float2 uv, float colorAlpha)
            {
                float alpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
                if (_EnableExternalAlpha > 0.5f)
                {
                    alpha *= ExternalAlpha(uv);
                }
                return alpha * colorAlpha;
            }

            half4 CombinedShapeLightFragmentOutline(Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float4 texSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                if (_EnableExternalAlpha > 0.5f)
                {
                    texSample.a *= ExternalAlpha(i.uv);
                }

                float4 main = i.color * texSample;
                if (_AlphaClip > 0.0f)
                {
                    clip(main.a - _AlphaClip);
                }

                half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);

                SurfaceData2D surfaceData;
                InputData2D inputData;
                InitializeSurfaceData(main.rgb, main.a, mask, surfaceData);
                InitializeInputData(i.uv, i.lightingUV, inputData);

#if defined(DEBUG_DISPLAY)
                SETUP_DEBUG_DATA_2D(inputData, i.positionWS);
#endif

                half4 litColor = CombinedShapeLightShared(surfaceData, inputData);

                float2 texel = _MainTex_TexelSize.xy;
                float2 stepUV = texel * max(_OutlineThickness, 0.0);
                float colorAlpha = i.color.a;

                float aCenter = main.a;
                float a1 = SampleSpriteAlpha(i.uv + float2( stepUV.x, 0), colorAlpha);
                float a2 = SampleSpriteAlpha(i.uv + float2(-stepUV.x, 0), colorAlpha);
                float a3 = SampleSpriteAlpha(i.uv + float2(0,  stepUV.y), colorAlpha);
                float a4 = SampleSpriteAlpha(i.uv + float2(0, -stepUV.y), colorAlpha);
                float a5 = SampleSpriteAlpha(i.uv + float2( stepUV.x,  stepUV.y), colorAlpha);
                float a6 = SampleSpriteAlpha(i.uv + float2(-stepUV.x,  stepUV.y), colorAlpha);
                float a7 = SampleSpriteAlpha(i.uv + float2( stepUV.x, -stepUV.y), colorAlpha);
                float a8 = SampleSpriteAlpha(i.uv + float2(-stepUV.x, -stepUV.y), colorAlpha);

                float neighMax = max(max(max(a1, a2), max(a3, a4)), max(max(a5, a6), max(a7, a8)));
                float outlineMask = saturate(neighMax - aCenter);

                if (_OutlineSoftness > 0.0f)
                {
                    float2 stepUV2 = stepUV * 2.0f;
                    float b1 = SampleSpriteAlpha(i.uv + float2( stepUV2.x, 0), colorAlpha);
                    float b2 = SampleSpriteAlpha(i.uv + float2(-stepUV2.x, 0), colorAlpha);
                    float b3 = SampleSpriteAlpha(i.uv + float2(0,  stepUV2.y), colorAlpha);
                    float b4 = SampleSpriteAlpha(i.uv + float2(0, -stepUV2.y), colorAlpha);
                    float neighMax2 = max(max(b1, b2), max(b3, b4));
                    float outer = saturate(neighMax2 - aCenter);
                    outlineMask = lerp(outlineMask, outer, _OutlineSoftness);
                }

                half4 glow = _OutlineColor * (outlineMask * _GlowStrength);
                half3 finalRGB = litColor.rgb + glow.rgb;
                half finalA = saturate(max(litColor.a, glow.a));

                return half4(finalRGB, finalA);
            }
            ENDHLSL
        }

        // Original normals rendering pass (unchanged)
        Pass
        {
            Tags { "LightMode" = "NormalsRendering" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"

            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment
            #pragma multi_compile_instancing

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                float4 tangent      : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
                float3 tangentWS    : TEXCOORD2;
                float3 bitangentWS  : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            half4 _NormalMap_ST;

            Varyings NormalsRenderingVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

#ifdef UNITY_INSTANCING_ENABLED
                attributes.positionOS = UnityFlipSprite(attributes.positionOS, unity_SpriteFlip);
#endif
                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = TRANSFORM_TEX(attributes.uv, _NormalMap);
                o.color = attributes.color;
                o.normalWS = -GetViewForwardDir();
                o.tangentWS = TransformObjectToWorldDir(attributes.tangent.xyz);
                o.bitangentWS = cross(o.normalWS, o.tangentWS) * attributes.tangent.w;
#ifdef UNITY_INSTANCING_ENABLED
                o.color *= unity_SpriteColor;
#endif
                return o;
            }

            half4 NormalsRenderingFragment(Varyings i) : SV_Target
            {
                const half4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                const half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv));

                return NormalsRenderingShared(mainTex, normalTS, i.tangentWS.xyz, i.bitangentWS.xyz, i.normalWS.xyz);
            }
            ENDHLSL
        }

        // Original forward (debug) pass (unchanged)
        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment
            #pragma multi_compile_instancing

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
#if defined(DEBUG_DISPLAY)
                float3 positionWS   : TEXCOORD2;
#endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float4 _Color;
            half4 _RendererColor;

            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

#ifdef UNITY_INSTANCING_ENABLED
                attributes.positionOS = UnityFlipSprite(attributes.positionOS, unity_SpriteFlip);
#endif
                o.positionCS = TransformObjectToHClip(attributes.positionOS);
#if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(attributes.positionOS);
#endif
                o.uv = TRANSFORM_TEX(attributes.uv, _MainTex);
                o.color = attributes.color * _Color * _RendererColor;
#ifdef UNITY_INSTANCING_ENABLED
                o.color *= unity_SpriteColor;
#endif
                return o;
            }

            float4 UnlitFragment(Varyings i) : SV_Target
            {
                float4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

#if defined(DEBUG_DISPLAY)
                SurfaceData2D surfaceData;
                InputData2D inputData;
                half4 debugColor = 0;

                InitializeSurfaceData(mainTex.rgb, mainTex.a, surfaceData);
                InitializeInputData(i.uv, inputData);
                SETUP_DEBUG_DATA_2D(inputData, i.positionWS);

                if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                {
                    return debugColor;
                }
#endif

                return mainTex;
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
