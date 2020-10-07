using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class grabQuad : XRGrabInteractable
{
    private Vector3 interactorPosition = Vector3.zero;
    private Quaternion interactorRotation = Quaternion.identity;

    private Renderer quadRenderer = null;
    private Material quadMaterial = null;

    private string outlineColorName = "_OutlineColor";
    //private Color outlineColor = Color.white;
    //private bool colorStored = false;

    private GameObject xrRig = null;
    private moveLocomotion moveLocomotionScript = null;
    private SnapTurnProvider snapTurnProviderScript = null;

    private rotateQuad rotateQuadScript = null;
    private adjustQuad adjustQuadScript = null;

    public XRNode handController = UnityEngine.XR.XRNode.RightHand;
    public XRNode joystickController = UnityEngine.XR.XRNode.LeftHand;

    public bool activated = false;
    public bool selected = false;

    public Color selectColorForHandMode = Color.red;
    public Color activateColorForHandMode = Color.blue;

    public Color selectColorForJoystickMode = Color.magenta;
    public Color activateColorForJoystickMode = Color.cyan;

    
    //public Color activateColorForJoystickMode = Color.cyan;

    public Color inactiveColor = Color.white;

    protected override void Awake()
    {
        base.Awake();

        quadRenderer = this.GetComponent<Renderer>();
        quadMaterial = quadRenderer.material;

        inactiveColor = quadMaterial.GetColor(outlineColorName);

        xrRig = GameObject.Find("XR Rig");
        moveLocomotionScript = xrRig.GetComponent<moveLocomotion>();
        snapTurnProviderScript = xrRig.GetComponent<SnapTurnProvider>();

        rotateQuadScript = this.GetComponent<rotateQuad>();
        adjustQuadScript = this.GetComponent<adjustQuad>();
        //textureRessourceName = screenPlane.GetComponent<importDicom>().textureRessourceName;

    }

    protected override void OnSelectEnter(XRBaseInteractor interactor)
    {
        selected = true;

        if(interactor.GetComponent<XRController>().controllerNode == handController)
        {
            base.OnSelectEnter(interactor);

            StoreInteractor(interactor);
            MatchAttachmentPoints(interactor);

            adjustQuadScript.SetAdjustListen(false);

            quadMaterial.SetColor(outlineColorName, selectColorForHandMode);
        }
        else if(interactor.GetComponent<XRController>().controllerNode == joystickController)
        {
            moveLocomotionScript.enabled = false;
            snapTurnProviderScript.enabled = false;

            rotateQuadScript.SetTranslate(true);
            rotateQuadScript.SetRotate(false);

            adjustQuadScript.SetAdjustListen(false);

            quadMaterial.SetColor(outlineColorName, selectColorForJoystickMode);
        }
        
    }

    protected override void OnSelectExit(XRBaseInteractor interactor)
    {
        selected = false;

        OnDeactivate(interactor);

        if(interactor.GetComponent<XRController>().controllerNode == handController)
        {
            base.OnSelectExit(interactor);

            ResetAttachmentPoints(interactor);
            ClearInteractor(interactor);
        }
        else if(interactor.GetComponent<XRController>().controllerNode == joystickController)
        {
            moveLocomotionScript.enabled = true;
            snapTurnProviderScript.enabled = true;

            rotateQuadScript.SetTranslate(false);
            rotateQuadScript.SetRotate(false);
        }
    }

    protected override void OnActivate(XRBaseInteractor interactor)
    {
        activated = true;

        //adjustQuadScript.SetAdjustListen(false);

        if(interactor.GetComponent<XRController>().controllerNode == handController)
        {
            base.OnActivate(interactor);

            MatchAttachmentPoints(interactor);

            adjustQuadScript.SetAdjustListen(false);

            quadMaterial.SetColor(outlineColorName, activateColorForHandMode);

            base.trackRotation = true;
            base.trackPosition = false;
        }
        else if(interactor.GetComponent<XRController>().controllerNode == joystickController)
        {
            rotateQuadScript.SetTranslate(false);
            rotateQuadScript.SetRotate(true);

            adjustQuadScript.SetAdjustListen(false);

            quadMaterial.SetColor(outlineColorName, activateColorForJoystickMode);
        }
    }

    protected override void OnDeactivate(XRBaseInteractor interactor)
    {
        activated = false;

        //adjustQuadScript.SetAdjustListen(true);

        if(interactor.GetComponent<XRController>().controllerNode == handController)
        {
            base.OnDeactivate(interactor);

            MatchAttachmentPoints(interactor);

            if(selected)
            {
                adjustQuadScript.SetAdjustListen(false);

                quadMaterial.SetColor(outlineColorName, selectColorForHandMode);
            }
            else
            {
                adjustQuadScript.SetAdjustListen(true);

                quadMaterial.SetColor(outlineColorName, inactiveColor);
            }

            base.trackRotation = false;
            base.trackPosition = true;
        }
        else if(interactor.GetComponent<XRController>().controllerNode == joystickController)
        {
            if(selected)
            {
                rotateQuadScript.SetTranslate(true);
                rotateQuadScript.SetRotate(false);

                adjustQuadScript.SetAdjustListen(false);

                quadMaterial.SetColor(outlineColorName, selectColorForJoystickMode);
            }
            else
            {
                rotateQuadScript.SetTranslate(false);
                rotateQuadScript.SetRotate(false);

                adjustQuadScript.SetAdjustListen(true);

                quadMaterial.SetColor(outlineColorName, inactiveColor);
            }
        }
    }

    protected override void OnHoverEnter(XRBaseInteractor interactor)
    {
        base.OnHoverEnter(interactor);

        adjustQuadScript.SetAdjustListen(true);
    }

    protected override void OnHoverExit(XRBaseInteractor interactor)
    {
        base.OnHoverExit(interactor);

        adjustQuadScript.SetAdjustListen(false);
    }

    private void StoreInteractor(XRBaseInteractor interactor)
    {
        interactorPosition = interactor.attachTransform.localPosition;
        interactorRotation = interactor.attachTransform.localRotation;
    }

    private void MatchAttachmentPoints(XRBaseInteractor interactor)
    {
        bool hasAttach = attachTransform != null;
        interactor.attachTransform.position = hasAttach ? attachTransform.position : transform.position;
        interactor.attachTransform.rotation = hasAttach ? attachTransform.rotation : transform.rotation;

    }

    private void ResetAttachmentPoints(XRBaseInteractor interactor)
    {
        interactor.attachTransform.localPosition = interactorPosition;
        interactor.attachTransform.localRotation = interactorRotation;
    }

    private void ClearInteractor(XRBaseInteractor interactor)
    {
        interactorPosition = Vector3.zero;
        interactorRotation = Quaternion.identity;
    }

    /*private void StoreOutlineColor(Material material, string colorName)
    {
        if(!colorStored)
        {
            outlineColor = material.GetColor(colorName);
            colorStored = true;
        }
    }*/

    private void SetOutlineColor(Material material, string colorName, Color color)
    {
        //StoreOutlineColor(material, colorName);
        material.SetColor(colorName, color);
    }

}
