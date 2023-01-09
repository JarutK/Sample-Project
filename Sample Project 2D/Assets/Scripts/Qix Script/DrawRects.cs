using System.Collections.Generic;
using UnityEngine;

public class DrawRects : MonoBehaviour
{
    static List<Rect> rects;
    static List<Rect> newRectList;
    static bool newRects = true;

    Vector3[] vertices;
    Vector2[] uv;
    int[] triangles;

    Mesh mesh;
    float score = 0.0f;

    public static Rect hitRect;

    void Start()
    {
        rects = new List<Rect>();
    }

    void Update()
    {
        if (newRects)
        {
            NewWay();
        }
    }


    private void NewWay()
    {
        List<Rect> sourceRects;
        float z = 0.03f;

        int verticesOffset;
        int uvOffset;
        int triangleOffset;

        if ((newRectList != null) && true)
        {

            verticesOffset = vertices.Length;
            uvOffset = uv.Length;
            triangleOffset = triangles.Length;

            System.Array.Resize(ref vertices, vertices.Length + newRectList.Count * 4);
            System.Array.Resize(ref uv, uv.Length + newRectList.Count * 4);
            System.Array.Resize(ref triangles, triangles.Length + newRectList.Count * 6);

            sourceRects = newRectList;
        }
        else
        {
            mesh = new Mesh();

            vertices = new Vector3[rects.Count * 4];
            uv = new Vector2[rects.Count * 4];
            triangles = new int[rects.Count * 6];

            verticesOffset = 0;
            uvOffset = 0;
            triangleOffset = 0;

            sourceRects = rects;

            score = 0.0f;
        }

        Rect rect;

        for (int i = 0; i < sourceRects.Count; i++)
        {
            rect = sourceRects[i];

            vertices[verticesOffset + i * 4 + 0] = new Vector3(rect.xMax, rect.yMax, z);
            vertices[verticesOffset + i * 4 + 1] = new Vector3(rect.xMax, rect.yMin, z);
            vertices[verticesOffset + i * 4 + 2] = new Vector3(rect.xMin, rect.yMax, z);
            vertices[verticesOffset + i * 4 + 3] = new Vector3(rect.xMin, rect.yMin, z);

            uv[uvOffset + i * 4 + 0] = new Vector2((rect.xMax + 1) / 2, (rect.yMax + 1) / 2);
            uv[uvOffset + i * 4 + 1] = new Vector2((rect.xMax + 1) / 2, 0);
            uv[uvOffset + i * 4 + 2] = new Vector2(0, (rect.yMax + 1) / 2);
            uv[uvOffset + i * 4 + 3] = new Vector2(0, 0);

            triangles[triangleOffset + i * 6 + 0] = (verticesOffset + 4 * i + 0);
            triangles[triangleOffset + i * 6 + 1] = (verticesOffset + 4 * i + 1);
            triangles[triangleOffset + i * 6 + 2] = (verticesOffset + 4 * i + 2);
            triangles[triangleOffset + i * 6 + 3] = (verticesOffset + 4 * i + 2);
            triangles[triangleOffset + i * 6 + 4] = (verticesOffset + 4 * i + 1);
            triangles[triangleOffset + i * 6 + 5] = (verticesOffset + 4 * i + 3);

            score += (rect.width * rect.height);
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        ScoreManager.Score = score;

        GetComponent<MeshFilter>().mesh = mesh;

        newRects = false;
    }

    public static void AddRects(List<Rect> rects)
    {
        DrawRects.rects.AddRange(rects);
        DrawRects.newRectList = rects;
        newRects = true;
    }


    public static bool InRects(Vector3 point)
    {
        bool rc = false;

        foreach (Rect rect in rects)
        {
            if (rect.Contains(point))
            {
                hitRect = rect;
                rc = true;
                break;
            }
        }

        return rc;
    }
}

