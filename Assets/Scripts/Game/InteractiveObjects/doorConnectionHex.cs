using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorConnectionHex : MonoBehaviour {

    public Door door;

    public void ShowHexesInRoom()
    {
        door.ShowHexesInRoom();
    }

    public void HideHexesInRoom()
    {
        door.HideHexesInRoom();
    }
}
