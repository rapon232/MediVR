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

    This script serves to allow the character to move around scene using controller.

*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class moveLocomotion : LocomotionProvider
{
    public float moveSpeed = 1.0f;
    public float gravityMultiplier = 10.0f;

    public List<XRController> controllers = null;

    private CharacterController characterController = null;
    private GameObject head = null;

    protected override void Awake()
    {
        characterController = this.GetComponent<CharacterController>();
        head = this.GetComponent<XRRig>().cameraGameObject;
    }

    private void Start()
    {
        PositionController();
    }

    private void Update()
    {
        PositionController();
        CheckForInput();
        ApplyGravity();
    }

    //POSITION CHARACTER CONTROLLER IN VR SPACE
    private void PositionController()
    {
        characterController.height = head.transform.localPosition.y - 1;

        // Cut in half, add skin
        Vector3 newCenter = Vector3.zero;
        newCenter.y = characterController.height;
        newCenter.y += characterController.skinWidth;

        // Let's move the capsule in local space as well
        newCenter.x = head.transform.localPosition.x;
        newCenter.z = head.transform.localPosition.z;

        // Apply
        characterController.center = newCenter;
    }

    //LISTEN FOR CONTROLLER INPUT
    private void CheckForInput()
    {
        foreach (XRController controller in controllers)
        {
            if(controller.enableInputActions)
                CheckForMovement(controller.inputDevice);
        }
    }

    //LISTEN FOR CONTROLLER INPUT
    private void CheckForMovement(InputDevice device)
    {
        if(device.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 position))
            StartMove(position);
    }

    //MOVE CHARACTER FORWARD AND SIDEWAYS ACCORDING TO HEAD DIRECTION
    private void StartMove(Vector2 position)
    {
        // Apply the touch position to the head's forward Vector
        Vector3 direction = new Vector3(position.x, 0, position.y);
        Vector3 headRotation = new Vector3(0, head.transform.eulerAngles.y, 0);

        // Rotate the input direction by the horizontal head rotation
        direction = Quaternion.Euler(headRotation) * direction;

        // Apply speed and move
        Vector3 movement = direction * moveSpeed;
        characterController.Move(movement * Time.deltaTime);
    }

    //PULL CHARACTER TO THE GROUND
    private void ApplyGravity()
    {
        Vector3 gravity = new Vector3(0, Physics.gravity.y * gravityMultiplier, 0);
        gravity.y *= Time.deltaTime;

        characterController.Move(gravity * Time.deltaTime);
    }
}
