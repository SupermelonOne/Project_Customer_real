using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Button : MonoBehaviour
{
    private int count;
    private Mesh mesh;
    private Collider[] colliders = new Collider[50];
    private bool prefabExists = false;
    private bool pressedBool;
    public  GameObject popup;

    public UnityEngine.Color meshcolor;
    public List<GameObject> objInSight = new List<GameObject>();
    public float distance = 5;
    public LayerMask layers;

    public static Button current;
    public GameObject prefab;
    public UnityEvent pressed;

    public float hitBoxOffsetX;
    public float hitBoxOffsetY;
    public float hitBoxOffsetZ;

    public float popUpOffsetX;
    public float popUpOffsetY;
    public float popUpOffsetZ;

    public float holdTime;

    public Guard guard;


    void Start()
    {
        current = this;
        guard = gameObject.GetComponent<Guard>();

        //creating the popup here to avoid a drop in framerate
        popup = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        popup.SetActive(false);

        holdTime = 0;
    }

    void Update()
    {
        Scan();

        if (objInSight.Count > 0 /*&& !guard.spinning && !guard.childFound*/)
        {
            Vector3 place = new Vector3(0, 0, popUpOffsetZ);
            CreatePopUp(place, "press E to activate!");  
        } 
        
        if((objInSight.Count <= 0 && popup != null) /*|| guard.spinning || guard.childFound*/)
        {
            popup.SetActive(false);
        }

        CheckPressed();

    }

    void CheckPressed()
    {

        if (objInSight.Count > 0 && Input.GetKey(key: KeyCode.E))
        {
            Debug.Log(holdTime);
            holdTime += .1f;
            popup.GetComponent<UnityEngine.UI.Slider>().value = holdTime;
            if (holdTime >= 1)
            {
                pressed.Invoke();
                Destroy(popup);
                Destroy(this);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshcolor;
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y - 5, transform.position.z );
            Vector3 vec = new Vector3(0, transform.position.y, 0) + transform.position.x * transform.right + transform.position.z * transform.forward;
            Gizmos.DrawMesh(mesh, newPos, transform.rotation);
        }


        Vector3 scanSpace = transform.position + transform.forward * 4;
        Gizmos.DrawWireSphere(scanSpace, distance);

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

    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();
        return mesh;
    }

    public void CreatePopUp(Vector3 position, string text)
    {
        Debug.Log(popup);

        //Vector3 vec = new Vector3(0, position.y, 0) + position.x * transform.right + position.z * transform.forward;
        popup.SetActive(true);
        popup.transform.localPosition = position;
        var temp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        temp.text = text;

        // Find the nearest player
        GameObject player = objInSight.Find(obj => obj.CompareTag("Player"));
        if (player != null)
        {
            // Make the popup face the player
            popup.transform.LookAt(player.transform);
            popup.transform.Rotate(0, 180, 0); // Optional: Rotate 180 degrees if the text is backward
        }

        prefabExists = true;
    }


    private void Scan()
    {
        Vector3 scanSpace = transform.position + transform.forward * 4;
        count = Physics.OverlapSphereNonAlloc(scanSpace, distance, colliders, layers, QueryTriggerInteraction.Collide);
        Debug.Log(count);
        objInSight.Clear();
        for (int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].gameObject;
            if (obj.CompareTag("Player"))
            {
                objInSight.Add(obj);
            }         
        }
    }
}
