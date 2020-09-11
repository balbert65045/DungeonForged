using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideWall : MonoBehaviour
{
    public LayerMask WallMask;

    public bool CheckIfOnAnotherWall()
    {
        Ray ray = new Ray(transform.position + Vector3.up*3f, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray, 5f, WallMask);
        if (hits.Length > 1)
        {
            foreach(RaycastHit hit in hits)
            {
                if (hit.transform != this.transform && hit.transform.GetComponentInParent<Room>() != GetComponentInParent<Room>())
                {
                    Debug.Log(hit.transform.gameObject);
                    return true;
                }
            }
        }
        return false;
    }
}
