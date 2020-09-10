using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyAmount : MonoBehaviour {

    public Text EnergyText;
    public int StartEnergy = 4;
    public int CurrentEnergyAmount = 4;

    private void Awake()
    {
        StartEnergy = CurrentEnergyAmount;
    }

    public void RefreshEnergy()
    {
        CurrentEnergyAmount = StartEnergy;
        EnergyText.text = CurrentEnergyAmount.ToString();
    }

    public void LoseEnergy(int amount)
    {
        CurrentEnergyAmount -= amount;
        EnergyText.text = CurrentEnergyAmount.ToString();
    }

    public void AddEnergy(int amount)
    {
        CurrentEnergyAmount += amount;
        EnergyText.text = CurrentEnergyAmount.ToString();
    }
}
