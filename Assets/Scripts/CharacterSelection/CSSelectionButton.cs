using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSSelectionButton : MonoBehaviour {

    public GameObject Character;

    public void FocusOnCharacter()
    {
        FindObjectOfType<CSCharacterGroupPanel>().HidePanel();
        FindObjectOfType<CSCharacterCardPanel>().ShowPanel(Character.GetComponent<CSCharacter>());
        FindObjectOfType<CSCharacterManager>().HideAllOtherCharacters(Character);
        FindObjectOfType<CharacterSelectionCamera>().setTarget(Character.transform.position);
    }
}
