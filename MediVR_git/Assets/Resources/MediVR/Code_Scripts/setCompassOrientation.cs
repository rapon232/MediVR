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

    This script serves to assign the correct letters to the orientation compass, as to indicate the 
    positioning of the patient relative to the three anatomical planes.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class setCompassOrientation : MonoBehaviour
{
    public bool showOrientationCompass = false;

    private GameObject dicomImageQuad = null;
    private importDicom importDicomScript = null;

    private GameObject orientationCompass = null;

    private GameObject orientationL = null;
    private GameObject orientationR = null;
    private GameObject orientationS = null;
    private GameObject orientationI = null;
    private GameObject orientationA = null;
    private GameObject orientationP = null;

    private string orientation = null;

    // Start is called before the first frame update
    void Start()
    {
        dicomImageQuad = GameObject.Find("Dicom_Image_Quad");
        importDicomScript = dicomImageQuad.GetComponent<importDicom>();

        orientationCompass = GameObject.Find("Dicom_Orientation_Compass");

        orientationL = GameObject.Find("Orientation_Sinister");
        orientationR = GameObject.Find("Orientation_Dexter");
        orientationS = GameObject.Find("Orientation_Superior");
        orientationI = GameObject.Find("Orientation_Inferior");
        orientationA = GameObject.Find("Orientation_Anterior");
        orientationP = GameObject.Find("Orientation_Posterior");

        if(importDicomScript.dicomInformation != null)
        {
            orientation = importDicomScript.dicomInformation.OrientationPatient;

            //APPLY LETTERS ACCORDING TO CALCULATED PATIENT ORIENTATION
            switch(orientation)
            {
                case ("Coronal AP"):
                    orientationL.GetComponent<TextMeshProUGUI>().text = "R";
                    orientationR.GetComponent<TextMeshProUGUI>().text = "L";
                    orientationS.GetComponent<TextMeshProUGUI>().text = "A";
                    orientationI.GetComponent<TextMeshProUGUI>().text = "P";
                    orientationA.GetComponent<TextMeshProUGUI>().text = "S";
                    orientationP.GetComponent<TextMeshProUGUI>().text = "I";
                    break;
                case ("Coronal PA"):
                    orientationL.GetComponent<TextMeshProUGUI>().text = "L";
                    orientationR.GetComponent<TextMeshProUGUI>().text = "R";
                    orientationS.GetComponent<TextMeshProUGUI>().text = "P";
                    orientationI.GetComponent<TextMeshProUGUI>().text = "A";
                    orientationA.GetComponent<TextMeshProUGUI>().text = "S";
                    orientationP.GetComponent<TextMeshProUGUI>().text = "I";
                    break;
                case ("Axial SI"):
                    orientationL.GetComponent<TextMeshProUGUI>().text = "R";
                    orientationR.GetComponent<TextMeshProUGUI>().text = "L";
                    orientationS.GetComponent<TextMeshProUGUI>().text = "S";
                    orientationI.GetComponent<TextMeshProUGUI>().text = "I";
                    orientationA.GetComponent<TextMeshProUGUI>().text = "P";
                    orientationP.GetComponent<TextMeshProUGUI>().text = "A";
                    break;
                case ("Axial IS"):
                    orientationL.GetComponent<TextMeshProUGUI>().text = "R";
                    orientationR.GetComponent<TextMeshProUGUI>().text = "L";
                    orientationS.GetComponent<TextMeshProUGUI>().text = "I";
                    orientationI.GetComponent<TextMeshProUGUI>().text = "S";
                    orientationA.GetComponent<TextMeshProUGUI>().text = "A";
                    orientationP.GetComponent<TextMeshProUGUI>().text = "P";
                    break;
                case ("Saggital LR"):
                    orientationL.GetComponent<TextMeshProUGUI>().text = "A";
                    orientationR.GetComponent<TextMeshProUGUI>().text = "P";
                    orientationS.GetComponent<TextMeshProUGUI>().text = "L";
                    orientationI.GetComponent<TextMeshProUGUI>().text = "R";
                    orientationA.GetComponent<TextMeshProUGUI>().text = "S";
                    orientationP.GetComponent<TextMeshProUGUI>().text = "I";
                    break;
                case ("Saggital RL"):
                    orientationL.GetComponent<TextMeshProUGUI>().text = "P";
                    orientationR.GetComponent<TextMeshProUGUI>().text = "A";
                    orientationS.GetComponent<TextMeshProUGUI>().text = "R";
                    orientationI.GetComponent<TextMeshProUGUI>().text = "L";
                    orientationA.GetComponent<TextMeshProUGUI>().text = "S";
                    orientationP.GetComponent<TextMeshProUGUI>().text = "I";
                    break;
                default:
                    orientationCompass.SetActive(false);
                    break;
            }
        }
        else
        {
            orientationCompass.SetActive(false);
        }

        if(showOrientationCompass)
        {
            orientationCompass.SetActive(true);
        }
        else
        {
            orientationCompass.SetActive(false);
        }

        //Debug.Log($"LOG: {orientation}");        
    }
}
