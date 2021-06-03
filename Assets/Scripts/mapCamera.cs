using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapCamera : MonoBehaviour
{
    Vector3 MovPos;
    float moveDistance = 1;
    public void setMovePosition(Vector3 position)
    {
        MovPos = new Vector3(transform.position.x, position.y, position.z);
    }
    // Start is called before the first frame update
    void Start()
    {
        MovPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
