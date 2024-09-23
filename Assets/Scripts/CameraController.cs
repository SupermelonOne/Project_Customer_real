using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class CameraController : MonoBehaviour
{
    public float sensitivity = 100.0f;

    public float camTopLimit = 40f;
    public float camBottomLimit = -30f;
    public float camSmoothBack = 1f;
    public Light flashLight;

    public Transform cameraPivotY;
    private Vector3 camPivYVec = new Vector3(0, 0, 0);
    public Transform cameraPivotX;
    private Vector3 camPivXVec = new Vector3(0, 0, 0);
    public Transform cameraPosition;
    private Vector3 cameraOriginalPos;
    private float cameraDistance;
    private float keepNewCamDist;
    private float keepCameraDistance;

    private Material playerMaterial;

    private bool firstPersonMode = false;


    // Start is called before the first frame update
    void Start()
    {
        Vector3 currentRotation = cameraPivotX.transform.rotation.eulerAngles;
        currentRotation.x = 0;
        cameraPivotX.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        cameraOriginalPos = cameraPosition.position;
        cameraDistance = Vector3.Distance(transform.position, cameraPosition.position);
        keepCameraDistance = cameraDistance;
        keepNewCamDist = cameraDistance;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        playerMaterial = GetComponentInChildren<MeshRenderer>().material;

        flashLight.intensity = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        #region Camera Rotation
        //cameraPivotY.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * sensitivity);
        camPivYVec += new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * sensitivity;
        cameraPivotY.transform.rotation = Quaternion.Euler(camPivYVec);



        camPivXVec -= new Vector3(Input.GetAxis("Mouse Y"), 0, 0) * Time.deltaTime * sensitivity;
        camPivXVec.y = camPivYVec.y;
        if (camPivXVec.x > camTopLimit)
        {
            camPivXVec.x += (camTopLimit - camPivXVec.x) / camSmoothBack;
        }
        if (camPivXVec.x < camBottomLimit)
        {
            camPivXVec.x += (camBottomLimit - camPivXVec.x) / camSmoothBack;
        }

        //Debug.Log("pivXVec = " +  camPivXVec);
        // Debug.Log("pivXVec = " +  camPivXVec);
        cameraPivotX.transform.rotation = Quaternion.Euler(camPivXVec);
        #endregion

        #region Camera avoid terrain
        Vector3 cameraDirection = cameraPosition.position - transform.position;
        RaycastHit cameraHit;
        Ray rayToCam = new Ray(transform.position, cameraDirection);
        Vector3 newCameraDistance = cameraPosition.position;

        //sends a raycast outward of the camera, checking if it hits anything WHEN NOT IN FIRST PERSON MODE
        //than adjusts the distance of the camera according to where the hit took place
        Debug.DrawRay(transform.position, cameraDirection.normalized * cameraDistance);

        if (Physics.Raycast(rayToCam, out cameraHit, cameraDistance) && !cameraHit.collider.CompareTag("Player") && !firstPersonMode)
        {
            newCameraDistance = transform.position + cameraDirection.normalized * (cameraHit.distance);
            keepNewCamDist = cameraHit.distance;
        }
        else if (keepNewCamDist < cameraDistance)
        {
            keepNewCamDist += 6f * Time.deltaTime;
            newCameraDistance = transform.position + cameraDirection.normalized * keepNewCamDist;
        }
        else
        {
            newCameraDistance = transform.position + cameraDirection.normalized * cameraDistance;
        }
        cameraPosition.position = newCameraDistance;
        #endregion

        #region Transparent Player
 
        if (keepNewCamDist >= cameraDistance) 
            playerMaterial.color = new Color(1.0f, 1.0f, 1.0f, cameraDistance / keepCameraDistance);
        else
            playerMaterial.color = new Color(1.0f, 1.0f, 1.0f, keepNewCamDist / cameraDistance);

        #endregion

        #region First Person Mode

        if (firstPersonMode)
        {
            if (cameraDistance > 1f)
            {
                cameraDistance -= 6f * Time.deltaTime;
            }
        }
        else if (cameraDistance < keepCameraDistance)
        {
            cameraDistance += 6f * Time.deltaTime;
        }

        #endregion
    }

    #region first person mode activation
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("FirstPersonArea"))
        {
            firstPersonMode = true;
            flashLight.intensity = 9f;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FirstPersonArea"))
        {
            firstPersonMode = false;
            keepNewCamDist = cameraDistance;
            flashLight.intensity = 0f;
        }
    }
    #endregion
}