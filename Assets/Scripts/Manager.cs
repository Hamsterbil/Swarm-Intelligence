using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public Boid[] boids;
    public Variables variables;

    void Start()
    {
        boids = FindObjectsOfType<Boid>();
        foreach (Boid boid in boids)
        {
            boid.StartBoid(variables);
        }
    }

    void Update()
    {
        foreach (Boid boid in boids)
        {            
            boid.neighborCount = 0;
            boid.velocity = Vector3.zero;
            boid.separation = Vector3.zero;
            boid.alignment = Vector3.zero;
            boid.cohesion = Vector3.zero;
            foreach (Boid otherBoid in boids)
            {
                if (boid != otherBoid)
                {
                    float distance = Vector3.Distance(boid.position, otherBoid.position);
                    if (distance < variables.cohesionRadius)
                    {
                        boid.alignment += otherBoid.forward;
                        boid.cohesion += otherBoid.position;
                        boid.neighborCount++;
                        if (distance < variables.separationRadius)
                        {
                            boid.separation += boid.transform.position - otherBoid.transform.position;
                        }
                    }
                }
            }
            boid.UpdateBoid();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(
            Vector3.zero,
            new Vector3(variables.cubeSize, variables.cubeSize, variables.cubeSize)
        );
    }
}