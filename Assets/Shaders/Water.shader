Shader "Custom/Water"
{
    Properties
    {
        _Color("Colour", Color) = (0, 0, 0, 1)
        _Strength("Amplitude", Range(0,20)) = 0.1
        _Speed("Speed", Range(-200,200)) = 100
        _MainTex("Albedo (RGB)", 2D) = "white" {}
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
                vertexOutPut o;

                float4 worldPos = mul(unity_ObjectToWorld, IN.vertex);

                float displacement = (cos(worldPos.y) + cos(worldPos.x + (_Speed * _Time)));
                
                
                worldPos.y = worldPos.y + (displacement * _Strength);
                
                
                o.pos = mul(UNITY_MATRIX_VP, worldPos);
                return o;
            }

            float4 fragFunc(vertexOutPut IN) : COLOR
            {
                return _Color;
            }
                
            
            
            ENDCG
        }
    }
}
