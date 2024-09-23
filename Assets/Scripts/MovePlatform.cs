using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public bool activated = true;
    public float xDistance;
    private float xStart;
    public float yDistance;
    private float yStart;
    public float zDistance;
    private float zStart;
    public float moveTime;
    private float time;
    private bool moveForward = true;
    private Rigidbody rb;
    
    // Start is called before the first frame update

    public void buttonPress()
    {
        activated = !activated;
    }

    private float getValue(float inputTime)
    {
        float sqr = inputTime * inputTime;
        return sqr / (2.0f * (sqr - inputTime) + 1.0f);
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        xStart = transform.position.x;
        yStart = transform.position.y;
        zStart = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            if (time < moveTime && moveForward)
            {
                time += Time.deltaTime;
            }
            else if (moveForward)
            {
                moveForward = false;
            }
            if (time > 0 && !moveForward)
            {
                time -= Time.deltaTime;
            }
            else if (!moveForward)
            {
                moveForward = true;
            }
            //Debug.Log(time);

            //rb.AddForce(new Vector3(0.1f, 0, 0));
            transform.position = new Vector3(xStart + getValue(time / moveTime) * xDistance, yStart + getValue(time / moveTime) * yDistance, zStart + getValue(time / moveTime) * zDistance);
        }
    }
}
