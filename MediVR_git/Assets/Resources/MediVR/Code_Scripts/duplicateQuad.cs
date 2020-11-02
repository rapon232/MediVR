using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class duplicateQuad : MonoBehaviour
{
    public XRNode leftControllerNode = XRNode.LeftHand;
    public XRNode rightControllerNode = XRNode.RightHand;

    //public bool cutBlackPixels = false;

    private InputFeatureUsage<bool> duplicateButton = CommonUsages.menuButton;

    private AudioSource audioFXSource = null;
    private AudioClip onButtonPressDown = null;
    private AudioClip onButtonPressUp = null;

    private Color duplicateColor = Color.clear;

    private Color inactiveColor = Color.clear;
    private Color snapshotColor = Color.black;

    private string outlineColorName = "_OutlineColor";

    private InputDevice leftController;
    private InputDevice rightController;

    private List<InputDevice> leftDevices = new List<InputDevice>();
    private List<InputDevice> rightDevices = new List<InputDevice>();

    private Renderer quadRenderer = null;
    private Material quadMaterial = null;

    private bool duplicateListen = false;
    private bool flag = false;

    private GameObject dicomImageQuad = null;
    private string savePath = null;
    //private bool duplicated = false;

    //private GameObject q = null;

    // Start is called before the first frame update
    void Start()
    {
        dicomImageQuad = GameObject.Find("Dicom_Image_Quad");

        audioFXSource = dicomImageQuad.GetComponent<setQuadAudio>().audioFXSource;
        onButtonPressDown = dicomImageQuad.GetComponent<setQuadAudio>().onButtonPressDown;
        onButtonPressUp = dicomImageQuad.GetComponent<setQuadAudio>().onButtonPressUp;

        savePath = dicomImageQuad.GetComponent<importDicom>().textureDestinationPath;

        duplicateColor = dicomImageQuad.GetComponent<setQuadFrameColors>().setDuplicate;
        inactiveColor = dicomImageQuad.GetComponent<setQuadFrameColors>().defaultFrameColor;

        quadRenderer = this.GetComponent<Renderer>();
        quadMaterial = quadRenderer.material;        

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
            if((leftController.TryGetFeatureValue(duplicateButton, out bool press) && press))
            {
                if(!flag)
                {
                    audioFXSource.PlayOneShot(onButtonPressDown);

                    //var check = this.GetComponent<isQuadDuplicate>();

                    if(this.tag == "Duplicate")
                    {
                        Destroy(this.gameObject);
                    }
                    else
                    {
                        DuplicateQuad();
                    }
                }

                quadMaterial.SetColor(outlineColorName, duplicateColor);

                flag = true;
            }
            else
            {
                //audioFXSource.PlayOneShot(onButtonPressUp);

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

    public void SaveAllDuplicates()
    {
        //Debug.Log($"Saving produced slice(s).");

        GameObject[] images;
        images = GameObject.FindGameObjectsWithTag("Duplicate");

        if(images.Length > 0)
        {
            Debug.Log($"{images.Length} Slice(s) being saved to: {savePath}.");

            DateTime nowTime = DateTime.Now;

            foreach (GameObject GO in images)
            {
                var tex = GO.GetComponent<Renderer>().material.GetTexture("_MainTex") as Texture2D;
                dicomImageTools.SaveTextureToPNGFile(tex, savePath, "Slice", nowTime);
            }

            Debug.Log($"Slice(s) saved.");
        }
        else
        {
            Debug.Log($"No slices produced.");
        }

        
    }

    public void SetDuplicateListen(bool state)
    {
        duplicateListen = state;
        //Debug.Log($"Duplicate Listener set to: {duplicateListen}!");
    }

    public GameObject InstantiateDuplicateQuad(GameObject quad, Material material, Texture2D tex)
    {
        GameObject newQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);

        newQuad.tag = "Duplicate";
        newQuad.layer = LayerMask.NameToLayer("Quad");

        Debug.Log($"{quad.name} copied and tagged: {newQuad.tag}!");

        newQuad.transform.position = quad.transform.position;
        newQuad.transform.rotation = quad.transform.rotation;
        newQuad.transform.localScale = quad.transform.localScale;
        newQuad.transform.Translate(Vector3.right * 3, Space.Self);

        var newQuadRend = newQuad.GetComponent<Renderer>();

        newQuadRend.material = material;
        newQuadRend.material.SetTexture("_MainTex", tex);
        newQuadRend.material.SetColor(outlineColorName, inactiveColor);

        /*if(!cutBlackPixels)
        {
            newQuadRend.material.SetInt("_CutBlackPixels", 0);
        }
        else
        {
            newQuadRend.material.SetInt("_CutBlackPixels", 1);
        }*/
        

        newQuad.AddComponent<Rigidbody>();
        newQuad.AddComponent<BoxCollider>();
        //newQuad.AddComponent<CharacterController>();
        //newQuad.AddComponent<SphereCollider>();

        //newQuad.AddComponent<isQuadDuplicate>();
        newQuad.AddComponent<duplicateQuad>();
        newQuad.AddComponent<rotateQuad>();
        //newQuad.AddComponent<adjustQuad>();
        newQuad.AddComponent<grabQuad>();
        //newQuad.AddComponent<detectQuadCollision>();

        var newQuadRb = newQuad.GetComponent<Rigidbody>();

        newQuadRb.useGravity = false;
        newQuadRb.isKinematic = true;
        //newQuadRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        //var newQuadGQ = newQuad.GetComponent<grabQuad>();
        //newQuadGQ.trackRotation = true;
        //newQuadGQ.

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
