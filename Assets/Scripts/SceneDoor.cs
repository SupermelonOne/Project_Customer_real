using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDoor : MonoBehaviour
{
    public string scene;
    private bool locked = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !locked)
        {
            SceneManager.LoadScene(scene);
        }
    }

    public void Unlock()
    {
        locked = false;
    }
}
