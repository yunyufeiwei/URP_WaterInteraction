Shader "Unlit/Add"
{
    Properties
    {
        
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" "RenderType"="Opaque"  }
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
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                
            };

            TEXTURE2D(_ObjectsRT);SAMPLER(sampler_ObjectsRT);
            TEXTURE2D(_CurrentRT);SAMPLER(sampler_CurrentRT);
            CBUFFER_START(UnityPerMaterial)
                float4 _ObjectsRT_ST;
                float4 _CurrentRT_ST;
            CBUFFER_END
            
            Varyings vert (Attributes v)
            {
                Varyings o=(Varyings)0;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.texcoord, _ObjectsRT);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half4 tex1 = SAMPLE_TEXTURE2D(_ObjectsRT, sampler_ObjectsRT , i.uv);
                half4 tex2 = SAMPLE_TEXTURE2D(_CurrentRT, sampler_CurrentRT , i.uv);
                return tex1 + tex2;
            }
            ENDHLSL
        }
    }
}
