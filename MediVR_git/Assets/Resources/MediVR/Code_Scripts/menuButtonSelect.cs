using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

public class menuButtonSelect : MonoBehaviour
{
    private string dicomAssetPath = "MediVR/Textures/Dicom 3D Textures";
    private static string savedDirectoryFileName = "importedDirectoryNames";
    private string savedDirectoryFilePath = null;
    private string savedDirectoryFileNames = null;
    public string[] savedDirectoryNames = null;

    //private static string cabinetScenePath = "Assets/Resources/MediVR/Scenes/VirtualDiagnosticsCabinet";
    private initialImportDicom initialImportDicomScript = null; 

    private GameObject buttonTemplate = null;
    private GameObject newButton = null;

    // Start is called before the first frame update
    void Start()
    {
        /*subDirectoryEntries = Directory.GetDirectories(dicomAssetPath);
        subDirectoryNames = new string[subDirectoryEntries.Length];

        #if UNITY_EDITOR
            dicomAssetPath = initialImportDicom.ressourceDestinationPath;
            Debug.Log(dicomAssetPath);
        #else
            dicomAssetPath = Path.Combine(Application.persistentDataPath, initialImportDicom.ressourceDestinationPath);
            Debug.Log(dicomAssetPath);
        #endif*/ 

        initialImportDicomScript = this.GetComponent<initialImportDicom>();

        savedDirectoryFilePath = Path.Combine(dicomAssetPath, savedDirectoryFileName);
        //Debug.Log(savedDirectoryFilePath);

        if(savedDirectoryFilePath != null)
        {
            var resource = Resources.Load<TextAsset>(savedDirectoryFilePath);
            if(resource != null)
            {
                savedDirectoryFileNames = resource.text;
                //Debug.Log(savedDirectoryFileNames);
            }
        }

        if(savedDirectoryFileNames != null)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(string[]));

            using(StringReader reader = new StringReader(savedDirectoryFileNames))
            {
                object obj = deserializer.Deserialize(reader);

                savedDirectoryNames = (string[])obj;
                //Debug.Log(savedDirectoryNames);
            }
        }

        buttonTemplate = transform.GetChild(0).gameObject;
        
        if(savedDirectoryNames != null)
        {
            for (int i = 0; i < savedDirectoryNames.Length; i++)
            {
                newButton = Instantiate(buttonTemplate, transform);
                newButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{savedDirectoryNames[i]}";

                newButton.GetComponent<Button>().AddEventListener(i, ButtonClicked);
            }

            Destroy(buttonTemplate);
        }
    }

    void ButtonClicked(int idx)
    {
        //Debug.Log($"{idx}");
        //Debug.Log($"Folder: {setCurrentPath.currentDirectory} clicked.");
        setCurrentDirectory.currentDirectory = savedDirectoryNames[idx];

        Debug.Log($"Clicked folder name: {setCurrentDirectory.currentDirectory}.");

        //if(!IsDirectoryEmpty(setCurrentDirectory.currentDirectoryPath))
        //{
            Debug.Log($"Loading Cabinet.");
            initialImportDicomScript.GoToNextScene();
        /*}
        else
        {
            Debug.Log($"Folder: {setCurrentDirectory.currentDirectory} is empty.");
        }*/
    }
}

public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
    {
        button.onClick.AddListener(delegate()
        {
            OnClick(param);
        });
    }
}
