// using System.Collections.Generic;
// using UnityEngine;

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

//     //Boid variables
//     public Vector3 velocity;
//     public Vector3 maxVelocity = new Vector3(10, 10, 10);
//     //public Vector3 acceleration;
//     public float radius;
//     public float mass;
//     public TrailRenderer trail;
//     public Color color;
//     MeshRenderer meshRenderer;
//     public float cubeSize; // Size of the cube
//     public float teleportOffset = 1.0f;
//     public float force;

//     public float separationForce;
//     public float cohesionForce;

//     new Vector3 separationDirection;
//     new Vector3 cohesionDirection;

//     Collider[] colliders;
//     public List<Boid> nearbyBoids = new List<Boid>();
//     BoidSpawner boidSpawner;
//     List<Boid> allBoids = new List<Boid>();

//     void Start()
//     {
//         //Start by having random velocity
//         velocity = new Vector3(5, 5, 5);
//         //Start by having random color
//         color = Random.ColorHSV();
//         GetComponent<MeshRenderer>().material.color = color;
//         boidSpawner = FindObjectOfType<BoidSpawner>();
//         foreach (GameObject boidObject in boidSpawner.returnBoids())
//         {
//             Boid boidComponent = boidObject.GetComponent<Boid>();
//             if (boidComponent != null)
//             {
//                 allBoids.Add(boidComponent);
//             }
//         }
//     }

//     void Update()
//     {
//         transform.LookAt(transform.position + velocity);

//         // separateBoids();
//         // allignBoids();
//         // followBoids();
        
//         transform.position += velocity * Time.deltaTime;

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

//         Vector3 separationDirection = Vector3.zero;
//     }

//     void separateBoids()
//     {
//         separationDirection = Vector3.zero;
//         foreach (Boid otherBoid in allBoids)
//         {
//             if (otherBoid != this)
//             {
//                 float distance = Vector3.Distance(transform.position, otherBoid.transform.position);
//                 if (distance < radius)
//                 {
//                     Vector3 awayFromNeighbor = transform.position - otherBoid.transform.position;
//                     separationDirection += awayFromNeighbor.normalized / distance;
//                     nearbyBoids.Add(otherBoid);
//                 }
//             }
//         }

//         if (nearbyBoids.Count > 0)
//         {
//             separationDirection /= nearbyBoids.Count;
//             separationDirection.Normalize(); // Normalize the accumulated direction
//             Vector3 separationForce = separationDirection * force;
//             GetComponent<Rigidbody>().AddForce(separationForce);
//         }
//         nearbyBoids.Clear();
//     }

//     void allignBoids()
//     {
//         foreach (Boid otherBoid in allBoids)
//         {
//             if (otherBoid != this)
//             {
//                 float distance = Vector3.Distance(transform.position, otherBoid.transform.position);
//                 if (distance < radius)
//                 {
//                     nearbyBoids.Add(otherBoid);
//                 }
//             }
//         }
//         if (nearbyBoids.Count > 0)
//         {
//             //allign with nearby boids

//         }
//         nearbyBoids.Clear();
//     }

//     void followBoids()
//     {
//         Vector3 cohesionDirection = Vector3.zero;
//         int neighborCount = 0;

//         foreach (Boid otherBoid in allBoids)
//         {
//             if (otherBoid != this)
//             {
//                 float distance = Vector3.Distance(transform.position, otherBoid.transform.position);
//                 if (distance < radius)
//                 {
//                     cohesionDirection += otherBoid.transform.position; // Accumulate positions
//                     neighborCount++;
//                 }
//             }
//         }

//         if (neighborCount > 0)
//         {
//             cohesionDirection /= neighborCount; // Calculate average position
//             Vector3 cohesionForce = (cohesionDirection - transform.position).normalized * force; // Calculate force toward average position
//             GetComponent<Rigidbody>().AddForce(cohesionForce);
//         }
//     }


//     void CalculateNearbyBoids()
//     {
//         colliders = Physics.OverlapSphere(transform.position, radius);
//         foreach (Boid otherBoid in FindObjectsOfType<Boid>())
//         {
//             if (otherBoid != this)
//             {
//                 float distance = Vector3.Distance(transform.position, otherBoid.transform.position);
//                 if (distance <= radius)
//                 {
//                     if (!nearbyBoids.Contains(otherBoid))
//                     {
//                         nearbyBoids.Add(otherBoid);
//                     }
//                 }
//             }
//         }
//         Vector3 sumPositions = Vector3.zero;
//         Vector3 sumLookDirections = Vector3.zero;

//         foreach (Boid boid in nearbyBoids)
//         {
//             sumPositions += boid.transform.position;
//             sumLookDirections += boid.velocity.normalized;
//         }
//     }



//     //draw look direction
//     void OnDrawGizmos()
//     {
//         Gizmos.color = Color.white;
//         Gizmos.DrawLine(transform.position, transform.position + velocity);

//         Gizmos.color = Color.yellow;
//         Gizmos.DrawLine(transform.position, cohesionDirection);

//         Gizmos.color = Color.red;
//         Gizmos.DrawWireSphere(transform.position, radius);


//         foreach (Boid boid in nearbyBoids)
//         {
//             Gizmos.color = Color.blue;
//             Gizmos.DrawLine(transform.position, boid.transform.position);
//         }

//         //draw cube
//         Gizmos.color = Color.green;
//         Gizmos.DrawWireCube(Vector3.zero, new Vector3(cubeSize, cubeSize, cubeSize));
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