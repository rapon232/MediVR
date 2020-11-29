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

    This script serves to choose and set colors of slicing frame for every operation.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setQuadFrameColors : MonoBehaviour
{
    public Color defaultFrameColor = new Color(0.847f, 0.541f, 0.192f, 1f); //orange

    public Color handTranslate = Color.red;
    public Color handRotate = Color.blue;

    public Color joystickTranslate = Color.magenta;
    public Color joystickRotate = Color.cyan;

    public Color setWindow = new Color(1f, 0.92f, 0.016f, 1f); //yellow
    public Color setScale = new Color(0.823f, 1f, 0.024f, 1f); //yellow-greenish

    public Color setDuplicate = Color.green;

    private string colorVariableName = "_Color";
    private string outlineColorName = "_OutlineColor";

    private GameObject defaultInstruction = null;

    private GameObject handTranslateInstruction = null;
    private GameObject handRotateInstruction = null;

    private GameObject joystickTranslateInstruction = null;
    private GameObject joystickRotateInstruction = null;

    private GameObject setWWInstruction = null;
    private GameObject setWLInstruction = null;
    private GameObject setScaleInstruction = null;

    private GameObject setDuplicateInstruction = null;

    private GameObject dicomInstructions = null;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<Renderer>().material.SetColor(outlineColorName, defaultFrameColor);

        //Set frame colors of each operation
        defaultInstruction = GameObject.Find("Indicator_Default");
        defaultInstruction.GetComponent<Renderer>().material.SetColor(colorVariableName, defaultFrameColor);

        handTranslateInstruction = GameObject.Find("Indicator_RTran");
        handTranslateInstruction.GetComponent<Renderer>().material.SetColor(colorVariableName, handTranslate);
        handRotateInstruction = GameObject.Find("Indicator_RRot");
        handRotateInstruction.GetComponent<Renderer>().material.SetColor(colorVariableName, handRotate);

        joystickTranslateInstruction = GameObject.Find("Indicator_LTran");
        joystickTranslateInstruction.GetComponent<Renderer>().material.SetColor(colorVariableName, joystickTranslate);
        joystickRotateInstruction = GameObject.Find("Indicator_LRot");
        joystickRotateInstruction.GetComponent<Renderer>().material.SetColor(colorVariableName, joystickRotate);

        setWWInstruction = GameObject.Find("Indicator_WW");
        setWWInstruction.GetComponent<Renderer>().material.SetColor(colorVariableName, setWindow);
        setWLInstruction = GameObject.Find("Indicator_WL");
        setWLInstruction.GetComponent<Renderer>().material.SetColor(colorVariableName, setWindow);
        setScaleInstruction = GameObject.Find("Indicator_Scale");
        setScaleInstruction.GetComponent<Renderer>().material.SetColor(colorVariableName, setScale);

        setDuplicateInstruction = GameObject.Find("Indicator_Duplicate");
        setDuplicateInstruction.GetComponent<Renderer>().material.SetColor(colorVariableName, setDuplicate);

        //Hide Instruction Field by Default
        dicomInstructions = GameObject.Find("Dicom_Instruction_Text_Fields");
        dicomInstructions.SetActive(false);
    }
}
