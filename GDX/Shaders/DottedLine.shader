// This does not batch with the SRP batcher, it looks like there are ways to make that possible
// but they were specific to each RP
Shader "GDX/DottedLine"
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
        ZTest [_HandleZTest]
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            struct v2f
            {
               float4 position : SV_POSITION;
            };

            v2f vert (float4 vertex : POSITION)
            {
                v2f o;
                o.position = UnityObjectToClipPos(vertex);
                return o;
            }

            uniform fixed4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                float total = i.position.x + i.position.y + i.position.z;

                if (step(sin(total), 0.5f))
                        discard;

                return _Color;
            }

            ENDCG
        }
    }
}