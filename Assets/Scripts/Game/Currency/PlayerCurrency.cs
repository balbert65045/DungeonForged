using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCurrency : MonoBehaviour {

    public Text GoldText;

    private void Start()
    {
        SetGoldValue(0);
    }
    public void SetGoldValue(int amount) { GoldText.text = amount.ToString(); }

    public int GoldHolding() { return int.Parse(GoldText.text); }
}
