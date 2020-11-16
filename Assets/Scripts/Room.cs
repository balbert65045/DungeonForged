using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int width;
    public int height;
    public bool start = false;

    public List<RoomSide> DoorOpenings = new List<RoomSide>();


    public DoorNew[] doors;
}
