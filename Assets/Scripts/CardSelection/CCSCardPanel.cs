using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCSCardPanel : MonoBehaviour {

    public GameObject Panel;
    public Text NameText;

    public string GetCharacterName() { return NameText.text; }

    public void ShowCharacterCards(string characterName)
    {
        Panel.SetActive(true);
        NameText.text = characterName;
        GetComponentInChildren<CCSCardAreaPanel>().ShowCards(characterName);
    }

    public void HideCharacterCards()
    {
        Panel.SetActive(false);
    }
}
