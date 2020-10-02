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
    private GameObject screenPlane;

    private Texture3D quadTexture;

    // Start is called before the first frame update
    void Start()
    {

        screenPlane = GameObject.Find("Screen_Plane");
        var screenPlaneScript = screenPlane.GetComponent<importDicom>();

        quadTexture = screenPlaneScript.threeDimTexture;
        
        /////Set 3D Texture to material of cube
        var quadRenderer = this.GetComponent<Renderer>();

        quadRenderer.material.SetTexture("_MainTex", quadTexture);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
