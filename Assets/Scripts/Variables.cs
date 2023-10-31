using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Variables : ScriptableObject {
    //All different variables
    public Color color;
    public float minSpeed;
    public float maxSpeed;
    public float maxSteerForce;

    public float cohesionRadius;
    public float separationRadius;
    
    public float cubeSize;

    [Range(0.0F, 10.0F)]
    public float separationWeight;
    [Range(0.0F, 10.0F)]
    public float alignmentWeight;
    [Range(0.0F, 10.0F)]
    public float cohesionWeight;

    public float collisionRadius;
    public float collisionAvoidDst;
    public float avoidCollisionWeight;
    public LayerMask obstacleMask;
}