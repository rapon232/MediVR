

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

//    This shader serves to render adjustable transparent color background for snap zones on pin wall.


Shader "MediVR/SnapZoneRendering" 
{
    Properties 
    {
        _Color ("Main Color", Color) = (1,1,1,1)
    }

    SubShader 
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        
        ZWrite Off
        Lighting Off
        Fog { Mode Off }

        Blend SrcAlpha OneMinusSrcAlpha 

        Pass 
        {
            Color [_Color]
        }
    }
}