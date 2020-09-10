using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    Movement = 1,
    Attack = 2,
    Heal = 3,
    Shield = 4,
    BuffAttack = 5,
    BuffMove = 6,
    BuffRange = 7,
    BuffArmor = 8,
    Stealth = 9,
    Scout = 10,
    LoseHealth = 11,
    DrawCard = 12,
    None = 13,
}

[System.Serializable]
public struct Action {

    public ActionType thisActionType;
    public AOE thisAOE;
    public int Range;
    public int Duration;

    public Action(ActionType actionType, AOE aoe, int amount)
    {
        thisActionType = actionType;
        thisAOE = aoe;
        Range = amount;
        Duration = 0;
    }

    public Action(ActionType actionType, AOE aoe, int amount, int duration)
    {
        thisActionType = actionType;
        thisAOE = aoe;
        Range = amount;
        Duration = duration;
    }
}
