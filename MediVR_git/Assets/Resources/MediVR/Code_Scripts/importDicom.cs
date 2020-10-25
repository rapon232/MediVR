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
    //private Texture2D singleTexture = null;

    private dicomInfoTools dicomInformation  = new dicomInfoTools();

    public Texture3D threeDimTexture = null;

    public string dirName = null;
    public string fileName = null;
    public string destinationTextureDirName = null;
    public string destinationDirName = null;
    public string dirPath = null;
    public string path = null;
    public string ressourceDestinationPath = null;
    public string fileDestinationPath = null;
    public string textureDestinationPath = null;
    public string textureDestinationDirName = null;
    // string threeDimTexturePath = null;

    public int textureWidth = 0;
    public int textureHeight = 0;
    public int textureDepth = 0;

    public string textureRessourceName = null;
    public string textureArrayName = null;


    // Start is called before the first frame update
    void Start()
    {


        //////// PATHS


        dirName = "CT_Series";
        //dirName = "MR_Series";
        //fileName = "image-000001.dcm";
        //fileName = "image-000000.dcm";
        destinationTextureDirName = "Dicom 3D Textures";
        destinationDirName = "Saved";
        textureDestinationDirName = "Saved Slices";

        #if UNITY_EDITOR
            var rootPath = "Assets";
        #else
            var rootPath = "sdcard/DCIM/MediVR";
        #endif

        dirPath = Path.Combine(rootPath, dirName);
        //path = Path.Combine(dirPath, fileName);
        ressourceDestinationPath = Path.Combine("Assets/Resources/MediVR/Textures", destinationTextureDirName);
        fileDestinationPath = Path.Combine(dirPath, destinationDirName);
        textureDestinationPath = Path.Combine(rootPath, textureDestinationDirName);
        

        Debug.Log($"Path to Directory: {dirPath}");
        //Debug.Log($"Path to first File in Directory: {path}");
        Debug.Log($"Path to Texture Ressource Directory: {ressourceDestinationPath}");
        //Debug.Log($"Path to Array Save Directory: {fileDestinationPath}");
        Debug.Log($"Path to Texture Save Directory: {textureDestinationPath}");

        //////// FILES AT PATH

        var dicomDirectoryInfo = new DirectoryInfo(dirPath);

        int dicomFileCount = dicomDirectoryInfo.GetFiles().Length;
        Debug.Log($"Files found in Directory: {dicomFileCount}");
        Debug.Log($"Loading Dicom files from Directory {dirPath} into Array");

        List<string> dicomFileNameList = new List<string>();

        foreach (var dicomFile in dicomDirectoryInfo.GetFiles(".", SearchOption.AllDirectories)) 
        {
            if (DicomFile.HasValidHeader(dicomFile.FullName))
            {
                dicomFileNameList.Add(dicomFile.FullName);
            }
        }

        Debug.Log($"Valid Dicom files found in Directory: {dicomFileNameList.Count}. File names loaded onto list.");

        var file = DicomFile.Open(dicomFileNameList[0]);
        dicomInformation.setDicomInfo(file);

        Debug.Log($"Metadata loaded from file: {dicomFileNameList[0]}");


        //////// TEXTURE SELECTION

        textureWidth = textureHeight = 256;
        textureDepth = dicomImageTools.NextPow2(dicomFileNameList.Count);
        textureRessourceName = dirName + "_3DTexture_" + textureWidth + "x" + textureHeight + "x" + textureDepth;
        textureArrayName = fileDestinationPath + "/" + dirName + "_3DTexture_Color_Array" + textureWidth + "x" + textureHeight + "x" + textureDepth + ".bytes";

        //threeDimTexturePath = Path.Combine(ressourceDestinationPath, textureRessourceName);


        ///////// 2D

        //singleTexture = dicomImageTools.CreateTextureFromDicom (path, false, ref dicomInformation);
        //singleTexture = dicomImageTools.CreateTextureFromFirstDicom (dirPath, false, ref dicomInformation);

        ///////// 3D 

        /////Create 3D Texture from Dicomfiles
        
        /////Load 3DTexture as Asset
        threeDimTexture = Resources.Load<Texture3D>("MediVR/Textures/" + destinationTextureDirName + "/" + textureRessourceName); 
        
        if(threeDimTexture != null)
        {
            Debug.Log($"3D texture exists as Ressource. {textureRessourceName} loaded from {ressourceDestinationPath}.");
        }
        else
        {
            /*if(File.Exists(textureArrayName))
            {
                Debug.Log($"Color Array for 3D Texture exists. Building 3D Texture from array at {textureArrayName}.");
                threeDimTexture = dicomImageTools.importColorArrayTo3DTexture(textureArrayName, textureWidth, textureHeight, textureDepth);
            }
            else
            {*/
                Debug.Log($"3D Texture does not exist. Initializing 3D Texture from {dirPath}.");
                //double scaleTexture = Convert.ToDouble((textureWidth+textureHeight)/2) / Convert.ToDouble(singleTexture.width); 
                //Debug.Log($"3D Texture scale set to {scaleTexture*100}%.");

                threeDimTexture = dicomImageTools.createTexture3DAsAssetScript(dirPath, ressourceDestinationPath, textureRessourceName, dicomFileNameList, textureWidth, textureHeight, textureDepth);
                //threeDimTexture = dicomImageTools.createTexture3DAsFileScript(dirPath, dirName, fileDestinationPath, scaleTexture, textureArrayName, textureWidth, textureHeight, textureDepth);
            //}
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

}

