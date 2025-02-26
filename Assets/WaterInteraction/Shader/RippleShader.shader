Shader "Unlit/RippleShader"
{
    Properties
    {
        
    }
    SubShader
    {
        Tags {"RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" "RenderType"="Opaque"}
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 texcoord     : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_PrevRT);     SAMPLER(sampler_PrevRT);
            TEXTURE2D(_CurrentRT);  SAMPLER(sampler_CurrentRT);
            CBUFFER_START(UnityPerMaterial)
                float4 _CurrentRT_TexelSize;
            CBUFFER_END

            Varyings vert (Attributes v)
            {
                Varyings o=(Varyings)0;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.texcoord;
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float3 e = float3(_CurrentRT_TexelSize.xy,0);
                float2 uv = i.uv;
                float speed = 1.0f;

                float p10 = SAMPLE_TEXTURE2D(_CurrentRT, sampler_CurrentRT , uv - e.zy * speed).x;
                float p01 = SAMPLE_TEXTURE2D(_CurrentRT, sampler_CurrentRT , uv - e.xz * speed).x;
                float p21 = SAMPLE_TEXTURE2D(_CurrentRT, sampler_CurrentRT , uv + e.xz * speed).x;
                float p12 = SAMPLE_TEXTURE2D(_CurrentRT, sampler_CurrentRT , uv + e.zy * speed).x;

                float p11 = SAMPLE_TEXTURE2D(_PrevRT, sampler_PrevRT , uv).x;

                float d = (p10 + p01 + p21 + p12)/2 - p11;
                d *= 0.99f;
                return d;
            }
            ENDHLSL
        }
    }
}
