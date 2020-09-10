using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickStatLook : MonoBehaviour {

    public GameObject ViewPanel;
    public GameObject AttackPanel;
    public Text AttackValue;
    public GameObject MovePanel;
    public Text MoveValue;
    public GameObject RangePanel;
    public Text RangeValue;
    public GameObject ShieldPanel;
    public Text ShieldValue;

    private int baseAttack;
    private int baseMove;
    private int baseRange;
    private int baseShield;

    public void ShowStats()
    {
        ViewPanel.SetActive(true);
    }

    public void SetCharacterStats(int attackValue, int moveValue, int rangeValue, int shieldValue)
    {

        baseAttack = attackValue;
        baseMove = moveValue;
        baseRange = rangeValue;
        baseShield = shieldValue;

        setValue(AttackPanel, AttackValue, attackValue, 0);
        setValue(MovePanel, MoveValue, moveValue, 0);
        setValue(RangePanel, RangeValue, rangeValue, 1);
        setValue(ShieldPanel, ShieldValue, shieldValue, 0);
        ViewPanel.SetActive(true);
    }

    void setValue(GameObject panel, Text valueString, int value, int threshold)
    {
        if (value > threshold)
        {
            panel.SetActive(true);
            valueString.text = value.ToString();
        }
        else
        {
            panel.SetActive(false);
        }
    }

    public void AddStats(bool attackAvailable, bool moveAvailable, int attackIncrease, int moveIncrease, int rangeAmount)
    {
        if (!attackAvailable) { AttackPanel.SetActive(false); }
        else {
            AttackPanel.SetActive(true);
            AttackValue.text = Mathf.Clamp((baseAttack + attackIncrease), 0, 100).ToString();
        }

        if (!moveAvailable) { MovePanel.SetActive(false); }
        else {
            if ((baseMove + moveIncrease) > 0)
            {
                MoveValue.text = (baseMove + moveIncrease).ToString();
                MovePanel.SetActive(true);
            }
        }

        if ((rangeAmount) > 1)
        {
            RangeValue.text = (rangeAmount).ToString();
            RangePanel.SetActive(true);
        } else
        {
            RangePanel.SetActive(false) ;
        }
    }

    public void GoBackToBaseStats()
    {
        setValue(AttackPanel, AttackValue, baseAttack, 0);
        setValue(MovePanel, MoveValue, baseMove, 0);
        setValue(RangePanel, RangeValue, baseRange, 1);
        setValue(ShieldPanel, ShieldValue, baseShield, 0);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
