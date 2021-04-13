using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPile : CardPileActioner
{
    public void SetInitialDeck(List<GameObject> cards) {
        InitialDeck = cards;
        foreach (GameObject prefabCard in InitialDeck)
        {
            GameObject newCard = Instantiate(prefabCard, this.transform);
            CardsCurrentlyInPile.Add(newCard.GetComponent<NewCard>());
            newCard.SetActive(false);
        }
    }
    public List<GameObject> InitialDeck;

    public DiscardPile discardPile { get { return GetComponentInParent<PlayerDeck>().DiscardPile; } }

    public NewCard DrawCard()
    {
        if (CardsCurrentlyInPile.Count == 0) { PutDiscardIntoDrawPile(); }
        NewCard card = CardsCurrentlyInPile[Random.Range(0, CardsCurrentlyInPile.Count)];
        CardsCurrentlyInPile.Remove(card);
        SetText();
        return card;
    }

    void PutDiscardIntoDrawPile()
    {
        NewCard[] cards = discardPile.GetAllDiscardedCards();
        foreach(NewCard card in cards)
        {
            card.transform.SetParent(this.transform);
            card.transform.localPosition = Vector3.zero;
            CardsCurrentlyInPile.Add(card);
        }
        SetText();
        //GetComponentInParent<PlayerDeck>().AddExaustToDiscardPile();
    }

    public void AddCardToDrawPile(NewCard card)
    {
        CardsCurrentlyInPile.Add(card);
        SetText();
        card.transform.SetParent(this.transform);
        card.transform.localPosition = Vector3.zero;
        card.gameObject.SetActive(false);
    }
}
