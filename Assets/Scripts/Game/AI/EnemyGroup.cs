using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ActionSet
{
    public List<Action> Actions;
}

[System.Serializable]
public struct SpawnInterval
{
    public int amount;
    public int turn;
}

public class EnemyGroup : MonoBehaviour {

    public Sprite CharacterIcon;
    public string CharacterNameLinkedTo;
    public GameObject EnemyPrefab;
    public List<SpawnInterval> Spawns = new List<SpawnInterval>();

    public int MaxIdealRange = 2;
    public int MinIdealRange = 1;
    public List<ActionSet> EngagingActions = new List<ActionSet>();
    public List<ActionSet> AvailableActions = new List<ActionSet>();
    public List<ActionSet> DisengagingActions = new List<ActionSet>();
    public ActionSet CurrentActionSet;

    public List<EnemyCharacter> linkedCharacters = new List<EnemyCharacter>();

    public int currentCharacterIndex = 0;

    public int RandomCharacterIndex = 0;

    MyCameraController myCamera;

    public int TotalSpawning()
    {
        int total = 0;
        foreach (SpawnInterval spawn in Spawns)
        {
            total += spawn.amount;
        }
        return total;
    }
    public void SpawnForTurn(GameObject spawner, int turnNumber, List<Hex> SpawnHexes)
    {
        IEnumerator SpawnEnemy = Spawn(spawner, turnNumber, SpawnHexes);
        StartCoroutine(SpawnEnemy);
    }
    IEnumerator Spawn(GameObject spawner, int turnNumber, List<Hex> SpawnHexes)
    {
        foreach (SpawnInterval spawn in Spawns)
        {
            if (spawn.turn == turnNumber)
            {
                for (int i = 0; i < spawn.amount; i++)
                {
                    int total = SpawnHexes.Count;
                    for (int j = 0; i < total; i++)
                    {
                        int randIndex = Random.Range(0, SpawnHexes.Count);
                        Hex hex = SpawnHexes[randIndex];
                        if (hex.EntityHolding == null)
                        {
                            hex.AddSpawner(spawner, EnemyPrefab);
                            myCamera.SetTarget(hex.transform);
                            yield return new WaitForSeconds(.7f);
                            break;
                        }
                        else
                        {
                            SpawnHexes.Remove(hex);
                        }
                    }
                }
            }
        }
        FindObjectOfType<EnemyController>().Spawn();
    }

    // Use this for initialization
    private void Awake()
    {
        if (AvailableActions.Count == 0) { return; }
        CurrentActionSet = AvailableActions[Random.Range(0, AvailableActions.Count)];
    }

    public void SetNewAction()
    {
        if (AvailableActions.Count == 0) { return; }
        foreach(EnemyCharacter character in linkedCharacters)
        {
            character.ShowNewAction();
        }
    }

    public ActionSet GetRandomEngageActionSet()
    {
        int randomNum = Random.Range(0, EngagingActions.Count);
        return EngagingActions[randomNum];
    }

    public ActionSet GetRandomDisengageActionSet()
    {
        int randomNum = Random.Range(0, DisengagingActions.Count);
        return DisengagingActions[randomNum];
    }

    public ActionSet GetRandomActionSet(ActionSet previousSet)
    {
        List<ActionSet> sets = new List<ActionSet>();
        foreach(ActionSet set in AvailableActions)
        {
            sets.Add(set);
        }
        if (sets.Contains(previousSet))
        {
            sets.Remove(previousSet);
        }
        int randomNum = Random.Range(0, sets.Count);
        return sets[randomNum];
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
            character.resetShield(character.GetArmor());
            character.SetSummonSickness(false);
        }
    }
}
