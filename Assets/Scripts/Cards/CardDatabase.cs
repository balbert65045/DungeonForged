using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour {


    public GameObject[] KnightCards;
    public GameObject[] BarbarianCards;
    public GameObject[] HuntressCards;
    public GameObject[] MageCards;

    private void Awake()
    {
        CardDatabase[] cardDatabases = FindObjectsOfType<CardDatabase>();
        if (cardDatabases.Length > 1) { Destroy(cardDatabases[1].gameObject); }
        DontDestroyOnLoad(this.gameObject);
    }

    public GameObject[] Select3RandomCards(PlayerCharacterType characterType)
    {
        List<GameObject> CardsList = new List<GameObject>();
        switch (characterType)
        {
            case PlayerCharacterType.Knight:
                CardsList = new List<GameObject>(KnightCards);
                break;
            case PlayerCharacterType.Barbarian:
                CardsList = new List<GameObject>(BarbarianCards);
                break;
            case PlayerCharacterType.Crossbow:
                CardsList = new List<GameObject>(HuntressCards);
                break;
            case PlayerCharacterType.Mage:
                CardsList = new List<GameObject>(MageCards);
                break;
        }
        GameObject Card1 = SelectRandomCard(CardsList);
        GameObject Card2 = SelectRandomCard(CardsList);
        GameObject Card3 = SelectRandomCard(CardsList);

        GameObject[] cards = new GameObject[3] { Card1, Card2, Card3 };
        return cards;
    }

    public GameObject SelectRandomCard(List<GameObject> Cards)
    {
        GameObject card = Cards[Random.Range(0, Cards.Count)];
        Cards.Remove(card);
        return card;
    }
}
