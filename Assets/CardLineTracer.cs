using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLineTracer : MonoBehaviour
{

    public GameObject CardFollowing;
    private Camera camera;

    private void Start()
    {
        camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CardFollowing != null)
        {
            transform.position = camera.ScreenToWorldPoint(new Vector3(CardFollowing.transform.position.x, CardFollowing.transform.position.y, camera.nearClipPlane));
        }
    }
}
