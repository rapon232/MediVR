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

    This script serves to set quad window parameters.

*/

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

using TMPro;

public class adjustQuad : MonoBehaviour
{
    public XRNode leftControllerNode = XRNode.LeftHand;
    public XRNode rightControllerNode = XRNode.RightHand;

    public float adjustStep = 2000.0f;

    private InputFeatureUsage<bool> adjustWindowButton = CommonUsages.secondaryButton;
    private InputFeatureUsage<bool> adjustThresholdButton = CommonUsages.primaryButton;
    private InputFeatureUsage<bool> resetButton = CommonUsages.primary2DAxisClick;
    private InputFeatureUsage<Vector2> joystick = CommonUsages.primary2DAxis;

    private AudioSource audioFXSource = null;
    private AudioClip onButtonPressDown = null;
    private AudioClip onButtonPressUp = null;

    private Color adjustWindowColor = Color.clear;
    private Color adjustScaleColor = Color.clear;
    
    private Color inactiveColor = Color.white;

    private string outlineColorName = "_OutlineColor";

    private InputDevice leftController;
    private InputDevice rightController;

    private List<InputDevice> leftDevices = new List<InputDevice>();
    private List<InputDevice> rightDevices = new List<InputDevice>();

    private GameObject xrRig = null;
    private moveLocomotion moveLocomotionScript = null;
    private SnapTurnProvider snapTurnProviderScript = null;

    private GameObject dicomImageQuad = null;

    private Renderer quadRenderer = null;
    private Material quadMaterial = null;

    private bool adjustListen = false;
    private bool adjustWindow = false;
    private bool adjustScale = false;

    private bool flag = false;
    private bool audioFlag = false;

    private string adjustWindowXName = "_WindowWidth";
    private string adjustWindowXMinName = "_WindowWidthMin";
    private string adjustWindowXMaxName = "_WindowWidthMax";

    private float adjustWindowXDefault = 0;
    private float adjustWindowXMax = 0;
    private float adjustWindowXMin = 0;
    private float adjustWindowXRange = 0;

    private string adjustWindowYName = "_WindowCenter";
    private string adjustWindowYMinName = "_WindowCenterMin";
    private string adjustWindowYMaxName = "_WindowCenterMax";

    private float adjustWindowYDefault = 0;
    private float adjustWindowYMax = 0;
    private float adjustWindowYMin = 0;
    private float adjustWindowYRange = 0;

    private string adjustScaleYName = "_Enlarge";
    private string adjustScaleYMinName = "_EnlargeMin";
    private string adjustScaleYMaxName = "_EnlargeMax";

    private float adjustScaleYDefault = 0;
    private float adjustScaleYMax = 0;
    private float adjustScaleYMin = 0;
    private float adjustScaleYRange = 0;

    private GameObject WW = null;
    private GameObject WL = null;
    private GameObject WMin = null;
    private GameObject WMax = null;
    private GameObject WScale = null;


    // Start is called before the first frame update
    void Start()
    {
        dicomImageQuad = GameObject.Find("Dicom_Image_Quad");

        audioFXSource = dicomImageQuad.GetComponent<setQuadAudio>().audioFXSource;
        onButtonPressDown = dicomImageQuad.GetComponent<setQuadAudio>().onButtonPressDown;
        onButtonPressUp = dicomImageQuad.GetComponent<setQuadAudio>().onButtonPressUp;

        adjustWindowColor = dicomImageQuad.GetComponent<setQuadFrameColors>().setWindow;
        adjustScaleColor = dicomImageQuad.GetComponent<setQuadFrameColors>().setScale;
        inactiveColor = dicomImageQuad.GetComponent<setQuadFrameColors>().defaultFrameColor;

        xrRig = GameObject.Find("XR Rig");
        moveLocomotionScript = xrRig.GetComponent<moveLocomotion>();
        snapTurnProviderScript = xrRig.GetComponent<SnapTurnProvider>();

        quadRenderer = this.GetComponent<Renderer>();
        quadMaterial = quadRenderer.material;

        adjustWindowXDefault = quadMaterial.GetFloat(adjustWindowXName);
        adjustWindowXMin = quadMaterial.GetFloat(adjustWindowXMinName);
        adjustWindowXMax = quadMaterial.GetFloat(adjustWindowXMaxName);
        adjustWindowXRange = adjustWindowXMax - adjustWindowXMin;

        adjustWindowYDefault = quadMaterial.GetFloat(adjustWindowYName);
        adjustWindowYMin = quadMaterial.GetFloat(adjustWindowYMinName);
        adjustWindowYMax = quadMaterial.GetFloat(adjustWindowYMaxName);
        adjustWindowYRange = adjustWindowYMax - adjustWindowYMin;

        adjustScaleYDefault = quadMaterial.GetFloat(adjustScaleYName);
        adjustScaleYMin = quadMaterial.GetFloat(adjustScaleYMinName);
        adjustScaleYMax = quadMaterial.GetFloat(adjustScaleYMaxName);
        adjustScaleYRange = adjustScaleYMax - adjustScaleYMin;

        WL = GameObject.Find("WL_Value");
        WW = GameObject.Find("WW_Value");
        WMin = GameObject.Find("WMin_Value");
        WMax = GameObject.Find("WMax_Value");
        WScale = GameObject.Find("WScale_Value");

        UpdateWindowScreenDisplay(adjustWindowYDefault, adjustWindowXDefault);
        UpdateScaleScreenDisplay(adjustScaleYDefault);

        GetControllers();
    }

    // Update is called once per frame
    void Update()
    {
        if(leftController == null || rightController == null)
        {
            GetControllers();
            //Debug.Log("Got Controllers Update");
        }

        AdjustListen();
        AdjustQuad();
    }

    //AWAKE CONTROLLERS
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

    //LISTEN FOR CONTROLLER INPUT
    private void AdjustListen()
    {
        if(adjustListen)
        {
            if(rightController.TryGetFeatureValue(adjustWindowButton, out bool lPress) && lPress)
            {
                if(!flag)
                {
                    audioFXSource.PlayOneShot(onButtonPressDown);
                }

                moveLocomotionScript.enabled = false;
                snapTurnProviderScript.enabled = false;

                SetAdjustWindow(true);

                quadMaterial.SetColor(outlineColorName, adjustWindowColor);

                flag = true;
            }
            else if(rightController.TryGetFeatureValue(adjustThresholdButton, out bool rPress) && rPress)
            {
                if(!flag)
                {
                    audioFXSource.PlayOneShot(onButtonPressDown);
                }

                moveLocomotionScript.enabled = false;
                snapTurnProviderScript.enabled = false;

                SetAdjustScale(true);

                quadMaterial.SetColor(outlineColorName, adjustScaleColor);

                flag = true;
            }
            else
            {
                if(flag)
                {
                    moveLocomotionScript.enabled = true;
                    snapTurnProviderScript.enabled = true;

                    SetAdjustWindow(false);
                    SetAdjustScale(false);

                    quadMaterial.SetColor(outlineColorName, inactiveColor);

                    flag = false;
                }
            }
        }
        
    }

    //SET QUAD WINDOW PARAMETERS
    private void AdjustQuad()
    {
        //LISTEN FOR WINDOW INPUT
        if(adjustWindow)
        {
            if (leftController.TryGetFeatureValue(joystick, out Vector2 lPosition) && lPosition != Vector2.zero)
            {
                if(adjustWindow)
                {
                    var xAxis = lPosition.x * adjustWindowXRange / adjustStep;
                    var yAxis = lPosition.y * adjustWindowYRange / adjustStep;

                    var xOrig = quadMaterial.GetFloat(adjustWindowXName);
                    var yOrig = quadMaterial.GetFloat(adjustWindowYName);
                    
                    float xTemp = xOrig + xAxis;
                    float yTemp = yOrig + yAxis;

                    if(xTemp <= adjustWindowXMax && xTemp >= adjustWindowXMin)
                    {
                        quadMaterial.SetFloat(adjustWindowXName, xTemp);
                    }
                    if(yTemp <= adjustWindowYMax && yTemp >= adjustWindowYMin)
                    {
                        quadMaterial.SetFloat(adjustWindowYName, yTemp);
                    }

                    UpdateWindowScreenDisplay(yTemp, xTemp);
                }
            }

            if(leftController.TryGetFeatureValue(resetButton, out bool lClick) && lClick)
            {
                if(!audioFlag)
                {
                    audioFXSource.PlayOneShot(onButtonPressDown);
                    audioFlag = true;
                }

                quadMaterial.SetFloat(adjustWindowXName, adjustWindowXDefault);
                quadMaterial.SetFloat(adjustWindowYName, adjustWindowYDefault);

                UpdateWindowScreenDisplay(adjustWindowYDefault, adjustWindowXDefault);
            }
            else
            {
                audioFlag = false;
            }
        }

        //LISTEN FOR ADJUST INPUT
        if(adjustScale)
        {

            if (leftController.TryGetFeatureValue(joystick, out Vector2 rPosition) && rPosition != Vector2.zero)
            {
                if(adjustScale)
                {
                    var zAxis = rPosition.y * adjustScaleYRange / adjustStep;

                    var zOrig = quadMaterial.GetFloat(adjustScaleYName);

                    float zTemp = zOrig + zAxis;

                    if(zTemp <= adjustScaleYMax && zTemp >= adjustScaleYMin)
                    {
                        quadMaterial.SetFloat(adjustScaleYName, zTemp);
                        UpdateScaleScreenDisplay(zTemp);
                    }
                }
            }

            if(leftController.TryGetFeatureValue(resetButton, out bool rClick) && rClick)
            {
                if(!audioFlag)
                {
                    audioFXSource.PlayOneShot(onButtonPressDown);
                    audioFlag = true;
                }

                quadMaterial.SetFloat(adjustScaleYName, adjustScaleYDefault); 

                UpdateScaleScreenDisplay(adjustScaleYDefault);  
            }
            else
            {
                audioFlag = false;
            }
        }
    }

    //SET LISTENERS
    public void SetAdjustListen(bool state)
    {
        adjustListen = state;
        //Debug.Log($"Adjust Listener set to: {adjustListen}!");
    }


    public void SetAdjustWindow(bool state)
    {
        adjustWindow = state;
        //Debug.Log($"Adjust set to: {adjust}!");
    }

    public void SetAdjustScale(bool state)
    {
        adjustScale = state;
        //Debug.Log($"Adjust set to: {adjust}!");
    }

    //ENABLE/DISABLE LOCOMOTION SYSTEM
    public void ToggleLocomotion()
    {
        moveLocomotionScript.enabled  = !moveLocomotionScript.enabled;
        //Debug.Log($"Locomotion set to: {moveLocomotionScript.enabled}!");
    }

    //DISPLAY WINDOW PARAMETERS ON VIRTUAL SCREEN
    public void UpdateWindowScreenDisplay(float wl, float ww)
    {
        WL.GetComponent<TextMeshProUGUI>().text = wl.ToString("F0");
        WW.GetComponent<TextMeshProUGUI>().text = ww.ToString("F0");
        WMin.GetComponent<TextMeshProUGUI>().text = (wl - (ww / 2)).ToString("F0");
        WMax.GetComponent<TextMeshProUGUI>().text = (wl + (ww / 2)).ToString("F0");
    }

    //DISPLAY SCALE ON VIRTUAL SCREEN
    public void UpdateScaleScreenDisplay(float wscale)
    {
        WScale.GetComponent<TextMeshProUGUI>().text = (wscale / adjustScaleYDefault * 100).ToString("F0") + "%";
    }

    //RESET WINDOWING AND SCALING
    public void ResetWindowAndScale()
    {
        quadMaterial.SetFloat(adjustWindowXName, adjustWindowXDefault);
        quadMaterial.SetFloat(adjustWindowYName, adjustWindowYDefault);
        quadMaterial.SetFloat(adjustScaleYName, adjustScaleYDefault);

        UpdateWindowScreenDisplay(adjustWindowYDefault, adjustWindowXDefault);
        UpdateScaleScreenDisplay(adjustScaleYDefault); 
    }

}
