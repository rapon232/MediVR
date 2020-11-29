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

    This script serves to rotate information screen according to head rotation of user.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class adjustScreenRotation : MonoBehaviour
{
    private GameObject mainCam = null;

    private Quaternion screenOrientation;

    private float mainCamYOrientation;
    
    // Start is called before the first frame update
    void Start()
    {
        mainCam = GameObject.Find("Main Camera");
        screenOrientation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        mainCamYOrientation = mainCam.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Euler(screenOrientation.x, mainCamYOrientation, screenOrientation.z);
    }
}
