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

    This class serves to manipulate image data from DICOM to 2D/3D Textures and more.

*/

using System;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq; 

using UnityEngine;

using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Codec;
using Dicom.Imaging.Render;
using Dicom.Log;
using Dicom.Network;
using Dicom.Media;

using TMPro;

public static class dicomImageTools
{

    //RENDER DICOM FILE TO TEXTURE2D
    public static Texture2D CreateTextureFromDicom(string path, bool anonymize, ref dicomInfoTools info)
    {
        var stream = File.OpenRead(path);

        var file = DicomFile.Open(stream);

        Debug.Log($"Dicom File at {path} loaded");

        info = new dicomInfoTools(file, anonymize);

        Texture2D texture = new DicomImage(file.Dataset).RenderImage().As<Texture2D>();

        Debug.Log($"Single 2D Texture loaded");
        
        return texture;
    }

    //RENDER FIRST DICOM FILE FROM DICOMDIR TO TEXTURE2D
    public static Texture2D CreateTextureFromFirstDicom(string path, bool anonymize, ref dicomInfoTools info)
    {
        string stream = null;

        DirectoryInfo dicomDirectoryInfo = new DirectoryInfo(path);

        foreach (var dicom in dicomDirectoryInfo.GetFiles(".", SearchOption.AllDirectories)) 
        {
            if (DicomFile.HasValidHeader(dicom.FullName))
            {
                stream = dicom.FullName;
                break;
            }
        }

        Debug.Log(stream);

        var file = DicomFile.Open(stream);

        Debug.Log($"Dicom File at {path} loaded");

        info = new dicomInfoTools(file, anonymize);

        Texture2D texture = new DicomImage(file.Dataset).RenderImage().As<Texture2D>();

        Debug.Log($"Single 2D Texture loaded");
        
        return texture;
    }

    //CREATE ARRAY OF TEXTURE2D SLICES FROM DICOMDIR
    public static Texture2D[] CreateNumberedTextureArrayFromDicomdir(List<string> fileNameList, int numberOfImages)
    {
        if(fileNameList.Count > numberOfImages )
        {
            Texture2D[] slices = new Texture2D[numberOfImages];

            int fileEvenSpaceCounter = fileNameList.Count / numberOfImages;

            int validFileCountAhead = 0;


            for(int i = 0; i < numberOfImages; i++) 
            {
                var tmpDicom = DicomFile.Open(fileNameList[validFileCountAhead]);

                var tmpTex = new DicomImage(tmpDicom.Dataset).RenderImage().As<Texture2D>();
                slices[i] = tmpTex;

                validFileCountAhead += fileEvenSpaceCounter;
                //Debug.Log(validFileCount);
                //Debug.Log(validFileCountAhead);
                //Debug.Log(filesInArrayCount);
            }

            Debug.Log($"Instances loaded into Array: {slices.Length}");

            return slices;
        }
        else
        {
            return null;
        }
        
    }

    //CREATE COLOR ARRAY FROM DICOM DIR. COLOR ARRAY IS USED TO SET PIXELS OF TEXTURE3D
    public static Color[] CreateTextureFromDicomdir(string dirPath, double scaleTexture, List<string> fileNameList, int textureWidth, int textureHeight, int textureDepth)
    {
        var w = textureWidth;
		var h = textureHeight;
		var d = textureDepth;

        var textureCount = 0;

		Color[] colors = new Color[w * h * d];

        Debug.Log($"Populating color array for 3D Texture");

		var slicesCount = -1;

        var sliceCountOffset = Mathf.FloorToInt(d - fileNameList.Count) / 2;
        var invSliceCountOffset = sliceCountOffset + fileNameList.Count;

        Texture2D tex = null;

		for(int z = 0; z < d; z++)
		{
            textureCount++;

            if(z > sliceCountOffset && z < invSliceCountOffset)
            {
                slicesCount++;

                var tmpDicom = DicomFile.Open(fileNameList[slicesCount]);
                tex = new DicomImage(tmpDicom.Dataset).RenderImage().As<Texture2D>();
                if(scaleTexture != 0)
                {
                    double newWidth = tex.width*scaleTexture;
                    double newHeight = tex.height*scaleTexture;
                    TextureScale.Bilinear (tex, Convert.ToInt32(newWidth), Convert.ToInt32(newHeight));
                }
                else if (scaleTexture == 1)
                {
                    // do nothing
                }
            }

			for(int x = 0; x < w; x++)
			{
				for(int y = 0; y < h; y++)
				{
					var idx = x + (y * w) + (z * (w * h));

                    Color c;

                    if(z > sliceCountOffset && z < invSliceCountOffset)
                    {
                        c = tex.GetPixel(x, y);
                    }
					else
                    {
                        c = Color.clear;
                    }

                    colors [idx] = c;

				}
			}
		}

        Debug.Log($"Textures loaded into color array: {textureCount}");

        return colors;
    }

    //CONVERT BYTE ARRAY TO SIGNED SHORT ARRAY
    public static short BAToInt16(byte[ ] bytes, int index)
    {
        short value = BitConverter.ToInt16( bytes, index );
        return value;
    }

    //CONVERT BYTE ARRAY TO UNSIGNED SHORT ARRAY
    public static ushort BAToUInt16(byte[ ] bytes, int index)
    {
        ushort value = BitConverter.ToUInt16( bytes, index );
        return value;
    }

    /////CREATE COLOR ARRAY FROM LIST OF FILES IN DICOMDIR. COLOR ARRAY IS USED TO SET PIXELS OF TEXTURE3D
    public static Color[] CreateColorArrayFromDicomdir(List<string> fileNameList, dicomInfoTools dicomInformation, int textureWidth, int textureHeight, int textureDepth)
    {
        //Debug.Log($"Preparing stuff for color array creation.");

        var w = textureWidth;
		var h = textureHeight;
		var d = textureDepth;

        var textureCount = 0;

		Color[] colors = new Color[w * h * d];

        Debug.Log($"Populating color array for 3D Texture");

		var slicesCount = -1;

        //CENTRE SLICES IN TEXTURE3D
        var sliceCountOffset = Mathf.FloorToInt(d - fileNameList.Count) / 2;
        var invSliceCountOffset = sliceCountOffset + fileNameList.Count;

        //SET INITIAL HOUNSFIELD VALUES
        short hounsfieldUnitMaximumIntenisty = 3071;
        short hounsfieldUnitMinimumIntenisty = -1024;
        short hounsfieldUnitRange = (short)(hounsfieldUnitMaximumIntenisty - hounsfieldUnitMinimumIntenisty);

        //GET FRAME SIZE
        int dicomFrameWidth = dicomInformation.ImageWidth;
        int dicomFrameHeight = dicomInformation.ImageHeight;

        Texture2D newDicomTex = null;

        Color[] dicomOriginalTextureRescaledHU = new Color[dicomFrameWidth * dicomFrameHeight];

        //SET COMPRESSION TYPE
        DicomTransferSyntax defaultDicomTransferSyntax = DicomTransferSyntax.ImplicitVRLittleEndian;

        bool rescale = false;

        //GET RESCALE PARAMETERS
        short rescaleSlope = (short)dicomInformation.ImageRescaleSlope;
        short rescaleIntercept = (short)dicomInformation.ImageRescaleIntercept;
        string modality = dicomInformation.Modality;

        //APPLY RESCALE PARAMETERS ONLY FOR CT IMAGES
        if(modality == "CT")
        {
            rescale = true;
        }

        Debug.Log($"Image frame width: {dicomFrameWidth} pixels and frame height: {dicomFrameHeight} pixels.");
        Debug.Log($"Image Transfer Syntax: {dicomInformation.ImageTransferSyntax}. Applying Decompression Transfer Syntax: {defaultDicomTransferSyntax}.");
        Debug.Log($"Image Rescale Slope: {rescaleSlope} and Rescale Intercept: {rescaleIntercept}.");

        Debug.Log($"Modality is: {modality} and Rescale is set to: {rescale}.");

        //Debug.Log($"Done peparing stuff for color array creation.");

		for(int z = 0; z < d; z++)
		{
            textureCount++;

            if(z > sliceCountOffset && z < invSliceCountOffset)
            {
                slicesCount++;

                //Debug.Log($"Opening Dicom File Nr. {z - sliceCountOffset}.");

                //DECOMPRESS DICOM FILE

                var dicomFileCompressed = DicomFile.Open(fileNameList[slicesCount]);

                var dicomFileUncompressed = new DicomFile();

                if(dicomFileCompressed.Dataset.InternalTransferSyntax.IsEncapsulated)
                {
                    dicomFileUncompressed = dicomFileCompressed.Clone(defaultDicomTransferSyntax);
                }
                else
                {
                    dicomFileUncompressed = dicomFileCompressed;
                }

                dicomFileUncompressed = dicomFileCompressed.Clone(defaultDicomTransferSyntax);

                //GET RAW PIXEL DATA

                var dicomFramePixelData = DicomPixelData.Create(dicomFileUncompressed.Dataset);

                var dicomFrame = dicomFramePixelData.GetFrame(0);

                //Debug.Log($"Copying Bytes into Array.");

                byte[] dicomFrameByteArray = dicomFrame.Data;

                //Debug.Log($"Transforming Bytes to Shorts.");

                //CONVERT BYTES TO SHORTS

                short[] dicomFrameShortArray = new short[(int)Math.Ceiling((double)(dicomFrameByteArray.Length / 2))];
                Buffer.BlockCopy(dicomFrameByteArray, 0, dicomFrameShortArray, 0, dicomFrameByteArray.Length);

                //Debug.Log($"Transforming complete.");

                //ITERATE THROUGH PIXEL DATA, PARALLELIZED FOR SPEED
                Parallel.For(0, dicomFramePixelData.Width * dicomFramePixelData.Height, x =>
                {
                    int dicomFileHUPixel = 0;

                    //APPLY RESCALE PARAMETERS IF IMAGE COMES FROM CT

                    if(rescale)
                    {
                        dicomFileHUPixel = (dicomFrameShortArray[x] * rescaleSlope) + rescaleIntercept;
                    }
                    else
                    {
                        dicomFileHUPixel = dicomFrameShortArray[x];
                    }

                    //CONVERT RAW PIXEL VALUES INTO HOUSFIELD UNITS AND THEN INTO COLOR FLOAT VALUES BETWEEN 0 AND 1
                    
                    float dicomFileRescaledHUIntensity = ((float)dicomFileHUPixel - (float)hounsfieldUnitMinimumIntenisty) / hounsfieldUnitRange;
                    Color readyRescaledHUColor = new Color(dicomFileRescaledHUIntensity, dicomFileRescaledHUIntensity, dicomFileRescaledHUIntensity);
                    dicomOriginalTextureRescaledHU[x] = readyRescaledHUColor;

                });

                //SET COLOR ARRAY TO TEXTURE2D

                newDicomTex = new Texture2D(dicomFramePixelData.Width, dicomFramePixelData.Height, TextureFormat.ARGB32, true, true);
                newDicomTex.SetPixels(dicomOriginalTextureRescaledHU);
                newDicomTex.Apply();

                //Debug.Log($"Texture created. Rescaling texture.");

                //RESCALE IMAGE, TYPICALLY IF LARGER THAN 256X256

                if(w < dicomFramePixelData.Width || h < dicomFramePixelData.Height)
                {
                    TextureScale.Bilinear (newDicomTex, w, h);
                }
            }

            //Debug.Log($"Building color slice from texture.");

            //BUILD COLOR ARRAY FOR 3D TEXTURE
			for(int x = 0; x < w; x++)
			{
			    for(int y = 0; y < h; y++)
				{
                    //INDEX FOR PIXEL VALUE IN COLOR ARRAY
					var idx = x + (y * w) + (z * (w * h));

                    Color c = Color.clear;

                    if(z > sliceCountOffset && z < invSliceCountOffset)
                    {
                        c = newDicomTex.GetPixel(x, (h - y));
                    }

                    colors [idx] = c;
				}
			}

            //Debug.Log($"Slice added to color array.");
		}

        Debug.Log($"Textures loaded into color array: {textureCount}");

        return colors;
    }

    //CREATE ARRAY OF TEXTURE2D SLICES FROM DICOMDIR
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

    //CREATE COLOR ARRAY FROM TEXTURE2D ARRAY
    public static Color[] CreateTexture3DColorArray(Texture2D[] slices)
    {
        var w = slices[0].width;
		var h = slices[0].height;
		var d = NextPow2(slices.Length);

        var textureCount = 0;

		Color[] colors = new Color[w * h * d];

        Debug.Log($"Populating color array for 3D Texture");

		var slicesCount = -1;

        var sliceCountOffset = Mathf.FloorToInt(d - slices.Length) / 2;
        var invSliceCountOffset = sliceCountOffset + slices.Length;

		for(int z = 0; z < d; z++)
		{
            textureCount++;

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
                        c = slices[slicesCount].GetPixel(x, y);
                    }
					else
                    {
                        c = Color.clear;
                    }

                    colors [idx] = c;
				}
			}
		}

        Debug.Log($"Textures loaded into color array: {textureCount}");

        return colors;
    }

    //CREATE TEXTURE3D FROM COLOR ARRAY
    public static Texture3D CreateTexture3D(Color[] colors, int textureWidth, int textureHeight, int textureDepth)
    {
        Texture3D texture = new Texture3D (textureWidth, textureHeight, textureDepth, TextureFormat.RGBA32, true);

		texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        texture.anisoLevel = 6;

        //Debug.Log($"Creating 3D Texture");

        // Copy the color values to the texture
        texture.SetPixels(colors);

        // Apply the changes to the texture and upload the updated texture to the GPU
        texture.Apply();

        //Debug.Log($"3D Texture created");
		
        return texture;
    }

    //SAVE TEXTURE3D AS A UNITY ASSET
    public static void exportTexture3DToAsset(Texture3D texture, string ressourceDestinationPath, string textureRessourceName)
    {
        // Save the texture Asset to your Unity Project - ONLY WORKS IN EDITOR, NOT ON OCULUS
        #if UNITY_EDITOR

            string assetName = Path.Combine(ressourceDestinationPath, (textureRessourceName + ".asset"));

            UnityEditor.AssetDatabase.CreateAsset(texture, assetName);

            Debug.Log($"3D Texture saved as Asset to path: {ressourceDestinationPath}");

        #endif
    }

    //EXPORT TEXTURE3D TO BYTES FILE
    public static void exportTexture3DToFile(Color[] toExport, string fileDestinationPath, string textureArrayName)
    {
        //Debug.Log(textureArrayName);

        Debug.Log("Allocating byte array.");
        byte[] export = new byte[toExport.Length * 4 * 4];
        float[] Export = new float[toExport.Length * 4];
        Debug.Log("Byte array allocated.");

        int colorCount = 0;

        Debug.Log("Filling float array.");

        for(int i = 0; i < Export.Length; i+=4)
        {
            Color c = toExport[colorCount];

            Export[i + 0] = c.r;
            Export[i + 1] = c.g;
            Export[i + 2] = c.b;
            Export[i + 3] = c.a;

            colorCount++;
        }

        Debug.Log("Float array filled.");

        if(export != null && textureArrayName != null && textureArrayName.Length != 0)
        {
            if(!Directory.Exists(fileDestinationPath))
            {
                Directory.CreateDirectory(fileDestinationPath);
                Debug.Log($"Directory {fileDestinationPath} created.");
            }

            Debug.Log("Starting to copy colors to bit array.");
            File.WriteAllBytes(textureArrayName, export);
        }

        Debug.Log($"Color Array for 3D Texture saved to {fileDestinationPath}.");
    }

    //BUILD TEXTURE3D FROM BYTES FILE
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

        Color[] colorImport = new Color[import.Length / (4 * 4)];

        Texture3D texture = new Texture3D (textureWidth, textureHeight, textureDepth, TextureFormat.RGBA32, true);

		texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        texture.anisoLevel = 6;

        if(import != null)
        {
            for(int i = 0; i < colorImport.Length; i++)
            {
                Color c = new Color(BitConverter.ToSingle(import, ((i * 4) + 0)),
                                    BitConverter.ToSingle(import, ((i * 4) + 1)),
                                    BitConverter.ToSingle(import, ((i * 4) + 2)),
                                    BitConverter.ToSingle(import, ((i * 4) + 3)));

                colorImport[i] = c;
                Debug.Log($"Copied pixel {i} of {colorImport.Length}.");
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

    //CREATE A COMPLETE TEXTURE3D FROM A DICOMDIR
    public static Texture3D createTexture3DAsAssetScript(List<string> fileNameList, dicomInfoTools dicomInformation, int textureWidth, int textureHeight, int textureDepth)
    {
        /////Copy pixel data of 2D Textures in array into color array
        var colorsForCubeTexture = CreateColorArrayFromDicomdir(fileNameList, dicomInformation, textureWidth, textureHeight, textureDepth);

        //Debug.Log($" Color: {colorsForCubeTexture[15000000]}");

        /////Map 2D Texture color pixels to 3D Texture
        var cubeTexture = CreateTexture3D(colorsForCubeTexture, textureWidth, textureHeight, textureDepth);
        //Debug.Log($"3D Texture created.");

        return cubeTexture;
    }

    //CREATE A COMPLETE TEXTURE3D FROM A BYTES FILE
    public static Texture3D createTexture3DAsFileScript(string dirPath, string dirName, string fileDestinationPath, double scaleTexture, string textureArrayName, List<string> fileNameList, int textureWidth, int textureHeight, int textureDepth)
    {
        /////Copy pixel data of 2D Textures in array into color array
        var colorsForCubeTexture = CreateTextureFromDicomdir(dirPath, scaleTexture, fileNameList, textureWidth, textureHeight, textureDepth);

        /////Save Color array as file
        dicomImageTools.exportTexture3DToFile(colorsForCubeTexture, fileDestinationPath, textureArrayName);

        /////Map 2D Texture color pixels to 3D Texture
        var cubeTexture = CreateTexture3D(colorsForCubeTexture, textureWidth, textureHeight, textureDepth);

        return cubeTexture;
    }

    //CREATE TEXTURE2DARRAY UNITY TYPE
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

    //ROTATE TEXTURE2D
    public static Texture2D rotateTexture(this Texture2D t)
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

    //FIND NEXT POWER OF 2 OF A NUMBER (E.G. 63->64)
    public static int NextPow2(int a)
	{
		int x = 2;

		while (x < a) 
        {
			x *= 2;
		}
		
		return x;

	}

    //EXPORT A TEXTURE2D TO A PNG FILE
    public static void SaveTextureToPNGFile(Texture2D tex, string destinationPath, string fileName, DateTime nowTime)
    { 
        byte[] bytes;
        bytes = tex.EncodeToPNG();

        string dateFormat = "yyyy|MM|dd";
        string hourFormat = "HH";
        string minuteFormat = "mm";
        string dateString = nowTime.ToString(dateFormat);
        string hourString = nowTime.ToString(hourFormat);
        string minuteString = nowTime.ToString(minuteFormat);
        
        string fullDestinationPath = Path.Combine(destinationPath, dateString);
        System.IO.Directory.CreateDirectory(fullDestinationPath);

        int counter = 0;
        string fullFileName = fileName + "_" + hourString + "h" + minuteString + "m_0" + counter + ".PNG";
        string fullFilePath = Path.Combine(fullDestinationPath, fullFileName);

        while(System.IO.File.Exists(fullFilePath))
        {
            counter++;
            fullFileName = fileName + "_" + hourString + "h" + minuteString + "m_0" + counter + ".PNG";
            fullFilePath = Path.Combine(fullDestinationPath, fullFileName);
        }

        File.WriteAllBytes(fullFilePath, bytes);
    }

    //EXPORT AN ARRAY OF TEXTURE2Ds AS A UNITY ASSET
    public static void SaveTextureArrayAsAssets(Texture2D[] tex, string destinationPath, string fileName)
    { 
        #if UNITY_EDITOR

            for (int i = 0; i < tex.Length; i++)
            {
                string fullFileName = fileName + "_0" + i + ".asset";
                string fullFilePath = Path.Combine(destinationPath, fullFileName);

                tex[i].wrapMode = TextureWrapMode.Clamp;

                UnityEditor.AssetDatabase.CreateAsset(tex[i], fullFilePath);
            }

            Debug.Log($"{tex.Length} Slices saved as 2D Texture Assets to path: {destinationPath}");

        #endif
    }

    //CHECK IF A DIRECTORY IS EMPTY
    public static bool IsDirectoryEmpty(string path)
    {
        return !Directory.EnumerateFileSystemEntries(path).Any();
    }
 
}
