using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterOverviewController : MonoBehaviour {

	void Start () {
        CCSSelectionButton[] Characters = FindObjectsOfType<CCSSelectionButton>();
        CardStorage[] storage = FindObjectOfType<NewGroupStorage>().MyGroupCardStorage;
        for(int i = 0; i < storage.Length; i++)
        {
            foreach(CCSSelectionButton characterButton in Characters)
            {
                if (characterButton.CharacterName == storage[i].CharacterName)
                {
                    characterButton.SetHp(storage[i].CharacterCurrentHealth, storage[i].CharacterMaxHealth);
                    break;
                }
            }
        }
	}
}
