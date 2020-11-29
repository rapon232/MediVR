/*

    MediVR, a medical Virtual Reality application for exploring 3D medical datasets on the Oculus Quest.

    Copyright (C) 2020  Dimitar Tahov

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    This script serves to import DICOM slices from a directory and reconstruct them to a 3D Texture. Their metadata gets serialized to an XML file and
    all imported directory names get serialized to an XML file too.

*/

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
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

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

    public string userDefinedFolderName = null; // Name of Directory to be imported, chosen from menu bar
    public string userDefinedDicomPath = null; // Path to Directory to be imported, chosen from menu bar
    public bool anonymizeDicomMetaData = false; // Toggle to anonymize data
    public bool reverseSliceOrder = false; //Toggle to reverse order of slices while importing
    
    [HideInInspector]
    public static string assetDestinationDirectory =  "Dicom 3D Textures"; // Name of folder containg saved 3D Textures
    [HideInInspector]
    public static string savedTextureDestinationDirectory = "Saved Slices"; // Name of folder to save duplicates at runtime
    [HideInInspector]
    public static string savedTextureDestinationPath = null; // Path to folder to save duplicates at runtime
    public static string ressourceDestinationPath = null;  // Path to Dicom 3D Textures, path to folder conatining saved 3d textures
    [HideInInspector]
    public string thisRessourceDestinationPath = null; // Path to Dicom 3D Textures/dicomFileDirectory, path folder containing 3d texture, metadata and planes
    [HideInInspector]
    public static string importedDirectoryXMLFileName = "importedDirectoryNames.XML"; // Name of saved XML file containing all imported directory names

    #if UNITY_EDITOR
        private string rootPath = "Assets/MediVR";
    #else
        private string rootPath = "sdcard/DCIM/MediVR";
    #endif

    private int textureWidth = 256;
    private int textureHeight = 256;

    private int textureDepth = 0;

    private string metadataRessourceName = null;  // Name of saved 3D Texture metadata in resources, with path and ending
    private string sliceRessourceName = null;  // Name of saved 3D Texture metadata in resources, without path and ending
    private string textureRessourceName = null;  //Name of saved 3D Texture in Resources, without path and without ending
    
    private string[] subDirectoryNames = null; // Array of Strings containing imported directory names
    private string[] subDirectoryEntries = null; // Array of Strings containing imported directory paths
    private string importedDirectoryXMLFilePath = null; // Path to saved XML file containing all imported directory names, with ending

    void Start()
    {
        savedTextureDestinationPath = Path.Combine(rootPath, savedTextureDestinationDirectory);
        bool exists = Directory.Exists(savedTextureDestinationPath); // Create/Check for Folder: Saved Slices

        if(!exists)
        {
            Directory.CreateDirectory(savedTextureDestinationPath);
        }

        ressourceDestinationPath = Path.Combine("Assets/Resources/MediVR/Textures", assetDestinationDirectory); // Set some paths
        thisRessourceDestinationPath = Path.Combine(ressourceDestinationPath, userDefinedFolderName);
    }

    //IMPORT DICOM SLICES
    public void CreateTexture3DAssets()
    {
        try
        {
            savedTextureDestinationPath = Path.Combine(rootPath, savedTextureDestinationDirectory);
            bool exists = Directory.Exists(savedTextureDestinationPath); // Create/Check for Folder: Saved Slices

            if(!exists)
            {
                Directory.CreateDirectory(savedTextureDestinationPath);
            }

            ressourceDestinationPath = Path.Combine("Assets/Resources/MediVR/Textures", assetDestinationDirectory); //Set some paths
            thisRessourceDestinationPath = Path.Combine(ressourceDestinationPath, userDefinedFolderName);

            exists = Directory.Exists(ressourceDestinationPath); // Create/Check for Folder: Dicom 3D Textures

            if(!exists)
            {
                Directory.CreateDirectory(ressourceDestinationPath);
            }

            exists = Directory.Exists(thisRessourceDestinationPath); // Create/Check for Folder: Dicom 3D Textures/dicomFileDirecory (CT_Series)

            if(!exists)
            {
                Directory.CreateDirectory(thisRessourceDestinationPath);
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

            //READ FILE NAMES

            List<string> dicomFileNameList = new List<string>();

            foreach (var dicomFile in dicomDirectoryInfo.GetFiles(".", SearchOption.AllDirectories)) 
            {
                if (DicomFile.HasValidHeader(dicomFile.FullName))
                {
                    dicomFileNameList.Add(dicomFile.FullName);
                }
            }

            if(dicomFileNameList.Count != 0)
            {
                Debug.Log($"Valid Dicom files found in Directory: {dicomFileNameList.Count}. File names loaded onto list.");

                if(reverseSliceOrder)
                {
                    dicomFileNameList.Reverse();
                }

                textureDepth = dicomImageTools.NextPow2(dicomFileNameList.Count);

                var file = DicomFile.Open(dicomFileNameList[0]);

                dicomInformation = new dicomInfoTools(file, anonymizeDicomMetaData);

                Debug.Log($"Metadata loaded from file: {dicomFileNameList[0]}.");

                //////// SAVE METADATA

                metadataRessourceName = thisRessourceDestinationPath + "/" + userDefinedFolderName + "_3DTexture_" + textureWidth + "x" + textureHeight + "x" + textureDepth + "_MetaData.XML"; 
        
                XmlSerializer serializer = new XmlSerializer(typeof(dicomInfoTools));
                using(TextWriter writer = new StreamWriter(metadataRessourceName))
                {
                    serializer.Serialize(writer, dicomInformation);
                }

                Debug.Log($"Metadata saved to file: {metadataRessourceName}.");

                //////// SAVE SINGLE SLICES

                sliceRessourceName = userDefinedFolderName + "_3DTexture_" + textureWidth + "x" + textureHeight + "x" + textureDepth + "_Slice";

                dicomSlices = dicomImageTools.CreateNumberedTextureArrayFromDicomdir(dicomFileNameList, 5);
                dicomImageTools.SaveTextureArrayAsAssets(dicomSlices, thisRessourceDestinationPath, sliceRessourceName);

                //////// SAVE 3D TEXTURE

                textureRessourceName = userDefinedFolderName + "_3DTexture_" + textureWidth + "x" + textureHeight + "x" + textureDepth;
                threeDimTexture = dicomImageTools.createTexture3DAsAssetScript(dicomFileNameList, dicomInformation, textureWidth, textureHeight, textureDepth);
                dicomImageTools.exportTexture3DToAsset(threeDimTexture, thisRessourceDestinationPath, textureRessourceName);

                //////// SAVE IMPORTED DIRECTORY NAMES

                SaveImportedTextureDirectoryNames();

            }
            else
            {
                Debug.Log($"Valid Dicom files found in Directory: {dicomFileNameList.Count}. Import failed. Try with valid directory.");
            }
        }
        catch(ArgumentException aE)//else
        {
            Debug.Log($"Exception: {aE.ToString()} thrown. Path to directory invalid. Try with valid directory.");
        }
    }

    //SERIALIZE IMPORTED DIRECTORY NAMES
    public void SaveImportedTextureDirectoryNames()
    {
        ressourceDestinationPath = Path.Combine("Assets/Resources/MediVR/Textures", assetDestinationDirectory);
        
        bool exists = Directory.Exists(ressourceDestinationPath); // Create/Check for Folder: Dicom 3D Textures

        if(exists)
        {
            subDirectoryEntries = Directory.GetDirectories(ressourceDestinationPath);
            subDirectoryNames = new string[subDirectoryEntries.Length];

            for (int i = 0; i < subDirectoryEntries.Length; i++)
            {
                subDirectoryNames[i] = subDirectoryEntries[i].Split(Path.DirectorySeparatorChar).Last();
            }

            importedDirectoryXMLFilePath = Path.Combine(ressourceDestinationPath, importedDirectoryXMLFileName);

            XmlSerializer serializer = new XmlSerializer(typeof(string[]));
            using(TextWriter writer = new StreamWriter(importedDirectoryXMLFilePath))
            {
                serializer.Serialize(writer, subDirectoryNames);
            }

            Debug.Log($"{subDirectoryEntries.Length} Imported Directory Name(s) saved to file: {importedDirectoryXMLFilePath}.");
        }
        else
        {
            Debug.Log($"Imported Directories NOT found.");
        }
    }

    public void GoToLastScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void GoToNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitApplication()
    {
        Debug.Log($"Quitting App. Stay safe and healthy :) !");
        Application.Quit();
    }

}

