using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class elevate : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = -1;
    Rigidbody rb;
    private bool moving = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moving)
        {
            rb.velocity = new Vector3(0, speed, 0);
            transform.position += rb.velocity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        speed *= -1;
    }

    public void MovePlatform()
    {
        moving = true;
    }
}
