using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeBuffIndicator : MonoBehaviour
{
    public DeBuff myDeBuff;
    public Sprite BleedSprite;
    public Sprite PoisonSprite;

    public void ShowDebuff(DeBuff debuff)
    {
        myDeBuff = debuff;
        SpriteRenderer deBuffImage = GetComponent<SpriteRenderer>();
        SpriteRenderer deBuffOutLineImage = GetComponentInChildren<SpriteRenderer>();
        switch (debuff)
        {
            case DeBuff.Bleed:
                deBuffImage.sprite = BleedSprite;
                deBuffOutLineImage.sprite = BleedSprite;
                deBuffImage.color = Color.red;
                break;
        }
    }
}
