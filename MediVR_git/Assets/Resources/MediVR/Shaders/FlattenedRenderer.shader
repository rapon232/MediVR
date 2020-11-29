

//    MediVR, a medical Virtual Reality application for exploring 3D medical datasets on the Oculus Quest.

//   Copyright (C) 2020  Dimitar Tahov

//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    This shader serves to render the slicing frame at runtime.


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
        _WindowWidth("Window Width", Range(1, 4095)) = 300
        [HideInInspector]
        _WindowCenterMin("Window Center Minimum", Int) = -1024
        [HideInInspector]
        _WindowCenterMax("Window Center Maximum", Int) = 3071
        _WindowCenter("Window Center", Range(-1024, 3071)) = 100

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

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;

                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 uv : TEXCOORD0;

                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            sampler3D _MainTex;
            float4 _MainTex_TexelSize;

        ENDCG

    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 200

        //RENDER TEXTURE SLICE
        Pass
        {
            ZWrite On
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = (v.uv -_StartCoords) / _Enlarge + float3(.5,.5,.5);

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                //CLIP TEXTURE IN Z AXIS
                clip(i.uv);
                clip(1.0 - i.uv);

                //GET PIXEL COLOR
                float4 col = tex3D(_MainTex, i.uv);
                float4 newCol = col;

                //CUT BLACK BACKGROUND OF DICOM FILE
                if(_CutBlackPixelsBefore == 1)
                {
                    if(newCol.r < _CutLowerBorder && newCol.g < _CutLowerBorder && newCol.b < _CutLowerBorder)
					discard;
                    
                }
                //CUT WHITE FOREGROUND OF DICOM FILE
                if(_CutWhitePixelsBefore == 1)
                {
                    if(newCol.r > _CutUpperBorder && newCol.g > _CutUpperBorder && newCol.b > _CutUpperBorder)
                    discard;
                }

                //CALCULATE WINDOW BOUNDS
                _WindowMax =  _WindowCenter + (_WindowWidth/2);
                _WindowMin =  _WindowCenter - (_WindowWidth/2);

                //RESCALE PIXEL VALUE BACK INTO HOUNSFIELD SCALE
                newCol = (newCol * 4095) - 1024;

                //WINDOW PIXEL VALUE
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

                //RESCALE PIXEL BACK FROM HOUNSFIELD SCALE TO 0..1
                newCol = (newCol - _WindowMin) / _WindowWidth;
                
                //CUT BLACK BACKGROUND AFTER WINDOWING
                if(_CutBlackPixelsAfter == 1)
                {
                    if(newCol.r < _CutLowerBorder && newCol.g < _CutLowerBorder && newCol.b < _CutLowerBorder)
					discard;
                }
                //CUT WHITE FOREGROUND AFTER WINDOWING
                if(_CutWhitePixelsAfter == 1)
                {
                    if(newCol.r > _CutUpperBorder && newCol.g > _CutUpperBorder && newCol.b > _CutUpperBorder)
                    discard;
                }

                return newCol;
            }
            ENDCG
        }

        //RENDER COLOR FRAME
        Pass
        {
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex + float3(.5,.5,.5);

                o.color = _OutlineColor;
                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
                fixed4 col = (0,0,0,1);

                //SET FRAME WIDTH
                if (i.uv.x <= _OutlineWidth || i.uv.y <= _OutlineWidth || i.uv.x >= (1.0 - _OutlineWidth) || i.uv.y >= (1.0 - _OutlineWidth))
                //SET FRAME COLOR
                col = i.color;

                else
                discard;

                return col;
            }
            ENDCG
        }
    }
}
