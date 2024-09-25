using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public GameObject popup;  // Assign the popup object in the Inspector

    // Update is called once per frame
    
    void Update()
    {
        // Find the player using the "Player" tag
        GameObject camera = GameObject.FindWithTag("MainCamera");
        if (camera != null)
        {
            // Make the popup face the player
            Vector3 pos = new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z);
            this.transform.LookAt(pos);
            this.transform.Rotate(0, 180, 0); // Optional: Rotate 180 degrees if the text is backward
        }
    }
}
