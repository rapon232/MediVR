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

    This script serves to show 3D Dataset in volume frame.

*/

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using Dicom;
using Dicom.Imaging;
using Dicom.Log;
using Dicom.Network;
using Dicom.Media;

using TMPro;

public class loadQuadTexture : MonoBehaviour
{
    private GameObject borderCube = null;

    private importDicom importDicomScript = null;

    private string mainTextureName = "_MainTex";
    private string adjustWindowWidthName = "_WindowWidth";
    private string adjustWindowCenterName = "_WindowCenter";

    private float originalWindowWidth = 300;
    private float originalWindowCenter = 0;

    private Texture3D quadTexture = null;
    private Renderer quadRenderer = null;
    private Renderer cubeRenderer = null;

    // Start is called before the first frame update
    void Start()
    {
        /////Find objects
        importDicomScript = this.GetComponent<importDicom>();
        quadRenderer = this.GetComponent<Renderer>();

        borderCube = GameObject.Find("Dicom_Image_Border_Cube");
        cubeRenderer = borderCube.GetComponent<Renderer>();

        if(importDicomScript.dicomInformation != null)
        {
            //////// Set Original window settings to shader of cube
            originalWindowWidth = (float)importDicomScript.dicomInformation.ImageWindowWidth;
            originalWindowCenter = (float)importDicomScript.dicomInformation.ImageWindowCenter;

            quadRenderer.material.SetFloat(adjustWindowWidthName, originalWindowWidth);
            quadRenderer.material.SetFloat(adjustWindowCenterName, originalWindowCenter);
        }

        if(importDicomScript.threeDimTexture != null)
        {
            //////// Set 3D Texture to shader of cube
            quadTexture = importDicomScript.threeDimTexture;
            quadRenderer.material.SetTexture(mainTextureName, quadTexture);

            cubeRenderer.material.SetTexture(mainTextureName, quadTexture);
        }
    }

    //////// Method for toggling shader parameters from UI Button. The parameters are boolean, but ShaderLab does not support bool, so int 0/1 is used instead
    //////// Used here for toggling between cutting black background pixels of 3D Texture
    public void ToggleShaderIntParam(string parameter)
    {
        int set = quadRenderer.material.GetInt(parameter);

        if(set == 1)
        {
            quadRenderer.material.SetInt(parameter, 0);
        }
        else
        {
            quadRenderer.material.SetInt(parameter, 1);
        }
    }
}
