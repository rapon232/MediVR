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

    //////// Method for toggleing shader parameters from UI Button. The parameters are boolean, but ShaderLab does not support bool, so int 0/1 is used instead
    //////// Used here for toggleing between cutting black background pixels of 3D Texture
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
