using UnityEngine;

public class QuadCreator : MonoBehaviour
{
    Vector3[] vertices = new Vector3[4];
    public int verticeIndex = 0;
    [SerializeField] Material imageMaterial;
    [SerializeField] GameObject quadPref;
    Mesh mesh;

    public void Start()
    {
        quadPref.GetComponent<MeshRenderer>().sharedMaterial = imageMaterial;
    }

    private void Update()
    {

    }

    void GenerateQuad()
    {
        mesh = new Mesh();
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

        mesh.uv = UV();

        quadPref.GetComponent<MeshFilter>().mesh = mesh;
        quadPref.GetComponent<MeshCollider>().sharedMesh = mesh;

        Instantiate(quadPref);
    }

    Vector2 MidPosition()
    {
        float totalX = 0f;
        float totalY = 0f;
        for (int i = 0; i < vertices.Length; i++)
        {
            totalX += vertices[i].x;
            totalY += vertices[i].y;
        }

        Vector2 center = new Vector2(totalX / vertices.Length, totalY / vertices.Length);
        return center;
    }

    void OrderPosition()
    {
        Vector2 center = MidPosition();
        Vector2[] tempPos = new Vector2[vertices.Length];
        for (int i = 0; i < 4; i++)
        {
            //vertice 0
            if (vertices[i].x <= center.x && vertices[i].y <= center.y)
            {
                tempPos[0] = vertices[i];
            }
            //vertice 1
            if (vertices[i].x >= center.x && vertices[i].y <= center.y)
            {
                tempPos[1] = vertices[i];
            }
            //vertice 2
            if (vertices[i].x <= center.x && vertices[i].y >= center.y)
            {
                tempPos[2] = vertices[i];
            }
            //vertice 3
            if (vertices[i].x >= center.x && vertices[i].y >= center.y)
            {
                tempPos[3] = vertices[i];
            }
        }
        for (int j = 0; j < vertices.Length; j++)
        {
            vertices[j] = tempPos[j];
        }
    }

    Vector2[] UV()
    {
        Vector2[] uv = new Vector2[4];

        for (int i = 0; i < vertices.Length; i++)
        {
            uv[i] = new Vector3((vertices[i].x + 8) / 16, (vertices[i].y + 4) / 8, vertices[i].z);
        }

        return uv;
    }

    public void OnChangeDir(Vector3 dir)
    {
        if (verticeIndex <= vertices.Length - 1)
        {
            vertices[verticeIndex] = dir;
            verticeIndex++;
            if (verticeIndex == vertices.Length)
            {
                verticeIndex = 0;
                OrderPosition();
                GenerateQuad();

            }
        }
    }
}
