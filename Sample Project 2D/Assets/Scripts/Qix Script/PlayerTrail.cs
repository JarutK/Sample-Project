using UnityEngine;

public class PlayerTrail : MonoBehaviour
{

    public PlayerMovement player;
    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        var lines = player.GetDrawingLinesInclLive().ToArray();
        GetComponent<MeshFilter>().mesh = DynamicLines.GetMesh(lines, 0.01f);
    }
}
