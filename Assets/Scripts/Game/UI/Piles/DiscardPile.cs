using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : CardPileActioner
{
    public DrawPile drawPile { get { return GetComponentInParent<PlayerDeck>().DrawPile; } }

    public NewCard[] GetAllDiscardedCards()
    {
        NewCard[] cards = CardsCurrentlyInPile.ToArray();
        CardsCurrentlyInPile.Clear();
        SetText();
        return cards;
    }

    public void DiscardCard(NewCard card)
    {
        CardsCurrentlyInPile.Add(card);
        card.SetPlayable();
        card.transform.SetParent(this.transform);
        card.Used = true;
        SetText();
    }
}
