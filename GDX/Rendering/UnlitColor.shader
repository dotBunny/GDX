// Based on https://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
// This does not batch with the SRP batcher, it looks like there are ways to make that possible
// but they were specific to each RP
Shader "GDX/UnlitColor"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        [Enum(UnityEngine.Rendering.CompareFunction)] _HandleZTest ("_HandleZTest", Int) = 8
    }
    SubShader
    {
        Tags { "ForceSupported" = "True" }
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        //ZTest [_HandleZTest]
        ZTest [_HandleZTest]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 vert (float4 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }

            uniform fixed4 _Color;

            fixed4 frag () : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}