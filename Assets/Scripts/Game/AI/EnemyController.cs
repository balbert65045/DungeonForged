using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public EnemyGroup[] enemyGroups;
    public GameObject EnemySpawner;
    public List<Hex> SpawnHexes;
    MyCameraController myCamera;
    TurnOrder turnOrder;
    public bool GameInPlay = false;

    public GameObject MoveArea;
    public GameObject OtherArea;

    int enemyGroupIndex = 0;

    public void CreateMoveArea(List<Vector3> points, ActionType type)
    {
        MoveArea.GetComponentInChildren<MeshGenerator>().CreateMesh(points);
        MoveArea.GetComponentInChildren<MeshGenerator>().SetCurrentMaterial(type);
        MoveArea.GetComponentInChildren<EdgeLine>().CreateLine(points.ToArray());
        MoveArea.GetComponentInChildren<EdgeLine>().SetCurrentMaterial(type);
    }

    public void CreateOtherArea(List<Vector3> points, ActionType type)
    {
        OtherArea.GetComponentInChildren<MeshGenerator>().CreateMesh(points);
        OtherArea.GetComponentInChildren<MeshGenerator>().SetCurrentMaterial(type);
        OtherArea.GetComponentInChildren<EdgeLine>().CreateLine(points.ToArray());
        OtherArea.GetComponentInChildren<EdgeLine>().SetCurrentMaterial(type);
    }

    public void RemoveAreas()
    {
        MoveArea.GetComponentInChildren<MeshGenerator>().DeleteMesh();
        MoveArea.GetComponentInChildren<EdgeLine>().DestroyLine();
        OtherArea.GetComponentInChildren<MeshGenerator>().DeleteMesh();
        OtherArea.GetComponentInChildren<EdgeLine>().DestroyLine();
    }

    public void DoEnemyActions()
    {
        GameInPlay = true;
        if (turnOrder.GetCurrentCharacter().IsPlayer())
        {
            foreach (EnemyGroup eg in enemyGroups)
            {
                eg.SetNewAction();
            }
            Spawn();
        }
        else if (turnOrder.GetCurrentCharacter().IsEnemySpawner())
        {
            turnOrder.GetCurrentCharacter().GetComponent<EnemySpawner>().Spawn();
        }
        else
        {
            StartCoroutine("DoNextEnemyCharacterAction", turnOrder.GetCurrentCharacter().GetComponent<EnemyCharacter>());
        }
    }

    IEnumerator DoNextEnemyCharacterAction(EnemyCharacter character)
    {
        myCamera.SetTarget(character.transform);
        yield return new WaitForSeconds(.5f);
        character.PerformActionSet();
    }

    int spawnGroupIndex = 0;
    public void Spawn()
    {
        int index = spawnGroupIndex;
        if (spawnGroupIndex == enemyGroups.Length) {
            spawnGroupIndex = 0;
            FindObjectOfType<PlayerController>().AllowNewTurns();
            return;
        }
        spawnGroupIndex++;
        enemyGroups[index].SpawnForTurn(EnemySpawner, turnOrder.TurnNumber, SpawnHexes);
    }

    public void CharacterEndedTurn()
    {
        turnOrder.EndTurn();
        if (FindObjectsOfType<PlayerCharacter>().Length == 0) { return; }
        if (turnOrder.GetCurrentCharacter().IsPlayer())
        {
            foreach (EnemyGroup eg in enemyGroups)
            {
                eg.SetNewAction();
            }
            Spawn();
        }
        else if (turnOrder.GetCurrentCharacter().IsEnemySpawner())
        {
            turnOrder.GetCurrentCharacter().GetComponent<EnemySpawner>().Spawn();
        }
        else
        {
            EnemyCharacter CurrentCharacter = turnOrder.GetCurrentCharacter().GetComponent<EnemyCharacter>();
            if (CurrentCharacter == null) { Debug.Log("Issue 1"); }
            StartCoroutine("DoNextEnemyCharacterAction", CurrentCharacter);
        } 
    }

    public void StartFirstActions()
    {
        EnemyCharacter[] characters = FindObjectsOfType<EnemyCharacter>();
        foreach(EnemyCharacter character in characters)
        {
            character.ShowNewAction();
        }
    }

    void Awake()
    {
        enemyGroups = GetComponentsInChildren<EnemyGroup>();
        myCamera = FindObjectOfType<MyCameraController>();
        turnOrder = FindObjectOfType<TurnOrder>();
    }

    public EnemyGroup GetGroupFromCharacter(EnemyCharacter character)
    {
        foreach (EnemyGroup group in enemyGroups)
        {
            if (group.linkedCharacters.Contains(character)) { return group; }
        }
        return null;
    }

    public void LinkSpawnedCharacter(EnemyCharacter character)
    {
        foreach (EnemyGroup group in enemyGroups)
        {
            if (group.CharacterNameLinkedTo == character.CharacterName)
            {
                group.LinkCharacterToGroup(character);
            }
        }
    }
}
