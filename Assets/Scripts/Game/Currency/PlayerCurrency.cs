using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCurrency : MonoBehaviour {

    public Text GoldText;

    public void SetGoldValue(int amount) { GoldText.text = amount.ToString(); }
}
