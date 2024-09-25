using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.Events;

public class ButtonScript : MonoBehaviour
{
    public float requiredDistance = 3f;
    public string displayText = "press 'E' to activate";
    public KeyCode buttonPress = KeyCode.E;
    private PlayerController player;
    public UnityEvent action;
    public GameObject popup;
    private bool functional = false;


    // Start is called before the first frame update
    void Start()
    {
        player = Object.FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.Log("I FAILED");
            functional = false;
        }
        else
        {
            functional = true;
            Debug.Log("I'm placing smthng but nothing");
            popup = Instantiate(popup, Vector3.zero, Quaternion.identity, transform);
            popup.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Vector3.Distance(transform.position, player.transform.position));
        if (functional && Vector3.Distance(transform.position, player.transform.position) < requiredDistance)
        {
            if (Input.GetKeyDown(buttonPress))
            {
                action.Invoke();
            }
            CreatePopUp(new Vector3(0,2,0), displayText);
        }
        else
        {
            popup.SetActive(false);
        }
    }

    public void CreatePopUp(Vector3 position, string text)
    {
        Debug.Log("Im running the CreatePopUp");
        popup.SetActive(true);
        popup.transform.localPosition = position;
        var temp = popup.transform.GetChild(1).GetComponent<TextMeshProUGUI>(); 
        temp.text = text;
    }

}