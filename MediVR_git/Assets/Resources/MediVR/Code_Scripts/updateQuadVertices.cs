using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class updateQuadVertices : MonoBehaviour
{
    private Vector3 startCoords;
    //public Vector3 updatedCoords;
    //public GameObject Quad;
    private Renderer quadRenderer;
    private MeshFilter quadMeshFilter;

    // Start is called before the first frame update
    void Start()
    {
        startCoords = this.transform.position;

        quadMeshFilter = this.GetComponent<MeshFilter>();
        quadRenderer = this.GetComponent<Renderer>();
        //Quad = GameObject.Find("Quad");
        
        //quadRenderer.material.SetTexture("_MainTex", Resources.Load<Texture3D>("Textures/Dicom 3D Textures/CT_Series_3DTexture_256x256x512"));
        quadRenderer.material.SetVector("_StartCoords", startCoords);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(transform.hasChanged)
        {
            //updatedCoords = this.transform.position;
            //quadRenderer.material.SetVector("_MovingCoords", (startCoords - updatedCoords));
            CalculateCoords();
            transform.hasChanged = false;
        }
        
    }

    void CalculateCoords()
    {
        List<Vector3> vertices = new List<Vector3>(quadMeshFilter.mesh.vertices);
        for (int i = 0; i < vertices.Count; i++)
        {
             vertices[i] = transform.TransformPoint(vertices[i]);
             quadMeshFilter.mesh.SetUVs(0, vertices);
        }
    }
}
