using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreen : MonoBehaviour {

    public GameObject Panel;

    public void ShowMenu()
    {
        Panel.SetActive(true);
    }

    public void HideMenu()
    {
        Panel.SetActive(false);
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
