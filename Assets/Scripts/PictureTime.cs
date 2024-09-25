using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureTime : Button
{
    public void CheckPressed()
    {
        if (objInSight.Count > 0 && Input.GetKeyDown(KeyCode.E))
        {
            pressed.Invoke();
            Destroy(popup);
            Destroy(this);
            
        }
    }
}
