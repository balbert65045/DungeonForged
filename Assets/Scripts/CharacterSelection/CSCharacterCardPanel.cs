using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSCharacterCardPanel : MonoBehaviour {

    public GameObject Panel;
    public Text Name;
    public Text Description;
    public GameObject DeckArea;

    public GameObject Card;
    public GameObject CardAmountPrefab;

    public void HidePanel()
    {
        NewCard[] cards = GetComponentsInChildren<NewCard>();
        foreach(NewCard card in cards)
        {
            Destroy(card.gameObject);
        }
        //if (Card != null) { Destroy(Card); }
        Panel.SetActive(false);
    }

    public void ShowPanel(CSCharacter character)
    {
        Panel.SetActive(true);
        Name.text = character.Name;

        int amountOfCards = 1;
        GameObject LastCard = null;
        GameObject LastCardAmount = null;
        int index = 0;
        for (int i = 0; i < character.StartingCards.Count; i++)
        {
            if (LastCard == character.StartingCards[i]) {
                amountOfCards++;
                LastCardAmount.GetComponentInChildren<Text>().text = "x" + amountOfCards.ToString();
                continue; 
            }
            amountOfCards = 1;
            LastCard = character.StartingCards[i];
            int column = index % 3;
            int row = index / 3;
            float x = ((column * 235f) - 230);
            float y = (220 - (row * 350f));
            Card = Instantiate(character.StartingCards[i], DeckArea.transform);
            Card.transform.rotation = Quaternion.identity;
            Card.transform.localPosition = new Vector3(x, y, 0);
            Card.transform.localScale = new Vector3(.8f, .8f, 2.385f);

            LastCardAmount = Instantiate(CardAmountPrefab, DeckArea.transform);
            LastCardAmount.transform.localPosition = new Vector3(x, y -150f, 0);
            LastCardAmount.transform.SetAsFirstSibling();
            LastCardAmount.GetComponentInChildren<Text>().text = "x1";
            index++;
        }
        //Description.text = character.Description;
        //Card = Instantiate(character.BasicAttackCard, BasicAttackArea.transform);
        //Card.transform.localScale = new Vector3(1.17f, 1.17f, 1.17f);
        //Card.transform.localPosition = Vector3.zero;
    }
}
