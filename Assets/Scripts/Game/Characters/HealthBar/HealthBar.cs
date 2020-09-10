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

    public GameObject Buff1;
    public SpriteRenderer Buff1Sprite;
    public GameObject Buff2;
    public SpriteRenderer Buff2Sprite;
    public GameObject Buff3;
    public SpriteRenderer Buff3Sprite;
    public GameObject Buff4;
    public SpriteRenderer Buff4Sprite;

    float AttackSymbolXStart;
    float AttackValueXStart;
    float HealSymbolXStart;
    float HealValueXStart;

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

    public void ShowActions(List<Action> actions)
    {
        ClearActions();
        for(int i = 0; i < actions.Count; i++)
        {
            GameObject actionIndicator = Instantiate(IndicatorPrefab, this.transform);
            actionIndicator.transform.localPosition = new Vector3(-0.25f, .94f + i * .5f, 0);
            ActionIndicator AI = actionIndicator.GetComponent<ActionIndicator>();
            AI.ShowAction(actions[i]);
            CurrnetIndicators.Add(AI);
        }
    }

    public void ShowAction(Action action)
    {
        ClearActions();
        GameObject actionIndicator = Instantiate(IndicatorPrefab, this.transform);
        actionIndicator.transform.localPosition = new Vector3(-0.25f, .94f, 0);
        ActionIndicator AI = actionIndicator.GetComponent<ActionIndicator>();
        AI.ShowAction(action);
        CurrnetIndicators.Add(AI);
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

    public void AddBuff(BuffType buff)
    {
        if (!Buff1.activeSelf)
        {
            setBuff(Buff1, Buff1Sprite, buff);
        }
        else if (!Buff2.activeSelf)
        {
            setBuff(Buff2, Buff2Sprite, buff);
        }
        else if (!Buff3.activeSelf)
        {
            setBuff(Buff3, Buff3Sprite, buff);
        }
        else if (!Buff4.activeSelf)
        {
            setBuff(Buff4, Buff4Sprite, buff);
        }
    }

    void setBuff(GameObject obj, SpriteRenderer SpriteRend, BuffType buffType)
    {
        obj.SetActive(true);
        switch (buffType)
        {
            case BuffType.Strength:
                SpriteRend.sprite = StrengthSprite;
                break;
            case BuffType.Agility:
                SpriteRend.sprite = AgilitySprite;
                break;
            case BuffType.Dexterity:
                SpriteRend.sprite = RangeSprite;
                break;
            case BuffType.Armor:
                SpriteRend.sprite = ArmorSprite;
                break;
        }
    }

    public void RemoveBuff(BuffType buffType)
    {
        switch (buffType)
        {
            case BuffType.Strength:
                removeBuff(StrengthSprite);
                break;
            case BuffType.Agility:
                removeBuff(AgilitySprite);
                break;
            case BuffType.Dexterity:
                removeBuff(RangeSprite);
                break;
            case BuffType.Armor:
                removeBuff(ArmorSprite);
                break;
        }
    }

    void removeBuff(Sprite sprite)
    {
        if (Buff1.activeSelf && Buff1Sprite.sprite == sprite) { Buff1.SetActive(false); }
        else if (Buff2.activeSelf && Buff2Sprite.sprite == sprite) { Buff2.SetActive(false); }
        else if (Buff3.activeSelf && Buff3Sprite.sprite == sprite) { Buff3.SetActive(false); }
        else if (Buff4.activeSelf && Buff4Sprite.sprite == sprite) { Buff4.SetActive(false); }
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
        CurrentHealth -= totalHealthLoss;
        CurrentHealthText.text = CurrentHealth.ToString();
        HpBar.SetHP((float)CurrentHealth / (float)MaxHealth);
        AttackValue.gameObject.SetActive(false);
        GetComponentInParent<Character>().finishedTakingDamage();
        yield return null;
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
