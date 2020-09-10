using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour {

    public GameObject Door;
    public GameObject DoorFrame;
    public GameObject Stairs;

    public void ShowStairsAndDoor()
    {
        //Door.SetActive(true);
        //DoorFrame.SetActive(true);
        //Stairs.SetActive(true);
    }

    public void HideStairsAndDoor()
    {
        //Door.SetActive(false);
        //DoorFrame.SetActive(false);
        //Stairs.SetActive(false);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
