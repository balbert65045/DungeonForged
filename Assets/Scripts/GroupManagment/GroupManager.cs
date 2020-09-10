using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour {

    public List<GameObject> CharactersPrefab = new List<GameObject>();
    public void SetCharacters(List<GameObject> newGroup) { CharactersPrefab = newGroup; }

    private void Awake()
    {
        GroupManager[] groupManagers = FindObjectsOfType<GroupManager>();
        if (groupManagers.Length > 1) { Destroy(groupManagers[1].gameObject); }
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (FindObjectOfType<HexMapController>() == null) { Cursor.visible = true; }
        if (FindObjectOfType<GameManager>() != null)
        {
            GroupManager[] groupManagers = FindObjectsOfType<GroupManager>();
            if (groupManagers.Length > 1 && groupManagers[1] == this) { return; }
            CreateLevel();
        }
        Time.timeScale = 1;
    }

    void CreateLevel()
    {
        List<GameObject> PlayerCharacters = new List<GameObject>();
        //PlayerCharacters.Add(CreateCharacterInstance(CharactersPrefab[0]));
        //PlayerCharacters.Add(CreateCharacterInstance(CharactersPrefab[1]));
        //PlayerCharacters.Add(CreateCharacterInstance(CharactersPrefab[2]));
        //PlayerCharacters.Add(CreateCharacterInstance(CharactersPrefab[3]));

        FindObjectOfType<GameManager>().BeginGame(CharactersPrefab);
        //CleanUpOldCharacters(PlayerCharacters);
    }

    //GameObject CreateCharacterInstance(GameObject characterPrefab)
    //{
    //    GameObject Character = Instantiate(characterPrefab);
    //    return Character;
    //}

}
