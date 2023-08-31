// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class BoidSpawner : MonoBehaviour
// {
//     public GameObject prefab;
//     public float radius;
//     public int number;
//     public List<GameObject> boids = new List<GameObject>();
    
//     // Start is called before the first frame update
//     void Start()
//     {
//         for(int i = 0; i < number; ++i) {
//             GameObject boid = Instantiate(prefab, transform.position + Random.insideUnitSphere * radius, Random.rotation);
//             boids.Add(boid);
//         }
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }

//     public List<GameObject> returnBoids()
//     {
//         return boids;
//     }
// }
