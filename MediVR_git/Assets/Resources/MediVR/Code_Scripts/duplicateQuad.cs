using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class duplicateQuad : MonoBehaviour
{
    public XRNode leftControllerNode = XRNode.LeftHand;
    public XRNode rightControllerNode = XRNode.RightHand;

    public bool duplicateListen = false;

    public Color duplicateColor = Color.green;

    private Color inactiveColor = Color.white;
    private Color snapshotColor = Color.black;

    private string outlineColorName = "_OutlineColor";

    private InputDevice leftController;
    private InputDevice rightController;

    private List<InputDevice> leftDevices = new List<InputDevice>();
    private List<InputDevice> rightDevices = new List<InputDevice>();

    private Renderer quadRenderer = null;
    private Material quadMaterial = null;

    private bool flag = false;
    //private bool duplicated = false;

    //private GameObject q = null;

    // Start is called before the first frame update
    void Start()
    {
        quadRenderer = this.GetComponent<Renderer>();
        quadMaterial = quadRenderer.material;

        inactiveColor = quadMaterial.GetColor(outlineColorName);

        GetControllers();

        //q = GameObject.Find("Quad");
    }

    // Update is called once per frame
    void Update()
    {
        if(leftController == null || rightController == null)
        {
            GetControllers();
            //Debug.Log("Got Controllers Update");
        }

        DuplicateListen();
        //DuplicateQuad();

        /*if(Input.GetMouseButtonDown(0))
            {
                //SetDuplicate(true);

                var tex = GetTextureFromShader(this.gameObject, quadRenderer.material.mainTexture.width, quadRenderer.material.mainTexture.height);

                var q = InstantiateDuplicateQuad(this.gameObject, tex);

                //var rend = q.GetComponent<Renderer>();

                //rend.material.SetTexture("_MainTex", tex);

                quadMaterial.SetColor(outlineColorName, duplicateColor);

                flag = true;
            }
            else
            {
                if(flag)
                {
                    //SetDuplicate(false);

                    quadMaterial.SetColor(outlineColorName, inactiveColor);

                    flag = false;
                }
            }*/
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

    private void DuplicateListen()
    {
        if(duplicateListen)
        {
            if((leftController.TryGetFeatureValue(CommonUsages.menuButton, out bool press) && press))
            {
                if(!flag)
                {
                    var check = this.GetComponent<isQuadDuplicate>();

                    if(check == null)
                    {
                        DuplicateQuad();
                    }
                    else
                    {
                        Destroy(this.gameObject);
                    }
                }

                quadMaterial.SetColor(outlineColorName, duplicateColor);

                flag = true;
            }
            else
            {
                if(flag)
                {
                    quadMaterial.SetColor(outlineColorName, inactiveColor);

                    flag = false;
                }
            }
            
        }
        
    }

    private void DuplicateQuad()
    {
        var newMaterial = Resources.Load<Material>("MediVR/Materials/duplicateMaterial");

        var newTexture = GetTextureFromShader(this.gameObject, 1024, 1024);

        var newQuad = InstantiateDuplicateQuad(this.gameObject, newMaterial, newTexture);

        //var rend = q.GetComponent<Renderer>();

        //rend.material.SetTexture("_MainTex", tex);
    }

    public void SetDuplicateListen(bool state)
    {
        duplicateListen = state;
        //Debug.Log($"Duplicate Listener set to: {duplicateListen}!");
    }

    public GameObject InstantiateDuplicateQuad(GameObject quad, Material material, Texture2D tex)
    {
        GameObject newQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);

        Debug.Log($"{quad.name} copied!");

        newQuad.transform.position = quad.transform.position;
        newQuad.transform.rotation = quad.transform.rotation;
        newQuad.transform.localScale = quad.transform.localScale;
        newQuad.transform.Translate(Vector3.left * 3, Space.Self);

        var newQuadRend = newQuad.GetComponent<Renderer>();

        newQuadRend.material = material;
        newQuadRend.material.SetTexture("_MainTex", tex);
        newQuadRend.material.SetColor(outlineColorName, inactiveColor);

        newQuad.AddComponent<Rigidbody>();

        newQuad.AddComponent<isQuadDuplicate>();
        newQuad.AddComponent<duplicateQuad>();
        newQuad.AddComponent<rotateQuad>();
        newQuad.AddComponent<adjustQuad>();
        newQuad.AddComponent<grabQuad>();

        var newQuadRb = newQuad.GetComponent<Rigidbody>();

        newQuadRb.useGravity = false;
        newQuadRb.isKinematic = true;

        return newQuad;
    }


    public Texture2D GetTextureFromShader(GameObject quad, int width, int height)
    {
        //Create render texture:
        RenderTexture temp = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
 
        //Create a Quad:
        /*GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        MeshRenderer rend = quad.GetComponent<MeshRenderer>();
        rend.material = mat;*/
        Vector3 quadScale = quad.transform.localScale;// / (float)((Screen.height / 2.0) / Camera.main.orthographicSize);
        //quad.transform.position = Vector3.forward;
 
        //Setup camera:
        GameObject camera = new GameObject("CaptureCam");
        Camera orthoCam = camera.AddComponent<Camera>();
        //cam.transform.LookAt(quad.transform);
        //cam.renderingPath = RenderingPath.Forward;
        orthoCam.transform.position = quad.transform.position;
        orthoCam.transform.rotation = quad.transform.rotation;
        //cam.transform.position = new Vector3(quad.transform.position.x, quad.transform.position.y, quad.transform.position.z - 1);
        orthoCam.transform.Translate(Vector3.back, Space.Self);

        orthoCam.cullingMask = 1 << LayerMask.NameToLayer("Quad");
        
        orthoCam.orthographic = true;
        orthoCam.clearFlags = CameraClearFlags.SolidColor;
        orthoCam.backgroundColor = new Color(0, 0, 0, 1);
        if (orthoCam.rect.width < 1 || orthoCam.rect.height < 1)
        {
            orthoCam.rect = new Rect(orthoCam.rect.x, orthoCam.rect.y, 1, 1);
        }
        orthoCam.orthographicSize = 1;
        orthoCam.rect = new Rect(0, 0, quadScale.x, quadScale.y);
        orthoCam.aspect = quadScale.x / quadScale.y;
        orthoCam.targetTexture = temp;
        orthoCam.allowHDR = false;

        quadMaterial.SetColor(outlineColorName, snapshotColor);
 
        //Capture image and write to the render texture:
        orthoCam.Render();
        temp = orthoCam.targetTexture;

        quadMaterial.SetColor(outlineColorName, inactiveColor);
 
        //Apply changes:
        Texture2D newTex = new Texture2D(temp.width, temp.height, TextureFormat.ARGB32, true, true);
        RenderTexture.active = temp;
        newTex.ReadPixels(new Rect(0, 0, temp.width, temp.height), 0, 0);
        newTex.Apply();
 
        //Clean up:
        RenderTexture.active = null;
        temp.Release();
        Destroy(camera);
 
        return newTex;
    }

}
