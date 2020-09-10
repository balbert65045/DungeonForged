using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPileActioner : CardPile {

    public delegate void OnPileChange(); // declare new delegate type
    public event OnPileChange notifyPileChangeObservers;

    protected void SetText()
    {
        CardPileNumber.text = CardsCurrentlyInPile.Count.ToString();
        notifyPileChangeObservers();
    }
}
