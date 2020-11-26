using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

    [CustomEditor(typeof(initialImportDicom))]
    public class customImportInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("To choose Directory containing dicom files, go up to menu bar");
            EditorGUILayout.LabelField("and select menu: MediVR/Choose Dicom Source Directory.");

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            DrawDefaultInspector();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Slices must be ordered from distal to proximal end of volume.");
            EditorGUILayout.LabelField("If in reverse order, check box Reverse Slice Order.");

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Press Button to Create a 3D Texture from files at directory.");
            EditorGUILayout.LabelField("The 3D Texture will be saved as an Asset for use at runtime.");

            EditorGUILayout.Space();

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
