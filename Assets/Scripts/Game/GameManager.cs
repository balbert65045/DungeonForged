using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public bool DebugMap = false; 
    

    public void BeginGame(List<GameObject> PlayerCharacters)
    {
        //FindObjectOfType<ProceduralMapCreator>().BuildMapToStart(PlayerCharacters);
        FindObjectOfType<PlayerStartPos>().GetComponent<Hex>().EntityToSpawn = PlayerCharacters[0].GetComponent<Entity>();
        FindObjectOfType<PlayerStartPos>().GetComponent<Hex>().CreateCharacter();
        FindObjectOfType<HexMapController>().SetHexes();
        int totalEnemiesOut = FindObjectsOfType<EnemyCharacter>().Length;
        EnemyGroup[] enemyGroups = FindObjectsOfType<EnemyGroup>();
        foreach (EnemyGroup eg in enemyGroups)
        {
            totalEnemiesOut += eg.TotalSpawning();
        }
        FindObjectOfType<ObjectiveArea>().SetTotalEnemies(totalEnemiesOut);
        FindObjectOfType<PlayerController>().BeginGame();
    }

    public void StartGameWithMapMade()
    {
        FindObjectOfType<HexMapController>().SetHexes();
        FindObjectOfType<PlayerController>().BeginGame();
    }

    public void LevelComplete()
    {
        FindObjectOfType<TurnOrder>().ResetTurnNumber();
        CardStorage[] storages = FindObjectOfType<NewGroupStorage>().MyGroupCardStorage;
        PlayerCharacter[] characters = FindObjectsOfType<PlayerCharacter>();
        foreach(CardStorage storage in storages)
        {
            bool characterFound = false;
            foreach(PlayerCharacter character in characters)
            {
                if (storage.CharacterName == character.CharacterName)
                {
                    storage.CharacterCurrentHealth = character.health;
                    characterFound = true;
                    break;
                }
            }
            if (!characterFound)
            {
                storage.CharacterCurrentHealth = 0;
            }
        }
        FindObjectOfType<NewGroupStorage>().IncrimentLevel();
        FindObjectOfType<LevelManager>().LoadLevel("Map");
    }

    private void OnLevelWasLoaded(int level)
    {
    }

    private void Start()
    {
        //Debug.Log("Level Loaded");
        //if (FindObjectOfType<HexMapController>() == null) { Cursor.visible = true; }
        if (DebugMap) { StartGameWithMapMade(); }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
