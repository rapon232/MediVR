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

public class loadCubeTexture : MonoBehaviour
{
    private GameObject screenPlane;

    private Texture3D cubeTexture;

    public string textureRessourceName;

    // Start is called before the first frame update
    void Start()
    {
        screenPlane = GameObject.Find("Screen_Plane");
        //var screenPlaneScript = screenPlane.GetComponent<importDicom>();
        textureRessourceName = screenPlane.GetComponent<importDicom>().textureRessourceName;
        //Debug.Log(textureRessourceName);

        cubeTexture = Resources.Load<Texture3D>(textureRessourceName); 
        
        
        /////Set 3D Texture to material of cube
        var dicomImageCubeRenderer = this.GetComponent<Renderer>();

        dicomImageCubeRenderer.material.SetTexture("_Data", cubeTexture);
        dicomImageCubeRenderer.material.SetInt("_Iterations", cubeTexture.width);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
