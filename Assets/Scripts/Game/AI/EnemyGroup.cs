using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ActionSet
{
    public List<Action> Actions;
}

public class EnemyGroup : MonoBehaviour {

    public Sprite CharacterIcon;
    public string CharacterNameLinkedTo;

    public List<ActionSet> AvailableActions = new List<ActionSet>();
    public ActionSet CurrentActionSet;

    public List<EnemyCharacter> linkedCharacters = new List<EnemyCharacter>();

    public int currentCharacterIndex = 0;

    public int RandomCharacterIndex = 0;

    MyCameraController myCamera;

    // Use this for initialization
    private void Awake()
    {
        if (AvailableActions.Count == 0) { return; }
        CurrentActionSet = AvailableActions[Random.Range(0, AvailableActions.Count)];
    }

    public void SetNewAction()
    {
        if (AvailableActions.Count == 0) { return; }
        CurrentActionSet = AvailableActions[Random.Range(0, AvailableActions.Count)];
        foreach(EnemyCharacter character in linkedCharacters)
        {
            character.ShowNewAction();
        }
    }

    void Start()
    {
        myCamera = FindObjectOfType<MyCameraController>();
    }

    public void selectRandomCharacter()
    {
        if (hasCharactersOut())
        {
            FindObjectOfType<HexVisualizer>().UnhighlightHexes();
            if (RandomCharacterIndex >= linkedCharacters.Count) { RandomCharacterIndex = 0; }
            EnemyCharacter character = linkedCharacters[RandomCharacterIndex];
            RandomCharacterIndex++;
            FindObjectOfType<HexVisualizer>().HighlightSelectionHex(character.HexOn);
            FindObjectOfType<MyCameraController>().UnLockCamera();
            FindObjectOfType<MyCameraController>().LookAt(character.transform);
        }
    }

    public bool hasCharactersOut()
    {
        return linkedCharacters.Count != 0;
    }

    public void LinkCharacterToGroup(EnemyCharacter character)
    {
        if (!linkedCharacters.Contains(character)) { linkedCharacters.Add(character); }
    }

    public void UnLinkCharacterToGroup(EnemyCharacter character)
    {
        linkedCharacters.Remove(character);
    }

    public void takeAwayBuffs()
    {
        foreach (Character character in linkedCharacters)
        {
            character.DecreaseBuffsDuration();
            character.resetShield(character.GetArmor());
            character.SetSummonSickness(false);
        }
    }
}
