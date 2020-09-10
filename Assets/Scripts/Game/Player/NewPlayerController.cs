using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController : MonoBehaviour {

    public PlayerCharacter SelectPlayerCharacter;

	void Start () {
        FindObjectOfType<MyCameraController>().LookAt(SelectPlayerCharacter.transform);
	}

    public void ShowStagedAction(Action action)
    {
        SelectPlayerCharacter.ShowAction(action.Range, action.thisActionType);
    }
}
