

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

//    This shader serves to render frame of loaded volume at runtime.


Shader "MediVR/BorderRendering"
{
    Properties
    {
        _MainTex("Texture", 3D) = "white" {}

        _MainTex_TexelSize("Texel Size", Vector) = (0,0,0,0)

        _OutlineColor("Outline Color", Color) = (1,0,0,1)
    }

        CGINCLUDE
        #include "UnityCG.cginc"

            float4  _OutlineColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 uv : TEXCOORD0;

                float4 color : COLOR;
            };

            sampler3D _MainTex;
            float4 _MainTex_TexelSize;

        ENDCG

    SubShader
    {
        Tags { "Queue"="Transparent"  "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        //RENDER FRAME
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
 
                o.color = _OutlineColor;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //CLIP FRAME IN Z AXIS
                clip(i.uv);
                clip(1.0 - i.uv);

                half4 col = (0,0,0,0);

                //SET FRAME WIDTH
                if ((i.uv.x <= _MainTex_TexelSize.x || i.uv.y <= _MainTex_TexelSize.y || i.uv.x >= (1.0 - _MainTex_TexelSize.x) 
                        || i.uv.y >= (1.0 - _MainTex_TexelSize.y)))
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
