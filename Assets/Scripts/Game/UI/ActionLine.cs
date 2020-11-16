using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionLine : MonoBehaviour {

    public Color EnhanceColor;
    public Color DehanceColor;

    public Text AbilityType;
    public Image AbilityImage;
    public Text AbilityAmount;

    public Text RangeAbilityType;
    public Image RangeAbilityImage;
    public Text RangeAbilityAmount;

    public Image StatusImage;
    public Text StatusText;

    public int ActionLineBaseAmount;
    public int RangeAmountBaseAmount;

    public int actionIndex = 0;

    public void HighlightAction()
    {
        AbilityType.color = Color.green;
        AbilityImage.color = Color.green;
        AbilityAmount.color = Color.green;
        RangeAbilityType.color = Color.green;
        RangeAbilityImage.color = Color.green;
        RangeAbilityAmount.color = Color.green;
    }

    public void ActionUsed()
    {
        AbilityType.color = Color.gray;
        AbilityImage.color = Color.gray;
        AbilityAmount.color = Color.gray;
        RangeAbilityType.color = Color.gray;
        RangeAbilityImage.color = Color.gray;
        RangeAbilityAmount.color = Color.gray;
    }

    public void ActionBackToNormal()
    {
        AbilityType.color = Color.black;
        AbilityImage.color = Color.black;
        AbilityAmount.color = Color.black;
        RangeAbilityType.color = Color.black;
        RangeAbilityImage.color = Color.black;
        RangeAbilityAmount.color = Color.black;
    }

    // Use this for initialization
    void Awake () {
        ActionLineBaseAmount = int.Parse(AbilityAmount.text);
        if (RangeAbilityAmount != null) { int.TryParse(RangeAbilityAmount.text, out RangeAmountBaseAmount); }
    }

    public void SetUpAmount(int attribute)
    {
        AbilityAmount.text = (ActionLineBaseAmount + attribute).ToString();
    }

    public void ResetAmount()
    {
        AbilityAmount.text = ActionLineBaseAmount.ToString();
    }
}
