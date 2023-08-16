using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 velocity;
    public float maxVelocity;

    // Start is called before the first frame update
    void Start() {
        velocity = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
    }

    // Update is called once per frame
    void Update()
    {
        if(velocity.magnitude > maxVelocity) {
            velocity = velocity.normalized * maxVelocity;
        }

        this.transform.position += velocity * Time.deltaTime;
        this.transform.rotation = Quaternion.LookRotation(velocity);

        if (transform.position.y > 50 || transform.position.y < 0 || transform.position.x > 25 || transform.position.x < -25 || transform.position.z > 25 || transform.position.z < -25)
        {
            transform.position = new Vector3(transform.position.x * -1, transform.position.y * -1, transform.position.z * -1);  
        }
    }
}
