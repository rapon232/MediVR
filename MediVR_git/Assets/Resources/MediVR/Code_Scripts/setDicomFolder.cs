using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

    public class setDicomFolder : EditorWindow
    {
        private static GameObject dicomImporter = null;
        private static initialImportDicom initialImportScript = null;

        [MenuItem("MediVR/Choose Dicom Source Directory")]
        static void Apply()
        {
            string path = EditorUtility.OpenFolderPanel("Load Dicom Directory", "", "");

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