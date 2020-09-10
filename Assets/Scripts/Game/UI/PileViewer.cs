using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PileViewer : CardPile
{

    public void LinkDrawPile(CardPileActioner cardPile)
    {
        CardsCurrentlyInPile = cardPile.CardsCurrentlyInPile;
        cardPile.notifyPileChangeObservers += UpdateText;
        UpdateText();
    }

    public void UpdateText()
    {
        CardPileNumber.text = CardsCurrentlyInPile.Count.ToString();
    }
}
