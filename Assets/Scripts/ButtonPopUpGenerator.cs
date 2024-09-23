using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonPopUpGenerator : MonoBehaviour
{
    public static ButtonPopUpGenerator current;
    public GameObject prefab;
    void Start()
    {
        current = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.U));
        {
            CreatePopUp(Vector3.one, Random.Range(0, 100).ToString());
        }
    }

    public void CreatePopUp(Vector3 position, string text)
    {
        var popup = Instantiate(prefab, position, Quaternion.identity);
        var temp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        temp.text = text;
    }
}
