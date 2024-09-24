using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.Events;

public class ButtonScript : MonoBehaviour
{
    private PlayerController player;
    public float requiredDistance = 5f;
    public KeyCode buttonPress = KeyCode.E;
    public UnityEvent activation;
    public GameObject prefab;
    public GameObject popup;
    private bool prefabExists = false;

    private bool functional = false;


    // Start is called before the first frame update
    void Start()
    {
        player = Object.FindObjectOfType<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {

        if (functional && Vector3.Distance(transform.position, player.transform.position) < requiredDistance)
        {
            if (Input.GetKeyDown(buttonPress))
            {
                activation.Invoke();
            }
            CreatePopUp(new Vector3(0, 5f, 0), "press 'E' to activate");
        }
        else
        {
            popup.SetActive(false);
        }
    }

    public void CreatePopUp(Vector3 position, string text)
    {
        //Vector3 vec = new Vector3(0, position.y, 0) + position.x * transform.right + position.z * transform.forward;
        popup.SetActive(true);
        popup.transform.localPosition = position;
        var temp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>(); 
        temp.text = text;

        prefabExists = true;
    }

}
