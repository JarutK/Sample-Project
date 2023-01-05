using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadCreator : MonoBehaviour
{
    Vector3[] vertices = new Vector3[4];
    public int verticeIndex = 0;
    MeshFilter meshFilter;
    Mesh mesh;

    public void Start()
    {
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        mesh = new Mesh();
        meshFilter = gameObject.AddComponent<MeshFilter>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            mousePos = new Vector3(mousePos.x, mousePos.y, mousePos.z + 1);

            Debug.Log(mousePos);
            if(verticeIndex <= 3 )
            {
                vertices[verticeIndex] = mousePos;
                verticeIndex++;
            }
            else
            {
                verticeIndex = 0;
            }

            if (verticeIndex == 4)
            {
                mesh.vertices = vertices;
                int[] tris = new int[6]
                {
                    // lower left triangle
                    0, 2, 1,
                    // upper right triangle
                    2, 3, 1
                };
                mesh.triangles = tris;

                Vector3[] normals = new Vector3[4]
                {
                    -Vector3.forward,
                    -Vector3.forward,
                    -Vector3.forward,
                    -Vector3.forward
                };
                mesh.normals = normals;

                Vector2[] uv = new Vector2[4]
                {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1)
                };
                mesh.uv = uv;

                meshFilter.mesh = mesh;
            }
        }

        
    }
}
