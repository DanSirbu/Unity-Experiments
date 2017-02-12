using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour
{
    public Vector3 dir;
    private Rigidbody rigidbody;
    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float dX = Input.GetAxis("Horizontal");
        float dY = Input.GetAxis("Vertical");

        dir = new Vector3(dX, 0, dY);
        rigidbody.velocity = dir * 5;
    }
}
