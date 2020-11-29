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

    This script serves to modify the appearence of initialImportDicom Script in the Inspector tab.

*/

using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

    //CREATE CUSTOM LAYOUT
    [CustomEditor(typeof(initialImportDicom))]
    public class customImportInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            //LABELS
            EditorGUILayout.LabelField("To choose Directory containing dicom files, go up to menu bar");
            EditorGUILayout.LabelField("and select menu: MediVR/Choose Dicom Source Directory.");

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            //DEFAULT LAYOUT
            DrawDefaultInspector();

            EditorGUILayout.Space();

            //LABELS
            EditorGUILayout.LabelField("Slices must be ordered from distal to proximal end of volume.");
            EditorGUILayout.LabelField("If in reverse order, check box Reverse Slice Order.");

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Press Button to Create a 3D Texture from files at directory.");
            EditorGUILayout.LabelField("The 3D Texture will be saved as an Asset for use at runtime.");

            EditorGUILayout.Space();

            //BUTTONS
            initialImportDicom import = (initialImportDicom)target;

            if(GUILayout.Button("Import Dicom Files"))
            {
                import.CreateTexture3DAssets();
            }

            if(GUILayout.Button("Save Imported Directory Names"))
            {
                import.SaveImportedTextureDirectoryNames();
            }
        }
    }

#endif
