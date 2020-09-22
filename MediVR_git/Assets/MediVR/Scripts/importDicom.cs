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



public class importDicom : MonoBehaviour
{
    private string dirName;
    private string fileName;

    private string dicomInfo;

    private GameObject dicomImagePlane;
    private GameObject dicomImagePlane2;
    private GameObject dicomImagePlane3;
    private GameObject dicomImagePlane4;
    private GameObject dicomImagePlane5;

    private GameObject dicomImageCube;

    private Texture2D[] planeTextureArray;

    private Texture2D planeTexture;
    private Texture2D planeTexture2;
    private Texture2D planeTexture3;
    private Texture2D planeTexture4;
    private Texture2D planeTexture5;

    private Texture2D[] textureArray;
    private Color[] colorsFor3DTexture;
    private Texture3D cubeTexture;


    // Start is called before the first frame update
    void Start()
    {


        //////// PATHS


        dirName = "CT_Series";
        fileName = "image-000001.dcm";

        #if UNITY_EDITOR
            var rootPath = "Assets";
        #else
            var rootPath = "sdcard/DCIM";
        #endif

        var dirPath = Path.Combine(rootPath, dirName);
        var path = Path.Combine(dirPath, fileName);
        

        Debug.Log($"Path to Directory: {dirPath}");
        Debug.Log($"Path to first File in Directory: {path}");


        //////// 2D
    

        /////Load single slice into Texture2D
        //planeTexture = imageTools.CreateTextureFromDicom(path, false, ref dicomInfo);


        /////Load multiple slices into Texture2D Array
        planeTextureArray = imageTools.CreateNumberedTextureArrayFromDicomdir (dirPath, false, ref dicomInfo, 5);

        /////Assign slice texture to each Plane
        dicomImagePlane = GameObject.Find("Dicom_Image_Plane");
        var dicomImagePlaneRenderer = dicomImagePlane.GetComponent<Renderer>();
        dicomImagePlaneRenderer.material.mainTexture = planeTextureArray[0];

        dicomImagePlane2 = GameObject.Find("Dicom_Image_Plane_2");
        var dicomImagePlaneRenderer2 = dicomImagePlane2.GetComponent<Renderer>();
        dicomImagePlaneRenderer2.material.mainTexture = planeTextureArray[1];

        dicomImagePlane3 = GameObject.Find("Dicom_Image_Plane_3");
        var dicomImagePlaneRenderer3 = dicomImagePlane3.GetComponent<Renderer>();
        dicomImagePlaneRenderer3.material.mainTexture = planeTextureArray[2];

        dicomImagePlane4 = GameObject.Find("Dicom_Image_Plane_4");
        var dicomImagePlaneRenderer4 = dicomImagePlane4.GetComponent<Renderer>();
        dicomImagePlaneRenderer4.material.mainTexture = planeTextureArray[3];

        dicomImagePlane5 = GameObject.Find("Dicom_Image_Plane_5");
        var dicomImagePlaneRenderer5 = dicomImagePlane5.GetComponent<Renderer>();
        dicomImagePlaneRenderer5.material.mainTexture = planeTextureArray[4];
        
        /////Assign slice dicom information to Canvas
        dicomImagePlane.GetComponentInChildren<TextMeshProUGUI>().text = dicomInfo;


        ///////// 3D 


        /////Create 3D Texture from Dicomfiles
        //double scaleTexture = .25; cubeTexture = imageTools.CreateTexture3DAsAssetScript(dirPath, dirName, scaleTexture);
        
        /////Load 3DTexture as Asset
        int textureXY = 256;
        int textureDepth = 512;
        string textureRessourceName = "Textures/" + dirName + "_3DTexture_" + textureXY + "x" + textureXY + "x" + textureDepth;

        cubeTexture = Resources.Load<Texture3D>(textureRessourceName); 
        
        if(cubeTexture != null)
        {
            Debug.Log($"3D Texture: {textureRessourceName} loaded from Ressources.");
        }
        else
        {
            Debug.Log($"3D Texture does not exist. Initializing 3D Texture from {dirPath}.");
            double scaleTexture = Convert.ToDouble(textureXY) / Convert.ToDouble(planeTextureArray[0].width); 
            Debug.Log($"3D Texture scale set to {scaleTexture*100}%.");
            cubeTexture = imageTools.CreateTexture3DAsAssetScript(dirPath, dirName, scaleTexture);
            Debug.Log($"3D Texture created from path {dirPath} and saved to Ressources.");
        }

        /////Set 3D Texture to material of cube
        dicomImageCube = GameObject.Find("Dicom_Cube");
        var dicomImageCubeRenderer = dicomImageCube.GetComponent<Renderer>();
        dicomImageCubeRenderer.material.SetTexture("_Data", cubeTexture);
        dicomImageCubeRenderer.material.SetInt("_Iterations", cubeTexture.width);
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    /*private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, _texture.width, _texture.height), _texture);
        GUI.Label(new Rect(_texture.width, 0, Screen.width - _texture.width, Screen.height), _dump);
    }*/

    

}

