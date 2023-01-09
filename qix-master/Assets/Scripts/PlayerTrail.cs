using UnityEngine;
using System.Collections;

public class PlayerTrail : MonoBehaviour
{
    void Update()
    {
        PlayerMovement mov = GameObject.Find("Player").GetComponent<PlayerMovement>();

        var lines = mov.GetDrawingLinesInclLive().ToArray();
        GetComponent<MeshFilter>().mesh = DynamicLines.GetMesh(lines, 0.01f);
    }
}
