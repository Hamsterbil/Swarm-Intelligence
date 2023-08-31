using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    public Boid prefab;
    public float radius;
    public int number;
    public Color color;
    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < number; i++)
        {
            Boid boid = Instantiate(prefab, transform.position + Random.insideUnitSphere * radius, Random.rotation);
        }
    }
}
