using UnityEngine;

public class PlayerTrail : MonoBehaviour
{

    public PlayerController player;
    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        var lines = player.GetDrawingLinesInclLive().ToArray();
        GetComponent<MeshFilter>().mesh = DynamicLines.GetMesh(lines, 0.1f);
    }
}
