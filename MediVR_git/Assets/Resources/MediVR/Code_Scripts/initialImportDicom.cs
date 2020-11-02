using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

using UnityEngine;

using Dicom;
using Dicom.Imaging;
using Dicom.Log;
using Dicom.Network;
using Dicom.Media;

using TMPro;



public class initialImportDicom : MonoBehaviour
{

    [HideInInspector]
    public dicomInfoTools dicomInformation = null;
    [HideInInspector]
    public Texture2D[] dicomSlices = null;
    [HideInInspector]
    public Texture3D threeDimTexture = null;

    //public UnityEngine.Object[] loadedTextures = null;

    public string userDefinedFolderName = null;
    public string userDefinedDicomPath = null;

    //public string dicomFileDirectory = "CT_Series"; // Name of folder containing dicom slices
    
    [HideInInspector]
    public string assetDestinationDirectory =  "Dicom 3D Textures"; // Name of folder containg saved 3D Textures
    [HideInInspector]
    public string savedTextureDestinationDirectory = "Saved Slices"; // Name of folder to save duplicates at runtime
    [HideInInspector]
    public string savedTextureDestinationPath = null; // Path to folder to save duplicates at runtime

    //private string dirPath = null;   //Path to dicomFileDirectory, path to folder containing dicom slices
    private string ressourceDestinationPath = null;  //Path to savedTextureDestinationDirectory, path to folder conatining saved 3d textures
    private string thisRessourceDestinationPath = null; //Path to Dicom 3D Textures/dicomFileDirectory, path folder containing 3d texture, metadata and planes

    #if UNITY_EDITOR
        private string rootPath = "Assets";
    #else
        private string rootPath = "sdcard/DCIM/MediVR";
    #endif

    private int textureWidth = 256;
    private int textureHeight = 256;

    private int textureDepth = 0;

    private string metadataRessourceName = null;  //Name of saved 3D Texture metadata in resources, with path and ending
    private string sliceRessourceName = null;  //Name of saved 3D Texture metadata in resources, without path and ending
    private string textureRessourceName = null;  //Name of saved 3D Texture in Resources, without path and without ending

    void Start()
    {
        //dirPath = Path.Combine(rootPath, dicomFileDirectory);
        savedTextureDestinationPath = Path.Combine(rootPath, savedTextureDestinationDirectory);
        ressourceDestinationPath = Path.Combine("Assets/Resources/MediVR/Textures", assetDestinationDirectory);
        thisRessourceDestinationPath = Path.Combine(ressourceDestinationPath, userDefinedFolderName);// dicomFileDirectory);
    }

    public void CreateTexture3DAssets()
    {
        if(userDefinedDicomPath != null)
        {
            //dirPath = Path.Combine(rootPath, dicomFileDirectory);
            savedTextureDestinationPath = Path.Combine(rootPath, savedTextureDestinationDirectory);
            ressourceDestinationPath = Path.Combine("Assets/Resources/MediVR/Textures", assetDestinationDirectory);
            thisRessourceDestinationPath = Path.Combine(ressourceDestinationPath, userDefinedFolderName);//dicomFileDirectory);

            bool exists = System.IO.Directory.Exists(ressourceDestinationPath); // Create/Check for Folder: Dicom 3D Textures

            if(!exists)
            {
                System.IO.Directory.CreateDirectory(ressourceDestinationPath);
            }

            exists = System.IO.Directory.Exists(thisRessourceDestinationPath); // Create/Check for Folder: Dicom 3D Textures/dicomFileDirecory (CT_Series)

            if(!exists)
            {
                System.IO.Directory.CreateDirectory(thisRessourceDestinationPath);
            }
            

            //Debug.Log($"Path to Directory: {dirPath}");
            Debug.Log($"Path to Directory: {userDefinedDicomPath}");
            Debug.Log($"Path to 3D Textures folder in Ressources Directory: {ressourceDestinationPath}");
            //Debug.Log($"Path to {dicomFileDirectory} folder in Ressources Directory: {thisRessourceDestinationPath}");
            Debug.Log($"Path to {userDefinedFolderName} folder in Ressources Directory: {thisRessourceDestinationPath}");

            string pathTo3DTextures = "MediVR/Textures/" + assetDestinationDirectory + "/" +  userDefinedFolderName;//dicomFileDirectory;

            //Debug.Log($"Initializing 3D Texture from {dirPath}.");
            Debug.Log($"Initializing 3D Texture from {userDefinedDicomPath}.");

            //var dicomDirectoryInfo = new DirectoryInfo(dirPath);
            var dicomDirectoryInfo = new DirectoryInfo(userDefinedDicomPath);

            int dicomFileCount = dicomDirectoryInfo.GetFiles().Length;
            Debug.Log($"Files found in Directory: {dicomFileCount}");
            //Debug.Log($"Loading Dicom files from Directory {dirPath} into Array");
            Debug.Log($"Loading Dicom files from Directory {userDefinedDicomPath} into Array");

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

            textureDepth = dicomImageTools.NextPow2(dicomFileNameList.Count);

            var file = DicomFile.Open(dicomFileNameList[0]);

            dicomInformation = new dicomInfoTools(file);

            Debug.Log($"Metadata loaded from file: {dicomFileNameList[0]}.");

            //////// SAVE METADATA

            //metadataRessourceName = thisRessourceDestinationPath + "/" + dicomFileDirectory + "_3DTexture_" + textureWidth + "x" + textureHeight + "x" + textureDepth + "_MetaData.XML";
            metadataRessourceName = thisRessourceDestinationPath + "/" + userDefinedFolderName + "_3DTexture_" + textureWidth + "x" + textureHeight + "x" + textureDepth + "_MetaData.XML"; 
    
            XmlSerializer serializer = new XmlSerializer(typeof(dicomInfoTools));
            using(TextWriter writer = new StreamWriter(metadataRessourceName))
            {
                serializer.Serialize(writer, dicomInformation);
            }

            Debug.Log($"Metadata saved to file: {metadataRessourceName}.");

            //////// SAVE SINGLE SLICES

            //sliceRessourceName = dicomFileDirectory + "_3DTexture_" + textureWidth + "x" + textureHeight + "x" + textureDepth + "_Slice"; 
            sliceRessourceName = userDefinedFolderName + "_3DTexture_" + textureWidth + "x" + textureHeight + "x" + textureDepth + "_Slice";

            dicomSlices = dicomImageTools.CreateNumberedTextureArrayFromDicomdir(dicomFileNameList, 5);
            dicomImageTools.SaveTextureArrayAsAssets(dicomSlices, thisRessourceDestinationPath, sliceRessourceName);

            //////// SAVE 3D TEXTURE

            //textureRessourceName = dicomFileDirectory + "_3DTexture_" + textureWidth + "x" + textureHeight + "x" + textureDepth; 
            textureRessourceName = userDefinedFolderName + "_3DTexture_" + textureWidth + "x" + textureHeight + "x" + textureDepth;
            threeDimTexture = dicomImageTools.createTexture3DAsAssetScript(dicomFileNameList, dicomInformation, textureWidth, textureHeight, textureDepth);
            dicomImageTools.exportTexture3DToAsset(threeDimTexture, thisRessourceDestinationPath, textureRessourceName);
        }
    }
}

