using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeBuffIndicator : MonoBehaviour
{
    public DeBuff myDeBuff;

    public void ShowDebuff(DeBuff debuff)
    {
        myDeBuff = debuff;
        DeBuffManager manager = FindObjectOfType<DeBuffManager>();
        SpriteRenderer deBuffImage = GetComponent<SpriteRenderer>();
        SpriteRenderer deBuffOutLineImage = deBuffImage.transform.GetChild(0).GetComponent<SpriteRenderer>();
        switch (debuff)
        {
            case DeBuff.Bleed:
                deBuffImage.sprite = manager.BleedSprite;
                deBuffOutLineImage.sprite = manager.BleedSprite;
                deBuffImage.color = manager.BleedColor;
                GetComponent<Tooltip>().tooltipTitle = manager.BleedTitle;
                GetComponent<Tooltip>().tooltipText = manager.BleedText;
                break;
            case DeBuff.Poison:
                deBuffImage.sprite = manager.PoisonSprite;
                deBuffOutLineImage.sprite = manager.PoisonSprite;
                deBuffImage.color = manager.PoisonColor;
                GetComponent<Tooltip>().tooltipTitle = manager.PoisonTitle;
                GetComponent<Tooltip>().tooltipText = manager.PoisonText;
                break;
            case DeBuff.Slow:
                deBuffImage.sprite = manager.SlowSprite;
                deBuffOutLineImage.sprite = manager.SlowSprite;
                deBuffImage.color = manager.SlowColor;
                GetComponent<Tooltip>().tooltipTitle = manager.SlowTitle;
                GetComponent<Tooltip>().tooltipText = manager.SlowText;
                break;
            case DeBuff.Immobelized:
                deBuffImage.sprite = manager.ImmobelizeSprite;
                deBuffOutLineImage.sprite = manager.ImmobelizeSprite;
                deBuffImage.color = manager.ImmobelizeColor;
                GetComponent<Tooltip>().tooltipTitle = manager.ImmobelizeTitle;
                GetComponent<Tooltip>().tooltipText = manager.ImmobelizeText;
                break;
            case DeBuff.Weaken:
                deBuffImage.sprite = manager.WeakenSprite;
                deBuffOutLineImage.sprite = manager.WeakenSprite;
                deBuffImage.color = manager.WeakenColor;
                GetComponent<Tooltip>().tooltipTitle = manager.WeakenTitle;
                GetComponent<Tooltip>().tooltipText = manager.WeakenText;
                break;
            case DeBuff.Stun:
                deBuffImage.sprite = manager.StunSprite;
                deBuffOutLineImage.sprite = manager.StunSprite;
                deBuffImage.color = manager.StunColor;
                GetComponent<Tooltip>().tooltipTitle = manager.StunTitle;
                GetComponent<Tooltip>().tooltipText = manager.StunText;
                break;
            case DeBuff.Disarm:
                deBuffImage.sprite = manager.DisarmSprite;
                deBuffOutLineImage.sprite = manager.DisarmSprite;
                deBuffImage.color = manager.DisarmColor;
                GetComponent<Tooltip>().tooltipTitle = manager.DisarmTitle;
                GetComponent<Tooltip>().tooltipText = manager.DisarmText;
                break;
        }
    }
}
