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

    private GameObject dicomImporter = null;
    private initialImportDicom initialImportScript = null;


    // Start is called before the first frame update
    void Start()
    {
        //////// PATHS

        dicomImporter = GameObject.Find("Dicom_Importer");
        initialImportScript = dicomImporter.GetComponent<initialImportDicom>();
        dirName = initialImportScript.dicomFileDirectory;
        destinationTextureDirName = initialImportScript.assetDestinationDirectory;
        textureDestinationPath = initialImportScript.savedTextureDestinationPath;

        //////// LOAD 3D TEXTURE

        pathTo3DTextures = "MediVR/Textures/" + destinationTextureDirName + "/" + dirName;

        //threeDimTexture = Resources.Load<Texture3D>("MediVR/Textures/" + destinationTextureDirName + "/" + textureRessourceName); 
        loadedTextures = Resources.LoadAll(pathTo3DTextures, typeof(Texture3D)); //TRY TO LOAD 3D TEXTURE FROM FOLDER

        if(loadedTextures != null)
        {
            if(loadedTextures.Length > 0)
            {
                //Debug.Log($"{loadedTextures.Length} 3D Texture(s) loaded. Using first texture from List.");

                threeDimTexture = (Texture3D)loadedTextures[0];

                //Debug.Log($"{loadedTextures[0].name}");

                //////// LOAD METADATA

                metadataName = pathTo3DTextures + "/" + loadedTextures[0].name + "_MetaData";

                //metadataName = loadedTextures[0].name + "_MetaData.XML";

                metaData = Resources.Load<TextAsset>(metadataName).text;

                metaData = metaData.Replace("&#x0;", "");

                //Debug.Log($"{metaData}");

                if(metaData != null)
                {
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

