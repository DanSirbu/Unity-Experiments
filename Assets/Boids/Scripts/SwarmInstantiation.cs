using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class SwarmInstantiation : MonoBehaviour
{
    public GameObject prefab;
    public int numberOfItems = 20;
    public float createRadius = 25f;
    public float Speed = 10f;

    public float neighbourSeparation = 0.7f;

    public bool debug;

    private List<Boid> boids;


    void Start()
    {
        boids = new List<Boid>();
        for (int x = 0; x < numberOfItems; x++)
        {
            Vector3 position = UnityEngine.Random.insideUnitSphere * (UnityEngine.Random.value * createRadius);

            var obj = Instantiate(prefab, position, Quaternion.identity) as GameObject;
            obj.gameObject.transform.parent = this.gameObject.transform;
            var boid = obj.GetComponent<Boid>();
            boids.Add(boid);
            boid.Controller = this;
        }
    }

    private void Update()
    {
        boids.ForEach(changeIt);
    }

    private void changeIt(Boid boid)
    {
        boid.Speed = Speed;
        boid.neightbourRepulsionThreshold = neighbourSeparation;
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Vector3.zero, createRadius);
        }
    }
}