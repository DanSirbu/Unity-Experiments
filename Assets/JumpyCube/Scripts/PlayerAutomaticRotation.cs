using UnityEngine;
using System.Collections;

public class PlayerAutomaticRotation : MonoBehaviour
{
    private Rigidbody rigidbody;
    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.angularVelocity = new Vector3(5, 0, -5);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
