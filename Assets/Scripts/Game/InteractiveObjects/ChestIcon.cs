using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestIcon : MonoBehaviour {

    Camera cam;
    public SpriteRenderer myImage;
	void Start () {
        cam = FindObjectOfType<Camera>();
    }

    public void SetIcon(Sprite Icon)
    {
        myImage.sprite = Icon;
    }
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(90 - cam.transform.rotation.eulerAngles.x, cam.transform.rotation.eulerAngles.y, 0);
    }
}
