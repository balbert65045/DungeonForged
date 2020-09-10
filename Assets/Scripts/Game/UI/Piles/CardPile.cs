using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardPile : MonoBehaviour, IPointerDownHandler
{
    public List<NewCard> CardsCurrentlyInPile = new List<NewCard>();
    public Text CardPileNumber;

    public void OnPointerDown(PointerEventData eventData)
    {
        FindObjectOfType<CardsPanel>().ShowCards(CardsCurrentlyInPile, this.gameObject);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
