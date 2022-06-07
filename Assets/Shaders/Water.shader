Shader "Custom/Water"
{
    Properties
    {
        _Color("Colour", Color) = (0, 0, 0, 1)
        _Strength("Amplitude", Range(0,5)) = 0.1
        _Speed("Speed", Range(-200,200)) = 100

    }
    SubShader
    {
        Tags{"RenderType" = "transparent"}
        Pass
        {
            Cull Off

            CGPROGRAM
            #pragma vertex vertexFunc
            #pragma fragment fragFunc
        
            float4 _Color;
            float _Strength;
            float _Speed;

            struct vextexInput
            {
                float4 vertex : POSITION;

            };

            struct vertexOutPut
            {
                float4 pos : SV_POSITION;
            };


            vertexOutPut vertexFunc(vextexInput IN)
            {
                vertexOutput o;
                float4 worldPos = mul(unity_ObjectToWorld, IN.vertex);
                float displacement = (cos(worldPos.y) + cos(worldPos.x + (_Speed)));
                return o;
            }

            float4 fragFunc(vertexOutput IN) : COLOR
            {
                return _Color
            }

            ENDCG


        }

        
       
        

    }
}
