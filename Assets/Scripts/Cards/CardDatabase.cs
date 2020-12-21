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
        List<GameObject> CommonCardList = new List<GameObject>();
        List<GameObject> UncommonCardList = new List<GameObject>();
        List<GameObject> RareCardList = new List<GameObject>();

        foreach(GameObject cardObj in Cards)
        {
            switch (cardObj.GetComponent<NewCard>().CardRarity)
            {
                case Rarity.Common:
                    CommonCardList.Add(cardObj);
                    break;
                case Rarity.Uncommon:
                    UncommonCardList.Add(cardObj);
                    break;
                case Rarity.Rare:
                    RareCardList.Add(cardObj);
                    break;
            }
        }
        int randomRoll = Random.Range(0, 100);
        Debug.Log(randomRoll);
        GameObject card = null;
        if (randomRoll >= 0 && randomRoll < 60) { card = CommonCardList[Random.Range(0, CommonCardList.Count)]; }
        if (randomRoll >= 60 && randomRoll < 90) { card = UncommonCardList[Random.Range(0, UncommonCardList.Count)]; }
        if (randomRoll >= 90 && randomRoll < 100) { card = RareCardList[Random.Range(0, RareCardList.Count)]; }
        Cards.Remove(card);
        return card;
    }
}
