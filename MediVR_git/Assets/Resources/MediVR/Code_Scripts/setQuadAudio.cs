/*

    MediVR, a medical Virtual Reality application for exploring 3D medical datasets on the Oculus Quest.

    Copyright (C) 2020  Dimitar Tahov

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    This script serves to set audio effects source files for click sounds at runtime.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setQuadAudio : MonoBehaviour
{
    public AudioSource audioFXSource = null;
    public AudioClip onButtonPressDown = null;
    public AudioClip onButtonPressUp = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
