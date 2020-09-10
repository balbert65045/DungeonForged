using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCSGroupButton : MonoBehaviour {

    public void ReturnToGroup()
    {
        FindObjectOfType<CCSCardPanel>().HideCharacterCards();
        FindObjectOfType<CSCharacterGroupPanel>().ShowPanel();
    }
}
