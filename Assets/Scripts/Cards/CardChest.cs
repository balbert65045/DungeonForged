using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Combat,
    OutOfCombat,
}

public class CardChest : MonoBehaviour {

    public CardType myChestType;
    public Material HexOnMaterial;
    public bool isOpen = false;
    public string myCharacterName;
    PlayerCharacter myCharacter;

    public void OpenChest()
    {
    //    isOpen = true;
    //    FindObjectOfType<HexVisualizer>().DeactivateHex(GetComponent<Entity>().HexOn);
    //    if (myCharacter == null) { return; }
    //    GameObject[] cardPrefabs = FindObjectOfType<PlayerCardDatabase>().Select3RandomCards(myCharacter, myChestType);
    //    PlayerCharacter currentActivePlayer = FindObjectOfType<PlayerController>().SelectPlayerCharacter;
    //    //currentActivePlayer.myDecks.SetActive(false);
    //    //myCharacter.myDecks.SetActive(true);
    //    //if (myChestType == CardType.Combat)
    //    //{
    //    //    myCharacter.GetMyCombatHand().ShowHand();
    //    //    myCharacter.GetMyOutOfCombatHand().HideHand();
    //    //}
    //    //else
    //    //{
    //    //    myCharacter.GetMyCombatHand().HideHand();
    //    //    myCharacter.GetMyOutOfCombatHand().ShowHand();
    //    //}
    //    List<Card> cards = FindObjectOfType<ChestPanel>().ActivePanel(myCharacter, cardPrefabs[0], cardPrefabs[1], cardPrefabs[2], this);
    //    FindObjectOfType<PlayerController>().ChestOpenedFor(cards);
    }
    private void Start()
    {
        //if (myCharacter == null)
        //{
        //    SetCharacter(myCharacterName);
        //}
    }

    public void SetCharacter(string characterName)
    {
        //myCharacterName = characterName;
        //PlayerCharacter[] playerCharacters = FindObjectsOfType<PlayerCharacter>();
        //foreach (PlayerCharacter character in playerCharacters)
        //{
        //    if (character.CharacterName == myCharacterName)
        //    {
        //        myCharacter = character;
        //        GetComponentInChildren<ChestIcon>().SetIcon(myCharacter.characterSymbol);
        //    }
        //}
    }
}
