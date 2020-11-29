/*

    MediVR, a medical Virtual Reality application for exploring 3D medical datasets on the Oculus Quest.

    Copyright (C) 2020  Dimitar Tahov

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    This script serves to pass quad vertices to flattened rendering shader.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updateQuadVertices : MonoBehaviour
{
    private Vector3 startCoords;
    private Renderer quadRenderer;
    private MeshFilter quadMeshFilter;

    // Start is called before the first frame update
    void Start()
    {
        startCoords = this.transform.position;

        quadMeshFilter = this.GetComponent<MeshFilter>();
        quadRenderer = this.GetComponent<Renderer>();
        
        quadRenderer.material.SetVector("_StartCoords", startCoords);
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.hasChanged)
        {
            CalculateCoords();
            transform.hasChanged = false;
        }
    }

    //GET LIST OF VERTICES OF QUAD AND UPDATE MESH RENDERER
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
