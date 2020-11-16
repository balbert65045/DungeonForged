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

public enum DeBuff
{
    None = 0,
    Bleed = 1,
}

[System.Serializable]
public struct Action {

    public ActionType thisActionType;
    public AOE thisAOE;
    public DeBuff thisDeBuff;
    public int Range;

    public Action(ActionType actionType, AOE aoe, int amount, DeBuff deBuff)
    {
        thisActionType = actionType;
        thisAOE = aoe;
        Range = amount;
        thisDeBuff = deBuff;
    }
}
