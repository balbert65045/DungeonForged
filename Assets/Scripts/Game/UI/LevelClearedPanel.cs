using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelClearedPanel : MonoBehaviour {

    public GameObject panel;
    public Button LootButton;
    public PlayerCharacter[] characters;
    public Image[] CharacterImages;
    int characterLootIndex = 0;

    // Use this for initialization
    void Start()
    {
        panel.SetActive(false);
        //StartCoroutine("DummyTurnOnPanel");
    }

    IEnumerator DummyTurnOnPanel()
    {
        yield return new WaitForSeconds(1f);
        TurnOnPanel();
    }


    public PlayerCharacter GetCurrentlootPlayer()
    {
        return characters[characterLootIndex];
    }

    public void TurnOnPanel()
    {
        FindObjectOfType<PlayersDecks>().gameObject.SetActive(false);
        panel.SetActive(true);
        characters = FindObjectsOfType<PlayerCharacter>();
        CharacterImages[0].sprite = characters[0].GetComponent<PlayerCharacter>().characterSymbol;
    }

    public void ShowNextLoot()
    {
        CharacterImages[characterLootIndex].transform.parent.GetComponent<Image>().color = Color.gray;
        characterLootIndex++;
        if (characterLootIndex >= characters.Length) {
            FindObjectOfType<PlayerController>().AddGold(20);
            FindObjectOfType<GameManager>().LevelComplete();
            return;
        }
        StartCoroutine("ShowingNextLoot");
    }

    IEnumerator ShowingNextLoot()
    {
        yield return new WaitForSeconds(.5f);
        ShowLoot();
    }

    public void ShowLoot()
    {
        LootButton.interactable = false;
        GetComponentInChildren<CardLoot>().ShowThreeCards(FindObjectOfType<NewGroupStorage>().MyGroupCardStorage[0].characterType);
    }
}
