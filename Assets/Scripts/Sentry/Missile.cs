using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        //move relative to up orientation
        transform.position += transform.up * speed * Time.deltaTime;
        transform.RotateAround(transform.position, transform.up, 120f * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        //
    }
}
