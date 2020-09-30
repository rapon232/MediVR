Shader "MediVR/FlattenedRendering"
{
    Properties
    {
        _MainTex("Texture", 3D) = "white" {}

        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline Width", Range(0,0.1)) = 0.02

        _Enlarge("Enlargement", Range(1,10)) = 1.8
        _Contrast("Contrast", Range(0,3)) = 1
        _Brightness("Brightness", Range(-1,1)) = 0
        _Threshold("Threshold", Range(0,1)) = 0
        _ThresholdInv("Inverted Threshold", Range(0,1)) = 1

        _StartCoords("Quad Start Coordinates", Vector) = (0,0,0) 
        _MovingCoords("Quad Updated Coordinates", Vector) = (0,0,0) 

    }

        CGINCLUDE
        #include "UnityCG.cginc"

            float4  _OutlineColor;
            float _OutlineWidth;

            float _Enlarge;
            float _Brightness;
            float _Contrast;
            float _Threshold;
            float _ThresholdInv;

            float3 _StartCoords;
            float3 _MovingCoords;

            struct appdata
            {
                float4 vertex : POSITION;
                //float2 uv : TEXCOORD0;
                float3 uv : TEXCOORD0;

                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                //float2 uv : TEXCOORD0;
                float3 uv : TEXCOORD0;

                //float depth : TEXCOORD1;

                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            sampler3D _MainTex;
            //float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

        ENDCG

    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 200

        Pass
        {
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            //#include "UnityCG.cginc"

            v2f vert (appdata v)
            {
                //v.vertex.xyz *= _OutlineWidth;

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.vertex + float3(.5,.5,.5);//(v.uv -_StartCoords) / _Enlarge + float3(.5,.5,.5);
                //o.uv = float2(_MovingCoords.x, _MovingCoords.y);
                //float3 updatedCoords = _StartCoords - _MovingCoords;
                //o.depth = - _MovingCoords.z;
                o.color = _OutlineColor;
                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
                fixed4 col = (0,0,0,1);

                if (i.uv.x <= _OutlineWidth || i.uv.y <= _OutlineWidth || i.uv.x >= (1.0 - _OutlineWidth) || i.uv.y >= (1.0 - _OutlineWidth))
                col = i.color;//half4 (1,0,0,1);

                else
                discard;

                return col;
            }
            ENDCG
        }
    
        //Tags { "RenderType"="Opaque" }
        //LOD 200

        Pass
        {
            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            v2f vert (appdata v)
            {
                //v.vertex.xyz *= _OutlineWidth;

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = (v.uv -_StartCoords) / _Enlarge + float3(.5,.5,.5);
                //o.uv = float2(_MovingCoords.x, _MovingCoords.y);
                //float3 updatedCoords = _StartCoords - _MovingCoords;
                //o.depth = - _MovingCoords.z;
                //o.color = _OutlineColor;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //_uv = float3(i.uv, i.depth);
                //fixed4 col = tex3Dlod(_MainTex, float4(i.uv,i.depth,0));
                clip(i.uv);
                clip(1.0 - i.uv);

                fixed4 col = tex3D(_MainTex, i.uv);

                if(col.r < _Threshold && col.g < _Threshold && col.b < _Threshold)
					discard;

                if(col.r > _ThresholdInv && col.g > _ThresholdInv && col.b > _ThresholdInv)
                discard;

                if(col.r < 0.08 && col.g < 0.08 && col.b < 0.08)
					discard;

                col*= _Contrast;
                col += _Brightness;
                
                if(col.r < 0.08 && col.g < 0.08 && col.b < 0.08)
					discard;

                //if (i.uv.x <= _MainTex_TexelSize.x || i.uv.y <= _MainTex_TexelSize.y || i.uv.x >= (1.0 - _MainTex_TexelSize.x) || i.uv.y >= (1.0 - _MainTex_TexelSize.y))
                //col = half4 (1,0,0,1);

                return col;
            }
            ENDCG
        }
    }
}
