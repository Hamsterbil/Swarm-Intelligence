using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Variables variables;
    public float cubeSize;

    public Vector3 velocity;
    public Vector3 acceleration;

    public Vector3 forward;
    public Vector3 position;

    public Vector3 cohesion;
    public Vector3 separation;
    public Vector3 alignment;

    public int neighborCount;
    public int separationCount;

    public bool isLeader;
    public bool isPredator;

    public bool drawToggle;

    public void StartBoid(Variables boidVariables)
    {
        neighborCount = 0;
        separationCount = 0;
        variables = boidVariables;
        cubeSize = variables.cubeSize / 2;
        position = transform.position;
        forward = transform.forward;
        velocity = forward * Random.Range(variables.minSpeed, variables.maxSpeed);
    }

    public void UpdateBoid()
    {
        acceleration = Vector3.zero;
        if (neighborCount > 0)
        {
            alignment /= neighborCount;
            cohesion /= neighborCount;

            acceleration += separation * variables.separationWeight;
            acceleration += alignment * variables.alignmentWeight;
            acceleration += cohesion * variables.cohesionWeight;
        }

        withinCube();
        if (imBouttaCollide())
        {
            acceleration += rayCircle() * variables.avoidCollisionWeight + acceleration * Time.deltaTime;
        }

        velocity += acceleration * Time.deltaTime;

        Vector3 dir = velocity / velocity.magnitude;
        float speed = Mathf.Clamp(velocity.magnitude, variables.minSpeed, variables.maxSpeed);
        velocity = dir * speed;

        transform.position += velocity * Time.deltaTime;
        transform.forward = velocity.normalized;
        position = transform.position;
        forward = transform.forward;

        GetComponentInChildren<MeshRenderer>().material.color = Color.Lerp(
            variables.color,
            new Color(1.0f, 0.64f, 0.0f),
            (float)neighborCount / 5
        );
        if (isLeader)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        }
    }
    

   public void withinCube()
{
    float x_offset = 0;
    float y_offset = 0;
    float z_offset = 0;

    if (transform.position.x > cubeSize)
    {
        x_offset = -2 * cubeSize;
    }
    else if (transform.position.x < -cubeSize)
    {
        x_offset = 2 * cubeSize;
    }

    if (transform.position.y > cubeSize)
    {
        y_offset = -2 * cubeSize;
    }
    else if (transform.position.y < -cubeSize)
    {
        y_offset = 2 * cubeSize;
    }

    if (transform.position.z > cubeSize)
    {
        z_offset = -2 * cubeSize;
    }
    else if (transform.position.z < -cubeSize)
    {
        z_offset = 2 * cubeSize;
    }

    transform.position += new Vector3(x_offset, y_offset, z_offset);
}


    bool imBouttaCollide()
    {
        RaycastHit hit;
        if (
            Physics.SphereCast(
                transform.position,
                variables.collisionRadius,
                transform.forward,
                out hit,
                variables.collisionAvoidDst,
                variables.obstacleMask
            )
        )
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            return true;
        }
        return false;
    }

    Vector3 rayCircle()
    {
        Vector3[] Directions = Collision.directions;

        for (int i = 0; i < Directions.Length; i++)
        {
            Vector3 dir = transform.TransformDirection(Directions[i]);
            Ray ray = new Ray(transform.position, dir);
            if (
                !Physics.SphereCast(
                    ray,
                    variables.collisionRadius,
                    variables.collisionAvoidDst,
                    variables.obstacleMask
                )
            )
            {
                return dir;
            }
        }
        return forward;
    }

    private void OnDrawGizmos()
    {
        if (drawToggle)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, variables.cohesionRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, variables.separationRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, cohesion);
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, separation);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, alignment);
            // Gizmos.color = Color.green;
            // Gizmos.DrawLine(transform.position, acceleration);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, cohesion + alignment + separation);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, forward * 2);
    }
}
