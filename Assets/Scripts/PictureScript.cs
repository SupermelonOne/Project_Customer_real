using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PictureScript : MonoBehaviour
{
    public Texture2D pictureImage;
    public float fadeOutDelay = 2f;
    public float fadeOutSpeed = 1f;
    public float fadeInSpeed = 5f;
    private RawImage picture;
    private float pictureAlpha = 0f;
    private bool pictureTaken = false;

    public float growSpeed = 1f;
    private float imgScale = 1.3f;
    private Vector3 startRot;
    private Vector3 endRot;

    void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Picture");
        picture = obj.GetComponent<RawImage>();

        startRot = new Vector3(1f, Random.Range(-0.4f, 0.4f), 0);
        if (startRot.y > 0)
            endRot = new Vector3(1f, Random.Range(-0.4f, -0.1f), 0);
        else
            endRot = new Vector3(1f, Random.Range(0.1f, 0.4f), 0);

        Debug.Log("startRot = " + startRot);
        Debug.Log("endRot = " + endRot);

        picture.rectTransform.localScale = new Vector3(imgScale, imgScale, imgScale);
    }

    public void takePicture()
    {
        if (!pictureTaken)
        {
            picture.texture = pictureImage;
            picture.rectTransform.rotation = Quaternion.Euler(0, 0, 0);
            Debug.Log(Random.Range(-10, 10));
            picture.rectTransform.Rotate(new Vector3(0, 0, Random.Range(-10, 10)));
            picture.color = new Color(1f, 1f, 1f, pictureAlpha);
            pictureTaken = true;

        float fadeOutDelay = 2f;
        float fadeOutSpeed = 1f;
        float fadeInSpeed = 5f;
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (pictureTaken && fadeOutDelay > 0)
        {
            if (imgScale > 0.4f)
                imgScale -= growSpeed * Time.deltaTime;
            picture.rectTransform.right = Vector3.Slerp(picture.rectTransform.right, startRot, 0.8f * Time.deltaTime);
            if (pictureAlpha < 1)
                pictureAlpha += fadeInSpeed * Time.deltaTime;
            picture.rectTransform.localScale = new Vector3(imgScale, imgScale, imgScale);
        }
        if (pictureTaken && fadeOutDelay < 0)
        {
            pictureAlpha -= Time.deltaTime * fadeOutSpeed;
            picture.rectTransform.right = Vector3.Slerp(picture.rectTransform.right, endRot, 0.8f * Time.deltaTime);
            if (imgScale > 0f)
                imgScale -= growSpeed * Time.deltaTime;
            picture.rectTransform.localScale = new Vector3(imgScale, imgScale, imgScale);
        }
        if (pictureTaken && fadeOutDelay >= 0)
        {
            fadeOutDelay -= Time.deltaTime;
        }
        if (pictureTaken)
        {
            picture.color = new Color(1f, 1f, 1f, pictureAlpha);
        }

    }
}
