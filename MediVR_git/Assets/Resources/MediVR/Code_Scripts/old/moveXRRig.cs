using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class moveXRRig : MonoBehaviour
{
    public float movingSpeed = 2.0f;

    public XRNode controllerNode = XRNode.LeftHand;

    public bool move = true;
    private InputDevice controller;
    private List<InputDevice> devices = new List<InputDevice>();

    // Start is called before the first frame update
    void Start()
    {
        GetController();
    }

    // Update is called once per frame
    void Update()
    {
        if(controller == null)
        {
            GetController();
        }

        MoveAround();
    }

    private void GetController()
    {
        InputDevices.GetDevicesAtXRNode(controllerNode, devices);
        if(devices.Count == 1)
        {
            controller = devices[0];
            Debug.Log(devices[0]);
        }
    }

    private void MoveAround()
    {
        if(move)
        {
            if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 position) && position != Vector2.zero)
            {
            var xAxis = position.x * movingSpeed * Time.deltaTime;
            var yAxis = position.y * movingSpeed * Time.deltaTime;

            Vector3 right = transform.TransformDirection(Vector3.right);
            Vector3 forward = transform.TransformDirection(Vector3.forward);

            transform.position += right * xAxis;
            transform.position += forward * yAxis;
            }
        }
    }

    public void SetMove()
    {
        move = !move;
        Debug.Log($"Move set to: {move}!");
    }
}
