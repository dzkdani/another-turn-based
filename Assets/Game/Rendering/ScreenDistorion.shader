Shader "Custom/ScreenDistortion"
{
    Properties
    {
        _NoiseTex ("Noise Texture", 2D) = "gray" {}

        _Intensity ("Intensity", Range(0,1)) = 0
        _NoiseScale ("Noise Scale", Float) = 5
        _Speed ("Speed", Float) = 1
        _DistortionStrength ("Distortion Strength", Float) = 0.03
    }

    HLSLINCLUDE

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    TEXTURE2D(_NoiseTex);
    SAMPLER(sampler_NoiseTex);

    float _Intensity;
    float _NoiseScale;
    float _Speed;
    float _DistortionStrength;

    float4 Frag(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        float2 uv = input.texcoord;

        //-----------------------------------
        // Animated Noise
        //-----------------------------------

        float2 noiseUV =
            uv * _NoiseScale +
            _Time.y * _Speed;

        float2 noise =
            SAMPLE_TEXTURE2D(
                _NoiseTex,
                sampler_NoiseTex,
                noiseUV).rg;

        noise = noise * 2.0 - 1.0;

        //-----------------------------------
        // Distort UV
        //-----------------------------------

        uv +=
            noise *
            (_Intensity * _DistortionStrength);

        //-----------------------------------
        // Sample screen
        //-----------------------------------

        return SAMPLE_TEXTURE2D_X(
            _BlitTexture,
            sampler_LinearClamp,
            uv);
    }

    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
        }

        ZWrite Off
        Cull Off
        ZTest Always

        Pass
        {
            Name "ScreenDistortion"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            ENDHLSL
        }
    }
}