using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCSCardAreaPanel : MonoBehaviour {

    public GameObject ContentPanel;
    public GameObject Pos;

    public List<GameObject> Positions;
    public int TotalCardsObtained = 0;
    

    public void ShowCards(string characterName)
    {
        TotalCardsObtained = 0;
        List<GameObject> CardsObtained = FindObjectOfType<NewGroupStorage>().GetDeck(characterName);
        AdjustContentSize(CardsObtained.Count);
        AddCardsToPanel(CardsObtained);
    }

    void AddCardsToPanel(List<GameObject> Cards)
    {
        foreach (GameObject pos in Positions) { Destroy(pos); }
        Positions.Clear();
        for (int i = 0; i < Cards.Count; i++)
        {
            AddCardToPanel(Cards[i], i);
        }
    }

    void AdjustContentSize(int cards)
    {
        int height = 550;
        int columns = (cards / 4) + 1;
        if (columns >= 3)
        {
            height += (700 + (columns - 3) * 300);
        }
        ContentPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    GameObject AddCardToPanel(GameObject prefab, int index)
    {
        GameObject newPos = Instantiate(Pos, ContentPanel.transform);
        int column = index % 4;
        int row = index / 4;
        float x = ((column * 240f) + 150);
        float y = (-200f - (row * 370f));
        //newPos.transform.localPosition = new Vector3(x, y, 0);
        Positions.Add(newPos);

        GameObject card = Instantiate(prefab, newPos.transform);
        card.transform.localPosition = Vector3.zero;
        card.transform.localScale = new Vector3(.85f, .85f, 2.385f);

        newPos.transform.localPosition = new Vector3(x, y, 0);
        TotalCardsObtained++;
        return card;
    }
}
