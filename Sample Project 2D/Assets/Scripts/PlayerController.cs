using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float Speed;
    float horizontal;
    float vertical;
    Rigidbody playerRgbd;
    string key;

    // Start is called before the first frame update
    void Start()
    {
        playerRgbd = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        //rigidbody.velocity = new Vector3(horizontal * Speed, vertical * Speed, 0);

        
    }
}
