using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestButton : MonoBehaviour
{
    public GameObject Character;
    public int RestAmount;
    public Text buttonText;

    private void Start()
    {
        int maxHealth = FindObjectOfType<NewGroupStorage>().MyGroupCardStorage[0].CharacterMaxHealth;
        RestAmount = (int)(maxHealth * .2f);
        buttonText.text = "Rest (+" + RestAmount.ToString() + ")";
    }

    public void Rest()
    {
        Character.GetComponent<Animator>().SetBool("Sit", true);
        string name = Character.GetComponent<CSCharacter>().Name;
        FindObjectOfType<CCSSelectionButton>().AddHealth(RestAmount);
        FindObjectOfType<NewGroupStorage>().AddHealth(RestAmount, name);
        FindObjectOfType<LevelManager>().LoadLevelWithDelay("Map", 1.5f);
        RestButton[] buttons = FindObjectsOfType<RestButton>();
        foreach (RestButton button in buttons) { button.gameObject.SetActive(false); }
    }
}
