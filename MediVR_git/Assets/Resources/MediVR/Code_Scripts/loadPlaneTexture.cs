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

    private dicomInfoTools dicomInformation = new dicomInfoTools();

    private GameObject dicomImagePlane = null;
    private GameObject dicomImagePlane2 = null;
    private GameObject dicomImagePlane3 = null;
    private GameObject dicomImagePlane4 = null;
    private GameObject dicomImagePlane5 = null;

    private GameObject studyText = null;
    private GameObject patientText = null;
    private GameObject modalityText = null;

    private Texture2D[] planeTextureArray = null;

    public string dirPath = null;

    // Start is called before the first frame update
    void Start()
    {
        dicomImageQuad = GameObject.Find("Dicom_Image_Quad");
        dirPath = dicomImageQuad.GetComponent<importDicom>().dirPath;

        /////Load multiple slices into Texture2D Array
        planeTextureArray = dicomImageTools.CreateNumberedTextureArrayFromDicomdir (dirPath, false, ref dicomInformation, 5);

        /////Assign slice texture to each Plane
        dicomImagePlane = GameObject.Find("Dicom_Image_Plane");
        var dicomImagePlaneRenderer = dicomImagePlane.GetComponent<Renderer>();
        dicomImagePlaneRenderer.material.mainTexture = planeTextureArray[0];

        dicomImagePlane2 = GameObject.Find("Dicom_Image_Plane_2");
        var dicomImagePlaneRenderer2 = dicomImagePlane2.GetComponent<Renderer>();
        dicomImagePlaneRenderer2.material.mainTexture = planeTextureArray[1];

        dicomImagePlane3 = GameObject.Find("Dicom_Image_Plane_3");
        var dicomImagePlaneRenderer3 = dicomImagePlane3.GetComponent<Renderer>();
        dicomImagePlaneRenderer3.material.mainTexture = planeTextureArray[2];

        dicomImagePlane4 = GameObject.Find("Dicom_Image_Plane_4");
        var dicomImagePlaneRenderer4 = dicomImagePlane4.GetComponent<Renderer>();
        dicomImagePlaneRenderer4.material.mainTexture = planeTextureArray[3];

        dicomImagePlane5 = GameObject.Find("Dicom_Image_Plane_5");
        var dicomImagePlaneRenderer5 = dicomImagePlane5.GetComponent<Renderer>();
        dicomImagePlaneRenderer5.material.mainTexture = planeTextureArray[4];
        
        /////Assign slice dicom information to Canvas
        studyText = GameObject.Find("Dicom_Info_Text_Study (TMP)");
        patientText = GameObject.Find("Dicom_Info_Text_Patient (TMP)");
        modalityText = GameObject.Find("Dicom_Info_Text_Modality (TMP)");

        studyText.GetComponent<TextMeshProUGUI>().text = dicomInformation.Strings.studyInfo;
        patientText.GetComponent<TextMeshProUGUI>().text = dicomInformation.Strings.patientInfo;
        modalityText.GetComponent<TextMeshProUGUI>().text = dicomInformation.Strings.modalityInfo;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
