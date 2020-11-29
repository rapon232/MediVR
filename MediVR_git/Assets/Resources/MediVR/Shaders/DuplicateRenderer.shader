

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

//    This shader serves to render duplicate slices at runtime.



Shader "MediVR/DuplicateRendering"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}

        _MainTex_TexelSize("Texel Size", Vector) = (0,0,0,0)

        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline Width", Range(0,0.1)) = 0.02

        [Toggle]
        _CutBlackPixels("Cut Black Background", Int) = 1
        
        [HideInInspector]
        _EnlargeMin("Enlargement Minimum", Float) = 1
        [HideInInspector]
        _EnlargeMax("Enlargement Maximum", Float) = 10
        [HideInInspector] // Uncomment for Use
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

            int _CutBlackPixels;

            float _Enlarge;
            float _Brightness;
            float _Contrast;
            float _Threshold;
            float _ThresholdInv;

            float3 _StartCoords;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;

                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

        ENDCG

    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 200

        //RENDER TEXTURE2D
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
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //CLIP TEXTURE IN Z AXIS
                clip(i.uv);
                clip(1.0 - i.uv);

                fixed4 col = tex2D(_MainTex, i.uv);

                return col;
            }
            ENDCG
        }

        //RENDER FRAME
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
