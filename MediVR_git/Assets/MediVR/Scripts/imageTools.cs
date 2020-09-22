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

public static class imageTools
{

    public static Texture2D CreateTextureFromDicom (string path, bool anonymize, ref string dicomInfo)
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

        dicomInfo = DumpDicomInfo(file);

        Texture2D texture = new DicomImage(file.Dataset).RenderImage().As<Texture2D>();

        Debug.Log($"Single 2D Texture loaded");
        
        return texture;
    }

    public static Texture2D[] CreateNumberedTextureArrayFromDicomdir (string dirPath, bool anonymize, ref string dicomInfo, int numberOfImages)
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
                            dicomInfo = DumpDicomInfo(tmpDicom);
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

    public static Texture2D[] CreateTextureArrayFromDicomdir (string dirPath, double scaleTexture)
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



    public static Color[] CreateTexture3DColorArray (Texture2D[] slices)
    {
        var w = slices[0].width;
		var h = slices[0].height;
		var d = NearestSuperiorPow2(slices.Length);

        var textureCount = 0;

        // skip some slices if we can't fit it all in
		var countOffset = (slices.Length - 1) / (float)d;

		Color[] colors = new Color[w * h * d];

        Debug.Log($"Populating color array for 3D Texture");

		var slicesCount = 0;
		var sliceCountFloat = 0f;
		for(int z = 0; z < d; z++)
		{
            textureCount++;
			sliceCountFloat += countOffset;
			slicesCount = Mathf.FloorToInt(sliceCountFloat);
			for(int x = 0; x < w; x++)
			{
				for(int y = 0; y < h; y++)
				{
					var idx = x + (y * w) + (z * (w * h));

					Color c = slices[slicesCount].GetPixelBilinear(x / (float)w, y / (float)h); 

					if (!(c.r < 0.1f && c.g < 0.1f && c.b < 0.1f))
						colors [idx] = c;

				}
			}
		}

        Debug.Log($"Textures loaded into color array: {textureCount}");

        return colors;

    }

    public static Texture3D CreateTexture3D (Texture2D[] slices, Color[] colors)
    {
        Texture3D texture = new Texture3D (slices[0].width, slices[0].height, NearestSuperiorPow2(slices.Length), TextureFormat.RGBA32, true);

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

    public static void SaveTexture3DToAsset (Texture3D texture, string textureAssetName)
    {
        #if UNITY_EDITOR

            // Save the texture Asset to your Unity Project

            string assetName = "Assets/Resources/Textures/" + textureAssetName + "_3DTexture_" + texture.width + "x" + texture.height + "x" + texture.depth + ".asset";

            UnityEditor.AssetDatabase.CreateAsset(texture, assetName);

            Debug.Log($"3D Texture saved as Asset to path: {assetName}");

        #endif

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

    public static Texture3D CreateTexture3DAsAssetScript (string dirPath, string dirName, double scaleTexture)
    {
        /////Load all slices from directory into array of 2D Textures
        var textureArray = CreateTextureArrayFromDicomdir(dirPath, scaleTexture);

        /////Copy pixel data of 2D Textures in array into color array
        var colorsFor3DTexture = CreateTexture3DColorArray (textureArray);

        /////Map 2D Texture color pixels to 3D Texture
        var cubeTexture = CreateTexture3D(textureArray, colorsFor3DTexture);

        /////Save 3D Texture as Asset in Unity Editor
        SaveTexture3DToAsset (cubeTexture, dirName);

        return cubeTexture;
    }

    public static Texture2D rotate (this Texture2D t)
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

    public static int NearestSuperiorPow2(int n)
	{
		int x = 2;

		while (x < n) {
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

    public static string DumpDicomInfo(DicomFile _file)
    {
        string dumpPatientId = "N/A";
        string dumpPatientSex = "N/A";
        string dumpPatientName = "N/A";
        string dumpPatientBd = "N/A";
        string dumpStudyId = "N/A";
        string dumpStudyDate = "N/A";
        string dumpStudyTime = "N/A";
        string dumpDoctorName = "N/A";
        string dumpModality = "N/A";
        string dumpModalityManufacturer = "N/A";
        
        if(_file.Dataset.Contains(DicomTag.PatientID))
        {
            dumpPatientId = _file.Dataset.Get<string>(DicomTag.PatientID).ToString();
        }
        if(_file.Dataset.Contains(DicomTag.PatientSex))
        {
            dumpPatientSex = _file.Dataset.Get<string>(DicomTag.PatientSex, "N/A").ToString();
        }
        if(_file.Dataset.Contains(DicomTag.PatientName))
        {
            dumpPatientName = _file.Dataset.Get<string>(DicomTag.PatientName, "N/A").ToString();
        }
        if(_file.Dataset.Contains(DicomTag.PatientBirthDate))
        {
            dumpPatientBd = _file.Dataset.Get<string>(DicomTag.PatientBirthDate, "N/A").ToString();
        }
        if(_file.Dataset.Contains(DicomTag.StudyID))
        {
            dumpStudyId= _file.Dataset.Get<string>(DicomTag.StudyID, "N/A").ToString();
        }
        if(_file.Dataset.Contains(DicomTag.StudyDate))
        {
            dumpStudyDate = _file.Dataset.Get<string>(DicomTag.StudyDate, "N/A").ToString();
        }
        if(_file.Dataset.Contains(DicomTag.StudyTime))
        {
            dumpStudyTime = _file.Dataset.Get<string>(DicomTag.StudyTime, "N/A").ToString();
        }
        if(_file.Dataset.Contains(DicomTag.ReferringPhysicianName))
        {
            dumpDoctorName = _file.Dataset.Get<string>(DicomTag.ReferringPhysicianName, "N/A").ToString();
        }
        if(_file.Dataset.Contains(DicomTag.Modality))
        {
            dumpModality = _file.Dataset.Get<string>(DicomTag.Modality, "N/A").ToString();
        }
        if(_file.Dataset.Contains(DicomTag.Manufacturer))
        {
            dumpModalityManufacturer = _file.Dataset.Get<string>(DicomTag.Manufacturer, "N/A").ToString(); 
        }
            
        var dicomInfo =   "<b>Patient Info:\n\n</b>" +
                        $"ID: N/A, Gender: {dumpPatientSex}\n" + 
                        $"Name: {dumpPatientName}, Birth Date: {dumpPatientBd}\n" +
                        "\n<b>Study Info:</b>\n\n" +
                        $"ID: {dumpStudyId}, Date: {dumpStudyDate}, Time: {dumpStudyTime}\n" +
                        $"Referring Physician: {dumpDoctorName}\n" +
                        "\n<b>Modality Info:</b>\n\n" +
                        $"Modality: {dumpModality}, Manufacturer: {dumpModalityManufacturer}\n";

        /*Debug.Log(dumpPatientId);
        Debug.Log(dumpPatientSex);
        Debug.Log(dumpPatientName);
        Debug.Log(dumpPatientBd);
        Debug.Log(dumpStudyId);
        Debug.Log(dumpStudyDate);
        Debug.Log(dumpStudyTime);
        Debug.Log(dumpDoctorName);
        Debug.Log(dumpModality);
        Debug.Log(dumpModalityManufacturer);*/
            
        return dicomInfo;
    }
        

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
