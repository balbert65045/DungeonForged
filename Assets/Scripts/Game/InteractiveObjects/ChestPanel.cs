using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestPanel : MonoBehaviour {

    public Text ReplaceText;

    public GameObject Mask;
    public GameObject Panel;
    public GameObject CardPosition1;
    public GameObject CardPosition2;
    public GameObject CardPosition3;

    public Button AddButton1;
    public Button AddButton2;
    public Button AddButton3;
    public Button ReplaceButton;
    public Button StoreButton;

    private PlayerCharacter CharacterOpeningChest;
    private GameObject cardFound1;
    private GameObject cardFound2;
    private GameObject cardFound3;
    GameObject card1;
    GameObject card2;
    GameObject card3;
    GameObject ChestCardSelected;

    private CardChest chestOpened;

    private Card cardSelectedForReplace;

    public List<Card> ActivePanel(PlayerCharacter Character, GameObject cardPrefab1, GameObject cardPrefab2, GameObject cardPrefab3, CardChest chest)
    {
        chestOpened = chest;
        CharacterOpeningChest = Character;
        //if (Character.OutOfActions()) { Character.GetMyOutOfCombatHand().RefeshActions(); }
        cardFound1 = cardPrefab1;
        cardFound2 = cardPrefab2;
        cardFound3 = cardPrefab3;
        Panel.SetActive(true);
        AddButton1.gameObject.SetActive(true);
        AddButton2.gameObject.SetActive(true);
        AddButton3.gameObject.SetActive(true);
        card1 = Instantiate(cardPrefab1, CardPosition1.transform);
        card2 = Instantiate(cardPrefab2, CardPosition2.transform);
        card3 = Instantiate(cardPrefab3, CardPosition3.transform);
        card1.transform.localPosition = Vector3.zero;
        card1.transform.localScale = new Vector3(.87f, .87f, 2.18f);
        card2.transform.localPosition = Vector3.zero;
        card2.transform.localScale = new Vector3(.87f, .87f, 2.18f);
        card3.transform.localPosition = Vector3.zero;
        card3.transform.localScale = new Vector3(.87f, .87f, 2.18f);

        List<Card> cards = new List<Card>{};
        cards.Add(card1.GetComponent<Card>());
        cards.Add(card2.GetComponent<Card>());
        cards.Add(card3.GetComponent<Card>());

        Mask.SetActive(true);

        //if (GetCorrectHand().CardsHolding.Count >= GetCorrectHand().handSize) { AddButton.interactable = false; }
        //ReplaceButton.interactable = false;
        return cards;
    }

    public void DeActivePanel()
    {
        Panel.SetActive(false);
    }

    public void ChestCardDeSelected(ChestCard card)
    {

    }

    public void CardSelected(Card card)
    {
        if (card != cardSelectedForReplace)
        {
            cardSelectedForReplace = card;
            ReplaceButton.interactable = true;
        }
        else
        {
            cardSelectedForReplace = null;
            ReplaceButton.interactable = false;
        }
    }

    public void Add(int index)
    {
        //ChestCardSelected = null;
        //switch (index)
        //{
        //    case 1:
        //        ChestCardSelected = cardFound1;
        //        break;
        //    case 2:
        //        ChestCardSelected = cardFound2;
        //        break;
        //    case 3:
        //        ChestCardSelected = cardFound3;
        //        break;
        //}
        ////if (GetCorrectHand().CardsHolding.Count >= GetCorrectHand().handSize)
        ////{
        ////    Destroy(card1);
        ////    Destroy(card2);
        ////    Destroy(card3);
        ////    ShowReplaceScreen();
        ////}
        //else
        //{
        //   // GetCorrectHand().AddCard(ChestCardSelected);
        //    //CharacterOpeningChest.AddCardToHand(ChestCardSelected);
        //    DoneOpeningChest();
        //}
    }

    void ShowReplaceScreen()
    {
        ReplaceText.gameObject.SetActive(true);
        ReplaceButton.gameObject.SetActive(true);
        ReplaceButton.interactable = false;
        StoreButton.gameObject.SetActive(true);
        AddButton1.gameObject.SetActive(false);
        AddButton2.gameObject.SetActive(false);
        AddButton3.gameObject.SetActive(false);

        card2 = Instantiate(ChestCardSelected, CardPosition2.transform);
        card2.transform.localPosition = Vector3.zero;
        card2.transform.localScale = new Vector3(.87f, .87f, 2.18f);
    }

    public void Replace()
    {
        //GetCorrectHand().ReplaceCard(ChestCardSelected, cardSelectedForReplace.gameObject);
        //GameObject CardReplacing = FindObjectOfType<PlayerCardDatabase>().FindCardInDatabase(cardSelectedForReplace);
       // CharacterOpeningChest.ReplacingCardInHand(ChestCardSelected, CardReplacing);
        DoneOpeningChest();
    }

    public void Store()
    {
        //CharacterOpeningChest.AddCardToBeStored(ChestCardSelected);
        DoneOpeningChest();
    }

    void DoneOpeningChest()
    {
        ReplaceText.gameObject.SetActive(false);
        ReplaceButton.gameObject.SetActive(false);
        StoreButton.gameObject.SetActive(false);
        if (CardPosition1.GetComponentInChildren<Card>() != null) { Destroy(CardPosition1.GetComponentInChildren<Card>().gameObject); }
        if (CardPosition2.GetComponentInChildren<Card>() != null) { Destroy(CardPosition2.GetComponentInChildren<Card>().gameObject); }
        if (CardPosition3.GetComponentInChildren<Card>() != null){ Destroy(CardPosition3.GetComponentInChildren<Card>().gameObject); }
        Panel.SetActive(false);
        Mask.SetActive(false);
        //if (CharacterOpeningChest.OutOfActions()) { CharacterOpeningChest.GetMyOutOfCombatHand().ActionsUsedForHand(); }
        //CharacterOpeningChest.myDecks.SetActive(false);
        FindObjectOfType<PlayerController>().AllowEndTurn();
    }

    //Hand GetCorrectHand()
    //{
    //    //if (cardFound1.GetComponent<CombatPlayerCard>() != null)
    //    //{
    //    //    return CharacterOpeningChest.GetMyCombatHand();
    //    //}
    //    //else if (cardFound1.GetComponent<OutOfCombatCard>() != null)
    //    //{
    //    //    return CharacterOpeningChest.GetMyOutOfCombatHand();
    //    //}
    //    return null;
    //}
}
