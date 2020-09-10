using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorWall : MonoBehaviour {

    public GameObject Door;
    public GameObject DoorFrame;
    public bool onStart = false;


    public void HideDoor()
    {
        //if (!onStart)
        //{
        //    Door.SetActive(false);
        //    DoorFrame.SetActive(false);
        //}
    }

    public void ShowDoor()
    {
        //Door.SetActive(true);
        //DoorFrame.SetActive(true);
    }
}
