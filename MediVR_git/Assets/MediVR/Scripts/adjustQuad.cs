using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class adjustQuad : MonoBehaviour
{
    public float adjustSpeed = .5f;

    public XRNode leftControllerNode = XRNode.LeftHand;
    public XRNode rightControllerNode = XRNode.RightHand;

    public bool adjustListen = false;
    public bool adjust = false;
    public bool flag = false;

    public Color adjustColor = Color.yellow;
    public Color inactiveColor = Color.white;

    private string outlineColorName = "_OutlineColor";

    private InputDevice leftController;
    private InputDevice rightController;

    private List<InputDevice> leftDevices = new List<InputDevice>();
    private List<InputDevice> rightDevices = new List<InputDevice>();

    private GameObject xrRig = null;
    private moveLocomotion moveLocomotionScript = null;
    private SnapTurnProvider snapTurnProviderScript = null;

    private Renderer quadRenderer = null;
    private Material quadMaterial = null;

    private float adjustStep = 1000.0f;

    private float brightnessMax = 0;
    private float brightnessMin = 0;
    private float brightnessRange = 0;

    private float contrastMax = 0;
    private float contrastMin = 0;
    private float contrastRange = 0;

    private float thresholdMax = 0;
    private float thresholdMin = 0;
    private float thresholdRange = 0;


    // Start is called before the first frame update
    void Start()
    {
        xrRig = GameObject.Find("XR Rig");
        moveLocomotionScript = xrRig.GetComponent<moveLocomotion>();
        snapTurnProviderScript = xrRig.GetComponent<SnapTurnProvider>();

        quadRenderer = this.GetComponent<Renderer>();
        quadMaterial = quadRenderer.material;

        inactiveColor = quadMaterial.GetColor(outlineColorName);

        brightnessMax = quadMaterial.GetFloat("_BrightnessMax");
        brightnessMin = quadMaterial.GetFloat("_BrightnessMin");
        brightnessRange = brightnessMax - brightnessMin;

        contrastMax = quadMaterial.GetFloat("_ContrastMax");
        contrastMin = quadMaterial.GetFloat("_ContrastMin");
        contrastRange = contrastMax - contrastMin;

        thresholdMax = quadMaterial.GetFloat("_ThresholdMax");
        thresholdMin = quadMaterial.GetFloat("_ThresholdMin");
        thresholdRange = thresholdMax - thresholdMin;

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

    private void AdjustListen()
    {
        if(adjustListen)
        {
            /*if(adjust)
            {
                if((leftController.TryGetFeatureValue(CommonUsages.gripButton, out bool lPress) && lPress) || 
                        (rightController.TryGetFeatureValue(CommonUsages.gripButton, out bool rPress) && rPress))
                {
                    moveLocomotionScript.enabled = true;

                    SetAdjust(false);

                    quadMaterial.SetColor(outlineColorName, inactiveColor);
                }

                if(rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool rPress) && rPress)
                {
                    SetAdjust(false);
                }
            }
            else
            {
                if((leftController.TryGetFeatureValue(CommonUsages.gripButton, out bool lPress) && lPress) || 
                        (rightController.TryGetFeatureValue(CommonUsages.gripButton, out bool rPress) && rPress))
                {
                    moveLocomotionScript.enabled = false;

                    SetAdjust(true);

                    quadMaterial.SetColor(outlineColorName, adjustColor);
                }

                if(rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool rPress) && rPress)
                {
                    SetAdjust(true);
                }
            }*/

            if((leftController.TryGetFeatureValue(CommonUsages.gripButton, out bool lPress) && lPress) || 
                        (rightController.TryGetFeatureValue(CommonUsages.gripButton, out bool rPress) && rPress))
            {
                moveLocomotionScript.enabled = false;
                snapTurnProviderScript.enabled = false;

                SetAdjust(true);

                quadMaterial.SetColor(outlineColorName, adjustColor);

                flag = true;
            }
            else
            {
                if(flag)
                {
                    moveLocomotionScript.enabled = true;
                    snapTurnProviderScript.enabled = true;

                    SetAdjust(false);

                    quadMaterial.SetColor(outlineColorName, inactiveColor);

                    flag = false;
                }
            }
            

            /*if(adjust)
            {
                quadMaterial.SetColor(outlineColorName, adjustColor);
            }
            else
            {
                quadMaterial.SetColor(outlineColorName, inactiveColor);
            }*/
        }
        
    }

    private void AdjustQuad()
    {
        /*if (leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool press) && press)
        {
            ToggleLocomotion();
            ToggleAdjust();
        }*/

        if(adjust)
        {
            //moveLocomotionScript.enabled = false;

            //quadMaterial.SetColor(outlineColorName, adjustColor);

            if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 lPosition) && lPosition != Vector2.zero)
            {
                if(adjust)
                {
                    //var xAxis = lPosition.x * adjustSpeed * Time.deltaTime;
                    //var yAxis = lPosition.y * adjustSpeed * Time.deltaTime;

                    var xAxis = lPosition.x * contrastRange / adjustStep;
                    var yAxis = lPosition.y * brightnessRange / adjustStep;

                    //Debug.Log(xAxis);
                    //Debug.Log(xOrig + xAxis);
                    var xOrig = quadMaterial.GetFloat("_Contrast");
                    var yOrig = quadMaterial.GetFloat("_Brightness");
                    
                    float xTemp = xOrig + xAxis;
                    float yTemp = yOrig + yAxis;

                    //quadMaterial.SetFloat("_Contrast", xOrig + xAxis);

                    if(xTemp <= contrastMax && xTemp >= contrastMin)
                    {
                        quadMaterial.SetFloat("_Contrast", xTemp);
                    }
                    if(yTemp <= brightnessMax && yTemp >= brightnessMin)
                    {
                        quadMaterial.SetFloat("_Brightness", yTemp);
                    }

                    //transform.Rotate(new Vector3 (yAxis, -xAxis, 0f), Space.Self);
                    //Debug.Log(lPosition);
                }
            }

            if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rPosition) && rPosition != Vector2.zero)
            {
                if(adjust)
                {
                    var xzAxis = rPosition.x * thresholdRange / adjustStep;
                    var zAxis = rPosition.y * thresholdRange / adjustStep;

                    //quadMaterial.SetFloat("_Threshold", xzAxis);
                    //quadMaterial.SetFloat("_ThresholdInv", zAxis);

                    var xzOrig = quadMaterial.GetFloat("_Threshold");
                    var zOrig = quadMaterial.GetFloat("_ThresholdInv");

                    float xzTemp = xzOrig + xzAxis;
                    float zTemp = zOrig + zAxis;

                    if(xzTemp <= thresholdMin && xzTemp >= thresholdMax)
                    {
                        quadMaterial.SetFloat("_Threshold", xzTemp);
                    }
                    if(zTemp <= thresholdMin && zTemp >= thresholdMax)
                    {
                        quadMaterial.SetFloat("_ThresholdInv", zTemp);
                    }

                    //transform.Rotate(new Vector3 (0f, 0f, zAxis), Space.Self);
                    //Debug.Log(rPosition);
                }
            }

            /*else
            {
                quadMaterial.SetColor(outlineColorName, inactiveColor);
            }*/

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

    public void SetAdjustListen(bool state)
    {
        adjustListen = state;
        Debug.Log($"Adjust Listener set to: {adjustListen}!");
    }

    public void SetAdjust(bool state)
    {
        adjust = state;
        Debug.Log($"Adjust set to: {adjust}!");
    }

    public void ToggleAdjust()
    {
        adjust = !adjust;
        Debug.Log($"Adjust set to: {adjust}!");
    }

    public void ToggleLocomotion()
    {
        moveLocomotionScript.enabled  = !moveLocomotionScript.enabled;
        Debug.Log($"Locomotion set to: {moveLocomotionScript.enabled}!");
    }

}
