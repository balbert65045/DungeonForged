using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCSSelectionButton : MonoBehaviour {

    public string CharacterName;
    public HPBar hpBar;
    public Text currentHealthText;
    public Text maxHealthText;

    public Button RestReviveButton;
    int health;
    int MaxHealth;

    public void FocusOnCharacter()
    {
        FindObjectOfType<CSCharacterGroupPanel>().HidePanel();
        FindObjectOfType<CCSCardPanel>().ShowCharacterCards(CharacterName);
    }

    public void SetHp(int currentHealth, int maxHealth)
    {
        health = currentHealth;
        MaxHealth = maxHealth;
        if (currentHealth == 0)
        {
            RestReviveButton.GetComponentInChildren<Text>().text = "Revive";
        }
        currentHealthText.text = currentHealth.ToString();
        maxHealthText.text = maxHealth.ToString();
        float percentage = (float)currentHealth / (float)maxHealth;
        hpBar.SetHP(percentage);
    }

    public void AddHealth()
    {
        if (health == 0) { health++; }
        else { health = Mathf.Clamp(health + 10, 0, MaxHealth); }
        currentHealthText.text = health.ToString();
        float percentage = (float)health / (float)MaxHealth;
        hpBar.SetHP(percentage);

        CardStorage cs = FindObjectOfType<NewGroupStorage>().GetStorageFromName(CharacterName);
        cs.CharacterCurrentHealth = health;

        FindObjectOfType<CharacterOverviewController>().RestorReviveHasHappened();
    }

    public void HideRestButton()
    {
        RestReviveButton.gameObject.SetActive(false);
    }
}
