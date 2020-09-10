using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorNew : MonoBehaviour
{
    public RoomSide DoorOpeningTowards;
    public LayerMask HexMask;
    public LayerMask WallMask;

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

    public bool CheckIfOnAnotherWall()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 3f, Vector3.down);
        if (Physics.RaycastAll(ray, 5f, WallMask).Length > 1)
        {
            DestroyImmediate(this.gameObject);
            return true;
        }
        return false;
    }
}
