using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterOverviewController : MonoBehaviour {

    public CCSSelectionButton[] Characters;

	// Use this for initialization
	void Start () {
        CardStorage[] storage = FindObjectOfType<NewGroupStorage>().MyGroupCardStorage;
        for(int i = 0; i < storage.Length; i++)
        {
            Characters[i].SetHp(storage[i].CharacterCurrentHealth, storage[i].CharacterMaxHealth);
        }
        FindObjectOfType<LevelMap>().SetCharacterPosition(FindObjectOfType<NewGroupStorage>().LevelIndex);
	}

    public void RestorReviveHasHappened()
    {
        CCSSelectionButton[] ccsbuttons = FindObjectsOfType<CCSSelectionButton>();
        foreach(CCSSelectionButton ccsbutton in ccsbuttons)
        {
            ccsbutton.HideRestButton();
        }
    }
}
