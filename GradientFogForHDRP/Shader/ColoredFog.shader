Shader "Renderers/GradientFog"
{
    Properties
    {
        _FogIntensity("Fog intensity", float) = 1
        _FogMinimumIntensity("Fog minimum intensity",  Range(0.0, 1.0)) = 0
        _FogStart("Fog start", float) = 0
        _FogEnd("Fog end", float) = 100
        _FogGradient("Fog gradient", 2D) = "white" {}
        _AlphaCutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0

       [HideInInspector] _NextFogGradient("DO NOT MODIDY", 2D) = "white" {}
       [HideInInspector] _LerpCursor("DO NOT MODIDY", float) = -1
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    #pragma multi_compile_instancing
    #pragma multi_compile _ DOTS_INSTANCING_ON

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "FirstPass"
            Tags { "LightMode" = "FirstPass" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual

            Cull Back

            HLSLPROGRAM

            #define _ALPHATEST_ON

            #define _SURFACE_TYPE_TRANSPARENT
            
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT

            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassRenderers.hlsl"

            float _FogIntensity;
            float _FogMinimumIntensity;
            sampler2D _FogGradient;
            float _FogStart;
            float _FogEnd;

            sampler2D _NextFogGradient;
            float _LerpCursor;

            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 viewDirection, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
            {
                float opacity = 0;
                float3 color = float3(1, 1, 1);
                float vertexDistance = posInput.linearDepth;

                if (vertexDistance > _FogStart)
                {
                    float fogCursor = (vertexDistance - _FogStart) / _FogEnd;
                    fogCursor = clamp(fogCursor, 0, 0.99);
                    float4 gradientColor = tex2D(_FogGradient, float2(fogCursor, 0));
                    float4 nextGradientColor = tex2D(_NextFogGradient, float2(fogCursor, 0));
                    float4 lerpGradientColor = _LerpCursor < 0 ? gradientColor : lerp(gradientColor, nextGradientColor, _LerpCursor);
                    color = lerpGradientColor.rgb;
                    opacity = lerpGradientColor.a * _FogIntensity;
                    opacity = max(_FogMinimumIntensity, opacity);
                }


#ifdef _ALPHATEST_ON
                DoAlphaTest(opacity, _AlphaCutoff);
#endif

                ZERO_BUILTIN_INITIALIZE(builtinData);
                ZERO_INITIALIZE(SurfaceData, surfaceData);
                surfaceData.color = color;
                builtinData.opacity = opacity;
                builtinData.emissiveColor = float3(0, 0, 0);
            }

            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassForwardUnlit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            ENDHLSL
        }
    }
}
