using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{   
    public string item;
    public UnityEvent<string> pickedUp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            pickedUp.Invoke(item);        
        }
    }
}
