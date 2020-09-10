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

        for (int i = 0; i < character.StartingCards.Count; i++)
        {
            int column = i % 4;
            int row = i / 4;
            float x = ((column * 180f) - 270);
            float y = (250 - (row * 230f));
            Card = Instantiate(character.StartingCards[i], DeckArea.transform);
            Card.transform.rotation = Quaternion.identity;
            Card.transform.localPosition = new Vector3(x, y, 0);
            Card.transform.localScale = new Vector3(0.6035785f, 0.6035785f, 2.385f);
        }
        //Description.text = character.Description;
        //Card = Instantiate(character.BasicAttackCard, BasicAttackArea.transform);
        //Card.transform.localScale = new Vector3(1.17f, 1.17f, 1.17f);
        //Card.transform.localPosition = Vector3.zero;
    }
}
