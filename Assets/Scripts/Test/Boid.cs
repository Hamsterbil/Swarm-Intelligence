// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class Boid : MonoBehaviour
// {
//     //Give boids velocity for movement, less than max speed
//     //Give boids acceleration for steering
//     //Give boids radius to check for nearby boids
//     //Give boids mass to affect other boids (maybe)
//     //Give boids trail to show where they've been
//     //Make boids have average color of nearby boids (warm colors for more boids, cool colors for less boids)
//     //Draw a line from boid to boid to show connections
//     //Draw radius to show where boid is looking
//     //Perform collision detection at increasing angles from the boid's forward direction until a clear path is found

//     //Boid variables--------------------------------------------------------
//     public Vector3 maxVelocity;
//     public Vector3 velocity;
//     public float separationRadius;
//     public float cohesionRadius;

//     //public Vector3 acceleration;
//     //public float mass;

//     public Color color;
//     MeshRenderer meshRenderer;

//     public float cubeSize;
//     float teleportOffset;

//     //public float force;

//     //Forces
//     [Range(0.0F, 10.0F)]
//     public float separationForce;
//     [Range(0.0F, 10.0F)]
//     public float coherenceForce;
//     [Range(0.0F, 10.0F)]
//     public float alignmentForce;  
//     [Range(0.0F, 10.0F)]
//     public float obstacleAvoidanceForce;  

//     public int nearbyBoidsCount = 0;

//     public Vector3 separationDirection;
//     public Vector3 averagePosition;
//     public Vector3 alignmentDirection;
//     public Vector3 obstacleAvoidanceDistance;

//     //How to implement:
//     // 1. Find all boids within radius
//     // 2. Steer away from boids that are too close in separationRadius
//     // 3. Find average position of all boids in cohesionRadius
//     // 4. Find average alignment of all boids in cohesionRadius
//     // 5. Align boids to average alignment
//     // 6. Move towards average position

//     void Start()
//     {
//         velocity = Random.insideUnitSphere * maxVelocity.magnitude;

//         //Start by having black color
//         color = Color.black;
//         GetComponent<MeshRenderer>().material.color = color;
//     }

//     void Update()
//     {
//         separationForce = GameObject.Find("SeparationSlider").GetComponent<Slider>().value;
//         coherenceForce = GameObject.Find("CohesionSlider").GetComponent<Slider>().value;
//         alignmentForce = GameObject.Find("AlignmentSlider").GetComponent<Slider>().value;
//         checkSurroundingBoids();
//         velocity += (averagePosition - transform.position).normalized * coherenceForce * Time.deltaTime;            
//         velocity += separationDirection.normalized * separationForce * Time.deltaTime;
//         // checkSurroundingBoids(separationRadius);
//         // Vector3 separationVector = (separationDirection - transform.position).normalized;
//         // velocity += separationVector * separationForce * Time.deltaTime;

//         // checkSurroundingBoids(cohesionRadius);
//         // Vector3 alignmentVector = (alignmentDirection - transform.position).normalized;
//         // velocity += alignmentVector * alignmentForce * Time.deltaTime;

//         // Vector3 cohesionVector = (averagePosition - transform.position).normalized;
//         // velocity += cohesionVector * coherenceForce * Time.deltaTime;

//         transform.position += velocity * Time.deltaTime;
//         if (velocity.magnitude > maxVelocity.magnitude)
//             velocity = velocity.normalized * maxVelocity.magnitude;
//         transform.rotation = Quaternion.LookRotation((alignmentDirection - transform.position).normalized * alignmentForce * Time.deltaTime);
//         if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, obstacleAvoidanceDistance))
//     {
//         if (hit.collider.gameObject.CompareTag("Wall"))
//         {
//             // Calculate obstacle avoidance force
//             Vector3 avoidanceDirection = transform.position - hit.point;
//             avoidanceDirection.y = 0; // If you want to avoid only horizontally
//             Vector3 avoidanceForce = avoidanceDirection.normalized * obstacleAvoidanceForce;

//             // Apply the avoidance force to the velocity
//             velocity += avoidanceForce * Time.deltaTime;
//         }
//     }

//         outOfBounds();
//     }

//     void checkSurroundingBoids()
//     {
//         nearbyBoidsCount = 0;

//         averagePosition = Vector3.zero;
//         alignmentDirection = Vector3.zero;
//         separationDirection = Vector3.zero;

//         foreach (GameObject boid in GameObject.Find("BoidSpawner").GetComponent<BoidSpawner>().returnBoids()) 
//         {
//             float distance = Vector3.Distance(transform.position, boid.transform.position);
//             if (distance < separationRadius)
//             {
//                 separationDirection += transform.position - boid.transform.position;
//             } else if (distance < cohesionRadius)
//             {
//                 averagePosition += boid.transform.position;
//                 alignmentDirection += boid.GetComponent<Boid>().velocity;
//                 nearbyBoidsCount++;
//             }

//             // Move towards the average position of nearby boids
//             if (nearbyBoidsCount > 0)
//             {
//                 Vector3 cohesionVector = (averagePosition / nearbyBoidsCount - transform.position).normalized;
//                 velocity += cohesionVector * coherenceForce * Time.deltaTime;

//                 // Align with the average heading of nearby boids
//                 Vector3 alignmentVector = (alignmentDirection / nearbyBoidsCount).normalized;
//                 velocity += alignmentVector * alignmentForce * Time.deltaTime;
//             }
//         }

        
//         // foreach (GameObject boid in GameObject.Find("BoidSpawner").GetComponent<BoidSpawner>().returnBoids())
//         // {
//         //     float distance = Vector3.Distance(transform.position, boid.transform.position);
//         //     if (distance < radius)
//         //     {
//         //         averagePosition += boid.transform.position;
//         //         alignmentDirection += boid.GetComponent<Boid>().velocity;
//         //         nearbyBoidsCount++;
//         //     }
//         // }
//         // if (nearbyBoidsCount > 0)
//         // {
//         //     averagePosition += transform.position;
//         //     averagePosition /= nearbyBoidsCount;
//         //     alignmentDirection += velocity;
//         //     alignmentDirection /= nearbyBoidsCount;
//         //     separationDirection = transform.position - averagePosition;
//         // }
//         // else
//         // {
//         //     averagePosition = transform.position;
//         //     alignmentDirection = transform.forward; // Default alignment direction
//         // }
//     }

//     void OnDrawGizmos()
//     {
//         //Look direction
//         Gizmos.color = Color.white;
//         Gizmos.DrawLine(transform.position, transform.position + velocity);

//         //Average position of flockmates
//         if (averagePosition != Vector3.zero)
//         {
//             Gizmos.color = Color.yellow;
//             Gizmos.DrawLine(transform.position, averagePosition);
//         }

//         //Draw separation direction
//         if (separationDirection != Vector3.zero)
//         {
//             Gizmos.color = Color.blue;
//             Gizmos.DrawLine(transform.position, separationDirection);
//         }

//         // //Cohesion Radius
//         // Gizmos.color = Color.green;
//         // Gizmos.DrawWireSphere(transform.position, cohesionRadius);

//         // //Separation Radius
//         // Gizmos.color = Color.red;
//         // Gizmos.DrawWireSphere(transform.position, separationRadius);

//         //Nearby boids
//         foreach (GameObject boid in GameObject.Find("BoidSpawner").GetComponent<BoidSpawner>().returnBoids()) 
//         {
//             float distance = Vector3.Distance(transform.position, boid.transform.position);
//             if (distance < cohesionRadius)
//             {
//                 Gizmos.color = Color.green;
//                 Gizmos.DrawLine(transform.position, boid.transform.position);
//             }
//             if (distance < separationRadius)
//             {
//                 Gizmos.color = Color.red;
//                 Gizmos.DrawLine(transform.position, boid.transform.position);
//             }
//         }

//         //Cube
//         Gizmos.color = Color.green;
//         Gizmos.DrawWireCube(Vector3.zero, new Vector3(cubeSize, cubeSize, cubeSize));
//     }

//     void outOfBounds()
//     {
//         Vector3 newPosition = transform.position;

//         if (newPosition.x < -cubeSize / 2)
//             newPosition.x = cubeSize / 2 - teleportOffset;
//         else if (newPosition.x > cubeSize / 2)
//             newPosition.x = -cubeSize / 2 + teleportOffset;

//         if (newPosition.y < -cubeSize / 2)
//             newPosition.y = cubeSize / 2 - teleportOffset;
//         else if (newPosition.y > cubeSize / 2)
//             newPosition.y = -cubeSize / 2 + teleportOffset;

//         if (newPosition.z < -cubeSize / 2)
//             newPosition.z = cubeSize / 2 - teleportOffset;
//         else if (newPosition.z > cubeSize / 2)
//             newPosition.z = -cubeSize / 2 + teleportOffset;

//         transform.position = newPosition;
//     }
// }

// //BOIDS RULES------------------------------------------------------------

// //The main three are:
// // separation: steer to avoid crowding local flockmates.
// // alignment: steer towards the average heading of local flockmates.
// // cohesion: steer to move toward the average position of local flockmates.

// //IDEAS FOR EXTENSIONS----------------------------------------------------
// // Avoid Obstacles:
// // You can implement obstacle avoidance. Boids should steer away from obstacles such as rocks, trees, or other static objects in the environment.

// // Leader Following:
// // Introduce a leader boid that the rest of the flock follows. The other boids could adjust their behavior based on the leader's position, allowing for more dynamic group movements.

// // Velocity Matching:
// // In addition to alignment, consider introducing a rule for velocity matching. Boids adjust their velocity to match the average velocity of nearby flockmates.

// // Fear or Predators:
// // Create a rule where boids are afraid of predators. When a "predator" object is nearby, the boids should flee or change their behavior to avoid being caught.

// // Flocking Modes:
// // Implement different "modes" of behavior that boids can switch between. For example, a "calm" mode might focus on cohesion and alignment, while an "alert" mode could prioritize separation and evasion.

// // Local Attraction Points:
// // Introduce attraction points in the environment that boids are drawn to. These could represent food sources, resting areas, or other points of interest.

// // Communication and Signaling:
// // Allow boids to "communicate" with each other by emitting and responding to signals. For instance, boids could emit signals to gather or disperse, and nearby boids could adjust their behavior accordingly.

// // Dynamic Obstacles:
// // Instead of just avoiding static obstacles, simulate dynamic obstacles that can move and affect boid behavior, such as moving vehicles or changing weather conditions.

// // Tight Formations:
// // Create a rule where boids attempt to maintain a specific formation, such as a V-shape or line formation. This can mimic the behavior of birds in flight.

// // Aging and Experience:
// // Introduce an element of learning over time. Boids could adapt their behavior based on their experiences, allowing the flock to develop more sophisticated strategies over time.

// // Territory and Personal Space:
// // Assign each boid a "territory" or personal space that they defend. This can result in more complex interactions and avoid overcrowding.
