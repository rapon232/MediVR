using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(initialImportDicom))]
public class customImportInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        initialImportDicom import = (initialImportDicom)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Press Button to Create a 3D Texture from files at directory.");
        EditorGUILayout.LabelField("The 3D Texture will be saved as an Asset for use at runtime.");

        EditorGUILayout.Space();

        if(GUILayout.Button("Import Dicom Files"))
        {
            import.CreateTexture3DAssets();
        }
    }
}
