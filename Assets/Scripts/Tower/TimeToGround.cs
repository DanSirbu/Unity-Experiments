using System;
using UnityEngine;
using System.Collections;

public class TimeToGround : MonoBehaviour
{
    private float startTime;
    public bool done = false;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < 0.30f && !done)
        {
            print("Time: " + (Time.fixedTime - startTime).ToString("0.##") + " (s)");
            done = true;
        }

    }

    void OnValidate()
    {
        startTime = Time.fixedTime;
    }
}
