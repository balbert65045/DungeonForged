using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCSSelectionButton : MonoBehaviour {

    public string CharacterName;
    public Text currentHealthText;
    public Text maxHealthText;

    int health;
    int MaxHealth;

    public void FocusOnCharacter()
    {
        FindObjectOfType<CCSCardPanel>().ShowCharacterCards(CharacterName);
    }

    public void SetHp(int currentHealth, int maxHealth)
    {
        health = currentHealth;
        MaxHealth = maxHealth;
        currentHealthText.text = currentHealth.ToString();
        maxHealthText.text = maxHealth.ToString();
    }

    public void AddHealth(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, MaxHealth);
        currentHealthText.text = health.ToString();
    }
}
