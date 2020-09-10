using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSCharacterGroupPanel : MonoBehaviour {

    public GameObject Panel;

    public void HidePanel()
    {
        Panel.SetActive(false);
    }

    public void ShowPanel()
    {
        Panel.SetActive(true);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
