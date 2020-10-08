Shader "MediVR/BorderRendering"
{
    Properties
    {
        _MainTex("Texture", 3D) = "white" {}

        _MainTex_TexelSize("Texel Size", Vector) = (0,0,0,0)

        _OutlineColor("Outline Color", Color) = (1,0,0,1)
        //_OutlineWidth("Outline Width", Range(0,0.1)) = 0.02

        //_StartCoords("Quad Start Coordinates", Vector) = (0,0,0) 
        //_MovingCoords("Quad Updated Coordinates", Vector) = (0,0,0) 

    }

        CGINCLUDE
        #include "UnityCG.cginc"

            float4  _OutlineColor;
            //float _OutlineWidth;

            //float3 _StartCoords;
            //float3 _MovingCoords;

            struct appdata
            {
                float4 vertex : POSITION;
                //float2 uv : TEXCOORD0;
                float3 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                //float2 uv : TEXCOORD0;
                float3 uv : TEXCOORD0;

                //float depth : TEXCOORD1;
                float4 color : COLOR;
            };

            sampler3D _MainTex;
            //float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

        ENDCG

    SubShader
    {
        Tags { "Queue"="Transparent"  "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        Pass
        {
            ZWrite On
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            v2f vert (appdata v)
            {
                //v.vertex.xyz *= _OutlineWidth;

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;// + float3(0,0,0.5);
                //o.uv = float2(_MovingCoords.x, _MovingCoords.y);
                //float3 updatedCoords = _StartCoords - _MovingCoords;
                //o.depth = - _MovingCoords.z;
                o.color = _OutlineColor;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //_uv = float3(i.uv, i.depth);
                //fixed4 col = tex3Dlod(_MainTex, float4(i.uv,i.depth,0));
                clip(i.uv);
                clip(1.0 - i.uv);

                half4 col = (0,0,0,0);

                if ((i.uv.x <= _MainTex_TexelSize.x || i.uv.y <= _MainTex_TexelSize.y || i.uv.x >= (1.0 - _MainTex_TexelSize.x) 
                        || i.uv.y >= (1.0 - _MainTex_TexelSize.y)))
                col = i.color;
                else
                discard;

                return col;
            }
            ENDCG
        }
    }
}
