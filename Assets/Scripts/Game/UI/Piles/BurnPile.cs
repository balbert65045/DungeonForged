using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnPile : CardPile
{
    public void BurnCard(NewCard card)
    {
        CardsCurrentlyInPile.Add(card);
        card.transform.SetParent(this.transform);
        card.InTheHand = false;
        card.Used = true;
        CardPileNumber.text = CardsCurrentlyInPile.Count.ToString();
    }
}
