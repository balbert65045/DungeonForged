using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayArea : MonoBehaviour {

    public GameObject ActionsAvailable;

    private Color OGAttackButtonColor;
    private Color OGMoveButtonColor;
    private Color OGHealButtonColor;
    private Color OGShieldButtonColor;

    public Button AttackButton;
    public Button MoveButton;
    public Button HealButton;
    public Button ShieldButton;

    public void showActions(CardAbility cardAbility)
    {
        ActionsAvailable.SetActive(true);
        CheckToDisableActions(cardAbility.Actions);
    }

    public void HideActions()
    {
        ResetButtonColors();
        AttackButton.interactable = true; 
        MoveButton.interactable = true; 
        HealButton.interactable = true; 
        ShieldButton.interactable = true; 
        ActionsAvailable.SetActive(false);
    }

    public void CheckToDisableActions(Action[] actions)
    {
        ResetButtonColors();
        bool AttackAvailable = false;
        bool MoveAvailable = false;
        bool HealAvailable = false;
        bool ShieldAvailable = false;
        foreach (Action action in actions)
        {
            switch (action.thisActionType)
            {
                case ActionType.Attack:
                    AttackAvailable = true;
                    break;
                case ActionType.Movement:
                    MoveAvailable = true;
                    break;
                case ActionType.Heal:
                    HealAvailable = true;
                    break;
                case ActionType.Shield:
                    ShieldAvailable = true;
                    break;
            }
        }
        if (!AttackAvailable) { AttackButton.interactable = false; }
        if (!MoveAvailable) { MoveButton.interactable = false; }
        if (!HealAvailable) { HealButton.interactable = false; }
        if (!ShieldAvailable) { ShieldButton.interactable = false; }
    }

    public void showCurrentAction(ActionType thisActionType)
    {
        ResetButtonColors();
        switch (thisActionType)
        {
            case ActionType.Attack:
                AttackButton.GetComponent<Image>().color = Color.blue;
                break;
            case ActionType.Movement:
                MoveButton.GetComponent<Image>().color = Color.blue;
                break;
            case ActionType.Heal:
                HealButton.GetComponent<Image>().color = Color.blue;
                break;
            case ActionType.Shield:
                ShieldButton.GetComponent<Image>().color = Color.blue;
                break;
        }
    }

    public void ResetButtonColors()
    {
        AttackButton.GetComponent<Image>().color = OGAttackButtonColor;
        MoveButton.GetComponent<Image>().color = OGMoveButtonColor;
        HealButton.GetComponent<Image>().color = OGHealButtonColor;
        ShieldButton.GetComponent<Image>().color = OGShieldButtonColor;
    }

	// Use this for initialization
	void Start () {
        OGAttackButtonColor = AttackButton.GetComponent<Image>().color;
        OGMoveButtonColor = MoveButton.GetComponent<Image>().color;
        OGHealButtonColor = HealButton.GetComponent<Image>().color;
        OGShieldButtonColor = ShieldButton.GetComponent<Image>().color;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
