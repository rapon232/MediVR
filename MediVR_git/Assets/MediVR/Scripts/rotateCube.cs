using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class rotateCube : MonoBehaviour
{
    private InputDevice controller;
    private List<InputDevice> devices = new List<InputDevice>();

    private GameObject button;
    //private GameObject dicomImageCube = GameObject.Find("Dicom_Cube");

    protected bool rotate = false;

    public void Start()
    {
        GetDevice();
        button = GameObject.Find("Rotate_Button");
    }

    public void Update() {

        if(controller == null)
        {
            GetDevice();
        }

        if(rotate)
        {
            var colors = button.GetComponent<Button> ().colors;
            colors.normalColor = Color.red;
            button.GetComponent<Button> ().colors = colors;

            if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 position) && position != Vector2.zero)
            {
                //Debug.Log(position);
                transform.Rotate (new Vector3 (position.y*100, -position.x*100, 0f) * Time.deltaTime, Space.World);
            }
        }
        else
        {
            var colors = button.GetComponent<Button> ().colors;
            colors.normalColor = Color.black;
            button.GetComponent<Button> ().colors = colors;
        }
    }

    private void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, devices);
        if(devices.Count == 1)
        {
            controller = devices[0];
            Debug.Log(devices[0]);
        }
    }

    public void RotateCube ()
    {
        rotate = !rotate;
    }
}
