using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreController : MonoBehaviour
{

    public GameObject[] Character1CardsPositions;
    public GameObject[] Character2CardsPositions;

    public void MakeAvailableCards()
    {
        NewGroupStorage groupManager = FindObjectOfType<NewGroupStorage>();
        CardDatabase cardDatabase = FindObjectOfType<CardDatabase>();
        GameObject[] newCards1 = cardDatabase.Select3RandomCards(groupManager.MyGroupCardStorage[0].characterType);
        string character1 = groupManager.MyGroupCardStorage[0].CharacterName;
        PlaceCards(newCards1, Character1CardsPositions, character1);
        GameObject[] newCards2 = cardDatabase.Select3RandomCards(groupManager.MyGroupCardStorage[1].characterType);
        string character2 = groupManager.MyGroupCardStorage[1].CharacterName;
        PlaceCards(newCards2, Character2CardsPositions, character2);
    }

    void PlaceCards(GameObject[] cards, GameObject[] positions, string name)
    {
        for(int i = 0; i < 3; i++)
        {
            GameObject Card = Instantiate(cards[i], positions[i].transform);
            Card.GetComponent<NewCard>().PrefabAssociatedWith = cards[i];
            Card.transform.localPosition = Vector3.zero;
            Card.transform.localScale = new Vector3(1.9f, 1.9f, 1.9f);
            positions[i].GetComponent<StoreSpot>().CharacterNameFor = name;
            positions[i].GetComponent<StoreSpot>().SetPrice(Card.GetComponent<NewCard>().price);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MakeAvailableCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
