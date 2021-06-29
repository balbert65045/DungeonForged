using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOrder : MonoBehaviour {

    public GameObject DeleteSpot;
  //  public GameObject CreateSpot;
    public GameObject ActiveSpot;
    public List<GameObject> OtherSpots;
    public List<GameObject> ActiveOtherSpots;

    public int TurnNumber = 0;
    public void ResetTurnNumber() { TurnNumber = 0; }

    public int characterIndex = 0;
    public int LastCharacterIndex = 0;
    public List<Entity> CharactersInCombat;
    public Entity GetCurrentCharacter() { return CharactersInCombat[characterIndex]; }

    public GameObject CharacterTurnIndicatorPrefab;
    public GameObject NewTurnPrefab;

    int PlayerCharacters()
    {
        int pcs = 0;
        foreach(Entity character in CharactersInCombat)
        {
            if (character.IsPlayer()) { pcs++; }
        }
        return pcs;
    }

    void OrderCharacters()
    {
        Entity[] myEntities = CharactersInCombat.ToArray();
        CharactersInCombat.Clear();
        List<int> indexesOfNonCharacters = new List<int>();
        for (int i = 0; i < myEntities.Length; i++)
        {
            if (myEntities[i].GetComponent<Character>() != null)
            {
                CharactersInCombat.Add(myEntities[i]);
            }
            else
            {
                indexesOfNonCharacters.Add(i);
            }
        }
        foreach(int index in indexesOfNonCharacters)
        {
            CharactersInCombat.Add(myEntities[index]);
        }
    }

    void CreateInitialIndicators()
    {
        OrderCharacters();
        SetSizeAndAmountOfSpots();
        LastCharacterIndex = characterIndex;
        if (LastCharacterIndex > CharactersInCombat.Count) { LastCharacterIndex--; }
        DestroyPreviousIndicators();
        for (int i = 0; i < ActiveOtherSpots.Count + 1; i++)
        {
            Entity character = CharactersInCombat[LastCharacterIndex];
            CharacterTurnIndicator newIndicator;
            if (i == 0) { newIndicator = CreateNewTurnIndicator(ActiveSpot, character); }
            else { newIndicator = CreateNewTurnIndicator(ActiveOtherSpots[i - 1], character); }
            if (character == CharactersInCombat[CharactersInCombat.Count - 1] && i != ActiveOtherSpots.Count)
            {
                CreateNewTurnSpot(newIndicator);
            }
            if (character.HasTimer) 
            {
                newIndicator.SetTurnsLeft(character.TurnsLeft);
            }
            IncrimentLastCharacterIndex();
        }
    }

    void CreateNewTurnSpot(CharacterTurnIndicator indicator)
    {
        if (GetComponentInChildren<NewTurnIndicator>() != null) { Destroy(GetComponentInChildren<NewTurnIndicator>().gameObject); }
        GameObject newTurn = Instantiate(NewTurnPrefab, indicator.transform);
        newTurn.transform.localPosition = new Vector3(32, 0, 0);
    }

    void SetSizeAndAmountOfSpots()
    {
        ActiveOtherSpots.Clear();
        int characterCount = CharactersInCombat.Count;
        float xVal = 250f - (25f * (characterCount - 1));
        transform.localPosition = new Vector3(xVal, transform.localPosition.y, transform.localPosition.z);
        for (int i = 0; i < OtherSpots.Count; i++)
        {
            if (i < characterCount)
            {
                ActiveOtherSpots.Add(OtherSpots[i]);
            }
        }
    }

    void DestroyPreviousIndicators()
    {
        CharacterTurnIndicator[] indicators = GetComponentsInChildren<CharacterTurnIndicator>();
        foreach(CharacterTurnIndicator indicator in indicators) { Destroy(indicator.gameObject); }
    }

    void IncrimentCharacterIndex()
    {
        characterIndex ++;
        characterIndex = characterIndex >= CharactersInCombat.Count ? characterIndex % CharactersInCombat.Count : characterIndex;
    }

    void IncrimentLastCharacterIndex()
    {
        LastCharacterIndex++;
        LastCharacterIndex = LastCharacterIndex >= CharactersInCombat.Count ? LastCharacterIndex % CharactersInCombat.Count : LastCharacterIndex;
    }

    public void EndTurn()
    {
        CharacterTurnIndicator AIndicator = ActiveSpot.GetComponentInChildren<CharacterTurnIndicator>();
        AIndicator.transform.SetParent(DeleteSpot.transform);
        AIndicator.ShrinkAndDestroy();
        AIndicator.SetShift();
        for (int i = 0; i < ActiveOtherSpots.Count; i++)
        {
            CharacterTurnIndicator Indicator = ActiveOtherSpots[i].GetComponentInChildren<CharacterTurnIndicator>();
            if (i == ActiveOtherSpots.Count - 1 && characterIndex == CharactersInCombat.Count - 1) { CreateNewTurnSpot(Indicator); }
            if (i == 0) {
                Indicator.transform.SetParent(ActiveSpot.transform);
                Indicator.Grow();
            }
            else { Indicator.transform.SetParent(ActiveOtherSpots[i - 1].transform); }
            Indicator.SetShift();
        }
        Entity character = CharactersInCombat[LastCharacterIndex];
        if (!character.IsEnemySpawner())
        {
            CharacterTurnIndicator NewIndicator = CreateNewTurnIndicator(OtherSpots[ActiveOtherSpots.Count], character);
            NewIndicator.transform.SetParent(OtherSpots[ActiveOtherSpots.Count - 1].transform);
            if (character.HasTimer) { NewIndicator.SetTurnsLeft(character.TurnsLeft); }
            NewIndicator.SetShift();
        }
        IncrimentCharacterIndex();
        IncrimentLastCharacterIndex();
        if (GetCurrentCharacter().IsPlayer()) { TurnNumber++; }
    }

    CharacterTurnIndicator CreateNewTurnIndicator(GameObject parent, Entity character)
    {
        GameObject IndicatorObj = Instantiate(CharacterTurnIndicatorPrefab, parent.transform);
        CharacterTurnIndicator Indicator = IndicatorObj.GetComponent<CharacterTurnIndicator>();
        Indicator.SetCharacter(character);
        Indicator.SetPosition();
        if (parent == ActiveSpot) { Indicator.SetAsActivePositionSize(); }
        return Indicator;
    }

    int GetCharacterCharacterIndex(Entity character)
    {
        for (int i = 0; i < CharactersInCombat.Count; i++)
        {
            if (CharactersInCombat[i] == character) { return i; }
        }
        return 0;
    }

    public void SetTurnsLeft(Entity character)
    {
        ActiveSpot.GetComponentInChildren<CharacterTurnIndicator>().ReduceTurns();
        ActiveOtherSpots[ActiveOtherSpots.Count - 1].GetComponentInChildren<CharacterTurnIndicator>().ReduceTurns();
    }

    public void CharacterRemoved(Entity character)
    {
        int deadCharacterIndex = GetCharacterCharacterIndex(character);
        CharactersInCombat.Remove(character);
        if (characterIndex == CharactersInCombat.Count || deadCharacterIndex <= characterIndex) { characterIndex--; }
        CreateInitialIndicators();
    }

    public void ChestRemoved(Entity character)
    {
        int deadCharacterIndex = GetCharacterCharacterIndex(character);
        CharactersInCombat.Remove(character);
        characterIndex = 0;
        CreateInitialIndicators();
    }


    public void AddCharacter(Entity character)
    {
        if (character.IsPlayer()) { CharactersInCombat.Insert(PlayerCharacters(), character);}
        else { CharactersInCombat.Add(character); }
        CreateInitialIndicators();
    }

    public void NewCharacter(Entity character)
    {
        CharactersInCombat[characterIndex] = character;
        CharacterTurnIndicator NewIndicator = CreateNewTurnIndicator(OtherSpots[ActiveOtherSpots.Count - 1], character);
    }
}
