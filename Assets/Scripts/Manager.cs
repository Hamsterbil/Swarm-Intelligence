using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private Boid[] boids;
    public Boid boidPrefab;
    public Variables variables;
    private float cubeSize;
    private int spawnRadius;
    public bool PSOrunning;

    void Start()
    {
        PSOrunning = false;
        spawnRadius = variables.spawnRadius;
        cubeSize = variables.cubeSize / 2;
        boids = new Boid[variables.boidCount];
        for (int i = 0; i < variables.boidCount; i++)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
            Quaternion spawnRotation = Random.rotation;

            boids[i] = SpawnBoids(spawnPosition, spawnRotation);
        }
        boids[0].drawToggle = true;
    }

    void Update()
    {
        if (!PSOrunning)
        {
            UpdateBoids(boids, false, true, variables.cohesionWeight, variables.alignmentWeight, variables.separationWeight);
        }
    }

    public Boid SpawnBoids(Vector3 position, Quaternion rotation)
    {
        Boid newBoid = Instantiate(boidPrefab, position, rotation);
        newBoid.StartBoid(variables);
        return newBoid;
    }

    public void UpdateBoids(Boid[] boids, bool PSO, bool CheckCollision, float cohesionWeight, float alignmentWeight, float separationWeight)
    {
        foreach (Boid boid in boids)
        {
            boid.neighborCount = 0;
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
                        boid.neighborCount++;
                        boid.alignment += otherBoid.velocity;
                        boid.cohesion += otherBoid.position;
                        if (distance < variables.separationRadius)
                        {
                            boid.separation += boid.position - otherBoid.position;
                        }
                    }
                }
                if (boid.drawToggle && !PSO)
                {
                    //Draw ray between boid and neighbor
                    float distance = Vector3.Distance(boid.position, otherBoid.position);
                    if (distance < variables.cohesionRadius)
                    {
                        Debug.DrawRay(boid.position, otherBoid.position - boid.position, Color.green);
                    }
                    if (distance < variables.separationRadius)
                    {
                        Debug.DrawRay(boid.position, otherBoid.position - boid.position, Color.red);
                    }
                }
            }
            if (!PSO)
            {
                withinCube(boid);
            }
            boid.UpdateBoid(CheckCollision, cohesionWeight, alignmentWeight, separationWeight);
        }
    }

    public void withinCube(Boid boid)
    {
        float x_offset = 0;
        float y_offset = 0;
        float z_offset = 0;

        if (boid.position.x > cubeSize + transform.position.x)
        {
            x_offset = -2 * cubeSize;
        }
        else if (boid.position.x < -cubeSize + transform.position.x)
        {
            x_offset = 2 * cubeSize;
        }

        if (boid.position.y > cubeSize + transform.position.y)
        {
            y_offset = -2 * cubeSize;
        }
        else if (boid.position.y < -cubeSize + transform.position.y)
        {
            y_offset = 2 * cubeSize;
        }

        if (boid.position.z > cubeSize + transform.position.z)
        {
            z_offset = -2 * cubeSize;
        }
        else if (boid.position.z < -cubeSize + transform.position.z)
        {
            z_offset = 2 * cubeSize;
        }

        boid.position += new Vector3(x_offset, y_offset, z_offset);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(variables.cubeSize, variables.cubeSize, variables.cubeSize)
        );
    }
}