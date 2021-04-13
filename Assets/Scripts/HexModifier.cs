using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModifierTypes
{
    Attack = 1,
    Move = 2,
    Shield = 3,
    Draw = 4,
    Heal = 5,
    Money = 6,
}

public class HexModifier : MonoBehaviour
{
    public ModifierTypes myModifier;

    public GameObject AttackMod;
    public GameObject MoveMod;
    public GameObject ShieldMod;
    public GameObject DrawMod;
    public GameObject HealMod;
    public GameObject MoneyMod;

    public void SetModifierType(ModifierTypes type)
    {
        myModifier = type;
        switch (type)
        {
            case ModifierTypes.Attack:
                AttackMod.SetActive(true); ;
                break;
            case ModifierTypes.Move:
                MoveMod.SetActive(true);
                break;
            case ModifierTypes.Shield:
                ShieldMod.SetActive(true);
                break;
            case ModifierTypes.Draw:
                DrawMod.SetActive(true);
                break;
            case ModifierTypes.Heal:
                HealMod.SetActive(true);
                break;
            case ModifierTypes.Money:
                MoneyMod.SetActive(true);
                break;
        }
    }

    void CreateRandomModifier()
    {
        int RandomIndex = Random.Range(1, 5);
        SetModifierType((ModifierTypes)RandomIndex);
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateRandomModifier();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
