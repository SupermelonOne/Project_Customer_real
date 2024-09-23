using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class fly : MonoBehaviour
{
    Rigidbody rb;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        /*rb.velocity = transform.forward * speed;*/
        rb.AddForce(transform.forward * speed);

    }
}
