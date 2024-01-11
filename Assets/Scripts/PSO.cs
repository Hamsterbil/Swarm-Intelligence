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
    private float time; // Time variable to measure how long the algorithm takes to run
    private bool isRunning; // Boolean to check if the algorithm is running

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
    private float maxVelocity; // Maximum velocity (Vmax)
    public float bestFitness; // Stores the best fitness score (fb)
    private float convergence;
    private float[] convergenceList;
    public float[] bestPositions; // Stores the best parameter values (gb)

    [System.Serializable]
    public class Particle
    {
        public List<Vector3> positions;
        public float[] position; // Current position (Xi)
        public float[] velocity; // Current velocity (Vi)
        public float[] pBestPosition; // Personal best position (Pi)
        public float pBestFitness; // Personal best fitness (Pb)
        public float simulationFitness; // Fitness during simulation. Used to divide by simulation ticks to get average fitness per simulation tick
    }

    private void Start()
    {
        isRunning = false;
    }

    private void Update()
    {
        //Press space to start the algorithm. This is step one.
        if (Input.GetKeyDown(KeyCode.Space) && !isRunning)
        {
            convergenceList = new float[iterations];
            maxVelocity = problemSpace * 0.1f;
            inertiaWeight = inertiaStart;
            bestFitness = float.MinValue;
            bestPositions = new float[3] {
            Random.Range(0f, problemSpace), // cohesionWeight
            Random.Range(0f, problemSpace), // alignmentWeight
            Random.Range(0f, problemSpace) // separationWeight
            };
            Debug.Log("Cohesion weight: " + bestPositions[0] + ", Alignment weight: " + bestPositions[1] + ", Separation weight: " + bestPositions[2]);
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

    //draw
    void OnDrawGizmos()
    {
        if (particles != null)
        {
            foreach (Particle particle in particles)
            {
                // Draw a line connecting the positions of the particle
                Gizmos.color = Color.yellow;
                for (int j = 1; j < particle.positions.Count; j++)
                {
                    Vector3 pos = new Vector3(particle.positions[j].x - 100, particle.positions[j].y + 100, particle.positions[j].z);
                    //Make last line blue
                    if (j == particle.positions.Count - 1)
                    {
                        Gizmos.color = Color.blue;
                    }
                    Gizmos.DrawLine(new Vector3(particle.positions[j - 1].x - 100, particle.positions[j - 1].y + 100, particle.positions[j - 1].z), pos);
                }

                // Draw spheres at each position

                foreach (Vector3 position in particle.positions)
                {
                    Vector3 pos = new Vector3(position.x - 100, position.y + 100, position.z);
                    Gizmos.color = Color.red;
                    //if first position, make blue
                    if (particle.positions.IndexOf(position) == 0)
                    {
                        Gizmos.color = Color.blue;
                    }
                    if (particle.positions.IndexOf(position) == particle.positions.Count - 1)
                    {
                        Gizmos.color = Color.green;
                        //Draw a green line going straight up, from the last position
                        Gizmos.DrawLine(pos, pos + Vector3.up * 10);
                    }
                    Gizmos.DrawSphere(pos, 0.5f);
                }
            }
        }
    }

    private void InitializeParticles()
    {
        particles = new Particle[numParticles];
        for (int i = 0; i < numParticles; i++)
        {
            particles[i] = new Particle();
            particles[i].position = new float[3];
            particles[i].velocity = new float[3];
            for (int j = 0; j < 3; j++)
            {
                particles[i].position[j] = Random.Range(0f, problemSpace);
                particles[i].velocity[j] = Random.Range(-maxVelocity, maxVelocity);
            }
            particles[i].pBestPosition = new float[3];
            particles[i].pBestFitness = float.MinValue;
            particles[i].positions = new List<Vector3>();
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
            boids[i] = manager.SpawnBoids(boidPositions[i], Quaternion.LookRotation(boidDirections[i]));
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
                particle.positions.Add(new Vector3(particle.position[0] * 10, particle.position[1] * 10, particle.position[2] * 10));
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
                    bestPositions = particle.position;
                }

                //Calculate convergence
                convergence += particle.simulationFitness / particles.Length;
                UpdateParticle(particle, i);
            }
            //every 5 iterations
            if (i % 5 == 0)
            {
                Debug.Log("Iteration " + i);
                float[] fitnessList = new float[particles.Length];
                foreach (Particle particle in particles)
                {
                    fitnessList[System.Array.IndexOf(particles, particle)] = particle.simulationFitness;
                }
                //return number of fitnesses between <0 and 0.1, 0.1 and 0.2, 0.2 and 0.3, etc.
                int[] fitnessDistribution = new int[10];
                foreach (float fitness in fitnessList)
                {
                    for (int j = 0; j < fitnessDistribution.Length; j++)
                    {
                        if (fitness >= j * 0.1f && fitness < (j + 1) * 0.1f)
                        {
                            if (fitness < 0)
                            {
                                fitnessDistribution[0]++;
                            }
                            fitnessDistribution[j]++;
                        }
                    }
                }
                for (int k = 0; k < fitnessDistribution.Length; k++)
                {
                    Debug.Log("Fitnesses between " + k * 0.1f + " and " + (k + 1) * 0.1f + ": " + fitnessDistribution[k]);
                }
            }

            convergenceList[i] = convergence;

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
        float cost1 = 0;
        float cost2 = 0;
        float cost3 = 0;

        Vector3 cohesion = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 separation = Vector3.zero;

        foreach (Boid boid in boids)
        {
            if (boid.neighborCount > 0)
            {
                cohesion = boid.cohesion / boid.neighborCount;
                alignment = boid.alignment / boid.neighborCount;

                float divergence = Vector3.Angle(alignment, boid.forward);
                cost1 += Mathf.Pow(divergence, 2);

                float distanceToCenter = Vector3.Distance(boid.position, cohesion);
                cost2 += distanceToCenter / Mathf.Pow(variables.cohesionRadius, 2);

                foreach (Boid otherBoid in boids)
                {
                    if (boid != otherBoid)
                    {
                        float distance = Vector3.Distance(boid.position, otherBoid.position);
                        if (distance < variables.separationRadius)
                        {
                            cost3 += Mathf.Pow(variables.separationRadius - distance, 2) / Mathf.Pow(variables.separationRadius, 2);
                        }
                    }
                }
                cost3 /= boid.neighborCount;
            }
        }

        float cost = (cost1 + cost2 + cost3) / boids.Length;
        fitness -= cost;
        return fitness;
    }


    private void UpdateParticle(Particle particle, int iteration)
    {
        inertiaWeight = Mathf.Lerp(inertiaStart, inertiaEnd, (float)iteration / iterations);

        for (int i = 0; i < 3; i++)
        {
            float u1 = Random.Range(0f, 1.0f);
            float u2 = Random.Range(0f, 1.0f);
            //v = w * v + c1 * u1 * (pbest - x) /  + c2 * u2 * (gbest - x)
            particle.velocity[i] = Mathf.Clamp(inertiaWeight * particle.velocity[i] +
             cognitiveWeight * u1 * (particle.pBestPosition[i] - particle.position[i]) +
             socialWeight * u2 * (bestPositions[i] - particle.position[i]), -maxVelocity, maxVelocity);
            particle.position[i] += particle.velocity[i];
        }
    }

    private void UpdateParameters(float bestCohesionWeight, float bestAlignmentWeight, float bestSeparationWeight)
    {
        variables.cohesionWeight = bestCohesionWeight;
        variables.alignmentWeight = bestAlignmentWeight;
        variables.separationWeight = bestSeparationWeight;
    }

    private void Finish()
    {
        UpdateParameters(bestPositions[0], bestPositions[1], bestPositions[2]);
        foreach (Boid boid in boids)
        {
            Destroy(boid.gameObject);
        }
        // for (int i = 0; i < convergenceList.Length; i++)
        // {
        //     Debug.Log(convergenceList[i]);
        // }

        Debug.Log("Best cohesion weight: " + bestPositions[0]);
        Debug.Log("Best alignment weight: " + bestPositions[1]);
        Debug.Log("Best separation weight: " + bestPositions[2]);
        foreach (Particle particle in particles)
        {
            //Print particle number of particle with best fitness
            if (particle.pBestFitness >= bestFitness)
            {
                Debug.Log("Particle " + System.Array.IndexOf(particles, particle) + " has the best fitness: " + bestFitness + ". With convergence at " + convergence + ".");
                break;
            }
        }
        Debug.Log("Algorithm took " + (Time.realtimeSinceStartup - time) + " seconds to run.");
        isRunning = false;
    }
}