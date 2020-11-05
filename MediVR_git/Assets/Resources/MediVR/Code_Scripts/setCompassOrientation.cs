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

    //private GameObject mainCam = null;

    //private Quaternion screenOrientation;

    //private float mainCamYOrientation;

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

        //mainCam = GameObject.Find("Main Camera");
        //screenOrientation = orientationL.transform.rotation;

        
    }

    // Update is called once per frame
    void Update()
    {
        //mainCamYOrientation = mainCam.transform.rotation.eulerAngles.y;
        //transform.rotation = new Quaternion(0, xrRigOrientation.y, 0);
        //orientationL.transform.localRotation = Quaternion.Euler(screenOrientation.x, mainCamYOrientation, screenOrientation.z);
        //orientationL.transform.Rotate( new Vector3(screenOrientation.x, mainCamYOrientation, screenOrientation.z), Space.Self);

    }
}
