using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    // const int threadGroupSize = 1024;
    public Boid[] boids;
    public Variables variables;
    // public ComputeShader compute;
    public float cubeSize;

    void Start()
    {
        cubeSize = variables.cubeSize / 2;
        boids = FindObjectsOfType<Boid>();
        foreach (Boid boid in boids)
        {
            boid.StartBoid(variables);
        }
        boids[0].drawToggle = true;
    }

    void Update()
    {
        UpdateBoids(boids, false, true, variables.cohesionWeight, variables.alignmentWeight, variables.separationWeight);

        // if (boids != null)
        // {
        //     int numBoids = boids.Length;
        //     var boidData = new BoidData[numBoids];

        //     for (int i = 0; i < boids.Length; i++)
        //     {
        //         boidData[i].position = boids[i].position;
        //         boidData[i].forward = boids[i].forward;
        //     }

        //     var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
        //     boidBuffer.SetData(boidData);

        //     compute.SetBuffer(0, "boids", boidBuffer);
        //     compute.SetInt("numBoids", boids.Length);
        //     compute.SetFloat("viewRadius", variables.cohesionRadius);
        //     compute.SetFloat("avoidRadius", variables.separationRadius);

        //     int threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize);
        //     compute.Dispatch(0, threadGroups, 1, 1);

        //     boidBuffer.GetData(boidData);

        //     for (int i = 0; i < boids.Length; i++)
        //     {
        //         boids[i].alignment = boidData[i].alignment;
        //         boids[i].cohesion = boidData[i].cohesion;
        //         boids[i].separation = boidData[i].separation;
        //         boids[i].neighborCount = boidData[i].neighborCount;
        //         withinCube(boids[i]);
        //         boids[i].UpdateBoid(true, variables.cohesionWeight, variables.alignmentWeight, variables.separationWeight);
        //     }

        //     boidBuffer.Release();
        // }
    }
    // public struct BoidData
    // {
    //     public Vector3 position;
    //     public Vector3 forward;

    //     public Vector3 alignment;
    //     public Vector3 cohesion;
    //     public Vector3 separation;
    //     public int neighborCount;

    //     public static int Size
    //     {
    //         get
    //         {
    //             return sizeof(float) * 3 * 5 + sizeof(int);
    //         }
    //     }
    // }

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
                        boid.alignment += otherBoid.forward;
                        boid.cohesion += otherBoid.position;
                        boid.neighborCount++;
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

    void withinCube(Boid boid)
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