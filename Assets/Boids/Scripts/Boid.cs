using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets;
using UnityEngine;
using UnityEngine.EventSystems;

class Boid : MonoBehaviour
{
    public float Speed = 5.0f;
    private Vector3 direction = Vector3.zero;
    public float neightbourRepulsionThreshold = 10.0f;
    public float neighbourRange = 20.0f;

    private Collider[] neighbours = new Collider[0];


    public LayerMask boidLayer;
    public SwarmInstantiation Controller { get; set; }

    IEnumerator GetNeighbours()
    {
        while (true)
        {
            neighbours = Physics.OverlapSphere(transform.position, neighbourRange, boidLayer);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void Start()
    {
        StartCoroutine(GetNeighbours());
    }

    private void FixedUpdate()
    {
        CalculateDir();
        transform.position += direction * Time.fixedDeltaTime;
        //LookAt
        /*transform.localRotation = Quaternion.Lerp(transform.localRotation,
            Quaternion.FromToRotation(transform.forward, direction), 0.5f);*/
    }

    public void CalculateDir()
    {
        Vector3 rule1Effect;
        if (Input.GetMouseButton(Mouse.LEFT_MOUSE_BUTTON))
            rule1Effect = rule1() * -2;
        else
        {
            rule1Effect = rule1();
        }

        direction = rule1Effect + rule2() + rule3() + rule4();
        direction = Vector3.ClampMagnitude(direction, Speed);
    }

    //Center Of Mass
    public Vector3 rule1()
    {
        Vector3 centerOfWeight = Vector3.zero;
        foreach (var neighbour in neighbours)
        {
            if (neighbour.gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                centerOfWeight += neighbour.transform.position - transform.position;
            }
        }

        //Find Center
        centerOfWeight /= neighbours.Length;

        return centerOfWeight;
    }

    //Keep distance
    public Vector3 rule2()
    {
        Vector3 repulsionForce = Vector3.zero;

        foreach (var neighbour in neighbours)
        {
            if (neighbour.gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                if (Vector3.Distance(neighbour.transform.position, transform.position) < neightbourRepulsionThreshold)
                {
                    float force = neightbourRepulsionThreshold /
                                  Vector3.Distance(neighbour.transform.position, transform.position);
                    //- distance
                    repulsionForce -= (neighbour.transform.position - transform.position) * force;
                }
            }
        }

        return repulsionForce;
    }

    //Match neighbour speeds
    public Vector3 rule3()
    {
        Vector3 averageVelocity = Vector3.zero;
        foreach (var neighbour in neighbours)
        {
            if (neighbour.gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                averageVelocity += neighbour.GetComponent<Boid>().GetVelocity();
            }
        }

        averageVelocity /= neighbours.Length;

        return averageVelocity - GetVelocity() / 8;
    }

    public Vector3 rule4()
    {
        var v3 = Input.mousePosition;
        v3.z = 10.0f;
        v3 = Camera.main.ScreenToWorldPoint(v3);
        return v3 - transform.position;
    }
    private Vector3 GetVelocity()
    {
        return direction;
    }

    private void OnDrawGizmos()
    {
        if (Controller.debug)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, neightbourRepulsionThreshold);

            GizmoExtensions.DrawArrow(transform.position, rule1(), Color.red);
            GizmoExtensions.DrawArrow(transform.position, rule2(), Color.green);
            GizmoExtensions.DrawArrow(transform.position, rule3(), Color.yellow);
        }
    }
}