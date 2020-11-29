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

    This script serves to show slices and metadata on information wall.

*/

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

public class loadPlaneTexture : MonoBehaviour
{
    private GameObject dicomImageQuad = null;
    private importDicom importDicomScript = null;

    private GameObject dicomImagePlane = null;
    private GameObject dicomImagePlane2 = null;
    private GameObject dicomImagePlane3 = null;
    private GameObject dicomImagePlane4 = null;
    private GameObject dicomImagePlane5 = null;

    private GameObject studyText = null;
    private GameObject patientText = null;
    private GameObject modalityText = null;

    // Start is called before the first frame update
    void Start()
    {
        dicomImageQuad = GameObject.Find("Dicom_Image_Quad");
        importDicomScript = dicomImageQuad.GetComponent<importDicom>();

        if(importDicomScript.dicomSlices != null)
        {
            if(importDicomScript.dicomSlices.Length == 5)
            {
                /////Assign slice texture to each Plane
                dicomImagePlane = GameObject.Find("Dicom_Image_Plane");
                var dicomImagePlaneRenderer = dicomImagePlane.GetComponent<Renderer>();
                dicomImagePlaneRenderer.material.mainTexture = importDicomScript.dicomSlices[0];

                dicomImagePlane2 = GameObject.Find("Dicom_Image_Plane_2");
                var dicomImagePlaneRenderer2 = dicomImagePlane2.GetComponent<Renderer>();
                dicomImagePlaneRenderer2.material.mainTexture = importDicomScript.dicomSlices[1];

                dicomImagePlane3 = GameObject.Find("Dicom_Image_Plane_3");
                var dicomImagePlaneRenderer3 = dicomImagePlane3.GetComponent<Renderer>();
                dicomImagePlaneRenderer3.material.mainTexture = importDicomScript.dicomSlices[2];

                dicomImagePlane4 = GameObject.Find("Dicom_Image_Plane_4");
                var dicomImagePlaneRenderer4 = dicomImagePlane4.GetComponent<Renderer>();
                dicomImagePlaneRenderer4.material.mainTexture = importDicomScript.dicomSlices[3];

                dicomImagePlane5 = GameObject.Find("Dicom_Image_Plane_5");
                var dicomImagePlaneRenderer5 = dicomImagePlane5.GetComponent<Renderer>();
                dicomImagePlaneRenderer5.material.mainTexture = importDicomScript.dicomSlices[4];
            }
        }

        /////Find objects
        studyText = GameObject.Find("Dicom_Info_Text_Study");
        patientText = GameObject.Find("Dicom_Info_Text_Patient");
        modalityText = GameObject.Find("Dicom_Info_Text_Modality");

        if(importDicomScript.dicomInformation != null)
        {
            /////Assign slice dicom information to Canvas
            studyText.GetComponent<TextMeshProUGUI>().text = importDicomScript.dicomInformation.Strings.studyInfo;
            patientText.GetComponent<TextMeshProUGUI>().text = importDicomScript.dicomInformation.Strings.patientInfo;
            modalityText.GetComponent<TextMeshProUGUI>().text = importDicomScript.dicomInformation.Strings.modalityInfo;
        }
        else
        {
            studyText.GetComponent<TextMeshProUGUI>().text = "N/A";
            patientText.GetComponent<TextMeshProUGUI>().text = "N/A";
            modalityText.GetComponent<TextMeshProUGUI>().text = "N/A";
        }
    }
}
