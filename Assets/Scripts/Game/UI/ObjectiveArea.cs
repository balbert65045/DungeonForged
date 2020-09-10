using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveArea : MonoBehaviour {

    public HPBar hpBar;
    public Text ObjectiveText;
    public Text ObjectiveBarText;
    public int TotalEnemies;
    public int CurrentEnemies;

    public void SetTotalEnemies(int amount)
    {
        TotalEnemies = amount;
        CurrentEnemies = amount;
        ObjectiveText.text = "Defeat " + amount.ToString() + " Enemies";
        ObjectiveBarText.text = amount.ToString() + " / " + amount.ToString();
    }

    public void EnemyDied()
    {
        CurrentEnemies--;
        hpBar.SetHP((float)CurrentEnemies / (float)TotalEnemies);
        ObjectiveBarText.text = CurrentEnemies.ToString() + " / " + TotalEnemies.ToString();
        if (CurrentEnemies == 0)
        {
            FindObjectOfType<LevelClearedPanel>().TurnOnPanel();
        }
    }
}
