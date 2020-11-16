using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorNew : MonoBehaviour
{
    public RoomSide DoorOpeningTowards;
    public LayerMask HexMask;
    public LayerMask WallMask;

    public GameObject Railing;
    public GameObject DoorObj;
    public GameObject[] Doors;

    private Door myDoor;

    public void TurnDoorToRailing()
    {
        myDoor.GetComponent<Node>().edge = true;
        myDoor.GetComponent<Hex>().HideHex();
        DestroyImmediate(myDoor);
        DoorObj.SetActive(false);
        Railing.SetActive(true);
    }

    public Door CreateDoorHex(string RoomNameToBuild)
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit Hit;
        if (Physics.Raycast(ray, out Hit, 5f, HexMask))
        {
            myDoor = Hit.transform.gameObject.AddComponent<Door>();
            myDoor.gameObject.AddComponent<doorConnectionHex>();
            myDoor.RoomSideToBuild = DoorOpeningTowards;
            myDoor.door = this.gameObject;
            myDoor.GetComponent<Node>().edge = false;
            myDoor.RoomNameToBuild = RoomNameToBuild;
            return myDoor;
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

    public bool CheckIfOnAnotherWall()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 3f, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray, 5f, WallMask);
        if (hits.Length > 1)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform != this.transform)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
