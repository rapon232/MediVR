using System;
using System.Globalization;
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

public static class dicomImageTools
{

    public static Texture2D CreateTextureFromDicom(string path, bool anonymize, ref dicomInfoTools info)
    {
        var stream = File.OpenRead(path);

        var file = DicomFile.Open(stream);

        Debug.Log($"Dicom File at {path} loaded");

        if(anonymize)
        {
            var anonymizer = new DicomAnonymizer();
            anonymizer.AnonymizeInPlace(file);
        }

        //dump = file.WriteToString();

        info.setDicomInfo(file);

        Texture2D texture = new DicomImage(file.Dataset).RenderImage().As<Texture2D>();

        Debug.Log($"Single 2D Texture loaded");
        
        return texture;
    }

    public static Texture2D[] CreateNumberedTextureArrayFromDicomdir(string dirPath, bool anonymize, ref dicomInfoTools info, int numberOfImages)
    {
        var dicomDirectoryInfo = new DirectoryInfo(dirPath);

        int fileCount = dicomDirectoryInfo.GetFiles().Length;

        Debug.Log($"Files found in Directory: {fileCount}");

        Debug.Log($"Loading {numberOfImages} Dicom files from Directory {dirPath} into Array");

        int validFileCount = 0;

        foreach (var file in dicomDirectoryInfo.GetFiles(".", SearchOption.AllDirectories)) 
        {
            if (DicomFile.HasValidHeader(file.FullName))
            {
                validFileCount++;
            }
        }

        Debug.Log($"Valid Dicom files found in Directory: {validFileCount}");

        Texture2D[] slices = new Texture2D[numberOfImages];

        int fileEvenSpaceCounter = validFileCount / numberOfImages;

        var filesInArrayCount = 0;

        validFileCount = 0;

        var validFileCountAhead = 0;

        bool infoGet = false;

        foreach (var file in dicomDirectoryInfo.GetFiles(".", SearchOption.AllDirectories)) 
        {
            if(filesInArrayCount != numberOfImages)
            {
                if (DicomFile.HasValidHeader(file.FullName))
                {
                    if(validFileCount == validFileCountAhead)
                    {
                        var tmpDicom = DicomFile.Open(file.FullName);

                        if(!infoGet)
                        {
                            if(anonymize)
                            {
                                var anonymizer = new DicomAnonymizer();
                                anonymizer.AnonymizeInPlace(tmpDicom);
                            }

                            info.setDicomInfo(tmpDicom);
                            infoGet = true;
                        }

                        var tmpTex = new DicomImage(tmpDicom.Dataset).RenderImage().As<Texture2D>();
                        slices[filesInArrayCount] = tmpTex;

                        validFileCountAhead += fileEvenSpaceCounter;
                        filesInArrayCount++;
                        //Debug.Log(validFileCount);
                        //Debug.Log(validFileCountAhead);
                        //Debug.Log(filesInArrayCount);
                    }
                    validFileCount++;
                }
            }
        }

        Debug.Log($"Instances loaded into Array: {slices.Length}");

        return slices;
    }

    public static Texture2D[] CreateTextureArrayFromDicomdir(string dirPath, double scaleTexture)
    {
        var dicomDirectoryInfo = new DirectoryInfo(dirPath);

        int fileCount = dicomDirectoryInfo.GetFiles().Length;

        Debug.Log($"Files found in Directory: {fileCount}");
        Debug.Log($"Loading Dicom files from Directory {dirPath} into Array");

        int validFileCount = 0;

        foreach (var file in dicomDirectoryInfo.GetFiles(".", SearchOption.AllDirectories)) 
        {
            if (DicomFile.HasValidHeader(file.FullName))
            {
                validFileCount++;
            }
        }

        Debug.Log($"Valid Dicom files found in Directory: {validFileCount}");

        Texture2D[] slices = new Texture2D[validFileCount];

        validFileCount = 0;

        foreach (var file in dicomDirectoryInfo.GetFiles(".", SearchOption.AllDirectories)) 
        {
            if (DicomFile.HasValidHeader(file.FullName))
            {
                var tmpDicom = DicomFile.Open(file.FullName);
                var tmpTex = new DicomImage(tmpDicom.Dataset).RenderImage().As<Texture2D>();
                if(scaleTexture != 0)
                {
                    double newWidth = tmpTex.width*scaleTexture;
                    double newHeight = tmpTex.height*scaleTexture;
                    TextureScale.Bilinear (tmpTex, Convert.ToInt32(newWidth), Convert.ToInt32(newHeight));
                }
                else if (scaleTexture == 1)
                {
                    // do nothing
                }
                slices[validFileCount] = tmpTex;
                validFileCount++;
            }
        }

        Debug.Log($"Instances loaded into Array: {slices.Length}");

        return slices;
    }

    public static Color[] CreateTexture3DColorArray(Texture2D[] slices)
    {
        var w = slices[0].width;
		var h = slices[0].height;
		var d = NextPow2(slices.Length);

        var textureCount = 0;

        // skip some slices if we can't fit it all in
		var countOffset = (slices.Length - 1) / (float)d;

		Color[] colors = new Color[w * h * d];

        Debug.Log($"Populating color array for 3D Texture");

		var slicesCount = -1;
		//var sliceCountFloat = 0f;

        var sliceCountOffset = Mathf.FloorToInt(d - slices.Length) / 2;
        var invSliceCountOffset = sliceCountOffset + slices.Length;

		for(int z = 0; z < d; z++)
		{
            textureCount++;
			//sliceCountFloat += countOffset;
			//slicesCount = Mathf.FloorToInt(sliceCountFloat);

            if(z > sliceCountOffset && z < invSliceCountOffset)
            {
                slicesCount++; 
            }

			for(int x = 0; x < w; x++)
			{
				for(int y = 0; y < h; y++)
				{
					var idx = x + (y * w) + (z * (w * h));

                    Color c;

                    if(z > sliceCountOffset && z < invSliceCountOffset)
                    {
                        //c = slices[slicesCount].GetPixelBilinear(x / (float)w, y / (float)h);
                        c = slices[slicesCount].GetPixel(x, y);
                    }
					else
                    {
                        c = Color.clear;
                    }

					//if (!(c.r < 0.1f && c.g < 0.1f && c.b < 0.1f))
						colors [idx] = c;

				}
			}
		}

        Debug.Log($"Textures loaded into color array: {textureCount}");

        return colors;

    }

    public static Texture3D CreateTexture3D(Texture2D[] slices, Color[] colors)
    {
        Texture3D texture = new Texture3D (slices[0].width, slices[0].height, NextPow2(slices.Length), TextureFormat.RGBA32, true);

		texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        texture.anisoLevel = 6;

        Debug.Log($"Creating 3D Texture");

        // Copy the color values to the texture
        texture.SetPixels(colors);

        // Apply the changes to the texture and upload the updated texture to the GPU
        texture.Apply();

        Debug.Log($"3D Texture created");
		
        return texture;
    }

    public static void exportTexture3DToAsset(Texture3D texture, string ressourceDestinationPath, string textureRessourceName)
    {
        // Save the texture Asset to your Unity Project

        string assetName = "Assets/Ressources" + textureRessourceName + ".asset";

        UnityEditor.AssetDatabase.CreateAsset(texture, assetName);

        Debug.Log($"3D Texture saved as Asset to path: {assetName}");
    }

    public static void exportTexture3DToFile(Texture3D texture, Color[] toExport, string fileDestinationPath, string textureArrayName)
    {
        //string objectPath = fileDestinationPath + "/" + textureAssetName + "_3DTexture_Color_Array" + texture.width + "x" + texture.height + "x" + texture.depth + ".txt";

        //Debug.Log(textureArrayName);

        byte[] export = new byte[toExport.Length * 4];

        int colorCount = 0;

        for(int i = 0; i < export.Length; i+=4)
        {
            UnityEngine.Color32 c = toExport[colorCount];

            export[i + 0] = c.r;
            export[i + 1] = c.g;
            export[i + 2] = c.b;
            export[i + 3] = c.a;

            colorCount++;
        }

        if(export != null && textureArrayName != null && textureArrayName.Length != 0)
        {
            if(!Directory.Exists(fileDestinationPath))
            {
                Directory.CreateDirectory(fileDestinationPath);
                Debug.Log($"Directory {fileDestinationPath} created.");
            }

            File.WriteAllBytes(textureArrayName, export);
        }

        Debug.Log($"Color Array for 3D Texture saved to {fileDestinationPath}.");
    }

    public static Texture3D importColorArrayTo3DTexture(string textureArrayName, int textureWidth, int textureHeight, int textureDepth)
    {
        byte[] import = null;

        if(textureArrayName != null && textureArrayName.Length != 0)
        {
            import = File.ReadAllBytes(textureArrayName);
            Debug.Log($"File at {textureArrayName} read.");
        }
        else
        {
            Debug.Log($"File at {textureArrayName} missing.");
        }

        Debug.Log($"Loading color array for 3D Texture.");

        Color[] colorImport = new Color[import.Length / 4];

        Texture3D texture = new Texture3D (textureWidth, textureHeight, textureDepth, TextureFormat.RGBA32, true);

		texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        texture.anisoLevel = 6;

        if(import != null)
        {
            for(int i = 0; i < import.Length; i+=4)
            {
                Color c = new Color(import[i + 0],
                                    import[i + 1],
                                    import[i + 2],
                                    import[i + 3]);

                colorImport[i / 4] = c;
            }

            Debug.Log($"Creating 3D Texture...");

            // Copy the color values to the texture
            texture.SetPixels(colorImport);

            // Apply the changes to the texture and upload the updated texture to the GPU
            texture.Apply();

            Debug.Log($"3D Texture created from file {textureArrayName}.");
        }
        else
        {
            texture = null;
            Debug.Log($"3D Texture empty.");
        }

        return texture;
    }

    public static Texture3D createTexture3DAsAssetScript(string dirPath, string dirName, string ressourceDestinationPath, double scaleTexture, string textureRessourceName)
    {
        /////Load all slices from directory into array of 2D Textures
        var textureArray = CreateTextureArrayFromDicomdir(dirPath, scaleTexture);

        /////Copy pixel data of 2D Textures in array into color array
        var colorsForCubeTexture = CreateTexture3DColorArray(textureArray);

        /////Map 2D Texture color pixels to 3D Texture
        var cubeTexture = CreateTexture3D(textureArray, colorsForCubeTexture);
        Debug.Log($"3D Texture created from path {dirPath}");

        /////Save 3D Texture as Asset in Unity Editor
        #if UNITY_EDITOR
            dicomImageTools.exportTexture3DToAsset(cubeTexture, ressourceDestinationPath, textureRessourceName);
        #endif

        return cubeTexture;
    }

    public static Texture3D createTexture3DAsFileScript(string dirPath, string dirName, string fileDestinationPath, double scaleTexture, string textureArrayName)
    {
        /////Load all slices from directory into array of 2D Textures
        var textureArray = CreateTextureArrayFromDicomdir(dirPath, scaleTexture);

        /////Copy pixel data of 2D Textures in array into color array
        var colorsForCubeTexture = CreateTexture3DColorArray(textureArray);

        /////Map 2D Texture color pixels to 3D Texture
        var cubeTexture = CreateTexture3D(textureArray, colorsForCubeTexture);

        /////Save 3D Texture as Asset in Unity Editor
        dicomImageTools.exportTexture3DToFile(cubeTexture, colorsForCubeTexture, fileDestinationPath, textureArrayName);

        return cubeTexture;
    }

    public static Texture2DArray CreateTexture2DArray(int w, int h, int d, Texture2D[] slices)
    {
        // Create Texture2DArray
        Texture2DArray texture2DArray = new Texture2DArray(w, h, d, TextureFormat.RGBA32, true, false);

        // Apply settings
        texture2DArray.wrapMode = TextureWrapMode.Clamp;
        texture2DArray.filterMode = FilterMode.Bilinear;
        texture2DArray.anisoLevel = 6;

        Debug.Log($"Creating 2DTextureArray");

        // Loop through ordinary textures and copy pixels to the Texture2DArray
        for (int i = 0; i < slices.Length; i++)
        {
            texture2DArray.SetPixels(slices[i].GetPixels(0),  
                i, 0);
        }

         Debug.Log($"Textures Loaded: {slices.Length}");

        // Apply our changes
        texture2DArray.Apply();

        Debug.Log($"Texture2DArray created");

        return texture2DArray;
    }

    public static Texture2D rotate(this Texture2D t)
    {
        Texture2D newTexture = new Texture2D(t.height, t.width, t.format, false);

        for(int i=0; i<t.width; i++)
        {
            for(int j=0; j<t.height; j++)
            {
                newTexture.SetPixel(j, i, t.GetPixel(i, t.height-j));
            }
        }

        Debug.Log($"2D Texture rotated");

        newTexture.Apply();
        return newTexture;
    }

    public static int NextPow2(int a)
	{
		int x = 2;

		while (x < a) 
        {
			x *= 2;
		}
		
		return x;

	}

    /*public static string getTag(DicomFile _file, DicomTag _tag)
    { 
        var tag = "N/A";

        if(_file.Dataset.Contains(_tag))
        {
            
            var temp = _file.Dataset.Get<string>(_tag);

            if (temp != null){
                tag = temp;
            }
        }

        return tag;
    }*/
        

//for opening DICOMDIR

/*var dicomDirectory = DicomDirectory.Open(dirPath);

        foreach (var patientRecord in dicomDirectory.RootDirectoryRecordCollection)
        {
            Debug.Log(
                $"Patient: {patientRecord.Get<string>(DicomTag.PatientName)} ({patientRecord.Get<string>(DicomTag.PatientID)})");

            foreach (var studyRecord in patientRecord.LowerLevelDirectoryRecordCollection)
            {
                Debug.Log(String.Format("\tStudy: {0}", studyRecord.Get<string>(DicomTag.StudyInstanceUID))); 

                foreach (var seriesRecord in studyRecord.LowerLevelDirectoryRecordCollection)
                {
                    Debug.Log(String.Format("\t\tSeries: {0}", seriesRecord.Get<string>(DicomTag.SeriesInstanceUID)));

                    foreach (var imageRecord in seriesRecord.LowerLevelDirectoryRecordCollection)
                    {
                        slices++;
                        Debug.Log(String.Format(
                            "\t\t\tImage: {0} [{1}]",
                            imageRecord.Get<string>(DicomTag.ReferencedSOPInstanceUIDInFile),
                            imageRecord.Get<string>(DicomTag.ReferencedFileID)));   

                        var dicomDataset = imageRecord.Get(DicomTag.IconImageSequence).Items.First();                     
                    }
                }
            }
        }*/








}
