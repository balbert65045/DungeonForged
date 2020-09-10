using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AOEType
{
    SingleTarget = 0,
    Line = 1,
    Cleave = 2,
    Triangle = 3,
    Surounding = 4, 
    Circle = 5,
    GreatCleave = 6,
    LargeLine = 7
}

[System.Serializable]
public struct AOE  {

    public AOEType thisAOEType;
    public int Targets;
    public int Damage;

    public AOE(AOEType aoeType, int targets, int damage)
    {
        Targets = targets;
        thisAOEType = aoeType;
        Damage = damage;
    }

}
