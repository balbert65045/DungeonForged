using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (FindObjectOfType<NewGroupStorage>() != null)
        {
            Destroy(FindObjectOfType<NewGroupStorage>().gameObject);
        }
        if (FindObjectOfType<LevelManager>() != null)
        {
            FindObjectOfType<LevelManager>().LevelIndex = 0;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
