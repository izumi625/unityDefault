Shader "Unlit/PointCloudShader"
{
    Properties
    {
        _pointSize ("Point Size", Float) = 0.01
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float3 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            float _pointSize;
            StructuredBuffer<float3> _points;

            v2f vert(uint id : SV_VertexID)
            {
                v2f o;
                float3 pos = _points[id];
                o.pos = UnityObjectToClipPos(float4(pos, 1.0));
                o.color = float4(1, 1, 1, 1); // îíêF
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
