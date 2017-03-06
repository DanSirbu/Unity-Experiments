using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{
    public float velocity;
    public float verticalPosition;
    public float openingSize;

    private GameObject topWall;
    private GameObject bottomWall;
    // Use this for initialization
    void Awake()
    {
        topWall = gameObject.transform.GetChild(0).gameObject;
        bottomWall = gameObject.transform.GetChild(1).gameObject;
    }

    public void Enable()
    {
        float secondPosition = 0 - (10 - verticalPosition) - openingSize;

        topWall.transform.position = new Vector3(transform.position.x, verticalPosition, transform.position.y);
        bottomWall.transform.position = new Vector3(transform.position.x, secondPosition, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * velocity * Time.deltaTime;
    }
}
