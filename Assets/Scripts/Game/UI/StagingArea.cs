using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagingArea : MonoBehaviour {

    public List<Action> CurrentActions;
    public Action LastActionStackingOn;

    public void DiscardUsedCards()
    {
        NewCard[] cards = GetComponentsInChildren<NewCard>();
        DiscardPile discardPile = FindObjectOfType<DiscardPile>();
        BurnPile burnPile = FindObjectOfType<BurnPile>();
        foreach (NewCard card in cards)
        {
            if (card.cardAbility.LostAbility) { burnPile.BurnCard(card); }
            else { discardPile.DiscardCard(card); }
        }
        //FindObjectOfType<NewHand>().MakeAllCardsPlayable();
        //ClearStagedAction();
    }

    public void DiscardCards()
    {
        NewCard[] cards = GetComponentsInChildren<NewCard>();
        DiscardPile discardPile = FindObjectOfType<DiscardPile>();
        foreach(NewCard card in cards) { discardPile.DiscardCard(card); }
        //FindObjectOfType<NewHand>().MakeAllCardsPlayable();
        //ClearStagedAction();
    }

    void ShowAction()
    {
        FindObjectOfType<PlayerController>().ShowStagedAction(CurrentActions);
    }

    void SetFirstAction(Action[] actions)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            Action newAction = new Action(actions[i].thisActionType, actions[i].thisAOE, actions[i].Range, actions[i].thisDeBuff);
            CurrentActions.Add(newAction);
            if (i == actions.Length - 1) { LastActionStackingOn = newAction; }
        }
        ShowAction();
    }


    void AddToStagedAction(Action[] actions)
    {
        for (int i = 0; i < actions.Length; i++)
        {
            Action AddedAction = actions[i];
            if (actions[i].thisActionType == ActionType.Movement)
            {
                if (LastActionStackingOn.thisActionType == ActionType.Movement)
                {
                    CurrentActions[CurrentActions.Count - 1] = new Action(ActionType.Movement, LastActionStackingOn.thisAOE, LastActionStackingOn.Range + AddedAction.Range, LastActionStackingOn.thisDeBuff);
                    LastActionStackingOn = CurrentActions[CurrentActions.Count - 1];
                }
                else
                {
                    Action Maction = new Action(AddedAction.thisActionType, AddedAction.thisAOE, AddedAction.Range, AddedAction.thisDeBuff);
                    CurrentActions.Add(Maction);
                    LastActionStackingOn = Maction;
                }
            }
            else
            {
                Action Aaction = new Action(AddedAction.thisActionType, AddedAction.thisAOE, AddedAction.Range, AddedAction.thisDeBuff);
                CurrentActions.Add(Aaction);
                LastActionStackingOn = Aaction;
            }
        }
        ShowAction();
    }

    void SubtractToStagedAction(Action[] actions)
    {
        for (int i = actions.Length - 1; i >= 0; i--)
        {
            if (actions[i].thisActionType == ActionType.Movement)
            {
                if (LastActionStackingOn.Range == actions[i].Range)
                {
                    CurrentActions.RemoveAt(CurrentActions.Count - 1);
                    LastActionStackingOn = CurrentActions[CurrentActions.Count - 1];
                }
                else
                {
                    CurrentActions[CurrentActions.Count - 1] = new Action(ActionType.Movement, LastActionStackingOn.thisAOE, LastActionStackingOn.Range - actions[i].Range, LastActionStackingOn.thisDeBuff);
                    LastActionStackingOn = CurrentActions[CurrentActions.Count - 1];
                }
            }
            else
            {
                CurrentActions.RemoveAt(CurrentActions.Count - 1);
                LastActionStackingOn = CurrentActions[CurrentActions.Count - 1];
            }
        }
        ShowAction();
    }

    public void ClearStagedAction()
    {
        CurrentActions.Clear();
        ShowAction();
    }

    public GameObject[] Positions;

    public void ReturnLastCardToHand()
    {
        NewCard card = GetLastCard();
        FindObjectOfType<EnergyAmount>().AddEnergy(card.EnergyAmount);
        int index = card.transform.parent.GetSiblingIndex();
        FindObjectOfType<NewHand>().PlaceCardOnNextAvailableSpot(card);
        if (index == 0) {
            ClearStagedAction();
            FindObjectOfType<NewHand>().MakeAllCardsPlayable();
        }
        else {
            SubtractToStagedAction(card.cardAbility.Actions);
        }
    }

    public NewCard GetLastCard()
    {
        for (int i = Positions.Length - 1; i >= 0; i--)
        {
            if (Positions[i].GetComponentInChildren<NewCard>() != null)
            {
                return Positions[i].GetComponentInChildren<NewCard>();
            }
        }
        return null;
    }

    public void PlaceCardOnNextAvailableSpot(NewCard card)
    {
        for(int i = 0; i < Positions.Length; i++)
        {
            if (Positions[i].GetComponentInChildren<NewCard>() == null)
            {
                PlaceCard(i, card);
                if (i == 0) { SetFirstAction(card.cardAbility.Actions); }
                else { AddToStagedAction(card.cardAbility.Actions); }
                return;
            }
        }
    }

    void PlaceCard(int index, NewCard card)
    {
        card.transform.SetParent(Positions[index].transform);
        card.transform.localScale = new Vector3(1, 1, 1);
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.identity;
        card.SetCurrentParent(card.transform.parent);
    }

    public void MakeAllCardsUnPlayable()
    {
        for (int i = Positions.Length - 1; i >= 0; i--)
        {
            if (Positions[i].GetComponentInChildren<NewCard>() != null)
            {
                Positions[i].GetComponentInChildren<NewCard>().SetUnPlayable();
            }
        }
    }
}
