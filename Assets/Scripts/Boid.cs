using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Variables variables;

    public Vector3 velocity;
    public Vector3 acceleration;

    public Vector3 forward;
    public Vector3 position;

    public Vector3 cohesion;
    public Vector3 separation;
    public Vector3 alignment;

    public int neighborCount;

    // public bool isLeader;
    // public bool isPredator;

    public bool drawToggle;

    public void StartBoid(Variables boidVariables)
    {
        neighborCount = 0;
        variables = boidVariables;
        position = transform.position;
        forward = transform.forward;
        velocity = forward * Random.Range(variables.minSpeed, variables.maxSpeed);
    }

    public void UpdateBoid(bool checkCollision, float cohesionWeight, float alignmentWeight, float separationWeight)
    {
        acceleration = Vector3.zero;

        if (neighborCount > 0)
        {
            alignment /= neighborCount;
            cohesion /= neighborCount;

            separation = SteerTowards(separation) * separationWeight;
            alignment = SteerTowards(alignment) * alignmentWeight;
            cohesion = SteerTowards(cohesion - position) * cohesionWeight;

            acceleration += separation;
            acceleration += alignment;
            acceleration += cohesion;
        }

        if (imBouttaCollide() && checkCollision)
        {
            acceleration += rayCircle() * variables.avoidCollisionWeight;
        }

        velocity *= 1.0f - variables.velocityDamping * Time.deltaTime;

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, variables.minSpeed, variables.maxSpeed);
        velocity = dir * speed;

        position += velocity * Time.deltaTime;
        forward = dir;

        transform.position = position;
        transform.forward = forward;

        // GetComponentInChildren<MeshRenderer>().material.color = Color.Lerp(
        //     variables.color,
        //     new Color(1.0f, 0.64f, 0.0f),
        //     (float)neighborCount / 5
        // );
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
    
    Vector3 SteerTowards (Vector3 vector) {
        return vector.normalized * variables.maxSpeed - velocity;
    }

    private void OnDrawGizmos()
    {
        if (drawToggle)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(position, variables.cohesionRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, variables.separationRadius);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(position, forward * 2);
    }
}
