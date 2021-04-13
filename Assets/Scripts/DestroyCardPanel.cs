using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCardPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string characterName = FindObjectOfType<NewGroupStorage>().MyGroupCardStorage[0].CharacterName;
        GetComponentInChildren<CardSelectionPanel>().ShowCards(characterName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
