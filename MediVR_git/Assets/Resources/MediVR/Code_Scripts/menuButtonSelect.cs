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

    This script serves to create scrollable list of imported directories at beginning menu.

*/

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
    public ScrollRect scrollView = null;

    public string[] savedDirectoryNames = null;

    private string dicomAssetPath = "MediVR/Textures/Dicom 3D Textures";
    private static string savedDirectoryFileName = "importedDirectoryNames";
    private string savedDirectoryFilePath = null;
    private string savedDirectoryFileNames = null;

    private initialImportDicom initialImportDicomScript = null; 

    private GameObject buttonTemplate = null;
    private GameObject newButton = null;

    // Start is called before the first frame update
    void Start()
    {
        initialImportDicomScript = this.GetComponent<initialImportDicom>();

        savedDirectoryFilePath = Path.Combine(dicomAssetPath, savedDirectoryFileName);
        //Debug.Log(savedDirectoryFilePath);

        //LOAD DIRECTORY PATH
        if(savedDirectoryFilePath != null)
        {
            var resource = Resources.Load<TextAsset>(savedDirectoryFilePath);
            if(resource != null)
            {
                savedDirectoryFileNames = resource.text;
                //Debug.Log(savedDirectoryFileNames);
            }
        }

        //DESERIALIZE DIRECTORY NAMES
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
        
        //CREATE ONE BUTTON IN LIST FOR EVERY IMPORTED DIRECTORY
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

        scrollView.verticalNormalizedPosition = 1;
    }

    //LOAD CLICKED DATASET
    void ButtonClicked(int idx)
    {
        setCurrentDirectory.currentDirectory = savedDirectoryNames[idx];

        Debug.Log($"Clicked folder name: {setCurrentDirectory.currentDirectory}.");

        Debug.Log($"Loading Cabinet.");
        initialImportDicomScript.GoToNextScene();
    }
}

//MODIFY BUTTON ONCLICK METHOD
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
