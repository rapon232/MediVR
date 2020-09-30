using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class offsetGrabInteractable : XRGrabInteractable
{
    private Vector3 interactorPosition = Vector3.zero;
    private Quaternion interactorRotation = Quaternion.identity;

    private Renderer quadRenderer;
    private Material quadMaterial;
    private Color outlineColor;
    string outlineColorName = "_OutlineColor";
    //private bool colorStored = false;

    public bool activated = false;
    public bool selected = false;

    public Color selectColor = Color.red;
    public Color activateColor = Color.blue;
    public Color inactiveColor = Color.white;

    protected override void Awake()
    {
        base.Awake();

        quadRenderer = this.GetComponent<Renderer>();
        quadMaterial = quadRenderer.material;

        inactiveColor = quadMaterial.GetColor(outlineColorName);
    }

    protected override void OnSelectEnter(XRBaseInteractor interactor)
    {
        selected = true;

        base.OnSelectEnter(interactor);

        StoreInteractor(interactor);
        MatchAttachmentPoints(interactor);

        quadMaterial.SetColor(outlineColorName, selectColor);
    }

    protected override void OnSelectExit(XRBaseInteractor interactor)
    {
        selected = false;

        OnDeactivate(interactor);

        base.OnSelectExit(interactor);

        ResetAttachmentPoints(interactor);
        ClearInteractor(interactor);
    }

    protected override void OnActivate(XRBaseInteractor interactor)
    {
        activated = true;

        base.OnActivate(interactor);

        quadMaterial.SetColor(outlineColorName, activateColor);

        base.trackRotation = true;
        base.trackPosition = false;
    }

    protected override void OnDeactivate(XRBaseInteractor interactor)
    {
        activated = false;

        base.OnDeactivate(interactor);

        if(selected)
        {
            quadMaterial.SetColor(outlineColorName, selectColor);
        }
        else
        {
            quadMaterial.SetColor(outlineColorName, inactiveColor);
        }

        MatchAttachmentPoints(interactor);

        base.trackRotation = false;
        base.trackPosition = true;
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
