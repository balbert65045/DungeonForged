using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCSCardPanel : MonoBehaviour {

    public GameObject Panel;
    public Text NameText;

    string CharacterShowing = null;

    public string GetCharacterName() { return NameText.text; }

    public void ShowCharacterCards(string characterName)
    {
        if (CharacterShowing == characterName)
        {
            Panel.SetActive(false);
            CharacterShowing = null;
        }
        else
        {
            CharacterShowing = characterName;
            Panel.SetActive(true);
            NameText.text = characterName;
            GetComponentInChildren<CCSCardAreaPanel>().ShowCards(characterName);
        }
    }

    public void HideCharacterCards()
    {
        Panel.SetActive(false);
        CharacterShowing = null;
    }
}
