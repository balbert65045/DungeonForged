using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine("Loading");
	}

    IEnumerator Loading()
    {
        yield return new WaitForSeconds(3f);
        FindObjectOfType<LevelManager>().LoadNextLevelFromIndex();
    }
}
