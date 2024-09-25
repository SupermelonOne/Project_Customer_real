using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject child;

    private Mesh mesh;
    private new MeshRenderer renderer;

    private Vector3 lastPlayerLocation   = Vector3.zero;
    private Vector3 closestPoint         = Vector3.zero;

    private float totalRotation = 0f;
    private float scanInterval;
    private float scanTimer;

    private bool lookingLeft = false;
    private bool lookingRight = false;
    private bool checkingArea = false;
    private bool PrimeCheckPoints = true;
    private bool checkable;

    private int count;
    private int placesNormal;

    private Collider[] colliders = new Collider[50];
    private List<Vector3> rotatePoints = new List<Vector3>();
    private List<Vector3> lastPath = new List<Vector3>();
    private List<Vector3> checkNodesList = new List<Vector3>();
    private List<GameObject> objInSight = new List<GameObject>();
    private float random;


    //-------------------------------------------//
    
    public float speed;
    public float spinSpeed;
    public float distance = 10;
    public float angle = 30;
    public float height = 1;

    public int scanFr = 30;
    public int rotatepoints = 4;
    public int checkNodes = 2;

    [Header("rotatePoints")]
    public Vector3 rotatePoint1;
    public Vector3 rotatePoint2;
    public Vector3 rotatePoint3;
    public Vector3 rotatePoint4;
    public Vector3 rotatePoint5;
    public Vector3 rotatePoint6;
    public Vector3 rotatePoint7;

    [Header("Points of interest")]
    public Vector3 checkPoint1;
    public Vector3 checkPoint2;
    public Vector3 checkPoint3;
    public Vector3 checkPoint4;
    public Vector3 checkPoint5;
    public Vector3 checkPoint6;
    public Vector3 checkPoint7;

    [Header("rest")]
    public Color meshcolor = Color.blue;

    public LayerMask layers;
    public LayerMask occlusionLayers;    
    public UnityEvent OnPlayerLost;

    public NavMeshAgent agent;

    [HideInInspector]
    public bool childFound = false;
    public bool goToLastPlace = false;
    public bool spinning = false;
    public bool goBackToPos = false;
    public bool alarm;
    private Vector3 startEuler;
    private Vector3 currentNode;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Only add points up to the number specified by rotatepoints
        if (rotatepoints >= 1) rotatePoints.Add(rotatePoint1);
        if (rotatepoints >= 2) rotatePoints.Add(rotatePoint2);
        if (rotatepoints >= 3) rotatePoints.Add(rotatePoint3);
        if (rotatepoints >= 4) rotatePoints.Add(rotatePoint4);
        if (rotatepoints >= 5) rotatePoints.Add(rotatePoint5);
        if (rotatepoints >= 6) rotatePoints.Add(rotatePoint6);
        if (rotatepoints >= 7) rotatePoints.Add(rotatePoint7);

        if (checkNodes >= 1) checkNodesList.Add(checkPoint1);
        if (checkNodes >= 2) checkNodesList.Add(checkPoint2);
        if (checkNodes >= 3) checkNodesList.Add(checkPoint3);
        if (checkNodes >= 4) checkNodesList.Add(checkPoint4);
        if (checkNodes >= 5) checkNodesList.Add(checkPoint5);
        if (checkNodes >= 6) checkNodesList.Add(checkPoint6);
        if (checkNodes >= 7) checkNodesList.Add(checkPoint7);

        for (int i = 0; i < rotatePoints.Count; i++)
        {
            var point = rotatePoints[i];
            point.y = this.transform.position.y;
            rotatePoints[i] = point; // Set the updated point back to the list
        }

        for (int i = 0; i < checkNodesList.Count; i++)
        {
            var point = checkNodesList[i];
            point.y = transform.position.y;
            checkNodesList[i] = point; // Set the updated point back to the list
        }

        if (rotatePoints.Count > 0)
        {
            this.transform.position = rotatePoints[0];
        }

        scanInterval = 1 / scanFr;
    }

    private void OnDrawGizmos()
    {
        // Draw spheres for the points
        if (rotatepoints >= 1) Gizmos.DrawSphere(rotatePoint1, 1);
        if (rotatepoints >= 2) Gizmos.DrawSphere(rotatePoint2, 1);
        if (rotatepoints >= 3) Gizmos.DrawSphere(rotatePoint3, 1);
        if (rotatepoints >= 4) Gizmos.DrawSphere(rotatePoint4, 1);
        if (rotatepoints >= 5) Gizmos.DrawSphere(rotatePoint5, 1);
        if (rotatepoints >= 6) Gizmos.DrawSphere(rotatePoint6, 1);
        if (rotatepoints >= 7) Gizmos.DrawSphere(rotatePoint7, 1);

        // Draw lines between points
        if (rotatepoints >= 2) Gizmos.DrawLine(rotatePoint1, rotatePoint2);
        if (rotatepoints >= 3) Gizmos.DrawLine(rotatePoint2, rotatePoint3);
        if (rotatepoints >= 4) Gizmos.DrawLine(rotatePoint3, rotatePoint4);
        if (rotatepoints >= 5) Gizmos.DrawLine(rotatePoint4, rotatePoint5);
        if (rotatepoints >= 6) Gizmos.DrawLine(rotatePoint5, rotatePoint6);
        if (rotatepoints >= 7) Gizmos.DrawLine(rotatePoint6, rotatePoint7);


        Gizmos.color = Color.magenta;
        if (checkNodes >= 1) Gizmos.DrawSphere(checkPoint1, 1);
        if (checkNodes >= 2) Gizmos.DrawSphere(checkPoint2, 1);
        if (checkNodes >= 3) Gizmos.DrawSphere(checkPoint3, 1);
        if (checkNodes >= 4) Gizmos.DrawSphere(checkPoint4, 1);
        if (checkNodes >= 5) Gizmos.DrawSphere(checkPoint5, 1);
        if (checkNodes >= 6) Gizmos.DrawSphere(checkPoint6, 1);
        if (checkNodes >= 7) Gizmos.DrawSphere(checkPoint7, 1);

        if (mesh)
        {
            Gizmos.color = meshcolor;
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y -5, transform.position.z);
            Gizmos.DrawMesh(mesh, newPos, transform.rotation);
        }

        Gizmos.DrawWireSphere(transform.position, distance);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere (agent.destination, 1.1f);
        Gizmos.color = meshcolor;
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

    private void Update()
    {
        WalkingHandler();
        PlayerDetectionHandler();
        CheckPointHandler();
    }

    private void CheckPointHandler()
    {
        if (checkable)
        {
            random = Random.Range(0,2);
            Debug.Log(random);
        }

        if (PrimeCheckPoints)
        {
            checkable = true;
            foreach (Vector3 checkPoint in checkNodesList)
            {                                  
                if (Vector3.Distance(transform.position, checkPoint) < 1f)
                {
                    if (random == 0)
                    {
                        lookingLeft = true;
                        lookingRight = true;
                    }
                    PrimeCheckPoints = false;
                    checkingArea = true;
                    currentNode = checkPoint;
                    break;
                }                 
            }
        }
        else
        {
            checkable = false;
            if (Vector3.Distance(transform.position, currentNode) > 1.1f)
            {
                PrimeCheckPoints = true;
                lookingLeft = false;
                lookingRight = false;
                
            }
            
        }
    }

    private void PlayerDetectionHandler()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0)
        {
            scanTimer += scanInterval;
            Scan();
        }

        if (objInSight.Count > 0)
        {
            childFound = true;
            child = objInSight[0];
        }
        else if (childFound)
        {
            goToLastPlace = true;
            childFound = false;
        }
        else
        {
            childFound = false;
        }
    }

    private void WalkingHandler()
    {

        if (childFound)
        {
            ChaseChild();
        }
        else if (goToLastPlace)
        {
            GoToLastPlayerPosition();
        }
        else if (spinning)
        {
            Survey();
        }
        else if (goBackToPos)
        {
            BackToPath();
        }
        else if (checkingArea)
        {
            Check();
        }
        else if (!alarm)
        {
            GoToClosest();
            WalkNormal(speed);
            startEuler = transform.eulerAngles;
        }
        else
        {
            CameraResponse(closestPoint);
        }
    }

    // © Hilfer inc. All rights reserved
    private void Check()
    {
        agent.isStopped = true;
        Vector3 targetEuler = startEuler;
        if (!lookingLeft)
            targetEuler.y += 90;
        else if (!lookingRight) 
            targetEuler.y -= 90;
        targetEuler.y = targetEuler.y % 360;


        if(Mathf.Abs(targetEuler.y - transform.eulerAngles.y) < 1)
        {
            if (!lookingLeft)
            {
                lookingLeft = true;
            }
            else
            {
                lookingRight = true;
                checkingArea = false;
                agent.isStopped = false;
                return;

            }
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(targetEuler), Time.deltaTime * 3);
            
    }

    private void Survey()
    {
        goBackToPos = false;
        if (spinning)
        {
            rb.velocity = Vector3.zero;
            

            // Calculate how much to rotate this frame
            float rotationThisFrame = spinSpeed * Time.deltaTime;

            // Apply the rotation to the Rigidbody
            rb.rotation *= Quaternion.Euler(0, rotationThisFrame, 0);
            

            // Accumulate the total rotation
            totalRotation += rotationThisFrame;

            //Debug.Log("spun");
            // Check if we have rotated 360 degrees
            if (totalRotation >= 360f)
            {
                // Assuming rotatePoints is a List<Vector3>
                Vector3 currentPosition = transform.position;
                for (int i = 0; i < rotatePoints.Count; i++)
                {
                    Vector3 point = rotatePoints[0];
                    Vector3 closestPoint = rotatePoints.OrderBy(p => Vector3.Distance(p, transform.position)).FirstOrDefault();

                    // If the current point is not the closest
                    if (point != closestPoint)
                    {
                        // Remove the current point from its position
                        rotatePoints.RemoveAt(0);

                        // Add it to the back of the array
                        rotatePoints.Add(point);
                    }
                }


                // We've completed a full 360-degree rotation
                OnPlayerLost.Invoke();
                alarm = false;
                spinning = false;  // Stop spinning
                
                if (lastPath.Count > 0)
                {
                    goBackToPos = true;
                }
                totalRotation = 0f;  // Reset the total rotation counter if needed
            }
        }
    }

    private void WalkNormal(float speed2)
    {
        GoTo(rotatePoints[0], speed2);
        
        if (Vector3.Distance(transform.position, rotatePoints[0]) < (.5f * (speed2/speed)))
        {
            Vector3 temp = rotatePoints[0];
            rotatePoints.RemoveAt(0);
            rotatePoints.Add(temp);
        }
    }

    private void GoTo(Vector3 location, float speed3)
    {
        agent.speed = speed3;
        agent.destination = location;
    }

    private void BackToPath()
    {
        //GoToClosest();
        agent.SetDestination(rotatePoints[0]);
        if (Vector3.Distance(transform.position, rotatePoints[0]) < .5f)
        {
            goBackToPos = false;
        }
    }

    private void ChaseChild()
    {
        agent.SetDestination(child.transform.position);
        lastPlayerLocation = child.transform.position;
    }

    private void GoToLastPlayerPosition()
    {
        lastPlayerLocation = new Vector3(lastPlayerLocation.x, this.transform.position.y, lastPlayerLocation.z);
        //Vector3 directionToTarget = lastPlayerLocation - transform.position;
        agent.SetDestination(lastPlayerLocation);

        if (Vector3.Distance(transform.position, lastPlayerLocation) < .5f)
        {
            spinning = true;
            goToLastPlace = false;
        }
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
            bool croutching = obj.GetComponent<PlayerController>().crouchMode;
            if (IsInSight(obj))
            {
                objInSight.Add(obj);
            } else if ((Vector3.Distance(colliders[i].transform.position, transform.position) < 3) && !croutching)
            {
                objInSight.Add(obj);
            }
        }


    }

    public bool IsInSight(GameObject obj)
    {
        ////Debug.Log("detected pplayer in sphere");
        Vector3 origin = transform.position;
        Vector3 destination = obj.transform.position;
        Vector3 direction = destination - origin;
        if (direction.y < -height || direction.y > height)
        {
            ////Debug.Log("player not in right y");
            return false;
        }
        ////Debug.Log("detected pplayer in height");

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if (deltaAngle > angle)
        {
            ////Debug.Log("player is outside triangle");
            return false;
        }

        origin.y += height / 2;
        destination.y = origin.y;
        if (Physics.Linecast(origin, destination, occlusionLayers))
        {
            return false;
        }

        return true;
    }

    private void GoToClosest()
    {
        foreach (var item in rotatePoints)
        {
            if (Physics.Linecast(transform.position, item, occlusionLayers))
            {
                continue;
            }
            else if (item != rotatePoints[0])
            {
                if (Vector3.Distance(transform.position, rotatePoints[0]) < .1f)
                {
                    Vector3 temp = rotatePoints[0];
                    rotatePoints.RemoveAt(0);
                    rotatePoints.Add(temp);
                }
                continue;
            }
            else
            {
                break;
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Lose");
        }
    }

    public void CameraResponse(Vector3 location)
    {
        alarm = true;
        closestPoint = rotatePoints.OrderBy(p => Vector3.Distance(p, location)).FirstOrDefault();
        WalkNormal(speed * 2);
        placesNormal = 0;

        foreach (var item in rotatePoints)
        {
            if (item == closestPoint)
            {
                break;
            }

            placesNormal += 1;
        }

                
        if (placesNormal > (rotatepoints - placesNormal))
        {
            rotatePoints.Reverse();
            Debug.Log("rotating");
        }
        
        if (Vector3.Distance(transform.position, closestPoint) < 1f)
        {
            lastPlayerLocation = location;
            goToLastPlace = true;
            alarm = false;
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
}


/*
 * OUTDATED METHODS JUST IN CASE THE PATHFINDING GOES DOWN
 * 

private void BackToPath2()
{
    if (lastPath.Count > 0)
    {
        GoTo(lastPath[0], speed);



        if (Vector3.Distance(transform.position, lastPath[0]) < .5)
        {
            if (lastPath.Count > 1)
            {
                lastPath.RemoveAt(0);
            }
            else
            {
                lastPath.RemoveAt(0);
                goBackToPos = false;
            }

        }
    }



    private void GoToLastPlayerPosition2()
    {
        rb.velocity = transform.forward * speed;
        lastPlayerLocation = new Vector3(lastPlayerLocation.x, this.transform.position.y, lastPlayerLocation.z);
        Vector3 directionToTarget = lastPlayerLocation - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, 0.1f);

        if (Vector3.Distance(transform.position, lastPlayerLocation) < .5f)
        {
            spinning = true;
            goToLastPlace = false;
        }
    }


    private void ChaseChild2()
    {
        childDelay--;
        if (childDelay < 0)
        {
            lastPath.Add(this.transform.position);
            childDelay = 10;
        }
        Vector3 directionToTarget = child.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, 0.5f);

        lastPlayerLocation = child.transform.position;
        rb.velocity = transform.forward * speed;
    }

}*/