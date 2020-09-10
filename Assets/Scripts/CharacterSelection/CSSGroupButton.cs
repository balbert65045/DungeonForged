using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSSGroupButton : MonoBehaviour {


    public void ReturnToGroup()
    {
        FindObjectOfType<CSCharacterCardPanel>().HidePanel();
        FindObjectOfType<CSCharacterGroupPanel>().ShowPanel();
        FindObjectOfType<CSCharacterManager>().RevealAllOtherCharacters();
        FindObjectOfType<CharacterSelectionCamera>().ClearTarget();
    }
}
