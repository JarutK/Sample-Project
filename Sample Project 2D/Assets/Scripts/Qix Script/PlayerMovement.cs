using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

enum PlayerState
{
    OnWall,
    Drawing
}

public class PlayerMovement : MonoBehaviour
{
    public float speedFact;

    public List<Line> lines { get; private set; }
    Line currentLine;
    PlayerState state;

    List<Vector3> drawingVector3s;
    List<Line> drawingLines;

    List<float> xVector3s;
    List<float> yVector3s;

    float curSign = 0.0f;
    float lastSign = 0.0f;

    bool drawingx;

    float spiraldis = 0.03f;

    float inputX;
    float inputY;

    void Start()
    {
        lines = new List<Line>();

        //Wall Line
        lines.Add(new Line(new Vector3(1, 1, 0), new Vector3(1, -1, 0)));
        lines.Add(new Line(new Vector3(1, -1, 0), new Vector3(-1, -1, 0)));
        lines.Add(new Line(new Vector3(-1, -1, 0), new Vector3(-1, 1, 0)));
        lines.Add(new Line(new Vector3(-1, 1, 0), new Vector3(1, 1, 0)));

        xVector3s = new List<float>();
        yVector3s = new List<float>();

        xVector3s.Add(-1.0f);
        xVector3s.Add(1.0f);
        yVector3s.Add(-1.0f);
        yVector3s.Add(1.0f);

        currentLine = lines[0];

        transform.position = (currentLine.start * 0.5f + currentLine.end * 0.5f);
        state = PlayerState.OnWall;

        drawingVector3s = new List<Vector3>();
        drawingLines = new List<Line>();

        //Draw Wall Line
        DrawLines.SetLines(lines);
    }

    void Update()
    {
        // @@ Issue: too latent
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        Vector3 movement;
        bool movingx = false;
        bool movingy = false;
        bool handled = false;
        float speed = speedFact * Time.deltaTime;
        float desSign;

        //Calculate Target Position to move
        if (Mathf.Max(Mathf.Abs(inputX), Mathf.Abs(inputY)) < 0.3f)
        {
            movement = Vector3.zero;
            desSign = 0.0f;
        }
        else
        {
            if (Mathf.Abs(inputX) > Mathf.Abs(inputY))
            {
                movement = new Vector3(speed * Mathf.Sign(inputX), 0.0f, 0.0f);
                movingx = true;
                desSign = Mathf.Sign(inputX);
            }
            else
            {
                movement = new Vector3(0.0f, speed * Mathf.Sign(inputY), 0.0f);
                movingy = true;
                desSign = Mathf.Sign(inputY);
            }
        }

        //Contain Target Position
        Vector3 newPos = transform.position + movement;

        if (state == PlayerState.OnWall)
        {
            //Check player on the solid line from target position
            if (currentLine.ContainsBound(newPos))
            {
                //Move player
                transform.position = newPos;
                //transform.Translate(inputX * speed, inputY * speed, 0);
                handled = true;
            }
            else if (currentLine.ContainsUnbound(newPos) && (!Input.GetButton("Fire1") || !ValidToMoveTo(newPos)))
            {
                if ((newPos - currentLine.start).magnitude < (newPos - currentLine.end).magnitude)
                {
                    transform.position = currentLine.start;
                }
                else
                {
                    transform.position = currentLine.end;
                }
                handled = true;
            }
            else
            {
                // @@ Issue: what about small movements
                foreach (Line line in lines)
                {
                    if (line.ContainsBound(newPos))
                    {
                        currentLine = line;
                        transform.position = newPos;
                        handled = true;
                        break;
                    }
                }
            }

            if (!handled && Input.GetButton("Fire1"))
            {
                if ((movingx || movingy) && (ValidToMoveTo(newPos)))
                {
                    state = PlayerState.Drawing;
                    drawingx = movingx;
                    PushDrawVector3(transform.position);
                    transform.position = newPos;
                    curSign = desSign;
                }
            }
        }
        else if (state == PlayerState.Drawing)
        {
            if (movingx || movingy)
            {
                //To Check player is crossing the drawing line
                bool crossing = false;

                foreach (Line crossLine in drawingLines)
                {
                    if (LineIntersectsLine(crossLine.start,
                                           crossLine.end,
                                           transform.position,
                                           transform.position + movement.normalized * spiraldis))
                    {
                        crossing = true;
                        break;
                    }
                }

                if (crossing)
                {
                    // can't move
                }
                else if (!ValidToMoveTo(newPos))
                {
                    if (InGrid(newPos))
                    {
                        if (drawingx)
                        {
                            // Moving horizontally.  
                            if (Mathf.Sign(inputX) > 0.0f)
                            {
                                newPos.x = DrawRects.hitRect.xMin;
                            }
                            else
                            {
                                newPos.x = DrawRects.hitRect.xMax;
                            }
                        }
                        else
                        {
                            // Moving vertically.  
                            if (Mathf.Sign(inputY) > 0.0f)
                            {
                                newPos.y = DrawRects.hitRect.yMin;
                            }
                            else
                            {
                                newPos.y = DrawRects.hitRect.yMax;
                            }
                        }
                    }
                    else
                    {
                        newPos = SnapToGrid(newPos); ;
                    }

                    foreach (Line line in lines)
                    {
                        if (line.ContainsBound(newPos))
                        {
                            currentLine = line;
                            break;
                        }
                    }

                    transform.position = newPos;
                    PushDrawVector3(transform.position);

                    FillDrawnArea();

                    lines.AddRange(drawingLines);
                    DrawLines.SetLines(lines);

                    drawingLines.Clear();
                    drawingVector3s.Clear();

                    state = PlayerState.OnWall;
                    curSign = 0.0f;
                    lastSign = 0.0f;
                }
                else
                {
                    if ((curSign == 0.0f) || // first movement always OK
                        ((movingx == drawingx) && (curSign == desSign)) || // moving the same direction as before
                        ((movingx != drawingx) &&
                         ((desSign == lastSign) ||
                          ((newPos - drawingVector3s[drawingVector3s.Count - 1]).magnitude > spiraldis))))
                    {
                        // Valid to move this direction

                        if (movingx != drawingx)
                        {
                            lastSign = curSign;
                            curSign = desSign;

                            drawingx = movingx;
                            PushDrawVector3(transform.position);
                        }

                        transform.position = newPos;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Adds a corner to the current line being drawn.
    /// 
    /// In so doing:
    ///  - adds the Vector3 to the drawingVector3s list
    ///  - if there's now a line formed, adds that line to the drawing line list
    /// </summary>
    /// <param name="Vector3"></param>
    void PushDrawVector3(Vector3 Vector3)
    {
        drawingVector3s.Add(Vector3);

        if (drawingVector3s.Count > 1)
        {
            drawingLines.Add(new Line(drawingVector3s[drawingVector3s.Count - 2], drawingVector3s[drawingVector3s.Count - 1]));
        }

        MaybeAddVector3(xVector3s, yVector3s, Vector3);
        xVector3s.Sort();
        yVector3s.Sort();
    }

    //Check player can move to target position?
    public static bool ValidToMoveTo(Vector3 newPos)
    {
        return InGrid(newPos) && !DrawRects.InRects(newPos);
    }

    //Check Player is moving in gridbackground
    public static bool InGrid(Vector3 newPos)
    {
        return ((newPos.x >= -1.0f) &&
                (newPos.x <= 1.0f) &&
                (newPos.y >= -1.0f) &&
                (newPos.y <= 1.0f));
    }

    /// <summary>
    /// Snaps a Vecto3 back to being within the main grid.
    /// </summary>
    /// <param name="newPos">Attempted position</param>
    /// <returns></returns>
    Vector3 SnapToGrid(Vector3 newPos)
    {
        Vector3 snap;

        if (InGrid(newPos))
        {
            snap = newPos;
        }
        else
        {
            snap = new Vector3(Mathf.Clamp(newPos.x, -1.0f, 1.0f),
                               Mathf.Clamp(newPos.y, -1.0f, 1.0f),
                               0.0f);
        }

        return snap;
    }

    void FillDrawnArea()
    {
        // Get dimensions
        int w = xVector3s.Count;
        int h = yVector3s.Count;

        // Work out edges.  
        // @@ Issue: includes irrelevant internal edges
        bool[,] vsides = new bool[w - 1, h];
        bool[,] hsides = new bool[w, h - 1];
        MapLinesToSides(vsides, hsides, drawingLines);
        MapLinesToSides(vsides, hsides, lines);

        // Work out where our floo start Vector3s are.
        int fillx1;
        int filly1;
        int fillx2;
        int filly2;
        ExtractFloodStartVector3s(out fillx1, out filly1, out fillx2, out filly2);

        // Flood fill from each of the two places.
        List<Rect> drawRects1 = new List<Rect>();
        List<Rect> drawRects2 = new List<Rect>();
        float area1 = 0.0f;
        float area2 = 0.0f;

        bool baddyIn1;
        bool baddyIn2;

        area1 = FloodFill(w, h, vsides, hsides, drawRects1, fillx1, filly1, out baddyIn1);
        area2 = FloodFill(w, h, vsides, hsides, drawRects2, fillx2, filly2, out baddyIn2);

        //check zone to draw rect
        if (baddyIn2)
        {
            AddRectsToDeadZone(drawRects1);
        }
        else
        {
            AddRectsToDeadZone(drawRects2);
        }
    }

    private static void AddRectsToDeadZone(List<Rect> drawRects1)
    {
        DrawRects.AddRects(drawRects1);
    }

    private void ExtractFloodStartVector3s(out int fillx1, out int filly1, out int fillx2, out int filly2)
    {
        // We now flood fill from where we just ended.
        // Work out where that is, and hence, given our movement direction, the two starting squares.
        int ourx = xVector3s.IndexOf(transform.position.x);
        int oury = yVector3s.IndexOf(transform.position.y);

        if (drawingx)
        {
            // Moving horizonally.  
            if (Mathf.Sign(inputX) > 0.0f)
            {
                fillx1 = ourx - 1;
            }
            else
            {
                fillx1 = ourx;
            }

            fillx2 = fillx1;
            filly1 = oury;
            filly2 = oury - 1;
        }
        else
        {
            // Moving vertically.
            if (Mathf.Sign(inputY) > 0.0f)
            {
                filly1 = oury - 1;
            }
            else
            {
                filly1 = oury;
            }

            filly2 = filly1;
            fillx1 = ourx;
            fillx2 = ourx - 1;
        }
    }

    private float FloodFill(int w,
                            int h,
                            bool[,] vsides,
                            bool[,] hsides,
                            List<Rect> drawRects,
                            int fillx1,
                            int filly1,
                            out bool containsBaddy)
    {
        float nArea = 0.0f;
        Queue<IntTuple> queue = new Queue<IntTuple>();
        bool[,] floodRects = new bool[w - 1, h - 1];
        queue.Enqueue(new IntTuple(fillx1, filly1));

        containsBaddy = false;

        while (queue.Count > 0)
        {
            IntTuple tuple = queue.Dequeue();
            if (!floodRects[tuple.x, tuple.y])
            {
                floodRects[tuple.x, tuple.y] = true;
                Rect rect = new Rect(xVector3s[tuple.x], yVector3s[tuple.y], xVector3s[tuple.x + 1] - xVector3s[tuple.x], yVector3s[tuple.y + 1] - yVector3s[tuple.y]);
                drawRects.Add(rect);

                if (!containsBaddy)
                {
                    containsBaddy = true;
                }

                nArea += ((xVector3s[tuple.x + 1] - xVector3s[tuple.x]) * (yVector3s[tuple.y + 1] - yVector3s[tuple.y]));

                if ((tuple.x > 0) && (!vsides[tuple.x - 1, tuple.y]))
                {
                    // Can move left

                    queue.Enqueue(new IntTuple(tuple.x - 1, tuple.y));
                }

                if ((tuple.x < (w - 2)) && (!vsides[tuple.x, tuple.y]))
                {
                    // Can move right

                    queue.Enqueue(new IntTuple(tuple.x + 1, tuple.y));
                }

                if ((tuple.y > 0) && (!hsides[tuple.x, tuple.y - 1]))
                {
                    // Can move down

                    queue.Enqueue(new IntTuple(tuple.x, tuple.y - 1));
                }

                if ((tuple.y < (h - 2)) && (!hsides[tuple.x, tuple.y]))
                {
                    // Can move up

                    queue.Enqueue(new IntTuple(tuple.x, tuple.y + 1));
                }
            }
        }
        return nArea;
    }

    struct IntTuple
    {
        public int x;
        public int y;
        public IntTuple(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

    }

    private void MapLinesToSides(bool[,] vsides, bool[,] hsides, List<Line> lines)
    {
        foreach (Line line in lines)
        {
            if ((Mathf.Abs(line.start.x) == 1.0f) &&
                (Mathf.Abs(line.start.y) == 1.0f))
            {
                // Starts in a corner - ignore it
                continue;
            }

            //JustLog("Checking side " + line.start + " to " + line.end);
            int xs = xVector3s.IndexOf(line.start.x);
            int xe = xVector3s.IndexOf(line.end.x);
            int ys = yVector3s.IndexOf(line.start.y);
            int ye = yVector3s.IndexOf(line.end.y);

            if (xs != xe)
            {
                int xl = Mathf.Min(xs, xe);
                int xr = Mathf.Max(xs, xe);

                for (int ii = xl; ii < xr; ii++)
                {

                    hsides[ii, ys - 1] = true;
                }
            }
            else if (ys != ye)
            {
                int yb = Mathf.Min(ys, ye);
                int yt = Mathf.Max(ys, ye);

                for (int ii = yb; ii < yt; ii++)
                {
                    vsides[xs - 1, ii] = true;
                }
            }
        }
    }

    private static Vector3 MaybeAddVector3(List<float> xVector3s, List<float> yVector3s, Vector3 Vector3)
    {
        if (!xVector3s.Contains(Vector3.x))
        {
            xVector3s.Add(Vector3.x);
        }

        if (!yVector3s.Contains(Vector3.y))
        {
            yVector3s.Add(Vector3.y);
        }
        return Vector3;
    }

    private static bool LineIntersectsLine(Vector3 l1p1, Vector3 l1p2, Vector3 l2p1, Vector3 l2p2)
    {
        float q = (l1p1.y - l2p1.y) * (l2p2.x - l2p1.x) - (l1p1.x - l2p1.x) * (l2p2.y - l2p1.y);
        float d = (l1p2.x - l1p1.x) * (l2p2.y - l2p1.y) - (l1p2.y - l1p1.y) * (l2p2.x - l2p1.x);

        if (d == 0)
        {
            return false;
        }

        float r = q / d;

        q = (l1p1.y - l2p1.y) * (l1p2.x - l1p1.x) - (l1p1.x - l2p1.x) * (l1p2.y - l1p1.y);
        float s = q / d;

        if (r < 0 || r > 1 || s < 0 || s > 1)
        {
            return false;
        }

        return true;
    }

    //Drawing Trail
    public List<Line> GetDrawingLinesInclLive()
    {
        List<Line> rcList = new List<Line>(drawingLines);

        if (drawingVector3s.Count > 0)
        {
            rcList.Add(new Line(drawingVector3s[drawingVector3s.Count - 1], transform.position));
        }

        return rcList;
    }
}