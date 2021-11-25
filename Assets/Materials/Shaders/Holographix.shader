Shader "Unlit/Holographix"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,0.5)
        _Speed("Speed",Range(0.001,0.2)) = 0.1
        _Scale("Scale",Float) = 1
        _BigLen("Len of main thing %", Range(0.01,1.)) = 0.5
        _Count("Count of sub lines",Int) = 3
        _SmallLen("Len of small things %",Range(0.5,1.)) = 0.5
        _Condition("Bool Hard Edges",Range(0,1)) = 1
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent"
               "Queue" = "Transparent"}
        LOD 200
        Pass
        {
            Cull Off
            ZWrite Off
            BLEND ONE ONE

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            float4 _Color;
            float _Speed;
            float _Scale;
            float _BigLen;
            float _Count;
            float _SmallLen;
            float _Condition;


            struct meshData
            {
                float4 vertex : POSITION;
            };

            struct Interpolator
            {
                float4 vertex : SV_POSITION;
                float y : TEXTCOORD0;
                float cond : TEXTCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            Interpolator vert(meshData v)
            {

                Interpolator o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.y = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).y;
                o.cond = _Condition > 0;
                return o;
            }

            float4 frag(Interpolator i) : SV_Target
            {
                float x = (i.y + _Time.y * _Speed) * _Scale;
                float MainMask = x % 1 < _BigLen;
                float smallMask = ((x%1)% _BigLen) % (_SmallLen/ _Count);
                smallMask *= _Count / _SmallLen;
                smallMask = i.cond ? smallMask < _SmallLen : smallMask;
                return _Color * MainMask * smallMask;
            }
            ENDCG
        }
    }
    SubShader
        {
            Tags { "RenderType" = "Transparent"
                    "Queue" = "Transparent"}
            LOD 100
            Pass
            {
                Cull Off
                ZWrite Off
                BLEND ONE ONE

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"


                float4 _Color;
                float _Speed;
                float _Scale;
                float _BigLen;
                float _Count;
                float _SmallLen;
                float _Condition;

                struct meshData
                {
                    float4 vertex : POSITION;
                };

                struct Interpolator
                {
                    float4 vertex : SV_POSITION;
                    float y : TEXTCOORD0;
                    float cond : TEXTCOORD1;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;

                Interpolator vert(meshData v)
                {

                    Interpolator o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.y = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).y;
                    o.cond = _Condition > 0;
                    return o;
                }

                float4 frag(Interpolator i) : SV_Target
                {
                    float x = (i.y + _Time.y * _Speed) * _Scale;
                    float MainMask = x % 1 > ((1-_BigLen) * i.cond);
                    return _Color * MainMask ;
                }
                ENDCG
            }
        }
}
