using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestButton : MonoBehaviour
{
    public GameObject Character;
    public int RestAmount;

    public void Rest()
    {
        Character.GetComponent<Animator>().SetBool("Sit", true);
        string name = Character.GetComponent<CSCharacter>().Name;
        CCSSelectionButton[] cssb = FindObjectsOfType<CCSSelectionButton>();
        foreach(CCSSelectionButton healthdisplay in cssb)
        {
            if (healthdisplay.CharacterName == name)
            {
                healthdisplay.AddHealth(RestAmount);
                break;
            }
        }
        FindObjectOfType<NewGroupStorage>().AddHealth(RestAmount, name);
        FindObjectOfType<LevelManager>().LoadLevelWithDelay("CardSelection", 1.5f);
        RestButton[] buttons = FindObjectsOfType<RestButton>();
        foreach (RestButton button in buttons) { button.gameObject.SetActive(false); }
    }
}
