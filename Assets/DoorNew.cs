using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorNew : MonoBehaviour
{
    public RoomSide DoorOpeningTowards;
    public LayerMask HexMask;
    public LayerMask WallMask;

    public GameObject[] Doors;

    public Door CreateDoorHex(string RoomNameToBuild)
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit Hit;
        if (Physics.Raycast(ray, out Hit, 5f, HexMask))
        {
            Door door = Hit.transform.gameObject.AddComponent<Door>();
            door.gameObject.AddComponent<doorConnectionHex>();
            door.RoomSideToBuild = DoorOpeningTowards;
            door.door = this.gameObject;
            door.GetComponent<Node>().edge = false;
            door.RoomNameToBuild = RoomNameToBuild;
            return door;
        }
        return null;
    }

    public void OpenDoor()
    {
        GetComponent<BoxCollider>().enabled = false;
        foreach(GameObject Door in Doors)
        {
            Door.SetActive(false);
        }
    }

    public void CheckIfOnAnotherWall()
    {
        Debug.Log("Door Destroying");
        Ray ray = new Ray(transform.position + Vector3.up * 3f, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray, 5f, WallMask);
        if (hits.Length > 1)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform != this.transform)
                {
                    DestroyImmediate(hit.transform.gameObject);
                }
            }
        }
    }
}
