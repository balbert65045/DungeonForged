using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsPanel : MonoBehaviour {

    public GameObject MaskPenel;
    public GameObject ContentPanel;
    GameObject OldParent;

    public void HideCards()
    {
        NewCard[] newCards = ContentPanel.GetComponentsInChildren<NewCard>();
        foreach(NewCard newCard in newCards)
        {
            Debug.Log("hiding new card");
            newCard.transform.SetParent(OldParent.transform);
            newCard.transform.localPosition = Vector3.zero;
            newCard.gameObject.SetActive(false);
        }
        MaskPenel.SetActive(false);
    }

    public void ShowCards(List<NewCard> cards, GameObject oldParent)
    {
        OldParent = oldParent;
        MaskPenel.SetActive(true);
        int index = 0;
        int height = 550;
        int columns = (cards.Count / 4) + 1;
        if (columns >= 3)
        {
            height += (150 + (columns - 3)*300);
        }
        ContentPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        foreach (NewCard card in cards)
        {
            card.gameObject.SetActive(true);
            AddCardToPanel(card, index);
            index++;
        }
    }

    void AddCardToPanel(NewCard card, int index)
    {
        int column = index % 4;
        int row = index / 4;
        float x = ((column * 180f) + 110);
        float y = (-120f - (row * 230f));

     //   card.ShowFront();
        card.transform.SetParent(ContentPanel.transform);
        card.transform.rotation = Quaternion.identity;
        card.transform.localPosition = new Vector3(x, y, 0);
        card.transform.localScale = new Vector3(0.6035785f, 0.6035785f, 2.385f);
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
