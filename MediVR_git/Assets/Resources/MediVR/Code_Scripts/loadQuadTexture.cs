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
    //private GameObject screenPlane;
    private GameObject borderCube = null;

    private importDicom importDicomScript = null;

    private string mainTextureName = "_MainTex";
    private string adjustWindowWidthName = "_WindowWidth";
    private string adjustWindowCenterName = "_WindowCenter";

    private float originalWindowWidth = 0;
    private float originalWindowCenter = 0;

    private Texture3D quadTexture = null;
    private Renderer quadRenderer = null;
    private Renderer cubeRenderer = null;

    // Start is called before the first frame update
    void Start()
    {
        importDicomScript = this.GetComponent<importDicom>();
        originalWindowWidth = (float)importDicomScript.dicomInformation.ImageWindowWidth;
        originalWindowCenter = (float)importDicomScript.dicomInformation.ImageWindowCenter;
        quadTexture = importDicomScript.threeDimTexture;
        
        /////Set 3D Texture to material of cube
        quadRenderer = this.GetComponent<Renderer>();
        quadRenderer.material.SetTexture(mainTextureName, quadTexture);
        quadRenderer.material.SetFloat(adjustWindowWidthName, originalWindowWidth);
        quadRenderer.material.SetFloat(adjustWindowCenterName, originalWindowCenter);

        borderCube = GameObject.Find("Dicom_Image_Border_Cube");
        cubeRenderer = borderCube.GetComponent<Renderer>();
        cubeRenderer.material.SetTexture(mainTextureName, quadTexture);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
