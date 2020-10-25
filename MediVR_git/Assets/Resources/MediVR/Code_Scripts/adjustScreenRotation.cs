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
        //xrRigOrientation = xrRig.transform.rotation;
        mainCamYOrientation = mainCam.transform.rotation.eulerAngles.y;
        //transform.rotation = new Quaternion(0, xrRigOrientation.y, 0);
        transform.rotation = Quaternion.Euler(screenOrientation.x, mainCamYOrientation, screenOrientation.z);
        //transform.rotation = xrRigOrientation;
        //transform.Rotate(0, xrRigOrientation.y, 0);
        //transform.LookAt(transform.position + xrRig.transform.rotation * Vector3.forward,
        //    xrRig.transform.rotation * Vector3.up);
    }
}
