using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIndicator : MonoBehaviour {

    public GameObject StatusObj;
    public SpriteRenderer StatusSpriteRenderer;
    public TextMesh StatusValue;

    public GameObject RangeObj;
    public SpriteRenderer RangeSpriteIndicator;
    public TextMesh RangeValue;
    public GameObject AOESprite;
    public SpriteRenderer ActionSpriteIndicator;
    public SpriteRenderer ActionSpriteIndicatorBackGround;
    public TextMesh ActionValue;

    public Sprite AttackIndicatorSprite;
    public Sprite MoveIndicatorSprite;
    public Sprite RangeIndicatorSprite;
    public Sprite ShieldIndicatorSprite;
    public Sprite HealIndicatorSprite;
    public Sprite DrawIndicatorSprite;

    public ActionType myActionType;

    public void DeBuffApplied(DeBuff deBuff)
    {
        switch (myActionType)
        {
            case ActionType.Attack:
                if (deBuff.thisDeBuffType == DeBuffType.Disarm)
                {
                    ActionValue.text = "0";
                    ActionValue.color = Color.red;
                }
                else if (deBuff.thisDeBuffType == DeBuffType.Weaken)
                {
                    int InitialDamage = int.Parse(ActionValue.text);
                    InitialDamage = Mathf.FloorToInt(InitialDamage * .75f);
                    ActionValue.text = InitialDamage.ToString();
                    ActionValue.color = Color.red;
                }
                break;
            case ActionType.Movement:
                if (deBuff.thisDeBuffType == DeBuffType.Immobelized)
                {
                    ActionValue.text = "0";
                    ActionValue.color = Color.red;
                }
                else if (deBuff.thisDeBuffType == DeBuffType.Slow)
                {
                    int InitialMovement = int.Parse(ActionValue.text);
                    ActionValue.text = (InitialMovement - 1).ToString();
                    ActionValue.color = Color.red;
                }
                break;
        }
        if (deBuff.thisDeBuffType == DeBuffType.Stun) {
            ActionValue.text = "0";
            ActionValue.color = Color.red;
        }
    }

    DeBuff FindDeBuff(List<DeBuff> deBuffs, DeBuffType debuffType)
    {
        foreach (DeBuff aDebuff in deBuffs)
        {
            if (aDebuff.thisDeBuffType == debuffType) { return aDebuff; }
        }
        return new DeBuff(DeBuffType.None, 0);
    }

    bool DeBuffsHasType(List<DeBuff> deBuffs, DeBuffType debuffType)
    {
        foreach (DeBuff aDebuff in deBuffs)
        {
            if (aDebuff.thisDeBuffType == debuffType) { return true; }
        }
        return false;
    }

    public void ShowAction(Action action, List<DeBuff> deBuffs)
    {
        myActionType = action.thisActionType;
        int InitialAmount = action.thisAOE.Damage;
        int EndAmount = InitialAmount;
        if (DeBuffsHasType(deBuffs, DeBuffType.Stun)) { EndAmount = 0; }
        switch (action.thisActionType)
        {
            case ActionType.Movement:
                RangeObj.SetActive(false);
                ActionSpriteIndicator.sprite = MoveIndicatorSprite;
                ActionSpriteIndicatorBackGround.sprite = MoveIndicatorSprite;
                InitialAmount = action.Range;
                EndAmount = InitialAmount;
                if (DeBuffsHasType(deBuffs, DeBuffType.Immobelized)) { EndAmount = 0; }
                else if (DeBuffsHasType(deBuffs, DeBuffType.Slow)) { EndAmount--; }
                ActionValue.text = EndAmount.ToString();
                ActionSpriteIndicator.color = Color.blue;
                break;
            case ActionType.Attack:
                if (action.thisAOE.thisAOEType != AOEType.SingleTarget)
                {
                    RangeObj.SetActive(true);
                    RangeValue.gameObject.SetActive(false);
                    RangeSpriteIndicator.gameObject.SetActive(false);
                    AOESprite.SetActive(true);
                }
                else if (action.Range > 1)
                {
                    RangeObj.SetActive(true);
                    RangeValue.text = action.Range.ToString();
                    RangeSpriteIndicator.color = Color.red;
                }
                else
                {
                    RangeObj.SetActive(false);
                }
                if (DeBuffsHasType(deBuffs, DeBuffType.Disarm)) { EndAmount = 0; }
                if (DeBuffsHasType(deBuffs, DeBuffType.PowerUp)) { EndAmount += FindDeBuff(deBuffs, DeBuffType.PowerUp).Amount; }
                if (DeBuffsHasType(deBuffs, DeBuffType.IncreaseAttack)) {
                    EndAmount += FindDeBuff(deBuffs, DeBuffType.IncreaseAttack).Amount;
                }
                if (DeBuffsHasType(deBuffs, DeBuffType.Weaken)) { EndAmount = Mathf.FloorToInt(EndAmount * .75f); }
                ActionValue.text = EndAmount.ToString();
                ActionSpriteIndicator.sprite = AttackIndicatorSprite;
                ActionSpriteIndicatorBackGround.sprite = AttackIndicatorSprite;
                ActionSpriteIndicator.color = Color.red;
                break;
            case ActionType.Shield:
                if (action.Range > 1)
                {
                    RangeObj.SetActive(true);
                    RangeValue.text = action.Range.ToString();
                    RangeSpriteIndicator.color = Color.gray;
                }
                else
                {
                    RangeObj.SetActive(false);
                }
                ActionValue.text = action.thisAOE.Damage.ToString();
                ActionSpriteIndicator.sprite = ShieldIndicatorSprite;
                ActionSpriteIndicatorBackGround.sprite = ShieldIndicatorSprite;
                ActionSpriteIndicator.color = Color.gray;
                break;
            case ActionType.Heal:
                if (action.Range > 1)
                {
                    RangeObj.SetActive(true);
                    RangeValue.text = action.Range.ToString();
                    RangeSpriteIndicator.color = Color.green;
                }
                else
                {
                    RangeObj.SetActive(false);
                }
                ActionValue.text = action.thisAOE.Damage.ToString();
                ActionSpriteIndicator.sprite = HealIndicatorSprite;
                ActionSpriteIndicatorBackGround.sprite = HealIndicatorSprite;
                ActionSpriteIndicator.color = Color.green;
                break;
            case ActionType.DrawCard:
                RangeObj.SetActive(false);
                ActionSpriteIndicator.sprite = DrawIndicatorSprite;
                ActionSpriteIndicatorBackGround.sprite = DrawIndicatorSprite;
                ActionValue.text = action.thisAOE.Damage.ToString();
                ActionSpriteIndicator.color = Color.gray;
                break;
        }

        if (EndAmount < InitialAmount) { ActionValue.color = Color.red; }
        if (EndAmount > InitialAmount) { ActionValue.color = Color.green; }

        if (action.thisDeBuff.thisDeBuffType != DeBuffType.None)
        {
            StatusObj.SetActive(true);
            if (!RangeObj.activeSelf)
            {
                StatusObj.transform.localPosition = new Vector3(12f, StatusObj.transform.localPosition.y, StatusObj.transform.localPosition.z);
            }
            else
            {
                StatusObj.transform.localPosition = new Vector3(31f, StatusObj.transform.localPosition.y, StatusObj.transform.localPosition.z);
            }
        }

        DeBuffManager manager = FindObjectOfType<DeBuffManager>();
        StatusValue.text = action.thisDeBuff.Amount.ToString();
        switch (action.thisDeBuff.thisDeBuffType)
        {
            case DeBuffType.Bleed:
                StatusSpriteRenderer.sprite = manager.BleedSprite;
                StatusSpriteRenderer.color = manager.BleedColor;
                StatusObj.GetComponent<Tooltip>().tooltipTitle = manager.BleedTitle;
                StatusObj.GetComponent<Tooltip>().tooltipText = manager.BleedText;
                break;
            case DeBuffType.Poison:
                StatusSpriteRenderer.sprite = manager.PoisonSprite;
                StatusSpriteRenderer.color = manager.PoisonColor;
                StatusObj.GetComponent<Tooltip>().tooltipTitle = manager.PoisonTitle;
                StatusObj.GetComponent<Tooltip>().tooltipText = manager.PoisonText;
                break;
            case DeBuffType.Immobelized:
                StatusSpriteRenderer.sprite = manager.ImmobelizeSprite;
                StatusSpriteRenderer.color = manager.ImmobelizeColor;
                StatusObj.GetComponent<Tooltip>().tooltipTitle = manager.ImmobelizeTitle;
                StatusObj.GetComponent<Tooltip>().tooltipText = manager.ImmobelizeText;
                break;
            case DeBuffType.Weaken:
                StatusSpriteRenderer.sprite = manager.WeakenSprite;
                StatusSpriteRenderer.color = manager.WeakenColor;
                StatusObj.GetComponent<Tooltip>().tooltipTitle = manager.WeakenTitle;
                StatusObj.GetComponent<Tooltip>().tooltipText = manager.WeakenText;
                break;
            case DeBuffType.Stun:
                StatusSpriteRenderer.sprite = manager.StunSprite;
                StatusSpriteRenderer.color = manager.StunColor;
                StatusObj.GetComponent<Tooltip>().tooltipTitle = manager.StunTitle;
                StatusObj.GetComponent<Tooltip>().tooltipText = manager.StunText;
                break;
            case DeBuffType.Disarm:
                StatusSpriteRenderer.sprite = manager.DisarmSprite;
                StatusSpriteRenderer.color = manager.DisarmColor;
                StatusObj.GetComponent<Tooltip>().tooltipTitle = manager.DisarmTitle;
                StatusObj.GetComponent<Tooltip>().tooltipText = manager.DisarmText;
                break;
            case DeBuffType.Slow:
                StatusSpriteRenderer.sprite = manager.SlowSprite;
                StatusSpriteRenderer.color = manager.SlowColor;
                StatusObj.GetComponent<Tooltip>().tooltipTitle = manager.SlowTitle;
                StatusObj.GetComponent<Tooltip>().tooltipText = manager.SlowText;
                break;
            case DeBuffType.PowerUp:
                StatusSpriteRenderer.sprite = manager.PowerUpSprite;
                StatusSpriteRenderer.color = manager.PowerUpColor;
                StatusObj.GetComponent<Tooltip>().tooltipTitle = manager.PowerUpTitle;
                StatusObj.GetComponent<Tooltip>().tooltipText = manager.PowerUpText;
                break;
        }
    }
}
