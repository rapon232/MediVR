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



public class oldInitialImport : MonoBehaviour
{

    public dicomInfoTools dicomInformation = null;

    public Texture3D threeDimTexture = null;

    public UnityEngine.Object[] loadedTextures = null;

    public string dirName = null; // Name of folder containing dicom slices
    
    public string destinationTextureDirName = null; // Name of folder containg saved 3D Textures
    
    public string dirPath = null;   //Path to dirname, path to folder containing dicom slices
    public string ressourceDestinationPath = null;  //Path to destinationtexturedirname, path to folder conatining saved 3d textures
    public string thisRessourceDestinationPath = null; //Path to folder containing 3d texture, metadata and planes

  

    public int textureWidth = 256;
    public int textureHeight = 256;
    private int textureDepth = 0;

    public string textureRessourceName = null;  //Name of saved 3D Texture in Resources, without path and without ending


    // Start is called before the first frame update
    void Start()
    {


        //////// PATHS


        dirName = "CT_Series";
        destinationTextureDirName = "Dicom 3D Textures";

        #if UNITY_EDITOR
            var rootPath = "Assets";
        #else
            var rootPath = "sdcard/DCIM/MediVR";
        #endif

        dirPath = Path.Combine(rootPath, dirName);

        ressourceDestinationPath = Path.Combine("Assets/Resources/MediVR/Textures", destinationTextureDirName);
        thisRessourceDestinationPath = Path.Combine(ressourceDestinationPath, dirName);
        

        bool exists = System.IO.Directory.Exists(ressourceDestinationPath); // Create/Check for Folder: Dicom 3D Textures

        if(!exists)
        {
            System.IO.Directory.CreateDirectory(ressourceDestinationPath);
        }

        exists = System.IO.Directory.Exists(thisRessourceDestinationPath); // Create/Check for Folder: Dicom 3D Textures/dirName (CT_Series)

        if(!exists)
        {
            System.IO.Directory.CreateDirectory(thisRessourceDestinationPath);
        }
        

        Debug.Log($"Path to Directory: {dirPath}");
        Debug.Log($"Path to 3D Textures folder in Ressources Directory: {ressourceDestinationPath}");
        Debug.Log($"Path to {dirName} folder in Ressources Directory: {thisRessourceDestinationPath}");

        string pathTo3DTextures = "MediVR/Textures/" + destinationTextureDirName + "/" + dirName;

        loadedTextures = Resources.LoadAll(pathTo3DTextures, typeof(Texture3D)); //TRY TO LOAD 3D TEXTURE FROM FOLDER

        if(loadedTextures != null)
        {
            Debug.Log($"3D texture exists as Ressource. {loadedTextures[0].name} loaded from {thisRessourceDestinationPath}.");

            threeDimTexture = (Texture3D)loadedTextures[0];

            ///load metadata 

            ///load planes
        }
        else
        {
            Debug.Log($"3D Texture does not exist. Initializing 3D Texture from {dirPath}.");

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

            dicomFileNameList.Reverse();

            Debug.Log($"Valid Dicom files found in Directory: {dicomFileNameList.Count}. File names loaded onto list.");

            var file = DicomFile.Open(dicomFileNameList[0]);

            dicomInformation = new dicomInfoTools(file);

            //save metadata

            Debug.Log($"Metadata loaded from file: {dicomFileNameList[0]}");

            textureDepth = dicomImageTools.NextPow2(dicomFileNameList.Count);
            textureRessourceName = dirName + "_3DTexture_" + textureWidth + "x" + textureHeight + "x" + textureDepth;

            //threeDimTexture = dicomImageTools.createTexture3DAsAssetScript(dirPath, ressourceDestinationPath, textureRessourceName, dicomFileNameList, dicomInformation, textureWidth, textureHeight, textureDepth);

            //load planes
        }

        //threeDimTexture = Resources.Load<Texture3D>("MediVR/Textures/" + destinationTextureDirName + "/" + dirName + "/" + textureRessourceName); 
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

}

