using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCardViewer : MonoBehaviour {

    public Image image;
    public string CharacterName;
    public PileViewer DrawPileviewer;
    public PileViewer DiscardPileviewer;

    public void SetUpCharacter(PlayerCharacter character)
    {
        image.sprite = character.characterIcon;
        CharacterName = character.CharacterName;
        DrawPileviewer.LinkDrawPile(character.MyNewDeck.GetComponent<PlayerDeck>().DrawPile);
        DiscardPileviewer.LinkDrawPile(character.MyNewDeck.GetComponent<PlayerDeck>().DiscardPile);
    } 
}
