using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class grabQuad : XRGrabInteractable
{
    public XRNode handController = UnityEngine.XR.XRNode.RightHand;
    public XRNode joystickController = UnityEngine.XR.XRNode.LeftHand;

    public Color selectColorForHandMode = Color.clear;
    public Color activateColorForHandMode = Color.clear;

    public Color selectColorForJoystickMode = Color.clear;
    public Color activateColorForJoystickMode = Color.clear;

    public float moveSpeedWhileSelected = 0.8f;

    public AudioSource audioFXSource = null;
    public AudioClip onButtonPressDown = null;
    public AudioClip onButtonPressUp = null;
    
    //public Color activateColorForJoystickMode = Color.cyan;

    private Color inactiveColor = Color.clear;
    private float inactiveMoveSpeed = 0f;

    private bool selected = false;
    //private bool activated = false;

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
    private duplicateQuad duplicateQuadScript = null;

    private GameObject dicomImageQuad = null;

    private bool audioSelectFlag = false;
    private bool audioActivateFlag = false;

    public bool Selected
    {
        get { return selected; }
    }

    protected override void Awake()
    {
        base.Awake();

        base.smoothPosition = true;
        base.smoothPositionAmount = 20.0f;
        base.tightenPosition = 0.345f;
        base.trackRotation = false;
        base.smoothRotationAmount = 20.0f;
        base.tightenRotation = 0.345f;
        base.throwOnDetach = false;

        dicomImageQuad = GameObject.Find("Dicom_Image_Quad");

        audioFXSource = dicomImageQuad.GetComponent<setQuadAudio>().audioFXSource;
        onButtonPressDown = dicomImageQuad.GetComponent<setQuadAudio>().onButtonPressDown;
        onButtonPressUp = dicomImageQuad.GetComponent<setQuadAudio>().onButtonPressUp;

        selectColorForHandMode = dicomImageQuad.GetComponent<setQuadFrameColors>().handTranslate;
        activateColorForHandMode = dicomImageQuad.GetComponent<setQuadFrameColors>().handRotate;
        selectColorForJoystickMode = dicomImageQuad.GetComponent<setQuadFrameColors>().joystickTranslate;
        activateColorForJoystickMode = dicomImageQuad.GetComponent<setQuadFrameColors>().joystickRotate;
        inactiveColor = dicomImageQuad.GetComponent<setQuadFrameColors>().defaultFrameColor;

        quadRenderer = this.GetComponent<Renderer>();
        quadMaterial = quadRenderer.material;

        xrRig = GameObject.Find("XR Rig");
        moveLocomotionScript = xrRig.GetComponent<moveLocomotion>();
        snapTurnProviderScript = xrRig.GetComponent<SnapTurnProvider>();

        inactiveMoveSpeed = moveLocomotionScript.moveSpeed;

        rotateQuadScript = this.GetComponent<rotateQuad>();
        duplicateQuadScript = this.GetComponent<duplicateQuad>();

        if(this.gameObject.tag != "Duplicate")
        {
            adjustQuadScript = this.GetComponent<adjustQuad>();
        }
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

            if(!audioSelectFlag)
            {
                audioFXSource.PlayOneShot(onButtonPressDown);
                audioSelectFlag = true;
            }

            if(this.gameObject.tag != "Duplicate")
            {
                moveLocomotionScript.moveSpeed = moveSpeedWhileSelected;
                adjustQuadScript.SetAdjustListen(false);
            }
            else
            {
                base.trackRotation = true;
            }

            quadMaterial.SetColor(outlineColorName, selectColorForHandMode);
        }
        else if(interactor.GetComponent<XRController>().controllerNode == joystickController)
        {
            if(!audioSelectFlag)
            {
                audioFXSource.PlayOneShot(onButtonPressDown);
                audioSelectFlag = true;
            }

            moveLocomotionScript.enabled = false;
            snapTurnProviderScript.enabled = false;

            rotateQuadScript.SetTranslate(true);
            rotateQuadScript.SetRotate(false);

            if(this.gameObject.tag != "Duplicate")
            {
                adjustQuadScript.SetAdjustListen(false);
            }

            quadMaterial.SetColor(outlineColorName, selectColorForJoystickMode);
        }
        
    }

    protected override void OnSelectExit(XRBaseInteractor interactor)
    {
        selected = false;

        //audioFXSource.PlayOneShot(onButtonPressUp);

        OnDeactivate(interactor);

        if(interactor.GetComponent<XRController>().controllerNode == handController)
        {
            base.OnSelectExit(interactor);

            ResetAttachmentPoints(interactor);
            ClearInteractor(interactor);

            audioSelectFlag = false;
            audioActivateFlag = false;

            if(this.gameObject.tag != "Duplicate")
            {
                moveLocomotionScript.moveSpeed = inactiveMoveSpeed;
            }
            else
            {
                base.trackRotation = false;
            }
        }
        else if(interactor.GetComponent<XRController>().controllerNode == joystickController)
        {
            audioSelectFlag = false;
            audioActivateFlag = false;

            moveLocomotionScript.enabled = true;
            snapTurnProviderScript.enabled = true;

            rotateQuadScript.SetTranslate(false);
            rotateQuadScript.SetRotate(false);
        }
    }

    protected override void OnActivate(XRBaseInteractor interactor)
    {
        //activated = true;

        //adjustQuadScript.SetAdjustListen(false);

        if(interactor.GetComponent<XRController>().controllerNode == handController)
        {
            base.OnActivate(interactor);

            MatchAttachmentPoints(interactor);

            if(!audioActivateFlag)
            {
                audioFXSource.PlayOneShot(onButtonPressUp);
                audioActivateFlag = true;
            }

            if(this.gameObject.tag != "Duplicate")
            {
                moveLocomotionScript.moveSpeed = moveSpeedWhileSelected;
                adjustQuadScript.SetAdjustListen(false);
            }

            quadMaterial.SetColor(outlineColorName, activateColorForHandMode);

            base.trackRotation = true;
            base.trackPosition = false;
        }
        else if(interactor.GetComponent<XRController>().controllerNode == joystickController)
        {
            if(!audioActivateFlag)
            {
                audioFXSource.PlayOneShot(onButtonPressUp);
                audioActivateFlag = true;
            }

            rotateQuadScript.SetTranslate(false);
            rotateQuadScript.SetRotate(true);

            if(this.gameObject.tag != "Duplicate")
            {
                adjustQuadScript.SetAdjustListen(false);
            }

            quadMaterial.SetColor(outlineColorName, activateColorForJoystickMode);
        }
    }

    protected override void OnDeactivate(XRBaseInteractor interactor)
    {
        //activated = false;

        //audioFXSource.PlayOneShot(onButtonPressUp);

        //adjustQuadScript.SetAdjustListen(true);

        if(interactor.GetComponent<XRController>().controllerNode == handController)
        {
            base.OnDeactivate(interactor);

            MatchAttachmentPoints(interactor);

            if(selected)
            {
                audioSelectFlag = true;
                audioActivateFlag = false;

                if(this.gameObject.tag != "Duplicate")
                {
                    adjustQuadScript.SetAdjustListen(false);
                }

                quadMaterial.SetColor(outlineColorName, selectColorForHandMode);
            }
            else
            {
                audioSelectFlag = false;
                audioActivateFlag = false;

                if(this.gameObject.tag != "Duplicate")
                {
                    moveLocomotionScript.moveSpeed = inactiveMoveSpeed;
                    adjustQuadScript.SetAdjustListen(true);
                }

                quadMaterial.SetColor(outlineColorName, inactiveColor);
            }

            base.trackRotation = false;
            base.trackPosition = true;
        }
        else if(interactor.GetComponent<XRController>().controllerNode == joystickController)
        {
            if(selected)
            {
                audioSelectFlag = true;
                audioActivateFlag = false;

                rotateQuadScript.SetTranslate(true);
                rotateQuadScript.SetRotate(false);

                if(this.gameObject.tag != "Duplicate")
                {
                    adjustQuadScript.SetAdjustListen(false);
                }

                quadMaterial.SetColor(outlineColorName, selectColorForJoystickMode);
            }
            else
            {
                audioSelectFlag = false;
                audioActivateFlag = false;

                rotateQuadScript.SetTranslate(false);
                rotateQuadScript.SetRotate(false);

                if(this.gameObject.tag != "Duplicate")
                {
                    adjustQuadScript.SetAdjustListen(true);
                }

                quadMaterial.SetColor(outlineColorName, inactiveColor);
            }
        }
    }

    protected override void OnHoverEnter(XRBaseInteractor interactor)
    {
        base.OnHoverEnter(interactor);

        if(this.gameObject.tag != "Duplicate")
        {
            adjustQuadScript.SetAdjustListen(true);
        }

        if(interactor.GetComponent<XRController>().controllerNode == joystickController)
        {
            duplicateQuadScript.SetDuplicateListen(true);
        }
    }

    protected override void OnHoverExit(XRBaseInteractor interactor)
    {
        base.OnHoverExit(interactor);

        if(this.gameObject.tag != "Duplicate")
        {
            adjustQuadScript.SetAdjustListen(false);
        }

        if(interactor.GetComponent<XRController>().controllerNode == joystickController)
        {
            duplicateQuadScript.SetDuplicateListen(false);
        }
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
