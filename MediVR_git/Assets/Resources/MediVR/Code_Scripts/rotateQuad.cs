﻿/*

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

    This script serves to move slice frame by joystick input.

*/

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;

public class rotateQuad : MonoBehaviour
{
    public XRNode leftControllerNode = XRNode.LeftHand;
    public XRNode rightControllerNode = XRNode.RightHand;

    public float rotateSpeed = 50.0f;
    public float translateSpeed = .5f;

    private InputFeatureUsage<bool> resetButton = CommonUsages.primary2DAxisClick;
    private InputFeatureUsage<Vector2> joystick = CommonUsages.primary2DAxis;

    private AudioSource audioFXSource = null;
    private AudioClip onButtonPressDown = null;
    private AudioClip onButtonPressUp = null;

    private InputDevice leftController;
    private InputDevice rightController;

    private List<InputDevice> leftDevices = new List<InputDevice>();
    private List<InputDevice> rightDevices = new List<InputDevice>();

    private Quaternion rotateDefault = Quaternion.identity;
    private Vector3 translateDefault = Vector3.zero;

    private bool rotate = false;
    private bool translate = false;

    private GameObject dicomImageQuad = null;

    private bool audioFlag = false;

    public bool Rotate
    {
        get { return rotate; }
    }
    public bool Translate
    {
        get { return translate; }
    }

    // Start is called before the first frame update
    void Start()
    {
        dicomImageQuad = GameObject.Find("Dicom_Image_Quad");

        audioFXSource = dicomImageQuad.GetComponent<setQuadAudio>().audioFXSource;
        onButtonPressDown = dicomImageQuad.GetComponent<setQuadAudio>().onButtonPressDown;
        onButtonPressUp = dicomImageQuad.GetComponent<setQuadAudio>().onButtonPressUp;

        GetControllers();
        //Debug.Log("Got Controllers");
        rotate = false;
        translate = false;
        rotateDefault = this.transform.rotation;
        translateDefault = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(leftController == null || rightController == null)
        {
            GetControllers();
            //Debug.Log("Got Controllers Update");
        }

        RotateQuad();
        //Debug.Log("Rotated Quad");
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

    //LISTEN FOR INPUT
    private void RotateQuad()
    {
        if(rotate || translate)
        {
            if (leftController.TryGetFeatureValue(joystick, out Vector2 lPosition) && lPosition != Vector2.zero)
            {
                if(rotate)
                {
                    var xAxis = lPosition.x * rotateSpeed * Time.deltaTime;
                    var yAxis = lPosition.y * rotateSpeed * Time.deltaTime;

                    this.transform.Rotate(new Vector3 (yAxis, -xAxis, 0f), Space.Self);
                    //Debug.Log(lPosition);
                }
                else if(translate)
                {
                    var xAxis = lPosition.x * translateSpeed * Time.deltaTime;
                    var yAxis = lPosition.y * translateSpeed * Time.deltaTime;

                    this.transform.Translate(new Vector3 (xAxis, yAxis, 0f), Space.Self);
                    //Debug.Log(lPosition);
                }
                
            }

            if (rightController.TryGetFeatureValue(joystick, out Vector2 rPosition) && rPosition != Vector2.zero)
            {
                if(rotate)
                {
                    var zAxis = rPosition.y * rotateSpeed * Time.deltaTime;

                    this.transform.Rotate(new Vector3 (0f, 0f, zAxis), Space.Self);
                    //Debug.Log(rPosition);
                }
                else if(translate)
                {
                    var zAxis = rPosition.y * translateSpeed * Time.deltaTime;

                    this.transform.Translate(new Vector3 (0f, 0f, zAxis), Space.Self);
                    //Debug.Log(rPosition);
                }
            }

            if(leftController.TryGetFeatureValue(resetButton, out bool lClick) && lClick)
            {
                if(rotate)
                {
                    this.transform.rotation = rotateDefault;

                    if(!audioFlag)
                    {
                        audioFXSource.PlayOneShot(onButtonPressDown);
                        audioFlag = true;
                    }
                }
                else if(translate)
                {
                    this.transform.position = translateDefault;

                    if(!audioFlag)
                    {
                        audioFXSource.PlayOneShot(onButtonPressDown);
                        audioFlag = true;
                    }
                }
            }
            else
            {
                audioFlag = false;
            }
        }
    }

    //SET LISTENERS
    public void SetRotate(bool state)
    {
        rotate = state;
        //Debug.Log($"Rotate set to: {rotate}!");
    }

    public void SetTranslate(bool state)
    {
        translate = state;
        //Debug.Log($"Translate set to: {translate}!");
    }

    //RESET POSITION AND ROTATION OF FRAME
    public void ResetPositionRotation()
    {
        this.transform.position = translateDefault;
        this.transform.rotation = rotateDefault;
    }
}
