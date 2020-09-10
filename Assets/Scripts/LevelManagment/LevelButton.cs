using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : MonoBehaviour {

    LevelManager levelManager;

	// Use this for initialization
	void Start () {
        levelManager = FindObjectOfType<LevelManager>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadLevelIndex()
    {
        levelManager.LevelIndex++;
        levelManager.LoadLevelWithLoading();
    }

    public void ReloadLevel()
    {
        levelManager.ReloadLevel();
    }

    public void LoadLevel(string name)
    {
        levelManager.LoadLevel(name);
    }

    public void Quit()
    {
        levelManager.QuitRequest();
    }

}
