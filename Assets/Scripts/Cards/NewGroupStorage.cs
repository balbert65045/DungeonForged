using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardStorage
{
    public PlayerCharacterType characterType;
    public string CharacterName;
    public int CharacterMaxHealth = 0;
    public int CharacterCurrentHealth = 0;
    public List<GameObject> CardsInDeck = new List<GameObject>();
    public void AddCardToDeck(GameObject Card) { CardsInDeck.Add(Card); }
    public void RemoveCardFromDeck(GameObject Card) { CardsInDeck.Remove(Card); }
}

public class NewGroupStorage : MonoBehaviour {

    public int LevelIndex = 0;
    public void IncrimentLevel() { LevelIndex++; }

    public CardStorage[] MyGroupCardStorage = new CardStorage[2];

    void Start()
    {
        index = 0;
    }

    int index = 0;
    public void CreateCharacterStorage(CSCharacter character)
    {
        if (index < MyGroupCardStorage.Length)
        {
            CardStorage CS = new CardStorage();
            CS.characterType = character.characterType;
            CS.CharacterName = character.Name;
            CS.CharacterMaxHealth = character.MaxHealth;
            CS.CharacterCurrentHealth = character.MaxHealth;
            MyGroupCardStorage[index] = CS;
            for(int i = 0; i < character.StartingCards.Count; i++)
            {
                CS.AddCardToDeck(character.StartingCards[i]);
            }
            index++;
        }
    }

    public void AddHealth(int amount, string characterName)
    {
        CardStorage character = GetStorageFromName(characterName);
        character.CharacterCurrentHealth = Mathf.Clamp(character.CharacterCurrentHealth + amount, 0, character.CharacterMaxHealth);
    }

    public CardStorage GetStorageFromName(string name)
    {
        for (int i = 0; i < 4; i++)
        {
            if (MyGroupCardStorage[i].CharacterName == name)
            {
                return MyGroupCardStorage[i];
            }
        }
        return null;
    }

    public void AddCardToStorage(string name, GameObject card)
    {
        CardStorage myStorage = GetStorageFromName(name);
        myStorage.AddCardToDeck(card);
    }

    public void RemoveCardToStorage(string name, GameObject card)
    {
        CardStorage myStorage = GetStorageFromName(name);
        myStorage.RemoveCardFromDeck(card);
    }

    public int GetCurrentHealth(string name)
    {
        for (int i = 0; i < MyGroupCardStorage.Length; i++)
        {
            if (MyGroupCardStorage[i].CharacterName == name)
            {
                return MyGroupCardStorage[i].CharacterCurrentHealth;
            }
        }
        return 0;
    }

    public int GetMaxHealth(string name)
    {
        for (int i = 0; i < MyGroupCardStorage.Length; i++)
        {
            if (MyGroupCardStorage[i].CharacterName == name)
            {
                return MyGroupCardStorage[i].CharacterMaxHealth;
            }
        }
        return 0;
    }

    public List<GameObject> GetDeck(string name)
    {
        for (int i = 0; i < MyGroupCardStorage.Length; i++)
        {
            if (MyGroupCardStorage[i].CharacterName == name)
            {
                return MyGroupCardStorage[i].CardsInDeck;
            }
        }
        return null;
    }
}
