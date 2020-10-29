using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snapQuad : MonoBehaviour
{
    public string objectToSnapName = "Quad";
    public float quadSnappedOffset = 0.05f;
    public float quadOrientatedOffset = 2f;
    public Color activeColor = Color.clear;

    private bool isGrabbed = false;
    private bool isInsideSnapZone = false;
    private bool isSnapped = false;

    private GameObject quad = null;
    private grabQuad grabQuadScript = null;
    private rotateQuad rotateQuadScript = null; 

    private BoxCollider snapZoneBoxCollider = null;

    private Material snapZoneMaterial = null;
    private string snapZoneColorName = "_Color";
    private Color inactiveColor = Color.clear;

    private bool objectIsOffset = false;

    public bool IsSnapped
    {
        get { return isSnapped; }
    }

    private bool isActivated = false;
    private bool isDeleted = false;

    // Start is called before the first frame update
    void Start()
    {
        snapZoneBoxCollider = this.GetComponent<BoxCollider>();
        snapZoneMaterial = this.GetComponent<Renderer>().material;

        inactiveColor = snapZoneMaterial.GetColor(snapZoneColorName);
    }

    // Update is called once per frame
    void Update()
    {
        if(quad != null && grabQuadScript != null && rotateQuadScript != null)
        {
            if(!grabQuadScript.Selected && !rotateQuadScript.Translate)
            {
                isGrabbed = false;
            }
            else 
            {
                isGrabbed = true;
            }
            //isDeleted = false;
        }
        else
        {
            //isDeleted = true;
            isGrabbed = false;
        }

        if(isSnapped)
        {
            //snapZoneBoxCollider.isTrigger = false;
            //snapZoneMaterial.SetColor(snapZoneColorName, activeColor);
            //snapZoneMaterial.SetColor(snapZoneColorName, Color.clear);
        }
        else
        {
            //snapZoneBoxCollider.isTrigger = true;
            //snapZoneMaterial.SetColor(snapZoneColorName, inactiveColor);
            /*if(isInsideSnapZone)
            {
                snapZoneMaterial.SetColor(snapZoneColorName, activeColor);
            }
            else
            {
                snapZoneMaterial.SetColor(snapZoneColorName, inactiveColor);
            }*/
            
        }

        /*if(isDeleted)
        {
            snapZoneBoxCollider.isTrigger = true;
        }*/

        /*if(isInsideSnapZone)
        {
            snapZoneMaterial.SetColor(snapZoneColorName, activeColor);
        }
        else
        {
            snapZoneMaterial.SetColor(snapZoneColorName, inactiveColor);
        }*/

        SnapQuad();
    }

    void OnTriggerEnter(Collider other)
    {
        if(!isActivated)
        {
            if(other.gameObject.name == objectToSnapName)
            {
                quad = other.gameObject;
                grabQuadScript = quad.GetComponent<grabQuad>();
                rotateQuadScript = quad.GetComponent<rotateQuad>();

                isInsideSnapZone = true;
                isActivated = true;
                //snapZoneBoxCollider.isTrigger = false;

                //snapZoneMaterial.SetColor(snapZoneColorName, activeColor);
            }
        }
    }

    /*void OnTriggerStay(Collider other)
    {
        if(isActivated)
        {
            if(other.gameObject.name == objectToSnapName)
            {
                if(isSnapped)
                {
                    snapZoneBoxCollider.isTrigger = false;
                }
            }
        }
    }*/

    void OnTriggerExit(Collider other)
    {
        if(isActivated)
        {
            if(other.gameObject.name == objectToSnapName)
            {
                quad.GetComponent<BoxCollider>().enabled = true;
                snapZoneBoxCollider.isTrigger = true;

                quad = null;
                grabQuadScript = null;
                rotateQuadScript = null;

                isInsideSnapZone = false;
                isActivated = false;
                //snapZoneBoxCollider.isTrigger = true;
                //objectIsOffset = true;

                //snapZoneMaterial.SetColor(snapZoneColorName, inactiveColor);
            }
        }
    }

    void SnapQuad()
    {
        if(!isGrabbed && isInsideSnapZone)
        {
            if(quad != null)
            {
                quad.gameObject.transform.position = this.transform.position;
                quad.gameObject.transform.position -= new Vector3(quadSnappedOffset, 0, 0);
                quad.gameObject.transform.rotation = this.transform.rotation;
                isSnapped = true;

                quad.GetComponent<BoxCollider>().enabled = false;
                snapZoneBoxCollider.isTrigger = false;
            }
            else
            {
                snapZoneBoxCollider.isTrigger = true;
                //isSnapped = false;
                isInsideSnapZone = false;
                //isActivated = false;
            }

            //isActivated = true;


            //snapZoneMaterial.SetColor(snapZoneColorName, inactiveColor);
        }
        //else if(isGrabbed && isInsideSnapZone)
        //{
            /*if(!objectIsOffset)
            {
                quad.gameObject.transform.position = this.transform.position;
                quad.gameObject.transform.position -= new Vector3(quadOrientatedOffset, 0, 0);
                quad.gameObject.transform.rotation = this.transform.rotation;

                objectIsOffset = true;
            }*/
            
            //isSnapped = false;

            //quad.GetComponent<BoxCollider>().enabled = true;
            //isActivated = true;

            //snapZoneMaterial.SetColor(snapZoneColorName, activeColor);
        //}
        /*else if(isGrabbed && !isInsideSnapZone)
        {
            //objectIsOffset = true;
            isSnapped = false;

        }*/
        else
        {
            //objectIsOffset = false;
            isSnapped = false;
            //isActivated = false;


            //snapZoneMaterial.SetColor(snapZoneColorName, inactiveColor);
        }
    }
}
