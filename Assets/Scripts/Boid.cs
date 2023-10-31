using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Variables variables;
    public float cubeSize;

    public Vector3 velocity;

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
        if (neighborCount > 0)
        {
            alignment /= neighborCount;
            cohesion /= neighborCount;
        }
        velocity += separation * variables.separationWeight;
        velocity += (alignment - velocity) * variables.alignmentWeight;
        velocity += (cohesion - velocity) * variables.cohesionWeight;

        withinCube();
        if (imBouttaCollide())
        {
            velocity += rayCircle() * variables.avoidCollisionWeight + velocity * Time.deltaTime;
        }

        velocity += velocity * Time.deltaTime;

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
        int x_offset = 0;
        int y_offset = 0;
        int z_offset = 0;
        if (
            transform.position.x > cubeSize
            || transform.position.x < -cubeSize
            || transform.position.y > cubeSize
            || transform.position.y < -cubeSize
            || transform.position.z > cubeSize
            || transform.position.z < -cubeSize
        )
        {
            if (transform.position.x > cubeSize)
            {
                x_offset = 1;
            }
            else if (transform.position.x > -cubeSize)
            {
                x_offset = -1;
            }
            if (transform.position.y > cubeSize)
            {
                y_offset = -1;
            }
            else if (transform.position.y > -cubeSize)
            {
                y_offset = 1;
            }
            if (transform.position.z > cubeSize)
            {
                z_offset = 1;
            }
            else if (transform.position.z > -cubeSize)
            {
                z_offset = -1;
            }
            transform.position = new Vector3(
                -transform.position.x + x_offset,
                -transform.position.y + y_offset,
                -transform.position.z + z_offset
            );
        }
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
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, forward * 2);
    }
}
