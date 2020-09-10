using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyActionBoard : MonoBehaviour {

    public GameObject panel;
    public ActionButton[] Actions;


    public void HidePanel()
    {
        panel.SetActive(false);
    }

    public void ShowPanel()
    {
        panel.SetActive(true);
    }

    public void showActions(Action[] actions, Character character)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            Actions[i].gameObject.SetActive(true);
            Actions[i].SetAction(actions[i], character);
        }
    }

    public void hideActions()
    {
        int i = 0;
        foreach (ActionButton action in Actions)
        {
            UnHighlightAction(i);
            i++;
            action.gameObject.SetActive(false);
        }
    }

    public void HighlightAction(int actionIndex)
    {
        Actions[actionIndex].HighlightAction();
    }

    public void UnHighlightAction(int actionIndex)
    {
        Actions[actionIndex].UnHighlightColor();
    }

    public void DisableAction(int actionIndex)
    {
        Actions[actionIndex].DisableButton();
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
