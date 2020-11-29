using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIndicator : MonoBehaviour {

    public GameObject StatusObj;
    public SpriteRenderer StatusSpriteRenderer;

    public GameObject RangeObj;
    public SpriteRenderer RangeSpriteIndicator;
    public TextMesh RangeValue;
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
                if (deBuff == DeBuff.Disarm)
                {
                    ActionValue.text = "0";
                    ActionValue.color = Color.red;
                }
                else if (deBuff == DeBuff.Weaken)
                {
                    int InitialDamage = int.Parse(ActionValue.text);
                    InitialDamage = Mathf.FloorToInt(InitialDamage * .75f);
                    ActionValue.text = InitialDamage.ToString();
                    ActionValue.color = Color.red;
                }
                break;
            case ActionType.Movement:
                if (deBuff == DeBuff.Immobelized)
                {
                    ActionValue.text = "0";
                    ActionValue.color = Color.red;
                }
                else if (deBuff == DeBuff.Slow)
                {
                    int InitialMovement = int.Parse(ActionValue.text);
                    ActionValue.text = (InitialMovement - 1).ToString();
                    ActionValue.color = Color.red;
                }
                break;
        }
        if (deBuff == DeBuff.Stun) {
            ActionValue.text = "0";
            ActionValue.color = Color.red;
        }
    }

    public void ShowAction(Action action, List<DeBuff> deBuffs)
    {
        myActionType = action.thisActionType;
        int InitialAmount = action.thisAOE.Damage;
        int EndAmount = InitialAmount;
        if (deBuffs.Contains(DeBuff.Stun)) { EndAmount = 0; }
        switch (action.thisActionType)
        {
            case ActionType.Movement:
                RangeObj.SetActive(false);
                ActionSpriteIndicator.sprite = MoveIndicatorSprite;
                ActionSpriteIndicatorBackGround.sprite = MoveIndicatorSprite;
                InitialAmount = action.Range;
                EndAmount = InitialAmount;
                if (deBuffs.Contains(DeBuff.Immobelized)) { EndAmount = 0; }
                else if (deBuffs.Contains(DeBuff.Slow)) { EndAmount--; }
                ActionValue.text = EndAmount.ToString();
                ActionSpriteIndicator.color = Color.blue;
                break;
            case ActionType.Attack:
                if (action.Range > 1)
                {
                    RangeObj.SetActive(true);
                    RangeValue.text = action.Range.ToString();
                    RangeSpriteIndicator.color = Color.red;
                }
                else
                {
                    RangeObj.SetActive(false);
                }
                if (deBuffs.Contains(DeBuff.Disarm)) { EndAmount = 0; }
                else if (deBuffs.Contains(DeBuff.Weaken)) { EndAmount = Mathf.FloorToInt(EndAmount * .75f); }
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

        if (action.thisDeBuff != DeBuff.None)
        {
            StatusObj.SetActive(true);
            if (!RangeObj.activeSelf)
            {
                StatusObj.transform.localPosition = new Vector3(6.3f, StatusObj.transform.localPosition.y, StatusObj.transform.localPosition.z);
            }
            else
            {
                StatusObj.transform.localPosition = new Vector3(26f, StatusObj.transform.localPosition.y, StatusObj.transform.localPosition.z);
            }
        }

        DeBuffManager manager = FindObjectOfType<DeBuffManager>();
        switch (action.thisDeBuff)
        {
            case DeBuff.Bleed:
                StatusSpriteRenderer.sprite = manager.BleedSprite;
                StatusSpriteRenderer.color = manager.BleedColor;
                StatusObj.GetComponent<Tooltip>().tooltipTitle = manager.BleedTitle;
                StatusObj.GetComponent<Tooltip>().tooltipText = manager.BleedText;
                break;
            case DeBuff.Poison:
                StatusSpriteRenderer.sprite = manager.PoisonSprite;
                StatusSpriteRenderer.color = manager.PoisonColor;
                StatusObj.GetComponent<Tooltip>().tooltipTitle = manager.PoisonTitle;
                StatusObj.GetComponent<Tooltip>().tooltipText = manager.PoisonText;
                break;
            case DeBuff.Immobelized:
                StatusSpriteRenderer.sprite = manager.ImmobelizeSprite;
                StatusSpriteRenderer.color = manager.ImmobelizeColor;
                StatusObj.GetComponent<Tooltip>().tooltipTitle = manager.ImmobelizeTitle;
                StatusObj.GetComponent<Tooltip>().tooltipText = manager.ImmobelizeText;
                break;
            case DeBuff.Weaken:
                StatusSpriteRenderer.sprite = manager.WeakenSprite;
                StatusSpriteRenderer.color = manager.WeakenColor;
                StatusObj.GetComponent<Tooltip>().tooltipTitle = manager.WeakenTitle;
                StatusObj.GetComponent<Tooltip>().tooltipText = manager.WeakenText;
                break;
            case DeBuff.Stun:
                StatusSpriteRenderer.sprite = manager.StunSprite;
                StatusSpriteRenderer.color = manager.StunColor;
                StatusObj.GetComponent<Tooltip>().tooltipTitle = manager.StunTitle;
                StatusObj.GetComponent<Tooltip>().tooltipText = manager.StunText;
                break;
            case DeBuff.Disarm:
                StatusSpriteRenderer.sprite = manager.DisarmSprite;
                StatusSpriteRenderer.color = manager.DisarmColor;
                StatusObj.GetComponent<Tooltip>().tooltipTitle = manager.DisarmTitle;
                StatusObj.GetComponent<Tooltip>().tooltipText = manager.DisarmText;
                break;
            case DeBuff.Slow:
                StatusSpriteRenderer.sprite = manager.SlowSprite;
                StatusSpriteRenderer.color = manager.SlowColor;
                StatusObj.GetComponent<Tooltip>().tooltipTitle = manager.SlowTitle;
                StatusObj.GetComponent<Tooltip>().tooltipText = manager.SlowText;
                break;
        }
    }
}
