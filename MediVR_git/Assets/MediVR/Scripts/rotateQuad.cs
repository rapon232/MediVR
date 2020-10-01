using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;

public class rotateQuad : MonoBehaviour
{
    public float rotateSpeed = 50.0f;
    public float translateSpeed = .5f;

    public XRNode leftControllerNode = XRNode.LeftHand;
    public XRNode rightControllerNode = XRNode.RightHand;

    public bool rotate = false;
    public bool translate = false;

    private InputDevice leftController;
    private InputDevice rightController;

    private List<InputDevice> leftDevices = new List<InputDevice>();
    private List<InputDevice> rightDevices = new List<InputDevice>();

    // Start is called before the first frame update
    void Start()
    {
        GetControllers();
        rotate = false;
        translate = false;
        Debug.Log("Got Controllers");
    }

    // Update is called once per frame
    void Update()
    {
        if(leftController == null || rightController == null)
        {
            GetControllers();
            Debug.Log("Got Controllers Update");
        }

        RotateQuad();
        //Debug.Log("Rotated Quad");
    }

    private void GetControllers()
    {
        InputDevices.GetDevicesAtXRNode(leftControllerNode, leftDevices);
        if(leftDevices.Count == 1)
        {
            leftController = leftDevices[0];
            //Debug.Log(leftDevices[0]);
        }

        InputDevices.GetDevicesAtXRNode(rightControllerNode, rightDevices);
        if(rightDevices.Count == 1)
        {
            rightController = rightDevices[0];
            //Debug.Log(rightDevices[0]);
        }
    }

    private void RotateQuad()
    {
        if(rotate || translate)
        {
            if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 lPosition) && lPosition != Vector2.zero)
            {
                if(rotate)
                {
                    var xAxis = lPosition.x * rotateSpeed * Time.deltaTime;
                    var yAxis = lPosition.y * rotateSpeed * Time.deltaTime;

                    transform.Rotate(new Vector3 (yAxis, -xAxis, 0f), Space.Self);
                }
                else if(translate)
                {
                    var xAxis = lPosition.x * translateSpeed * Time.deltaTime;
                    var yAxis = lPosition.y * translateSpeed * Time.deltaTime;

                    transform.Translate(new Vector3 (xAxis, yAxis, 0f), Space.Self);
                }
                
            }

            if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rPosition) && rPosition != Vector2.zero)
            {
                if(rotate)
                {
                    var zAxis = rPosition.y * rotateSpeed * Time.deltaTime;

                    transform.Rotate(new Vector3 (0f, 0f, zAxis), Space.Self);
                }
                else if(translate)
                {
                    var zAxis = rPosition.y * translateSpeed * Time.deltaTime;

                    transform.Translate(new Vector3 (0f, 0f, zAxis), Space.Self);
                }
            }

            /*if (leftController.TryGetFeatureValue(CommonUsages.gripButton, out bool lGrip) && lGrip)
            {
                SetRotate();
                SetTranslate();
            }

            if (rightController.TryGetFeatureValue(CommonUsages.gripButton, out bool rGrip) && rGrip)
            {
                SetRotate();
                SetTranslate();
            }*/
        }
    }

    public void SetRotate(bool state)
    {
        rotate = state;
        Debug.Log($"Rotate set to: {rotate}!");
    }

    public void SetTranslate(bool state)
    {
        translate = state;
        Debug.Log($"Translate set to: {translate}!");
    }
}
