using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelClearedPanel : MonoBehaviour {

    public GameObject panel;
    public Button LootButton;
    public List<GameObject> characters;
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
        return characters[characterLootIndex].GetComponent<PlayerCharacter>();
    }

    public void TurnOnPanel()
    {
        FindObjectOfType<PlayersDecks>().gameObject.SetActive(false);
        panel.SetActive(true);
        characters = FindObjectOfType<ProceduralMapCreator>().PlayerCharacters;
        for(int i = 0; i < 2; i++)
        {
            CharacterImages[i].sprite = characters[i].GetComponent<PlayerCharacter>().characterSymbol;
        }
    }

    public void ShowNextLoot()
    {
        CharacterImages[characterLootIndex].transform.parent.GetComponent<Image>().color = Color.gray;
        characterLootIndex++;
        if (characterLootIndex >= characters.Count) {
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
        GetComponentInChildren<CardLoot>().ShowThreeCards(characters[characterLootIndex].GetComponent<PlayerCharacter>().myType);
    }
}
