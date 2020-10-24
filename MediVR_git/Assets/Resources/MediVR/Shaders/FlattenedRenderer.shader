Shader "MediVR/FlattenedRendering"
{
    Properties
    {
        _MainTex("Texture", 3D) = "white" {}

        _MainTex_TexelSize("Texel Size", Vector) = (0,0,0,0)

        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline Width", Range(0,0.1)) = 0.02

        [HideInInspector]
        _WindowWidthMin("Window Width Minimum", Int) = 1
        [HideInInspector]
        _WindowWidthMax("Window Width Maximum", Int) = 4095
        _WindowWidth("Window Width", Range(1, 4095)) = 350
        [HideInInspector]
        _WindowCenterMin("Window Center Minimum", Int) = -1024
        [HideInInspector]
        _WindowCenterMax("Window Center Maximum", Int) = 3071
        _WindowCenter("Window Center", Range(-1024, 3071)) = 50

        [Toggle]
        _CutBlackPixelsBefore("Cut Black Background", Int) = 1
        [Toggle]
        _CutBlackPixelsAfter("Cut Black Foreground", Int) = 0
        _CutLowerBorder("Black Cutoff level", Range(0,1)) = 0.05
        [Toggle]
        _CutWhitePixelsBefore("Cut White Background", Int) = 0
        [Toggle]
        _CutWhitePixelsAfter("Cut White Foreground", Int) = 0
        _CutUpperBorder("White Cutoff level", Range(0,1)) = 0.95
       
        //_WindowMax("Window Max", Int) = 225
        //_WindowMin("Window Min", Int) = -125
        
        [HideInInspector]
        _EnlargeMin("Enlargement Minimum", Float) = 1
        [HideInInspector]
        _EnlargeMax("Enlargement Maximum", Float) = 10
        _Enlarge("Enlargement", Range(1, 10)) = 1.8

        [HideInInspector]
        _ContrastMin("Contrast Minimum", Float) = 0
        [HideInInspector]
        _ContrastMax("Contrast Maximum", Float) = 3
        [HideInInspector] // Uncomment for Use
        _Contrast("Contrast", Range(0,3)) = 1

        [HideInInspector]
        _BrightnessMin("Brightness Minimum", Float) = -1
        [HideInInspector]
        _BrightnessMax("Brightness Maximum", Float) = 1
        [HideInInspector] // Uncomment for Use
        _Brightness("Brightness", Range(-1,1)) = 0

        [HideInInspector]
        _ThresholdMin("Threshold Minimum", Float) = 0
        [HideInInspector]
        _ThresholdMax("Threshold Maximum", Float) = 1
        [HideInInspector] // Uncomment for Use
        _Threshold("Threshold", Range(0,1)) = 0
        [HideInInspector] // Uncomment for Use
        _ThresholdInv("Inverted Threshold", Range(0,1)) = 1

        _StartCoords("Quad Start Coordinates", Vector) = (0,0,0) 
        //_MovingCoords("Quad Updated Coordinates", Vector) = (0,0,0) 

    }

        CGINCLUDE
        #include "UnityCG.cginc"

            float4  _OutlineColor;
            float _OutlineWidth;

            int _WindowWidth;
            int _WindowCenter;
            int _WindowMax;
            int _WindowMin;

            int _CutBlackPixelsBefore;
            int _CutBlackPixelsAfter;
            float _CutLowerBorder;

            int _CutWhitePixelsBefore;
            int _CutWhitePixelsAfter;
            float _CutUpperBorder;

            float _Enlarge;
            float _Brightness;
            float _Contrast;
            float _Threshold;
            float _ThresholdInv;

            float3 _StartCoords;
            //float3 _MovingCoords;

            
            

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
                o.uv = (v.uv -_StartCoords) / _Enlarge + float3(.5,.5,.5);
                //o.uv = float2(_MovingCoords.x, _MovingCoords.y);
                //float3 updatedCoords = _StartCoords - _MovingCoords;
                //o.depth = - _MovingCoords.z;
                //o.color = _OutlineColor;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //_uv = float3(i.uv, i.depth);
                //fixed4 col = tex3Dlod(_MainTex, float4(i.uv,i.depth,0));
                clip(i.uv);
                clip(1.0 - i.uv);

                float4 col = tex3D(_MainTex, i.uv);

                float4 newCol = col;

                //_CutUpperBorder = 1 - _CutLowerBorder;

                if(_CutBlackPixelsBefore == 1)
                {
                    if(newCol.r < _CutLowerBorder && newCol.g < _CutLowerBorder && newCol.b < _CutLowerBorder)
					discard;
                    
                }
                if(_CutWhitePixelsBefore == 1)
                {
                    if(newCol.r > _CutUpperBorder && newCol.g > _CutUpperBorder && newCol.b > _CutUpperBorder)
                    discard;
                }

                _WindowMax =  _WindowCenter + (_WindowWidth/2);
                _WindowMin =  _WindowCenter - (_WindowWidth/2);

                newCol = (newCol * 4095) - 1024;

                if(newCol.r > _WindowMax && newCol.g > _WindowMax && newCol.b > _WindowMax)
                {
                    newCol = _WindowMax;
                }
                else if(newCol.r < _WindowMin && newCol.g < _WindowMin && newCol.b < _WindowMin)
                {
                    newCol = _WindowMin;
                }
                else if((newCol.r > _WindowMin && newCol.r < _WindowMax) && (newCol.g > _WindowMin && newCol.g < _WindowMax) && (newCol.b > _WindowMin && newCol.b < _WindowMax))
                {
                    newCol = newCol;
                }

                newCol = (newCol - _WindowMin) / _WindowWidth;

                /*if(col.r < _Threshold && col.g < _Threshold && col.b < _Threshold)
					discard;

                if(col.r > _ThresholdInv && col.g > _ThresholdInv && col.b > _ThresholdInv)
                discard;*/

                //col*= _Contrast;
                //col += _Brightness;
                
                if(_CutBlackPixelsAfter == 1)
                {
                    if(newCol.r < _CutLowerBorder && newCol.g < _CutLowerBorder && newCol.b < _CutLowerBorder)
					discard;
                }
                if(_CutWhitePixelsAfter == 1)
                {
                    if(newCol.r > _CutUpperBorder && newCol.g > _CutUpperBorder && newCol.b > _CutUpperBorder)
                    discard;
                }

                //if (i.uv.x <= _MainTex_TexelSize.x || i.uv.y <= _MainTex_TexelSize.y || i.uv.x >= (1.0 - _MainTex_TexelSize.x) || i.uv.y >= (1.0 - _MainTex_TexelSize.y))
                //col = half4 (1,0,0,1);

                return newCol;
            }
            ENDCG
        }

        Pass
        {
            ZWrite Off
            //Cull Off

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
    }
}
