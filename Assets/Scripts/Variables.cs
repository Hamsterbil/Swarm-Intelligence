using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Variables : ScriptableObject {

    [Header("Spawner and playfield")]
    public float cubeSize;
    public int boidCount;

    [Header("Boid variables")]
    public Color color;
    public int leaderCount;
    public int predatorCount;

    public float minSpeed;
    public float maxSpeed;

    public float cohesionRadius;
    public float separationRadius;

    [Header("Boid weights / factors")]
    [Range(0.0F, 1F)]
    public float separationWeight;
    [Range(0.0F, 10F)]
    public float alignmentWeight;
    [Range(0.0F, 10F)]
    public float cohesionWeight;


    [Header("Collision detection")]
    public float collisionRadius;
    public float collisionAvoidDst;
    public float avoidCollisionWeight;
    public LayerMask obstacleMask;
}