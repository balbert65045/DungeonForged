using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeBuffIndicator : MonoBehaviour
{
    public DeBuff myDeBuff;
    public TextMesh amount;

    public void SetAmount(int Amount)
    {
        amount.text = Amount.ToString();
    }

    public void ShowDebuff(DeBuff debuff)
    {
        myDeBuff = debuff;
        DeBuffManager manager = FindObjectOfType<DeBuffManager>();
        SpriteRenderer deBuffImage = GetComponentInChildren<SpriteRenderer>();
        SpriteRenderer deBuffOutLineImage = deBuffImage.transform.GetChild(0).GetComponent<SpriteRenderer>();
        amount.text = debuff.Amount.ToString();
        switch (debuff.thisDeBuffType)
        {
            case DeBuffType.Bleed:
                deBuffImage.sprite = manager.BleedSprite;
                deBuffOutLineImage.sprite = manager.BleedSprite;
                deBuffImage.color = manager.BleedColor;
                GetComponent<Tooltip>().tooltipTitle = manager.BleedTitle;
                GetComponent<Tooltip>().tooltipText = manager.BleedText;
                break;
            case DeBuffType.Poison:
                deBuffImage.sprite = manager.PoisonSprite;
                deBuffOutLineImage.sprite = manager.PoisonSprite;
                deBuffImage.color = manager.PoisonColor;
                GetComponent<Tooltip>().tooltipTitle = manager.PoisonTitle;
                GetComponent<Tooltip>().tooltipText = manager.PoisonText;
                break;
            case DeBuffType.Slow:
                deBuffImage.sprite = manager.SlowSprite;
                deBuffOutLineImage.sprite = manager.SlowSprite;
                deBuffImage.color = manager.SlowColor;
                GetComponent<Tooltip>().tooltipTitle = manager.SlowTitle;
                GetComponent<Tooltip>().tooltipText = manager.SlowText;
                break;
            case DeBuffType.Immobelized:
                deBuffImage.sprite = manager.ImmobelizeSprite;
                deBuffOutLineImage.sprite = manager.ImmobelizeSprite;
                deBuffImage.color = manager.ImmobelizeColor;
                GetComponent<Tooltip>().tooltipTitle = manager.ImmobelizeTitle;
                GetComponent<Tooltip>().tooltipText = manager.ImmobelizeText;
                break;
            case DeBuffType.Weaken:
                deBuffImage.sprite = manager.WeakenSprite;
                deBuffOutLineImage.sprite = manager.WeakenSprite;
                deBuffImage.color = manager.WeakenColor;
                GetComponent<Tooltip>().tooltipTitle = manager.WeakenTitle;
                GetComponent<Tooltip>().tooltipText = manager.WeakenText;
                break;
            case DeBuffType.Stun:
                deBuffImage.sprite = manager.StunSprite;
                deBuffOutLineImage.sprite = manager.StunSprite;
                deBuffImage.color = manager.StunColor;
                GetComponent<Tooltip>().tooltipTitle = manager.StunTitle;
                GetComponent<Tooltip>().tooltipText = manager.StunText;
                break;
            case DeBuffType.Disarm:
                deBuffImage.sprite = manager.DisarmSprite;
                deBuffOutLineImage.sprite = manager.DisarmSprite;
                deBuffImage.color = manager.DisarmColor;
                GetComponent<Tooltip>().tooltipTitle = manager.DisarmTitle;
                GetComponent<Tooltip>().tooltipText = manager.DisarmText;
                break;
            case DeBuffType.IncreaseAttack:
                deBuffImage.sprite = manager.AttackIncreaseSprite;
                deBuffOutLineImage.sprite = manager.AttackIncreaseSprite;
                deBuffImage.color = Color.white;
                GetComponent<Tooltip>().tooltipTitle = manager.AttackIncreaseTitle;
                GetComponent<Tooltip>().tooltipText = manager.AttackIncreaseText;
                break;
        }
    }
}
