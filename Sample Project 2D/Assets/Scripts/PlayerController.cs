using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float Speed;
    float horizontal;
    float vertical;
    Rigidbody playerRgbd;

    List<Vector3> drawingVector3s;
    List<Line> drawingLines;

    [SerializeField] QuadCreator quadCreator;

    // Start is called before the first frame update
    void Start()
    {
        playerRgbd = GetComponent<Rigidbody>();
        drawingVector3s = new List<Vector3>();
        drawingLines = new List<Line>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        playerRgbd.velocity = new Vector3(horizontal * Speed, vertical * Speed, 0);

        if(Input.GetKeyDown("w") || Input.GetKeyDown("a") || Input.GetKeyDown("s") || Input.GetKeyDown("d"))
        {
            quadCreator.OnChangeDir(transform.position);
        }

        PushDrawVector3(transform.position);
    }

    public List<Line> GetDrawingLinesInclLive()
    {
        List<Line> rcList = new List<Line>(drawingLines);

        if (drawingVector3s.Count > 0)
        {
            rcList.Add(new Line(drawingVector3s[drawingVector3s.Count - 1], transform.position));
        }
        Debug.Log(drawingVector3s);
        return rcList;
    }

    void PushDrawVector3(Vector3 vector3)
    {
        drawingVector3s.Add(vector3);

        if (drawingVector3s.Count > 1)
        {
            drawingLines.Add(new Line(drawingVector3s[drawingVector3s.Count - 2], drawingVector3s[drawingVector3s.Count - 1]));
        }
    }
}
