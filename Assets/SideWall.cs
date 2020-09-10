using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideWall : MonoBehaviour
{
    public LayerMask WallMask;
    public Vector3 CenterOffset;

    public void CheckIfOnAnotherWall()
    {
        Ray ray = new Ray(transform.position + transform.right * -1.5f*CenterOffset.magnitude + Vector3.up*3f, Vector3.down);
        if (Physics.RaycastAll(ray, 5f, WallMask).Length > 1)
        {
            DestroyImmediate(this.gameObject);
        }
    }
}
