using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionButton : MonoBehaviour {

    bool openingDoor = false;
    bool exiting = false;

    public void allowOpenDoorAction()
    {
        openingDoor = true;
        GetComponentInChildren<Text>().text = "Open Door";
    }

    public void allowHideDoorAction()
    {
        openingDoor = false;
        GetComponentInChildren<Text>().text = "";
    }

    public void AllowExit()
    {
        exiting = true;
        GetComponentInChildren<Text>().text = "Exit Floor";
    }

    public void DisableExit()
    {
        exiting = false;
        GetComponentInChildren<Text>().text = "";
    }

    public void PerformAction()
    {
        if (openingDoor) {
            openingDoor = false;
          //  FindObjectOfType<PlayerController>().OpenDoor();
        }
        else if (exiting) {
            exiting = false;
            FindObjectOfType<PlayerController>().ExitLevel();
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
