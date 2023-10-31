using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidVariables : ScriptableObject {
    //All different variables
    public float minSpeed;
    public float maxSpeed;
    public float maxSteerForce;

    public float cohesionRadius;
    public float separationRadius;
    
    public float cubeSize;

    [Range(0.0F, 10.0F)]
    public float separationForce;
    [Range(0.0F, 10.0F)]
    public float alignmentForce;
    [Range(0.0F, 10.0F)]
    public float cohesionForce;

    public float collisionRadius;
    public float collisionAvoidDst;
    public float avoidCollisionForce;
    public LayerMask obstacleMask;
}