using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewHand : MonoBehaviour {

    public GameObject[] Positions;

    public DrawPile drawPile { get { return GetComponentInParent<PlayerDeck>().DrawPile; } }
    public DiscardPile discardPile { get { return GetComponentInParent<PlayerDeck>().DiscardPile; } }
    public BurnPile burnPile { get { return GetComponentInParent<PlayerDeck>().BurnPile; } }
    public EnergyAmount energyAmount { get { return GetComponentInParent<PlayerDeck>().EnergyAmount; } }

    public bool hiding = false;
    Vector3 MovingPosition;

    private void Start()
    {
        MovingPosition = transform.localPosition;
    }

    private void Update()
    {
        if ((MovingPosition - transform.localPosition).magnitude > .1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, MovingPosition, .2f);
        }
    }

    public void ShowHand()
    {
        if (hiding == true)
        {
            hiding = false;
            MovingPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 200f);
        }
    }

    public void HideHand()
    {
        if (hiding == false)
        {
            hiding = true;
            MovingPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 200f);
        }
    }

    public void DrawNewHand()
    {
        energyAmount.RefreshEnergy();
        DrawCards(5);
    }

    public void DrawCards(int amount)
    {
        StartCoroutine("Drawing", amount);
    }

    IEnumerator Drawing(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            DrawCard();
            yield return new WaitForSeconds(.1f);
        }
        yield return null;
    }

    public void DiscardHand()
    {
        NewCard[] cards = GetComponentsInChildren<NewCard>();
        foreach(NewCard card in cards)
        {
            discardPile.DiscardCard(card);
        }
    }

    void DrawCard()
    {
        if (GetComponentsInChildren<NewCard>().Length >= 9) { return; }
        NewCard card = drawPile.DrawCard();
        card.gameObject.SetActive(true);
        PlaceCardOnNextAvailableSpot(card);
    }

    public void PutCardInStaging(NewCard card)
    {
        energyAmount.LoseEnergy(card.EnergyAmount);
        FindObjectOfType<StagingArea>().PlaceCardOnNextAvailableSpot(card);
        ShiftHand();
    }

    public void UseCard(NewCard card)
    {
        energyAmount.LoseEnergy(card.EnergyAmount);
        FindObjectOfType<PlayerController>().UseUnstagedAction(card.cardAbility.Actions[0]);
        if (card.cardAbility.LostAbility)
        {
            burnPile.BurnCard(card);
        }
        else
        {
            discardPile.DiscardCard(card);
        }
        ShiftHand();
    }

    public void MakeAllCardsUnPlayable()
    {
        NewCard[] cards = GetComponentsInChildren<NewCard>();
        foreach (NewCard card in cards)
        {
            card.SetUnPlayable();
        }
    }

    public void MakeAllCardsPlayable()
    {
        NewCard[] cards = GetComponentsInChildren<NewCard>();
        foreach (NewCard card in cards)
        {
            card.SetPlayable();
        }
    }

    public void ShiftHand()
    {
        for (int i = 4; i < Positions.Length - 1; i++)
        {
            if (Positions[i].GetComponentInChildren<NewCard>() == null)
            {
                if (Positions[i + 1].GetComponentInChildren<NewCard>() != null)
                {
                    NewCard card = Positions[i + 1].GetComponentInChildren<NewCard>();
                    PlaceCard(i, card);
                }
            }
        }

        for(int i = 3; i > 0; i--)
        {
            if (Positions[i].GetComponentInChildren<NewCard>() == null)
            {
                if (Positions[i - 1].GetComponentInChildren<NewCard>() != null)
                {
                    NewCard card = Positions[i - 1].GetComponentInChildren<NewCard>();
                    PlaceCard(i, card);
                }
            }
        }
    }

    public void PlaceCardOnNextAvailableSpot(NewCard card)
    {
        for (int i = 0; i < Positions.Length; i++)
        {
            int index = 0;
            if (i == 0) { index = 4; }
            if (i == 1) { index = 5; }
            if (i == 2) { index = 3; }
            if (i == 3) { index = 6; }
            if (i == 4) { index = 2; }
            if (i == 5) { index = 7; }
            if (i == 6) { index = 1; }
            if (i == 7) { index = 0; }
            if (i == 8) { index = 8; }
            if (Positions[index].GetComponentInChildren<NewCard>() == null)
            {
                PlaceCard(index, card);
                return;
            }
        }
    }

    void PlaceCard(int index, NewCard card)
    {
        card.transform.SetParent(Positions[index].transform);
        card.Returning = true;
        card.transform.localRotation = Quaternion.identity;
        card.SetCurrentParent(card.transform.parent);
        card.transform.localScale = new Vector3(1, 1, 1);
    }
}
