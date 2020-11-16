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
        if (FindObjectOfType<NewGroupStorage>() != null)
        {
            FindObjectOfType<NewGroupStorage>().LevelIndex = 0;
        }
		if (FindObjectOfType<MapCanvas>() != null)
        {
			Destroy(FindObjectOfType<MapCanvas>().gameObject);
		}
        if (FindObjectOfType<PlayerUICanvas>() != null)
        {
            Destroy(FindObjectOfType<PlayerUICanvas>().gameObject);
        }
	}
}
