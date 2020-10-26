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

    public static Texture2D CreateTextureFromFirstDicom(string path, bool anonymize, ref dicomInfoTools info)
    {
        string stream = null;

        DirectoryInfo dicomDirectoryInfo = new DirectoryInfo(path);

        //var stream = dir.GetFiles().Select(fi => fi.Name).FirstOrDefault();

        foreach (var dicom in dicomDirectoryInfo.GetFiles(".", SearchOption.AllDirectories)) 
        {
            if (DicomFile.HasValidHeader(dicom.FullName))
            {
                stream = dicom.FullName;
                break;
            }
        }

        Debug.Log(stream);

        //var stream = File.OpenRead(path);

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

    public static Color[] CreateTextureFromDicomdir(string dirPath, double scaleTexture, List<string> fileNameList, int textureWidth, int textureHeight, int textureDepth)
    {
        var w = textureWidth;
		var h = textureHeight;
		var d = textureDepth;

        var textureCount = 0;

		Color[] colors = new Color[w * h * d];

        Debug.Log($"Populating color array for 3D Texture");

		var slicesCount = -1;
		//var sliceCountFloat = 0f;

        var sliceCountOffset = Mathf.FloorToInt(d - fileNameList.Count) / 2;
        var invSliceCountOffset = sliceCountOffset + fileNameList.Count;

        Texture2D tex = null;

		for(int z = 0; z < d; z++)
		{
            textureCount++;
			//sliceCountFloat += countOffset;
			//slicesCount = Mathf.FloorToInt(sliceCountFloat);

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
                        //c = slices[slicesCount].GetPixelBilinear(x / (float)w, y / (float)h);
                        c = tex.GetPixel(x, y);
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

    public static short BAToInt16(byte[ ] bytes, int index)
    {
        short value = BitConverter.ToInt16( bytes, index );
        return value;
    }

    public static ushort BAToUInt16(byte[ ] bytes, int index)
    {
        ushort value = BitConverter.ToUInt16( bytes, index );
        return value;
    }

    public static Color[] CreateColorArrayFromDicomdir(string dirPath, List<string> fileNameList, dicomInfoTools dicomInformation, int textureWidth, int textureHeight, int textureDepth)
    {
        Debug.Log($"Preparing stuff for color array creation.");

        var w = textureWidth;
		var h = textureHeight;
		var d = textureDepth;

        var textureCount = 0;

		Color[] colors = new Color[w * h * d];

        Debug.Log($"Populating color array for 3D Texture");

		var slicesCount = -1;
		//var sliceCountFloat = 0f;

        var sliceCountOffset = Mathf.FloorToInt(d - fileNameList.Count) / 2;
        var invSliceCountOffset = sliceCountOffset + fileNameList.Count;

        //Texture2D tex = null;
        //IPixelData pixelData = null;
        
        //ushort dicomFileMinimumIntensity = ushort.MaxValue;
        //ushort dicomFileMaximumIntensity = ushort.MinValue;

        short hounsfieldUnitMaximumIntenisty = 3071;
        short hounsfieldUnitMinimumIntenisty = -1024;
        short hounsfieldUnitRange = (short)(hounsfieldUnitMaximumIntenisty - hounsfieldUnitMinimumIntenisty);
        //short dicomFileIntensityRange = dicomFileMaximumIntensity - dicomFileMinimumIntensity;

        int dicomFrameWidth = dicomInformation.ImageWidth;
        int dicomFrameHeight = dicomInformation.ImageHeight;

        Texture2D newDicomTex = null;

        //short[,] dicomFilePixelHUIntensities = new short[dicomFrameWidth, dicomFrameHeight];
        Color[] dicomOriginalTextureRescaledHU = new Color[dicomFrameWidth * dicomFrameHeight];

        DicomTransferSyntax dicomFileTransferSyntax = dicomInformation.ImageTransferSyntax;

        DicomTransferSyntax defaultDicomTransferSyntax = dicomInformation.DefaultDicomTransferSyntax;

        short rescaleSlope = (short)dicomInformation.ImageRescaleSlope;
        short rescaleIntercept = (short)dicomInformation.ImageRescaleIntercept;

        Debug.Log($"Image frame width: {dicomFrameWidth} pixels and frame height: {dicomFrameHeight} pixels.");
        Debug.Log($"Image Transfer Syntax: {dicomFileTransferSyntax}. Applying Decompression Transfer Syntax: {defaultDicomTransferSyntax}");
        Debug.Log($"Image Rescale Slope: {rescaleSlope} and Rescale Intercept: {rescaleIntercept}");

        /*var dicomFile = DicomFile.Open(fileNameList[0]);

        var dicomFilePixelData = DicomPixelData.Create(dicomFile.Dataset);

        //var fileFrame = filePixelData.GetFrame(0);

        int dicomFrameWidth = dicomFilePixelData.Width;
        int dicomFrameHeight = dicomFilePixelData.Height;

        Debug.Log($"Image frame width: {dicomFrameWidth} pixels and frame height: {dicomFrameHeight} pixels.");

        Texture2D newDicomTex = null;

        //short[,] dicomFilePixelHUIntensities = new short[dicomFrameWidth, dicomFrameHeight];
        Color[] dicomOriginalTextureRescaledHU = new Color[dicomFrameWidth * dicomFrameHeight];

        //var dts  = DicomTransferSyntax.Parse( compressed.Dataset.Get ( DicomTag.TransferSyntaxUID, compressed.Dataset.InternalTransferSyntax.ToString ( ) ) ) ;

        var dicomFileTransferSyntax = dicomFile.Dataset.InternalTransferSyntax.ToString();

        DicomTransferSyntax defaultDicomTransferSyntax = DicomTransferSyntax.ImplicitVRLittleEndian;

        Debug.Log($"Image Transfer Syntax: {dicomFileTransferSyntax}. Applying Decompression Transfer Syntax: {defaultDicomTransferSyntax}");

        short rescaleSlope, rescaleIntercept;

        if(dicomFile.Dataset.Contains(DicomTag.RescaleSlope))
        {
            rescaleSlope = (short)dicomFile.Dataset.Get<int>(DicomTag.RescaleSlope);
            Debug.Log($"Rescale Slope found in dataset.");
        }
        else
        {
            rescaleSlope = 1;
            Debug.Log($"Rescale Slope NOT found in dataset. Defaulting to 1.");
        }

        if(dicomFile.Dataset.Contains(DicomTag.RescaleIntercept))
        {
            rescaleIntercept = (short)dicomFile.Dataset.Get<int>(DicomTag.RescaleIntercept);
            Debug.Log($"Rescale Intercept found in dataset.");
        }
        else
        {
            rescaleIntercept = -1024;
            Debug.Log($"Rescale Intercept NOT found in dataset. Defaulting to -1024.");
        }

        Debug.Log($"Image Rescale Slope: {rescaleSlope} and Rescale Intercept: {rescaleIntercept}");*/

        Debug.Log($"Done peparing stuff for color array creation.");


		for(int z = 0; z < d; z++)
		{
            textureCount++;
			//sliceCountFloat += countOffset;
			//slicesCount = Mathf.FloorToInt(sliceCountFloat);

            if(z > sliceCountOffset && z < invSliceCountOffset)
            {
                slicesCount++;

                

                //var di = new DicomImage(tmpDicom.Dataset);
                //var pixeldatatest = di.PixelData; // returns DicomPixelData type
                //Debug.Log($"{pixeldatatest.NumberOfFrames}");
                //pixelData = PixelDataFactory.Create(pixeldatatest, 0); // returns IPixelData type
                //Debug.Log($"{pixelData.Width}");
                //Debug.Log($"{pixelData.Height}");

                //var pixelData = DicomPixelData.Create(dataset, true);
                //pixelData.Width
                //pixelData.Height 

                //var tmpDicom = DicomFile.Open(fileNameList[slicesCount]);
                /*var header = DicomPixelData.Create(tmpDicom.Dataset);
                Debug.Log($"{header.NumberOfFrames}");
                pixelData = PixelDataFactory.Create(header, 0);

                Debug.Log($"{pixelData.Width}");
                Debug.Log($"{pixelData.Height}");*/

                Debug.Log($"Opening Dicom File Nr. {z - sliceCountOffset}.");

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

                var dicomFramePixelData = DicomPixelData.Create(dicomFileUncompressed.Dataset);

                var dicomFrame = dicomFramePixelData.GetFrame(0);

                Debug.Log($"Copying Bytes into Array.");

                byte[] dicomFrameByteArray = dicomFrame.Data;

                Debug.Log($"Transforming Bytes to Shorts.");

                short[] dicomFrameShortArray = new short[(int)Math.Ceiling((double)(dicomFrameByteArray.Length / 2))];
                Buffer.BlockCopy(dicomFrameByteArray, 0, dicomFrameShortArray, 0, dicomFrameByteArray.Length);

                Debug.Log($"Transforming complete.");

                Debug.Log($"Reversing short array.");

                Array.Reverse(dicomFrameShortArray);

                Debug.Log($"Short Array reversed. Begining transform to color array.");

                //ushort[] shorts = new ushort[bytes.Length/2];
                //short[,] shorts = new short[pixel_data.Width, pixel_data.Height];

                int count = 0;

                /*for(int x = 0; x < bytes.Length; x+=2)
                {
                    shorts[count] = BAToInt16(bytes, x);
                    count++;
                }*/

                //Debug.Log($"byte array lentgh: {bytes.Length} and short array length: {shorts.Length}");

                //for(int y = dicomFramePixelData.Height - 1; y >= 0 ; y--)
                //{
                    for(int x = 0; x < dicomFramePixelData.Width * dicomFramePixelData.Height ; x++)
                    {
                        //var dicomFilePixel = BAToInt16(dicomFrameByteArray, count);
                        //var dicomFilePixel = (short)(dicomFrameShortArray[count] | (dicomFrameByteArray[count+1] << 8));
                        var dicomFileHUPixel = (dicomFrameShortArray[x] * rescaleSlope) + rescaleIntercept;
                        float dicomFileRescaledHUIntensity = ((float)dicomFileHUPixel - (float)hounsfieldUnitMinimumIntenisty) / hounsfieldUnitRange;
                        Color readyRescaledHUColor = new Color(dicomFileRescaledHUIntensity, dicomFileRescaledHUIntensity, dicomFileRescaledHUIntensity);
                        dicomOriginalTextureRescaledHU[x] = readyRescaledHUColor;

                        //dicomFilePixelHUIntensities[x,y] = (short)dicomFileHUPixel;

                        //Debug.Log($"byte:{bytes[count]} and byte:{bytes[count+1]} make short: {shorts[x,y]} ");

                        /*if(dicomFilePixel > dicomFileMaximumIntensity)
                        {
                            dicomFileMaximumIntensity = dicomFilePixel;
                        }
                        if(dicomFilePixel < dicomFileMinimumIntensity)
                        {
                            dicomFileMinimumIntensity = dicomFilePixel;
                        }*/

                        count+=2;
                    }
                //}

                //Debug.Log($"Minimum intensity: {min} and maximum intenisty in frame: {max}.");


                //var shorts = Array.ConvertAll(bytes, b => (short)b);

                //count = 0;

                Debug.Log($"Creating 2D Texture from colors.");

                newDicomTex = new Texture2D(dicomFramePixelData.Width, dicomFramePixelData.Height, TextureFormat.ARGB32, true, true);
                newDicomTex.SetPixels(dicomOriginalTextureRescaledHU);
                newDicomTex.Apply();

                Debug.Log($"Texture created. Rescaling texture.");

                if(w < dicomFramePixelData.Width || h < dicomFramePixelData.Height)
                {
                    TextureScale.Bilinear (newDicomTex, w, h);
                }

                Debug.Log($"Texture rescaled.");

                /*for(int x = 0; x < bytes.Length; x+=2)
			    {
                    Debug.Log($"byte:{bytes[x]} and byte:{bytes[x+1]} make short: {shorts[count]} ");
                    count++;
                }*/

                /*for(int x = 0; x < bytes.Length; x+=2)
                {
                    BAToInt16(bytes, x);
                }*/

                //tex = new DicomImage(tmpDicom.Dataset).RenderImage().As<Texture2D>();
                /*if(scaleTexture != 0)
                {
                    double newWidth = tex.width*scaleTexture;
                    double newHeight = tex.height*scaleTexture;
                    TextureScale.Bilinear (tex, Convert.ToInt32(newWidth), Convert.ToInt32(newHeight));
                }
                else if (scaleTexture == 1)
                {
                    // do nothing
                }*/
            }

            Debug.Log($"Building color slice from texture.");

			for(int x = 0; x < w; x++)
			{
				for(int y = 0; y < h; y++)
				{
					var idx = x + (y * w) + (z * (w * h));

                    Color c = Color.clear;

                    if(z > sliceCountOffset && z < invSliceCountOffset)
                    {
                        //c = slices[slicesCount].GetPixelBilinear(x / (float)w, y / (float)h);
                        c = newDicomTex.GetPixel(x, y);
                       /* if (pixelData is Dicom.Imaging.Render.GrayscalePixelDataU16)
                        {
                            var pixel = pixelData.GetPixel(x,y);
                            //Console.WriteLine("{0}",Convert.ToSingle(pixelData.GetPixel(i,j)));
                            Debug.Log($"{pixel}");
                        }*/

                        //float dicomFileRescaledHUIntensity = ((float)dicomFilePixelHUIntensities[x, y] - (float)hounsfieldUnitMinimumIntenisty) / hounsfieldUnitRange;
                        //Debug.Log($"Minimum intensity: {min} and maximum intenisty in frame: {max}.");
                        //Debug.Log($"Intenisty {hus[x,y]} at position {x}, {y} has color: {rescaledIntensity.ToString("0.000000000")}");
                        //Color readyRescaledHUColor = new Color(dicomFileRescaledHUIntensity, dicomFileRescaledHUIntensity, dicomFileRescaledHUIntensity);
                        //c = readyRescaledHUColor;

                    }
					/*else
                    {
                        c = Color.clear;
                    }*/

					//if (!(c.r < 0.1f && c.g < 0.1f && c.b < 0.1f))
						colors [idx] = c;

				}
			}

            Debug.Log($"Slice added to color array.");
		}

        Debug.Log($"Textures loaded into color array: {textureCount}");

        return colors;
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

    public static Texture3D CreateTexture3D(Color[] colors, int textureWidth, int textureHeight, int textureDepth)
    {
        Texture3D texture = new Texture3D (textureWidth, textureHeight, textureDepth, TextureFormat.RGBA32, true);

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
        // Save the texture Asset to your Unity Project - ONLY WORKS IN EDITOR, NOT ON OCULUS
        #if UNITY_EDITOR

            string assetName = Path.Combine(ressourceDestinationPath, (textureRessourceName + ".asset"));

            UnityEditor.AssetDatabase.CreateAsset(texture, assetName);

            Debug.Log($"3D Texture saved as Asset to path: {assetName}");

        #endif
    }

    public static void exportTexture3DToFile(Color[] toExport, string fileDestinationPath, string textureArrayName)
    {
        //string objectPath = fileDestinationPath + "/" + textureAssetName + "_3DTexture_Color_Array" + texture.width + "x" + texture.height + "x" + texture.depth + ".txt";

        //Debug.Log(textureArrayName);

       
        Debug.Log("Allocating byte array.");
        byte[] export = new byte[toExport.Length * 4 * 4];
        float[] Export = new float[toExport.Length * 4];
        Debug.Log("Byte array allocated.");

        int colorCount = 0;

        Debug.Log("Filling float array.");

        for(int i = 0; i < Export.Length; i+=4)
        {
            //UnityEngine.Color32 c = toExport[colorCount];
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

            //File.WriteAllBytes(textureArrayName, export);
            /*using(var stream = new MemoryStream())
            using(var binWriter = new BinaryWriter(stream))
            {
                Debug.Log("bin writed opened");
                foreach(var fl in export)
                {
                    binWriter.Write(fl);
                }
                binWriter.Flush();
                File.WriteAllBytes(textureArrayName, stream.ToArray());
                stream.Close();
                Debug.Log("done");
            }*/

            Debug.Log("Starting to copy colors to bit array.");
            File.WriteAllBytes(textureArrayName, export);

            //Buffer.BlockCopy(Export, 0, export, 0, Export.Length);

            //for(int i = 0; i < toExport.Length; i++)
            //{
                /*floatImport[i] = BitConverter.ToSingle(import, i * 4);
                colorCounter++;

                if(colorCounter == 4)
                {
                    Color c = new Color(floatImport[i - 3],
                                        floatImport[i - 2],
                                        floatImport[i - 1],
                                        floatImport[i - 0]);

                    colorImport[colorImportCounter] = c;
                    colorCounter = 0;
                    colorImportCounter++;
                }*/
                /*Color c = toExport[i];
                //byte[] tmp = new byte[4];

                //Array.Copy(BitConverter.GetBytes(c.r), 0, tmp, 0);
                Buffer.BlockCopy(BitConverter.GetBytes(c.r), 0, export, (i * 4) + 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(c.g), 0, export, (i * 4) + 1, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(c.b), 0, export, (i * 4) + 2, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(c.a), 0, export, (i * 4) + 3, 4);
                Debug.Log($"Copied pixel {i} of {export.Length / 4}.");

                //export[i + 0] = c.r;
                //export[i + 1] = c.g;
                //export[i + 2] = c.b;
                //export[i + 3] = c.a;

                //colorCount++;

            }

            Debug.Log("Finished copying colors to bit array. Writing file to memory...");
            File.WriteAllBytes(textureArrayName, export);
            Debug.Log("Done!");*/

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

        //float[] floatImport = new float[import.Length / 4];
        Color[] colorImport = new Color[import.Length / (4 * 4)];

        Texture3D texture = new Texture3D (textureWidth, textureHeight, textureDepth, TextureFormat.RGBA32, true);

		texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        texture.anisoLevel = 6;

        //int colorCounter = 0;
        //int colorImportCounter = 0;

        if(import != null)
        {
            for(int i = 0; i < colorImport.Length; i++)
            {
                /*floatImport[i] = BitConverter.ToSingle(import, i * 4);
                colorCounter++;

                if(colorCounter == 4)
                {
                    Color c = new Color(floatImport[i - 3],
                                        floatImport[i - 2],
                                        floatImport[i - 1],
                                        floatImport[i - 0]);

                    colorImport[colorImportCounter] = c;
                    colorCounter = 0;
                    colorImportCounter++;
                }*/
                Color c = new Color(BitConverter.ToSingle(import, ((i * 4) + 0)),
                                    BitConverter.ToSingle(import, ((i * 4) + 1)),
                                    BitConverter.ToSingle(import, ((i * 4) + 2)),
                                    BitConverter.ToSingle(import, ((i * 4) + 3)));

                colorImport[i] = c;
                Debug.Log($"Copied pixel {i} of {colorImport.Length}.");
                //colorCounter = 0;
                //colorImportCounter++;
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

    public static Texture3D createTexture3DAsAssetScript(string dirPath, string ressourceDestinationPath, string textureRessourceName, List<string> fileNameList, dicomInfoTools dicomInformation, int textureWidth, int textureHeight, int textureDepth)
    {
        /////Load all slices from directory into array of 2D Textures
        //var textureArray = CreateTextureArrayFromDicomdir(dirPath, scaleTexture);

        /////Copy pixel data of 2D Textures in array into color array
        //var colorsForCubeTexture = CreateTextureFromDicomdir(dirPath, scaleTexture, fileNameList, textureWidth, textureHeight, textureDepth);
        var colorsForCubeTexture = CreateColorArrayFromDicomdir(dirPath, fileNameList, dicomInformation, textureWidth, textureHeight, textureDepth);

        //Debug.Log($" Color: {colorsForCubeTexture[15000000]}");

        /////Map 2D Texture color pixels to 3D Texture
        var cubeTexture = CreateTexture3D(colorsForCubeTexture, textureWidth, textureHeight, textureDepth);
        Debug.Log($"3D Texture created from path {dirPath}");

        /////Save 3D Texture as Asset in Unity Editor
        dicomImageTools.exportTexture3DToAsset(cubeTexture, ressourceDestinationPath, textureRessourceName);

        return cubeTexture;
    }

    public static Texture3D createTexture3DAsFileScript(string dirPath, string dirName, string fileDestinationPath, double scaleTexture, string textureArrayName, List<string> fileNameList, int textureWidth, int textureHeight, int textureDepth)
    {
        /////Load all slices from directory into array of 2D Textures
        //var textureArray = CreateTextureArrayFromDicomdir(dirPath, scaleTexture);

        /////Copy pixel data of 2D Textures in array into color array
        //var colorsForCubeTexture = CreateTexture3DColorArray(textureArray);
        var colorsForCubeTexture = CreateTextureFromDicomdir(dirPath, scaleTexture, fileNameList, textureWidth, textureHeight, textureDepth);

        /////Save Color array as file
        dicomImageTools.exportTexture3DToFile(colorsForCubeTexture, fileDestinationPath, textureArrayName);

        /////Map 2D Texture color pixels to 3D Texture
        var cubeTexture = CreateTexture3D(colorsForCubeTexture, textureWidth, textureHeight, textureDepth);

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

    public static int NextPow2(int a)
	{
		int x = 2;

		while (x < a) 
        {
			x *= 2;
		}
		
		return x;

	}

    public static void SaveTextureToPNGFile(Texture2D tex, string destinationPath, string fileName, DateTime nowTime)
    { 
        byte[] bytes;
        bytes = tex.EncodeToPNG();

        //DateTime nowTime = DateTime.Now;
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
