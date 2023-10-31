using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    BoidVariables variables;

    public Vector3 velocity;
    public Vector3 position;
    public Vector3 forward;

    public Vector3 acceleration;
    public Vector3 averageAlignment;
    public Vector3 averageAvoidance;
    public Vector3 averageCohesion;

    public int numNeighbors;

    public void Assemble(BoidVariables variables)
    {
        this.variables = variables;
        position = transform.position;
        forward = transform.forward;
        velocity = forward * ((variables.minSpeed + variables.maxSpeed) / 2);
    }
    
    // Update is called once per frame from boid manager
    public void UpdateBoid()
    {
        acceleration = Vector3.zero;
        if (numNeighbors > 0)
        {
            averageCohesion /= numNeighbors;
            averageAlignment /= numNeighbors;
            

        //Separation
        acceleration += navigate(averageAvoidance) * variables.separationForce;
        //Alignment
        acceleration += navigate(averageAlignment) * variables.alignmentForce;
        //Cohesion
        acceleration += navigate(averageCohesion) * variables.cohesionForce;
        }

        if (imBouttaCollide()) {
            Vector3 avoidForce = navigate(rayCircle()) * variables.avoidCollisionForce;
            acceleration += avoidForce;
        }

        if (!withinCube()) {
            Vector3 avoidForce = navigate(-transform.position) * variables.avoidCollisionForce;
            acceleration += avoidForce;
        }

        velocity += acceleration * Time.deltaTime;
        Vector3 dir = velocity / velocity.magnitude;
        float speed = Mathf.Clamp(velocity.magnitude, variables.minSpeed, variables.maxSpeed);
        velocity = dir * speed;

        transform.position += velocity * Time.deltaTime;
        transform.forward = dir;
        position = transform.position;
        forward = dir;

        //Color boids based on flockmates
        GetComponentInChildren<MeshRenderer>().material.color = Color.Lerp(Color.black, new Color(1.0f, 0.64f, 0.0f), (float)numNeighbors / 5);        
    }

    bool imBouttaCollide() {
        RaycastHit hit;
        if (Physics.SphereCast (transform.position, variables.collisionRadius, transform.forward, out hit, variables.collisionAvoidDst, variables.obstacleMask)) {
            GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            return true;
        }
        return false;
    }

    bool withinCube() {
        if (transform.position.x + 1 > (variables.cubeSize / 2) || transform.position.x + 1 < -(variables.cubeSize / 2)) {
            return false;
        } else if (transform.position.y + 1 > (variables.cubeSize / 2) || transform.position.y + 1 < -(variables.cubeSize / 2)) {
            return false;
        } else if (transform.position.z + 1 > (variables.cubeSize / 2) || transform.position.z + 1 < -(variables.cubeSize / 2)) {
            return false;
        }
        return true;
    }

    Vector3 rayCircle() {
        Vector3[] Directions = CollisionDetection.directions;

        for (int i = 0; i < Directions.Length; i++) {
            Vector3 dir = transform.TransformDirection (Directions[i]);
            Ray ray = new Ray (transform.position, dir);
            if (!Physics.SphereCast (ray, variables.collisionRadius, variables.collisionAvoidDst, variables.obstacleMask)) {
                return dir;
            }
        }
        return forward;
    }

    Vector3 navigate(Vector3 direction)
    {
        Vector3 steer = direction.normalized * variables.maxSpeed - velocity;
        return Vector3.ClampMagnitude(steer, variables.maxSteerForce);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(variables.cubeSize, variables.cubeSize, variables.cubeSize));

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, variables.cohesionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, variables.separationRadius);
    }    
}
