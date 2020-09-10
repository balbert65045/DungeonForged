using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIndicator : MonoBehaviour {


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

    public void ShowAction(Action action)
    {
        switch (action.thisActionType)
        {
            case ActionType.Movement:
                RangeObj.SetActive(false);
                ActionSpriteIndicator.sprite = MoveIndicatorSprite;
                ActionSpriteIndicatorBackGround.sprite = MoveIndicatorSprite;
                ActionValue.text = action.Range.ToString();
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
                ActionValue.text = action.thisAOE.Damage.ToString();
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
    }
}
