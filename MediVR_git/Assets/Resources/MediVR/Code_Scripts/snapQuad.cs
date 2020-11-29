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

    This script serves to snap duplicate slices to pin wall.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snapQuad : MonoBehaviour
{
    public string objectToSnapName = "Quad";
    public float quadSnappedOffset = 0.05f;
    public float quadOrientatedOffset = 2f;
    public Color activeColor = Color.clear;

    private bool isGrabbed = false;
    private bool isInsideSnapZone = false;
    private bool isSnapped = false;

    private GameObject quad = null;
    private grabQuad grabQuadScript = null;
    private rotateQuad rotateQuadScript = null; 

    private BoxCollider snapZoneBoxCollider = null;

    private Material snapZoneMaterial = null;
    private string snapZoneColorName = "_Color";
    private Color inactiveColor = Color.clear;

    public bool IsSnapped
    {
        get { return isSnapped; }
    }

    private bool isActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        snapZoneBoxCollider = this.GetComponent<BoxCollider>();
        snapZoneMaterial = this.GetComponent<Renderer>().material;

        inactiveColor = snapZoneMaterial.GetColor(snapZoneColorName);
    }

    // Update is called once per frame
    void Update()
    {
        if(quad != null && grabQuadScript != null && rotateQuadScript != null)
        {
            if(!grabQuadScript.Selected && !rotateQuadScript.Translate)
            {
                isGrabbed = false;
            }
            else 
            {
                isGrabbed = true;
            }
        }
        else
        {
            isGrabbed = false;
        }

        SnapQuad();
    }

    //SNAP SLICE TO WALL
    void OnTriggerEnter(Collider other)
    {
        if(!isActivated)
        {
            if(other.gameObject.name == objectToSnapName)
            {
                quad = other.gameObject;
                grabQuadScript = quad.GetComponent<grabQuad>();
                rotateQuadScript = quad.GetComponent<rotateQuad>();

                isInsideSnapZone = true;
                isActivated = true;
            }
        }
    }

    //UNSNAP SLICE FROM WALL
    void OnTriggerExit(Collider other)
    {
        if(isActivated)
        {
            if(other.gameObject.name == objectToSnapName)
            {
                quad = null;
                grabQuadScript = null;
                rotateQuadScript = null;

                isInsideSnapZone = false;
                isActivated = false;
            }
        }
    }

    //UPDATE SLICE SNAP TO WALL
    void SnapQuad()
    {
        if(!isGrabbed && isInsideSnapZone)
        {
            if(quad != null)
            {
                quad.gameObject.transform.position = this.transform.position;
                quad.gameObject.transform.position -= new Vector3(quadSnappedOffset, 0, 0);
                quad.gameObject.transform.rotation = this.transform.rotation;
                isSnapped = true;
            }
        }
        else
        {
            isSnapped = false;
        }
    }
}
