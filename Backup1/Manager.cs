using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public Boid[] boids;
    public BoidVariables variables;
    public ComputeShader compute;

    // Start is called before the first frame update
    void Start()
    {
        boids = FindObjectsOfType<Boid>();
        foreach (Boid boid in boids)
        {
            boid.Assemble(variables);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (boids != null) {

            int numBoids = boids.Length;
            var boidData = new BoidData[numBoids];

            for (int i = 0; i < boids.Length; i++) {
                boidData[i].position = boids[i].position;
                boidData[i].forward = boids[i].forward;
            }

            var boidBuffer = new ComputeBuffer (numBoids, BoidData.Size);
            boidBuffer.SetData (boidData);

            compute.SetBuffer (0, "boids", boidBuffer);
            compute.SetInt ("numBoids", boids.Length);
            compute.SetFloat ("viewRadius", variables.cohesionRadius);
            compute.SetFloat ("avoidRadius", variables.separationRadius);

            int threadGroups = Mathf.CeilToInt (numBoids / (float) 1024);
            compute.Dispatch (0, threadGroups, 1, 1);

            boidBuffer.GetData (boidData);

            for (int i = 0; i < boids.Length; i++) {
                boids[i].averageAlignment = boidData[i].flockAlignment;
                boids[i].averageCohesion = boidData[i].flockCohesion;
                boids[i].averageAvoidance = boidData[i].flockAvoidance;
                boids[i].numNeighbors = boidData[i].numNeighbors;

                boids[i].UpdateBoid();
            }

            boidBuffer.Release ();
        }
    }

    public struct BoidData
    {
        public Vector3 position;
        public Vector3 forward;

        public Vector3 flockAlignment;
        public Vector3 flockAvoidance;
        public Vector3 flockCohesion;
        public int numNeighbors;

        public static int Size {
            get {
                return sizeof (float) * 3 * 5 + sizeof (int);
            }
        }
    }
}
