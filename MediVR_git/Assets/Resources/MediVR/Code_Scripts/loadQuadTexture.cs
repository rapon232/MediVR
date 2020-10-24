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
    private GameObject borderCube;

    private Texture3D quadTexture;
    private Renderer quadRenderer;
    private Renderer cubeRenderer;

    // Start is called before the first frame update
    void Start()
    {
        quadTexture = this.GetComponent<importDicom>().threeDimTexture;
        
        /////Set 3D Texture to material of cube
        quadRenderer = this.GetComponent<Renderer>();
        quadRenderer.material.SetTexture("_MainTex", quadTexture);

        borderCube = GameObject.Find("Dicom_Image_Border_Cube");
        cubeRenderer = borderCube.GetComponent<Renderer>();
        cubeRenderer.material.SetTexture("_MainTex", quadTexture);
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
