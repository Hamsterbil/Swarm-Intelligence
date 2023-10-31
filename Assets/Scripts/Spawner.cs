using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Variables variables;
    public Boid prefab;
    public float radius;

    void Awake()
    {
        for (int i = 0; i < variables.boidCount; i++) {
            Boid boid = Instantiate(prefab, transform.position + Random.insideUnitSphere * radius, Random.rotation);
        }
    }
}
