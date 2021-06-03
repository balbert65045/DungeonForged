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

public enum DeBuffType
{
    None = 0,
    Bleed = 1,
    Poison = 2,
    Immobelized = 3,
    Weaken = 4,
    Stun = 5,
    Disarm = 6,
    Slow = 7,

    IncreaseAttack = 9,
    IncreaseMove = 10,
    PowerUp = 11,
}

[System.Serializable]
public struct DeBuff
{
    public DeBuffType thisDeBuffType;
    public int Amount;

    public DeBuff(DeBuffType deBuff, int amount)
    {
        thisDeBuffType = deBuff;
        Amount = amount;
    }
}

[System.Serializable]
public struct Action {

    public ActionType thisActionType;
    public AOE thisAOE;
    public DeBuff thisDeBuff;
    public int Range;
    public string TriggerAnimation;

    public Action(ActionType actionType, AOE aoe, int amount, DeBuff debuff, string triggerAnimation = null)
    {
        thisActionType = actionType;
        thisAOE = aoe;
        Range = amount;
        thisDeBuff = debuff;
        TriggerAnimation = triggerAnimation;
    }
}
