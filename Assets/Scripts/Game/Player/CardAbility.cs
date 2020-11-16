using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardAbility : MonoBehaviour {

    public bool LostAbility = false;
    public bool Staging = true;
    public Action[] Actions;

    Color OGColor;

	// Use this for initialization
	void Start () {
        OGColor = GetComponent<Image>().color;
    }

    //public void setUpCard(int Strength, int Agility, int Dexterity)
    //{
    //    List<ActionLine> actionLines = new List<ActionLine>();
    //    actionLines.AddRange(GetComponentsInChildren<ActionLine>());
    //    for(int i = 0; i < Actions.Length; i++)
    //    {
    //        switch (Actions[i].thisActionType)
    //        {
    //            case ActionType.Attack:
    //                for (int j = 0; j < actionLines.Count; j++)
    //                {
    //                    if (actionLines[j].MyActionType == BuffType.Strength)
    //                    {
    //                        actionLines[j].SetUpAmount(Strength);
    //                        actionLines.Remove(actionLines[j]);
    //                        break;
    //                    }
    //                }
    //                break;
    //            case ActionType.Movement:
    //                for (int j = 0; j < actionLines.Count; j++)
    //                {
    //                    if (actionLines[j].MyActionType == BuffType.Agility)
    //                    {
    //                        actionLines[j].SetUpAmount(Agility);
    //                        actionLines.Remove(actionLines[j]);
    //                        break;
    //                    }
    //                }
    //                break;
    //        }
    //    }
    //}

    public void HideAbility() { Color HideColor = new Color(0, 0, 0, .5f); }

    public void UnHighlightAbility() { GetComponent<Image>().color = OGColor; }

    public void HighlightAbility()
    {
        Color HighlightColor = new Color(0, 1, 0, .5f);
        GetComponent<Image>().color = HighlightColor;
    }

}
