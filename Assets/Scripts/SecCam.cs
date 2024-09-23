using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using System.Drawing;
using System.Threading;
using UnityEngine.XR;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SecCam : MonoBehaviour
{
    private Rigidbody rb;
    private Light child;
    private UnityEngine.Color lightColour;
    private Vector3 lastPlayerLocation = Vector3.zero;
    private List<Vector3> rotatePoints = new List<Vector3>();
    private Collider[] colliders = new Collider[50];
    private Mesh mesh;
    private new MeshRenderer renderer;

    private bool childFound = false;
    private int count;
    private float scanInterval;
    private float scanTimer;

    //-------------------------------------------//
    public int scanFr = 30;

    public float distance = 10;
    public float angle = 30;
    public float height = 1;
    public UnityEngine.Color meshcolor = UnityEngine.Color.blue;

    public LayerMask layers;
    public LayerMask occlusionLayers;
    public UnityVector3Event OnPlayerDetected;
    public List<GameObject> objInSight = new List<GameObject>();
    public Light alarm;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        scanInterval = 1 / scanFr;
        alarm.intensity = 0;
        child = this.GetComponentInChildren<Light>();
        lightColour = child.color;
    }

    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshcolor;
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y -5, transform.position.z);
            //Gizmos.DrawMesh(mesh, newPos, transform.rotation);
        }

        Gizmos.DrawWireSphere(transform.position, distance);

        for (int i = 0; i < count; i++)
        {
            Gizmos.DrawSphere(colliders[i].transform.position, .2f);
        }

        Gizmos.color = UnityEngine.Color.blue;
        foreach (var obj in objInSight)
        {
            Gizmos.DrawSphere(obj.transform.position, 5);
        }
    }

    void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0)
        {
            scanTimer += scanInterval;
            Scan();
        }

        if (objInSight.Count > 0)
        {
            if (child == null)
            {
                child = this.GetComponentInChildren<Light>();
            }
            Debug.Log("rice cooker");
            UnityEngine.Color currentColor = child.color; // Retrieve current color
            currentColor.r = Mathf.Clamp(currentColor.r + .005f, 0, 1); 
            currentColor.g = Mathf.Clamp(currentColor.g - .005f, 0, 1);
            currentColor.b = Mathf.Clamp(currentColor.b - .005f, 0, 1);
            child.color = currentColor; // Apply the new color
            if(currentColor.r == 1)
            {
                SceneManager.LoadScene("Lose");
            }

            
        }
        else if (childFound)
        {
            
            childFound = false;
        }
        else
        {
            UnityEngine.Color currentColor = child.color; // Retrieve current color
       //     if (currentColor.r >= lightColour.r)
         //   {
                currentColor.r = Mathf.Clamp(currentColor.r - .005f, 0, 1);
/*            }
            if(currentColor.g <= lightColour.g)
            {
                currentColor.g = Mathf.Clamp(currentColor.g + .005f, 0, 1);
            }
            if(currentColor.b <= lightColour.b)
            {*/
                currentColor.b = Mathf.Clamp(currentColor.b + .005f, 0, 1);
          //  }
            
            child.color = currentColor; // Apply the new color
        }
    }
  
    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int numTriangles = 8;
        int numVertices = numTriangles * 3;

        Vector3[] verticles = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;


        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;

        int vert = 0;

        verticles[vert++] = bottomCenter;
        verticles[vert++] = bottomLeft;
        verticles[vert++] = topLeft;

        verticles[vert++] = topLeft;
        verticles[vert++] = topCenter;
        verticles[vert++] = bottomCenter;

        verticles[vert++] = bottomCenter;
        verticles[vert++] = topCenter;
        verticles[vert++] = topRight;

        verticles[vert++] = topRight;
        verticles[vert++] = bottomRight;
        verticles[vert++] = bottomCenter;

        verticles[vert++] = bottomLeft;
        verticles[vert++] = bottomRight;
        verticles[vert++] = topRight;

        verticles[vert++] = topRight;
        verticles[vert++] = topRight;
        verticles[vert++] = bottomLeft;

        verticles[vert++] = topCenter;
        verticles[vert++] = topLeft;
        verticles[vert++] = topRight;

        verticles[vert++] = bottomCenter;
        verticles[vert++] = bottomRight;
        verticles[vert++] = bottomLeft;

        for (int i = 0; i < numVertices; i++)
        {
            triangles[i] = i;
        }

        mesh.vertices = verticles;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
        scanInterval = 1 / scanFr;
    }

    private void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);

        objInSight.Clear();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].gameObject;
            if (IsInSight(obj))
            {
                objInSight.Add(obj);
            }
        }
    }

    public bool IsInSight(GameObject obj)
    {
        //Debug.Log("detected pplayer in sphere");
        Vector3 origin = transform.position;
        Vector3 destination = obj.transform.position;
        Vector3 direction = destination - origin;
        if (direction.y < -height || direction.y > height)
        {
            Debug.Log("player not in right y");
            return false;


        }
        //Debug.Log("detected pplayer in height");

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > angle)
        {
            //Debug.Log("player is outside triangle");
            return false;
        }

        origin.y += height / 2;
       // destination.y = origin.y;
        if (Physics.Linecast(origin, destination, occlusionLayers))
        {
            return false;
        }

        return true;
    }

    
}

[System.Serializable]
public class UnityVector3Event : UnityEvent<Vector3> { }
