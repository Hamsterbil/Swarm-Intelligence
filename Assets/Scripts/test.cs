// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PSO : MonoBehaviour
// {
//     public Boid[] boids;
//     public Variables variables;

//     // PSO parameters
//     public int numParticles = 30;
//     public float maxVelocity = 1.0f;
//     public float inertiaWeight = 0.5f;
//     public float cognitiveWeight = 1.0f;
//     public float socialWeight = 1.0f;

//     // Fitness-related constants
//     public float separationThreshold = 2.0f;
//     public float cohesionThreshold = 5.0f;

//     void Start()
//     {
//         boids = FindObjectsOfType<Boid>();
//         foreach (Boid boid in boids)
//         {
//             boid.StartBoid(variables);
//         }

//         // Optimize the parameters using PSO
//         OptimizeParameters();
//     }

//     void OptimizeParameters()
//     {
//         float[] bestPosition = new float[3];  // Stores the best parameter values.
//         float bestFitness = float.MinValue;    // Stores the best fitness score.

//         // Initialize PSO particles
//         Particle[] particles = new Particle[numParticles];
//         for (int i = 0; i < numParticles; i++)
//         {
//             particles[i] = new Particle();
//             particles[i].position = new float[3] {
//             Random.Range(0f, 10f), // cohesionWeight
//             Random.Range(0f, 10f), // alignmentWeight
//             Random.Range(0f, 10f) // separationWeight
//         };
//             particles[i].velocity = new float[3];
//             particles[i].pBestPosition = particles[i].position;
//         }

//         int maxIterations = 100;
//         int iteration = 0;

//         while (iteration < maxIterations)
//         {
//             foreach (Particle particle in particles)
//             {
//                 // Evaluate fitness using your boid simulation and the parameters
//                 float fitness = EvaluateFitness(particle.position[0], particle.position[1], particle.position[2]);

//                 // Update personal best (pBest) if fitness improves
//                 if (fitness > EvaluateFitness(particle.pBestPosition[0], particle.pBestPosition[1], particle.pBestPosition[2]))
//                 {
//                     particle.pBestPosition = particle.position;
//                 }

//                 // Update global best (gBest) if fitness improves
//                 if (fitness > bestFitness)
//                 {
//                     bestPosition = particle.position;
//                     bestFitness = fitness;
//                 }
//             }

//             // Update particle velocities and positions based on PSO equations
//             foreach (Particle particle in particles)
//             {
//                 UpdateParticleVelocity(particle, bestPosition);
//                 UpdateParticlePosition(particle);
//             }

//             // Update weights based on fitness performance
//             UpdateWeightsBasedOnFitness(bestPosition);

//             iteration++;
//         }

//         // Apply the best parameters to your Unity boid simulation
//         ApplyParametersToBoidSimulation(bestPosition[0], bestPosition[1], bestPosition[2]);
//     }

//     void UpdateWeightsBasedOnFitness(float[] bestPosition)
//     {
//         // Update weights based on fitness performance
//         variables.cohesionWeight = Mathf.Clamp(bestPosition[0], 0f, 10f);  // Adjust the range as needed
//         variables.alignmentWeight = Mathf.Clamp(bestPosition[1], 0f, 10f);
//         variables.separationWeight = Mathf.Clamp(bestPosition[2], 0f, 10f);

//         // Apply weights to boid simulation
//         ApplyParametersToBoidSimulation(variables.cohesionWeight, variables.alignmentWeight, variables.separationWeight);
//     }


//     // Other methods remain unchanged


//     void UpdateParticleVelocity(Particle particle, float[] gBestPosition)
//     {
//         // Update particle velocities based on PSO equations
//         for (int i = 0; i < 3; i++)
//         {
//             float r1 = Random.value;
//             float r2 = Random.value;
//             particle.velocity[i] = Mathf.Clamp(
//                 inertiaWeight * particle.velocity[i] +
//                 cognitiveWeight * r1 * (particle.pBestPosition[i] - particle.position[i]) +
//                 socialWeight * r2 * (gBestPosition[i] - particle.position[i]),
//                 -maxVelocity, maxVelocity
//             );
//         }
//     }

//     void UpdateParticlePosition(Particle particle)
//     {
//         // Update particle positions based on the calculated velocities
//         for (int i = 0; i < 3; i++)
//         {
//             particle.position[i] += particle.velocity[i];
//         }
//     }

//     void ApplyParametersToBoidSimulation(float cohesionWeight, float alignmentWeight, float separationWeight)
//     {
//         foreach (Boid boid in boids)
//         {
//             boid.variables.cohesionWeight = cohesionWeight;
//             boid.variables.alignmentWeight = alignmentWeight;
//             boid.variables.separationWeight = separationWeight;
//         }
//     }

//     float EvaluateFitness(float cohesionWeight, float alignmentWeight, float separationWeight)
//     {
//         // Implement your evaluation logic here, using the provided fitness functions
//         float separationFitness = SeparationFitness(boids, separationWeight);
//         float alignmentFitness = AlignmentFitness(boids, alignmentWeight);
//         float cohesionFitness = CohesionFitness(boids, cohesionWeight);

//         // Combine fitness components based on your optimization goals
//         // Multiply each fitness component by its weight
//         return separationWeight * separationFitness + alignmentWeight * alignmentFitness + cohesionWeight * cohesionFitness;
//     }


//     float SeparationFitness(Boid[] boids, float separationWeight)
//     {
//         float totalSeparation = 0.0f;

//         foreach (Boid boid in boids)
//         {
//             foreach (Boid otherBoid in boids)
//             {
//                 if (boid != otherBoid)
//                 {
//                     float distance = Vector3.Distance(boid.position, otherBoid.position);

//                     // Penalize if too close
//                     if (distance < separationThreshold)
//                     {
//                         totalSeparation += 1.0f / (distance + 1.0f) * separationWeight; // Adjust as needed
//                     }
//                 }
//             }
//         }

//         return totalSeparation / boids.Length;
//     }

//     float AlignmentFitness(Boid[] boids, float alignmentWeight)
//     {
//         Vector3 averageVelocity = Vector3.zero;

//         foreach (Boid boid in boids)
//         {
//             averageVelocity += boid.velocity;
//         }

//         averageVelocity /= boids.Length;

//         float totalAlignment = 0.0f;

//         foreach (Boid boid in boids)
//         {
//             totalAlignment += Vector3.Dot(boid.velocity.normalized, averageVelocity.normalized) * alignmentWeight;
//         }

//         return totalAlignment / boids.Length;
//     }

//     float CohesionFitness(Boid[] boids, float cohesionWeight)
//     {
//         float totalCohesion = 0.0f;

//         foreach (Boid boid in boids)
//         {
//             foreach (Boid otherBoid in boids)
//             {
//                 if (boid != otherBoid)
//                 {
//                     float distance = Vector3.Distance(boid.position, otherBoid.position);

//                     // Reward for staying close
//                     if (distance < cohesionThreshold)
//                     {
//                         totalCohesion += 1.0f / (distance + 1.0f) * cohesionWeight; // Adjust as needed
//                     }
//                 }
//             }
//         }

//         return totalCohesion / boids.Length;
//     }

//     public class Particle
//     {
//         public float[] position;        // Particle position [cohesionWeight, alignmentWeight, separationWeight]
//         public float[] velocity;        // Particle velocity
//         public float[] pBestPosition;   // Personal best position
//     }
// }
