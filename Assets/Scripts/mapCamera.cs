using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapCamera : MonoBehaviour
{
    Vector3 MovPos;
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
        //if ((MovPos - transform.position).magnitude > .02f)
        //{
        //    transform.position = Vector3.Lerp(transform.position, MovPos, .002f);
        //}
    }
}
