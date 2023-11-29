using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSO : MonoBehaviour
{
    public Manager manager; // Manager script to access boids
    public Variables variables; // Variables scriptable object for main boid simulation
    // private Particle[] batch; // Batch of particles to run simulations on, to evaluate fitness
    public Particle[] particles; // Array of particles
    private Boid[] boids; // Array of Boids. Used to run each simulation, where position is reset for each particle
    public Boid boidPrefab; // Boid prefab to instantiate Boids. I don't know how to instantiate a Boid without a prefab
    private float time; // Time variable to measure how long the algorithm takes to run
    private bool isRunning = false; // Boolean to check if the algorithm is running

    [Header("Simulation Parameters (For Fitness Evaluation)")]
    // public int concurrentSimulations; // Number of concurrent simulations (N)
    public int simulationTicks; // Number of ticks per simulation (S)
    public int numBoids; // Number of boids per simulation (B)
    private Vector3[] boidPositions; // Array of boid positions
    private Vector3[] boidDirections; // Array of boid forwards

    [Header("PSO parameters")]
    public int iterations; // Number of iterations (T)
    public int numParticles; // Population size (A)
    private float inertiaWeight; // Inertia weight (W)
    public float inertiaStart; // Initial inertia weight (W1)
    public float inertiaEnd; // Final inertia weight (W2)
    public float cognitiveWeight; // Positive constant (C1)
    public float socialWeight; // Positive constant (C2)
    public float problemSpace; // Problem space (X)
    public float bestFitness; // Stores the best fitness score (fb)
    public float convergence;
    public float[] bestPosition; // Stores the best parameter values (gb)

    [System.Serializable]
    public class Particle
    {
        public float[] position; // Current position (Xi)
        public float[] velocity; // Current velocity (Vi)
        public float[] pBestPosition; // Personal best position (Pi)
        public float pBestFitness; // Personal best fitness (Pb)
        public float simulationFitness; // Fitness during simulation. Used to divide by simulation ticks to get average fitness per simulation tick
    }

    private void Update()
    {
        //Press space to start the algorithm. This is step one.
        if (Input.GetKeyDown(KeyCode.Space) && !isRunning)
        {
            inertiaWeight = inertiaStart;
            bestFitness = float.MinValue;
            bestPosition = new float[3] {
            Random.Range(0f, problemSpace), // cohesionWeight
            Random.Range(0f, problemSpace), // alignmentWeight
            Random.Range(0f, problemSpace) // separationWeight
            };
            Debug.Log("Cohesion weight: " + bestPosition[0] + ", Alignment weight: " + bestPosition[1] + ", Separation weight: " + bestPosition[2]);
            //Multiple simulations (Batches) are possible, but it has been disabled since speed is not a priority
            // if (concurrentSimulations > numParticles)
            // {
            //     concurrentSimulations = numParticles;
            // }
            // else if (concurrentSimulations < 1)
            // {
            //     concurrentSimulations = 1;
            // }
            time = Time.realtimeSinceStartup;
            isRunning = true;
            //--------------------STEP ONE! INITIALIZE PARTICLES--------------------//
            InitializeParticles();
            InitializeBoids();
            OptimizeParameters();
        }
    }

    private void InitializeParticles()
    {
        //Particles are initialized with random positions. Positions are the values meant to be optimized (cohesion, alignment, separation)
        particles = new Particle[numParticles];
        for (int i = 0; i < numParticles; i++)
        {
            particles[i] = new Particle();
            particles[i].position = new float[3] {
                Random.Range(0f, problemSpace), // cohesionWeight
                Random.Range(0f, problemSpace), // alignmentWeight
                Random.Range(0f, problemSpace) // separationWeight
            };
            particles[i].velocity = new float[3];
            particles[i].pBestPosition = new float[3];
            particles[i].pBestFitness = float.MinValue;
        }
    }
    private void InitializeBoids()
    {
        boids = new Boid[numBoids];
        boidPositions = new Vector3[numBoids];
        boidDirections = new Vector3[numBoids];
        for (int i = 0; i < numBoids; i++)
        {
            boidPositions[i] = transform.position + Random.insideUnitSphere * 2;
            boidDirections[i] = Random.rotation * Vector3.forward;
            boids[i] = Instantiate(boidPrefab, boidPositions[i], Quaternion.LookRotation(boidDirections[i]));
            boids[i].StartBoid(variables);
        }
    }

    private void OptimizeParameters()
    {
        //--------------------STEP TWO! ITERATE X AMOUNT OF TIMES--------------------//
        //BEGINNING OF ITERATION LOOP IS FITNESS EVALUATION
        for (int i = 0; i < iterations; i++)
        {
            // for (int j = 0; j < particles.Length; j += concurrentSimulations)
            // {
            //Batches are created to run multiple simulations at the same time. Not necessary, since simulations are not run in parallel
            // int remainingParticles = Mathf.Min(concurrentSimulations, particles.Length - j);
            // batch = new Particle[remainingParticles];
            // System.Array.Copy(particles, j, batch, 0, remainingParticles);

            //CREATE BOID SIMULATION(S) TO EVALUATE FITNESS. MAIN SIM IS NOT USED, ONLY FOR VIEWING RESULTS
            //PART OF STEP TWO!
            convergence = 0;
            foreach (Particle particle in particles) //batch can be used instead of particles, if batches are chosen to be used
            {
                particle.simulationFitness = 0;
                ShortBoidSimulation(particle);
                float averageFitness = particle.simulationFitness;
                //If the fitness is better than the personal best fitness, update the personal best fitness and position
                if (averageFitness > particle.pBestFitness)
                {
                    // Debug.Log("Particle " + System.Array.IndexOf(particles, particle) + " has a new personal best fitness: " + averageFitness + ", at iteration " + i + ".");
                    particle.pBestFitness = averageFitness;
                    particle.pBestPosition = particle.position;
                }
                //If the fitness is better than the global best fitness, update the global best fitness and position
                if (averageFitness > bestFitness)
                {
                    bestFitness = averageFitness;
                    bestPosition = particle.position;
                }
                //Calculate convergence
                convergence += particle.simulationFitness / particles.Length;
                //--------------------STEP THREE! UPDATE PARTICLE VELOCITY AND POSITION--------------------//
                UpdateParticle(particle, i);
            }
            // If convergence is 2% close to bestfitness, then stop the algorithm
            if (convergence >= bestFitness * 0.98f)
            {
                Debug.Log("Stopping criteria met at iteration " + i + " with convergence " + convergence + " and best fitness " + bestFitness + ".");
                break;
            }
            // }
        }
        //--------------------STEP FOUR! WHEN STOPPING CRITERIA IS MET, FINISH--------------------//
        Finish();
    }

    private void ShortBoidSimulation(Particle particle)
    {
        //Variables are set
        float cohesionWeight = particle.position[0];
        float alignmentWeight = particle.position[1];
        float separationWeight = particle.position[2];

        //Boid simulation is run for a short amount of ticks
        for (int i = 0; i < simulationTicks; i++)
        {
            //Boid vectors are calculated
            manager.UpdateBoids(boids, true, false, cohesionWeight, alignmentWeight, separationWeight);
            //Fitness is calculated each tick
            particle.simulationFitness += EvaluateFitness() / simulationTicks;
        }
        //Reset boid positions for next particle
        for (int i = 0; i < numBoids; i++)
        {
            boids[i].position = boidPositions[i];
            boids[i].forward = boidDirections[i];
        }
    }

    private float EvaluateFitness()
    {
        float fitness = 1;
        float cost = 0;
        float cost1 = 0;
        float cost2 = 0;
        float cost3 = 0;

        Vector3 averageAlignment = Vector3.zero;
        Vector3 center = Vector3.zero;

        foreach (Boid boid in boids)
        {
            foreach (Boid otherBoid in boids)
            {
                if (boid != otherBoid)
                {
                    float distance = Vector3.Distance(boid.position, otherBoid.position);
                    if (distance < variables.cohesionRadius)
                    {
                        averageAlignment += otherBoid.forward;
                        center += otherBoid.position;
                    }
                }
            }

            averageAlignment /= boid.neighborCount;
            float divergence = Vector3.Angle(boid.forward, averageAlignment);
            cost1 += Mathf.Pow(divergence, 2);

            center /= boid.neighborCount;
            float distanceToCenter = Vector3.Distance(boid.position, center);
            float keepDistance = variables.cohesionRadius * 0.5f;

            foreach (Boid otherBoid in boids)
            {
                if (boid != otherBoid)
                {
                    float distance = Vector3.Distance(boid.position, otherBoid.position);
                    if (distance <= keepDistance)
                    {
                        cost2 += distanceToCenter * Mathf.Pow(distance - keepDistance, 2) / Mathf.Pow(keepDistance, 2);
                    }
                    else if (distance > keepDistance)
                    {
                        cost3 += distanceToCenter * Mathf.Pow(distance - keepDistance, 2) / Mathf.Pow(distanceToCenter - keepDistance, 2);
                    }
                }
            }
            cost2 /= boid.neighborCount;
            cost3 /= boid.neighborCount;
        }

        cost = (cost1 + cost2 + cost3) / boids.Length;
        fitness -= cost;
        return fitness;
    }


    private void UpdateParticle(Particle particle, int iteration)
    {
        float maxVelocity = problemSpace * 0.1f;
        float u1 = Random.Range(0f, 1.0f);
        float u2 = Random.Range(0f, 1.0f);

        if (iteration > iterations * 0.7f)
        {
            inertiaWeight = inertiaEnd;
        }

        for (int i = 0; i < 3; i++)
        {
            particle.velocity[i] = inertiaWeight * particle.velocity[i] +
             cognitiveWeight * u1 * (particle.pBestPosition[i] - particle.position[i]) +
             socialWeight * u2 * (bestPosition[i] - particle.position[i]);

            particle.position[i] += particle.velocity[i];
        }
    }

    private void UpdateParameters(float BestCohesionWeight, float BestAlignmentWeight, float BestSeparationWeight)
    {
        variables.cohesionWeight = BestCohesionWeight;
        variables.alignmentWeight = BestAlignmentWeight;
        variables.separationWeight = BestSeparationWeight;
    }

    private void Finish()
    {
        UpdateParameters(bestPosition[0], bestPosition[1], bestPosition[2]);
        foreach (Boid boid in boids)
        {
            Destroy(boid.gameObject);
        }
        Debug.Log("Best cohesion weight: " + bestPosition[0]);
        Debug.Log("Best alignment weight: " + bestPosition[1]);
        Debug.Log("Best separation weight: " + bestPosition[2]);
        foreach (Particle particle in particles)
        {
            //Print particle number of particle with best fitness
            if (particle.pBestFitness >= bestFitness)
            {
                Debug.Log("Particle " + System.Array.IndexOf(particles, particle) + " has the best fitness: " + bestFitness);
                break;
            }
        }
        Debug.Log("Algorithm took " + (Time.realtimeSinceStartup - time) + " seconds to run.");
        isRunning = false;
    }
}