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
    //private GameObject screenPlane;

    //private Texture3D cubeTexture;

    //public string textureRessourceName;

    // Start is called before the first frame update

    //private GameObject borderCube = null;

    //private importDicom importDicomScript = null;

    private string mainTextureName = "_Data";
    private string mainTextureIterationsName = "_Iterations";
    //private string adjustWindowWidthName = "_WindowWidth";
    //private string adjustWindowCenterName = "_WindowCenter";

    //private float originalWindowWidth = 300;
    //private float originalWindowCenter = 0;

    public Texture3D dicomImageCubeTexture = null;
    private Renderer dicomImageCubeRenderer = null;
    //private Renderer cubeRenderer = null;

    void Start()
    {
        /*screenPlane = GameObject.Find("Screen_Plane");
        //var screenPlaneScript = screenPlane.GetComponent<importDicom>();
        //textureRessourceName = screenPlane.GetComponent<importDicom>().textureRessourceName;
        //Debug.Log(textureRessourceName);

        cubeTexture = Resources.Load<Texture3D>(textureRessourceName); 
        
        
        /////Set 3D Texture to material of cube
        var dicomImageCubeRenderer = this.GetComponent<Renderer>();

        dicomImageCubeRenderer.material.SetTexture("_Data", cubeTexture);
        dicomImageCubeRenderer.material.SetInt("_Iterations", cubeTexture.width);*/

        //importDicomScript = this.GetComponent<importDicom>();
        dicomImageCubeRenderer = this.GetComponent<Renderer>();

        //borderCube = GameObject.Find("Dicom_Image_Border_Cube");
        //cubeRenderer = borderCube.GetComponent<Renderer>();

        dicomImageCubeRenderer.material.SetTexture(mainTextureName, dicomImageCubeTexture);
        dicomImageCubeRenderer.material.SetInt(mainTextureIterationsName, dicomImageCubeTexture.width);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
