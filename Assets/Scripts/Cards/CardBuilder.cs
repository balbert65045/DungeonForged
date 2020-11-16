using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardBuilder : MonoBehaviour {

    public Sprite AttackSprite;
    public Sprite MoveSprite;
    public Sprite RangeSprite;
    public Sprite HealSprite;
    public Sprite ArmorSprite;
    public Sprite SneakSprite;
    public Sprite DurationSprite;
    public Sprite ScoutSprite;
    public Sprite DrawSprite;

    public GameObject CardPrefab;
    public GameObject AbilityLinePrefab;

    public string CardName;
    public int EnergyValue;
    public bool Lost = false;
    public Action[] Actions;

    int StartingPos = 0;

    public void BuildCard()
    {
        GameObject Card = Instantiate(CardPrefab, this.transform);
        Card.name = CardName;
        NewCard newCard = Card.GetComponent<NewCard>();
        newCard.NameText.text = CardName;
        newCard.EnergyAmount = EnergyValue;
        newCard.EnergyText.text = EnergyValue.ToString();
        CardAbility Ability = Card.GetComponentInChildren<CardAbility>();
        Ability.LostAbility = Lost;
        Ability.Actions = Actions;
        StartingPos = (newCard.AbilityLinePositions.Length - Actions.Length) / 2;
        for (int i = 0; i < Actions.Length; i++)
        {
            if (StartingPos >= newCard.AbilityLinePositions.Length) { return; }
            BuildActionLine(i, newCard);
            StartingPos++;
        }
    }


    void BuildCombatCard()
    {
        //GameObject Card = Instantiate(CombatCardPrefab, this.transform);
        //Card.name = CardName;
        //CombatPlayerCard CombatCard = Card.GetComponent<CombatPlayerCard>();
        //CombatCard.myType = characterForCard;
        //CombatCard.NameText.text = CardName;
        //CombatCard.Initiative = Initiative;
        //CombatCard.InitiativeText.text = Initiative.ToString();

        //CardAbility Ability = Card.GetComponentInChildren<CardAbility>();
        //Ability.LostAbility = Lost;
        //Ability.Actions = Actions;

        //StartingPos = (CombatCard.AbilityLinePositions.Length - Actions.Length) / 2;
        //for (int i = 0; i < Actions.Length; i++)
        //{
        //    if (StartingPos >= CombatCard.AbilityLinePositions.Length) { return; }
        //    BuildActionLine(i, CombatCard);
        //    StartingPos++;
        //}

    }

    void BuildOutOfCombatCard()
    {
        //GameObject Card = Instantiate(OutOfCombatCardPrefab, this.transform);
        //Card.name = CardName;
        //OutOfCombatCard OutOfCombatCard = Card.GetComponent<OutOfCombatCard>();
        //OutOfCombatCard.myType = characterForCard;
        //OutOfCombatCard.NameText.text = CardName;

        //CardAbility Ability = Card.GetComponentInChildren<CardAbility>();
        //Ability.Actions = Actions;
        //StartingPos = (OutOfCombatCard.AbilityLinePositions.Length - Actions.Length) / 2;
        //for (int i = 0; i < Actions.Length; i++)
        //{
        //    if (StartingPos >= OutOfCombatCard.AbilityLinePositions.Length) { return; }
        //    BuildActionLine(i, OutOfCombatCard);
        //    StartingPos++;
        //}
    }

    void BuildActionLine(int index, Card Card)
    {
        GameObject actionLineObj = Instantiate(AbilityLinePrefab, Card.AbilityLinePositions[StartingPos].transform);
        actionLineObj.transform.localPosition = Vector3.zero;
        ActionLine actionLine = actionLineObj.GetComponent<ActionLine>();
        actionLine.actionIndex = index;
        switch (Actions[index].thisActionType)
        {
        case ActionType.Attack:
            actionLine.AbilityType.text = "Attack";
            actionLine.AbilityImage.sprite = AttackSprite;
            actionLine.AbilityAmount.text = Actions[index].thisAOE.Damage.ToString();
            SetRange(actionLine, Actions[index].Range, 1);
            break;
        case ActionType.Movement:
            actionLine.AbilityType.text = "Move";
            actionLine.AbilityImage.sprite = MoveSprite;
            actionLine.AbilityAmount.text = Actions[index].Range.ToString();
            SetRange(actionLine, 0, 1);
            break;
        case ActionType.Shield:
            actionLine.AbilityType.text = "Shield";
            actionLine.AbilityImage.sprite = ArmorSprite;
            actionLine.AbilityAmount.text = Actions[index].thisAOE.Damage.ToString();
            SetRange(actionLine, Actions[index].Range, 1);
            break;
        case ActionType.Heal:
            actionLine.AbilityType.text = "Heal";
            actionLine.AbilityImage.sprite = HealSprite;
            actionLine.AbilityAmount.text = Actions[index].thisAOE.Damage.ToString();
            SetRange(actionLine, Actions[index].Range, 1);
            break;
        case ActionType.BuffArmor:
            actionLine.AbilityType.text = "Buff";
            actionLine.AbilityImage.sprite = ArmorSprite;
            actionLine.AbilityAmount.text = Actions[index].thisAOE.Damage.ToString();
            SetRange(actionLine, Actions[index].Range, 1);
            break;
        case ActionType.BuffAttack:
            actionLine.AbilityType.text = "Buff";
            actionLine.AbilityImage.sprite = AttackSprite;
            actionLine.AbilityAmount.text = Actions[index].thisAOE.Damage.ToString();
            SetRange(actionLine, Actions[index].Range, 1);
            break;
        case ActionType.BuffMove:
            actionLine.AbilityType.text = "Buff";
            actionLine.AbilityImage.sprite = MoveSprite;
            actionLine.AbilityAmount.text = Actions[index].thisAOE.Damage.ToString();
            SetRange(actionLine, Actions[index].Range, 1);
            break;
        case ActionType.BuffRange:
            actionLine.AbilityType.text = "Buff";
            actionLine.AbilityImage.sprite = RangeSprite;
            actionLine.AbilityAmount.text = Actions[index].thisAOE.Damage.ToString();
            SetRange(actionLine, Actions[index].Range, 1);
            break;
        case ActionType.Stealth:
            actionLine.AbilityType.text = "Stealth";
            actionLine.AbilityImage.sprite = SneakSprite;
            break;
        case ActionType.Scout:
            actionLine.AbilityType.text = "Scout";
            actionLine.AbilityImage.sprite = ScoutSprite;
            actionLine.AbilityAmount.text = Actions[index].Range.ToString();
            break;
        case ActionType.DrawCard:
            actionLine.AbilityType.text = "Draw";
            actionLine.AbilityImage.sprite = DrawSprite;
            actionLine.AbilityAmount.text = Actions[index].thisAOE.Damage.ToString();
            SetRange(actionLine, Actions[index].Range, 1);
            break;
        }
    }

    void SetRange(ActionLine line, int Range, int threshhold)
    {
        if (Range > threshhold)
        {
            line.RangeAbilityType.gameObject.SetActive(true);
            line.RangeAbilityImage.gameObject.SetActive(true);
            line.RangeAbilityAmount.gameObject.SetActive(true);
            line.RangeAbilityAmount.text = Range.ToString();
        }
        else
        {
            line.RangeAbilityType.gameObject.SetActive(false);
            line.RangeAbilityAmount.gameObject.SetActive(false);
            line.RangeAbilityImage.gameObject.SetActive(false);
        }
    }
}
