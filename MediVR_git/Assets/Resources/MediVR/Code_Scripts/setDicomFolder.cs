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

    This script serves to create a drop down menu to choose source directory for file import.

*/

using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

    public class setDicomFolder : EditorWindow
    {
        private static GameObject dicomImporter = null;
        private static initialImportDicom initialImportScript = null;

        //CREATE MENU ITEM
        [MenuItem("MediVR/Choose Dicom Source Directory")]
        static void Apply()
        {
            string path = EditorUtility.OpenFolderPanel("Load Dicom Directory", "", "");

            //CHECK IF FOLDER EMPTY AND SET FOLDER PATH

            if (path.Length != 0)
            {
                dicomImporter = GameObject.Find("Dicom_Importer");
                initialImportScript = dicomImporter.GetComponent<initialImportDicom>();
            
                initialImportScript.userDefinedDicomPath = path;
                initialImportScript.userDefinedFolderName = path.Split(Path.DirectorySeparatorChar).Last();
            }
        }
    }

#endif