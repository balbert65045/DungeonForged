using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActionCard : MonoBehaviour {

    public bool Shuffle = false;
    public int Initiative;
    public bool AttackAvailable = true;
    public int Damage = 0;
    public bool MovementAvailable = true;
    public int Movement = 0;

    public int Range = 0;
    public int HealAmount = 0;

    public int ShieldAmount = 0;

    public string characterName;

    Vector3 initPosition;

    public void hideCard()
    {
        transform.position = initPosition;
    }

    void Start () {
        initPosition = transform.position;
    }

    public void setUpCard(int Strength, int Agility, int Dexterity)
    {
        List<ActionLine> actionLines = new List<ActionLine>();
        actionLines.AddRange(GetComponentsInChildren<ActionLine>());
        if (AttackAvailable)
        {
            for (int j = 0; j < actionLines.Count; j++)
            {
                if (actionLines[j].MyActionType == BuffType.Strength)
                {
                    actionLines[j].SetUpAmount(Strength);
                    actionLines.Remove(actionLines[j]);
                    break;
                }
            }
            for (int j = 0; j < actionLines.Count; j++)
            {
                if (actionLines[j].MyActionType == BuffType.Dexterity)
                {
                    actionLines[j].SetUpAmount(Dexterity);
                    actionLines.Remove(actionLines[j]);
                    break;
                }
            }
        }
        if (MovementAvailable)
        {
            for (int j = 0; j < actionLines.Count; j++)
            {
                if (actionLines[j].MyActionType == BuffType.Agility)
                {
                    actionLines[j].SetUpAmount(Agility);
                    actionLines.Remove(actionLines[j]);
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
