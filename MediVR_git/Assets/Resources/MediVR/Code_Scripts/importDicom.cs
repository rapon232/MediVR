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

    This script serves to initialize imported data and open it at runtime.

*/

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Linq;

using UnityEngine;

using Dicom;
using Dicom.Imaging;
using Dicom.Log;
using Dicom.Network;
using Dicom.Media;

using TMPro;

public class importDicom : MonoBehaviour
{
    public Texture3D threeDimTexture = null;
    [HideInInspector]
    public dicomInfoTools dicomInformation = null;
    public Texture2D[] dicomSlices = null;

    [HideInInspector]
    public string textureDestinationPath = null;

    private string metaData = null;

    private string dirName = null;
    private string pathTo3DTextures = null;

    private string destinationTextureDirName = null;
    private string metadataName = null;

    private UnityEngine.Object[] loadedTextures = null;
    private UnityEngine.Object[] loaded2DTextures = null;

    // Start is called before the first frame update
    void Start()
    {
        //////// PATHS

        dirName = setCurrentDirectory.currentDirectory;
        destinationTextureDirName = initialImportDicom.assetDestinationDirectory;
        textureDestinationPath = initialImportDicom.savedTextureDestinationPath;

        //////// LOAD 3D TEXTURE

        if(dirName != null)
        {
            pathTo3DTextures = "MediVR/Textures/" + destinationTextureDirName + "/" + dirName; 

            loadedTextures = Resources.LoadAll(pathTo3DTextures, typeof(Texture3D)); //TRY TO LOAD 3D TEXTURE FROM FOLDER
        }
       
        if(loadedTextures != null)
        {
            if(loadedTextures.Length > 0)
            {
                threeDimTexture = (Texture3D)loadedTextures[0];

                //Debug.Log($"{loadedTextures[0].name}");

                //////// LOAD METADATA

                metadataName = pathTo3DTextures + "/" + loadedTextures[0].name + "_MetaData";

                var resource = Resources.Load<TextAsset>(metadataName);
                if(resource != null)
                {
                    metaData = resource.text;
                }

                //Debug.Log($"{metaData}");

                if(metaData != null)
                {
                    metaData = metaData.Replace("&#x0;", "");
                    
                    XmlSerializer deserializer = new XmlSerializer(typeof(dicomInfoTools));

                    using(StringReader reader = new StringReader(metaData))
                    {
                        object obj = deserializer.Deserialize(reader);

                        dicomInformation = (dicomInfoTools)obj;
                        dicomInformation.GetDicomInfoString();
                    }
                }

                //Debug.Log($"{dicomInformation.PatientId}");

                //Debug.Log($"{metaData}");

                //////// LOAD SINGLE SLICES
                
                loaded2DTextures = Resources.LoadAll(pathTo3DTextures, typeof(Texture2D)); //TRY TO LOAD 2D TEXTURES FROM FOLDER

                if(loaded2DTextures != null)
                {
                    if(loaded2DTextures.Length == 5)
                    {
                        dicomSlices = new Texture2D[5];

                        for(int i = 0; i < loaded2DTextures.Length; i++)
                        {
                            dicomSlices[i] = (Texture2D)loaded2DTextures[i];
                        }
                    }
                }
            }
            
        }
    }
}

