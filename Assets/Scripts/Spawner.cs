using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Boid prefab;
    public float radius;
    public int amountOfBoids;

    void Awake()
    {
        for (int i = 0; i < amountOfBoids; i++) {
            Boid boid = Instantiate(prefab, transform.position + Random.insideUnitSphere * radius, Random.rotation);
        }
    }
}
