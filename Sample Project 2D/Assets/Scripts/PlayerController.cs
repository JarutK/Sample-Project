using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float Speed;
    float horizontal;
    float vertical;
    Rigidbody rigidbody;

    List<Vector3> drawingVector3s;
    List<Line> drawingLines;

    List<float> xVector3s;
    List<float> yVector3s;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        drawingVector3s = new List<Vector3>();
        drawingLines = new List<Line>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        rigidbody.velocity = new Vector3(horizontal * Speed, vertical * Speed, 0);

        PushDrawVector3(transform.position);
    }

    public List<Line> GetDrawingLinesInclLive()
    {
        List<Line> rcList = new List<Line>(drawingLines);

        if (drawingVector3s.Count > 0)
        {
            rcList.Add(new Line(drawingVector3s[drawingVector3s.Count - 1], transform.position));
        }

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
