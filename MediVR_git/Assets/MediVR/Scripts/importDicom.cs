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
    private Texture2D singleTexture = null;

    private dicomInfo dicomInformation  = new dicomInfo();

    public Texture3D threeDimTexture = null;

    public string dirName = null;
    public string fileName = null;
    public string dirPath = null;
    public string path = null;

    public int textureWidth = 0;
    public int textureHeight = 0;
    public int textureDepth = 0;

    public string textureRessourceName = null;


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

        dirPath = Path.Combine(rootPath, dirName);
        path = Path.Combine(dirPath, fileName);
        

        Debug.Log($"Path to Directory: {dirPath}");
        Debug.Log($"Path to first File in Directory: {path}");


        //////// TEXTURE SELECTION

        textureWidth = textureHeight = 256;
        textureDepth = 512;
        textureRessourceName = "Textures/Dicom 3D Textures/" + dirName + "_3DTexture_" + textureWidth + "x" + textureHeight + "x" + textureDepth;

        ///////// 2D

        singleTexture = imageTools.CreateTextureFromDicom (path, false, ref dicomInformation);

        ///////// 3D 

        /////Create 3D Texture from Dicomfiles
        
        /////Load 3DTexture as Asset
        threeDimTexture = Resources.Load<Texture3D>(textureRessourceName); 
        
        if(threeDimTexture != null)
        {
            Debug.Log($"3D Texture: {textureRessourceName} loaded from Ressources/Textures/Dicom 3D Textures.");
        }
        else
        {
            Debug.Log($"3D Texture does not exist. Initializing 3D Texture from {dirPath}.");
            double scaleTexture = Convert.ToDouble((textureWidth+textureHeight)/2) / Convert.ToDouble(singleTexture.width); 
            Debug.Log($"3D Texture scale set to {scaleTexture*100}%.");
            threeDimTexture = imageTools.CreateTexture3DAsAssetScript(dirPath, dirName, scaleTexture);
            Debug.Log($"3D Texture created from path {dirPath} and saved to Ressourcess/Textures/Dicom 3D Textures.");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

}

