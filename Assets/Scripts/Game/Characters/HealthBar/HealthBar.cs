using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    public TextMesh CurrentHealthText;
    public TextMesh MaxHealthText;

    public HPBar HpBar;

    public GameObject XpBar;
    public GameObject backGround;

    public GameObject AttackSymbol;
    public TextMesh AttackValue;

    public GameObject HealSymbol;
    public TextMesh HealValue;

    public GameObject ArmorSymbol;
    public TextMesh ArmorValue;

    public GameObject GoldObj;
    public TextMesh GoldValue;

    public Sprite StrengthSprite;
    public Sprite AgilitySprite;
    public Sprite RangeSprite;
    public Sprite ArmorSprite;

    List<DeBuffIndicator> DeBuffsIndicatorsActive = new List<DeBuffIndicator>();
    public GameObject DeBuffPrefab;

    Camera cam;

    int MaxHealth;
    int CurrentHealth;
    int CurrentShield = 0;

    public GameObject IndicatorPrefab;
    List<ActionIndicator> CurrnetIndicators = new List<ActionIndicator>();

    public void ClearActions()
    {
        foreach (ActionIndicator ai in CurrnetIndicators)
        {
            Destroy(ai.gameObject);
        }
        CurrnetIndicators.Clear();
    }

    public void RemoveAction()
    {
        ActionIndicator AI = CurrnetIndicators[0];
        CurrnetIndicators.Remove(AI);
        Destroy(AI.gameObject);
        if (CurrnetIndicators.Count > 0)
        {
            for(int i = 0; i < CurrnetIndicators.Count; i++)
            {
                CurrnetIndicators[i].transform.localPosition = new Vector3(-0.25f, CurrnetIndicators[i].transform.localPosition.y - .5f, 0);
            }
        }
    }

    public void ShowActions(List<Action> actions, List<DeBuff> deBuffs)
    {
        ClearActions();
        for(int i = 0; i < actions.Count; i++)
        {
            GameObject actionIndicator = Instantiate(IndicatorPrefab, this.transform);
            actionIndicator.transform.localPosition = new Vector3(-0.25f, .94f + i * .5f, 0);
            ActionIndicator AI = actionIndicator.GetComponent<ActionIndicator>();
            AI.ShowAction(actions[i], deBuffs);
            CurrnetIndicators.Add(AI);
        }
    }

    public void UpdateDamageVisualsFromDeBuff(DeBuff deBuff)
    {
        ActionIndicator[] AIs = GetComponentsInChildren<ActionIndicator>();
        foreach(ActionIndicator AI in AIs)
        {
            AI.DeBuffApplied(deBuff);
        }
    }

    public void ShowAction(Action action, List<DeBuff> deBuffs)
    {
        ClearActions();
        GameObject actionIndicator = Instantiate(IndicatorPrefab, this.transform);
        actionIndicator.transform.localPosition = new Vector3(-0.25f, .94f, 0);
        ActionIndicator AI = actionIndicator.GetComponent<ActionIndicator>();
        AI.ShowAction(action, deBuffs);
        CurrnetIndicators.Add(AI);
    }

    public void SetDeBuffAmount(DeBuffType deBuffType, int Amount)
    {
        for (int i = 0; i < DeBuffsIndicatorsActive.Count; i++)
        {
            if (DeBuffsIndicatorsActive[i].myDeBuff.thisDeBuffType == deBuffType)
            {
                DeBuffsIndicatorsActive[i].SetAmount(Amount);
            }
        }
    }

    public void RemoveDeBuff(DeBuffType debuff)
    {
        bool shift = false;
        for(int i = 0; i < DeBuffsIndicatorsActive.Count; i++)
        {
            if (DeBuffsIndicatorsActive[i].myDeBuff.thisDeBuffType == debuff)
            {
                DeBuffIndicator indicatorToRemove = DeBuffsIndicatorsActive[i];
                DeBuffsIndicatorsActive.Remove(indicatorToRemove);
                Destroy(indicatorToRemove.transform.parent.gameObject);
                shift = true;
                continue;
            }
            if (shift)
            {
                DeBuffsIndicatorsActive[i].transform.localPosition = new Vector3(DeBuffsIndicatorsActive[i].transform.localPosition.x - .4f, -0.1f, 0);
            }
        }
    }

    public void ShowDeBuff(DeBuff debuff)
    {
        GameObject thisDeBuff = Instantiate(DeBuffPrefab, transform);
        thisDeBuff.transform.localRotation = Quaternion.identity;
        thisDeBuff.transform.localPosition = new Vector3(-.8f + .8f* DeBuffsIndicatorsActive.Count, -0.185f, 0);
        DeBuffsIndicatorsActive.Add(thisDeBuff.GetComponentInChildren<DeBuffIndicator>());
        thisDeBuff.GetComponentInChildren<DeBuffIndicator>().ShowDebuff(debuff);
        UpdateDamageVisualsFromDeBuff(debuff);
    }

    public void AddGold(int amount)
    {
        StartCoroutine("GoldAdded", amount);
    }

    IEnumerator GoldAdded(int amount)
    {
        GoldObj.SetActive(true);
        GoldValue.text = "+ " + amount.ToString();
        yield return new WaitForSeconds(1f);
        GoldObj.SetActive(false);
    }

    public void CreateHealthBar(int currentHealth, int maxHealth)
    {
        backGround.SetActive(true);
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        CurrentHealthText.text = CurrentHealth.ToString();
        MaxHealthText.text = MaxHealth.ToString();
        HpBar.SetHP((float)currentHealth/(float)maxHealth);
    }
    public void ResetShield(int amount)
    {
        CurrentShield = 0;
        if (amount == 0)
        {
            ArmorSymbol.gameObject.SetActive(false);
            ArmorValue.gameObject.SetActive(false);
        }
    }

    public void SetShield(int amount)
    {
        if (CurrentShield == 0)
        {
            ArmorSymbol.gameObject.SetActive(true);
            ArmorValue.gameObject.SetActive(true);
        }

        ArmorValue.text = (CurrentShield + amount).ToString();
        CurrentShield += amount;
    }

    public void RemoveShield(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int index = CurrentShield - 1;
            if (index < 0) { break; }
            CurrentShield--;
            ArmorValue.text = CurrentShield.ToString();
        }
        if (CurrentShield == 0)
        {
            ArmorSymbol.gameObject.SetActive(false);
            ArmorValue.gameObject.SetActive(false);
        }
    }

    public void AddShield(int shieldAmount)
    {
        IEnumerator AddArmorCoroutine = AddingArmor(shieldAmount);
        StartCoroutine(AddArmorCoroutine);
    }

    IEnumerator AddingArmor(int shieldAmount)
    {
        yield return null;
        if (CurrentShield == 0)
        {
            ArmorSymbol.gameObject.SetActive(true);
            ArmorValue.gameObject.SetActive(true);
        }

        ArmorValue.text = (CurrentShield + shieldAmount).ToString();
        CurrentShield += shieldAmount;
        GetComponentInParent<Character>().FinishedShielding();
    }

    public void DummyHealHealth()
    {
        AddHealth(3);
    }

    public void AddHealth(int amount)
    {
        IEnumerator AddHealthCoroutine = AddingHealth(amount);
        StartCoroutine(AddHealthCoroutine);
    }

    IEnumerator AddingHealth(int healthAmount)
    {
        yield return null;
        HealValue.gameObject.SetActive(true);
        HealValue.text = healthAmount.ToString();
        yield return new WaitForSeconds(.2f);
        HealValue.gameObject.SetActive(false);
        CurrentHealth = Mathf.Clamp(CurrentHealth + healthAmount, 0, MaxHealth);
        CurrentHealthText.text = CurrentHealth.ToString();
        HpBar.SetHP((float)CurrentHealth / (float)MaxHealth);
        GetComponentInParent<Character>().FinishedHealing();
    }

    public void LoseHealth(int totalHealthLoss)
    {
        IEnumerator LoseHealthCoroutine = LosingHealth(totalHealthLoss);
        StartCoroutine(LoseHealthCoroutine);
    }

    IEnumerator LosingHealth(int totalDamageIncomming)
    {
        yield return new WaitForSeconds(.2f);
        int totalHealthLoss = Mathf.Clamp(totalDamageIncomming - CurrentShield, 0, 100000);
        AttackValue.gameObject.SetActive(true);
        AttackValue.text = totalHealthLoss.ToString();
        yield return new WaitForSeconds(.2f);

        if (CurrentShield > 0)
        {
            RemoveShield(totalDamageIncomming);
        }
        CurrentHealth -= totalHealthLoss;
        
        CurrentHealthText.text = CurrentHealth.ToString();
        HpBar.SetHP((float)CurrentHealth / (float)MaxHealth);
        AttackValue.gameObject.SetActive(false);
        GetComponentInParent<Character>().finishedTakingDamage();
        yield return null;
    }

    public void PredictDamage(int totalDamageIncomming)
    {
        int totalHealthLoss = Mathf.Clamp(totalDamageIncomming - CurrentShield, 0, 100000);
        int predictedHealth = CurrentHealth - totalHealthLoss;
        CurrentHealthText.text = predictedHealth.ToString();
        CurrentHealthText.color = Color.red;
        HpBar.ShowPredictedDamage((float)predictedHealth / (float)MaxHealth);
    }

    public void RemovePredication()
    {
        if (CurrentHealthText == null) { return; }
        CurrentHealthText.text = CurrentHealth.ToString();
        CurrentHealthText.color = Color.white;
        HpBar.SetHP((float)CurrentHealth / (float)MaxHealth);
    }

	// Use this for initialization
	void Start () {
        cam = FindObjectOfType<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(90 - cam.transform.rotation.eulerAngles.x, cam.transform.rotation.eulerAngles.y, 0);
	}
}
