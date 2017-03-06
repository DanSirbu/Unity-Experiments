using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Player : MonoBehaviour
{
    public float upwardForce = 10f;
    public Vector3 restingPosition;

    private bool _isVisible;
    private bool _isHit;

    private Rigidbody rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        _isHit = false;
        _isVisible = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        _isVisible = Mathf.Abs(transform.position.y) < 5.5f;
    }

    void Jump()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(new Vector3(0, upwardForce, 0));
    }

    public bool isVisible()
    {
        return _isVisible;
    }

    /**
    ** Enables gravity
    **/

    public void Enable()
    {
        rigidbody.useGravity = true;
    }

    /**
            ** Resets the position to zero and disables gravity
            **/

    public void Reset()
    {
        rigidbody.transform.position = restingPosition;
        rigidbody.useGravity = false;
        rigidbody.velocity = Vector3.zero;
        _isHit = false;
        _isVisible = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tag.FINISH))
        {
            _isHit = true;
        }
    }
    public bool isHit()
    {
        return _isHit;
    }
}