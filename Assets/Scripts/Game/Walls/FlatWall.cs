using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatWall : MonoBehaviour {

    public List<string> RoomLinkedTo = new List<string>();

    public void LinkWallToRoom(string room)
    {
        RoomLinkedTo.Add(room);
    }
}
