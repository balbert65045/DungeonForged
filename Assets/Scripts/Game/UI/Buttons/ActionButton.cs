using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour {

    public Sprite AttackSprite;
    public Sprite MoveSprite;
    public Sprite HealSprite;
    public Sprite ShieldSprite;


    public Image MainImage;
    public Text MainValue;
    public Image RangeImage;
    public Text RangeValue;

    public void HighlightAction()
    {
        GetComponentInChildren<Button>().GetComponent<Image>().color = Color.blue;
    }

    public void UnHighlightColor()
    {
        GetComponentInChildren<Button>().GetComponent<Image>().color = Color.white;
    }

    public void DisableButton()
    {
        UnHighlightColor();
        GetComponentInChildren<Button>().interactable = false;
    }

    public void SetAction(Action action, Character character)
    {
        GetComponentInChildren<Button>().interactable = true;
        switch (action.thisActionType)
        {
            case ActionType.Attack:
                MainImage.sprite = AttackSprite;
                MainValue.text = (action.thisAOE.Damage + character.GetStrength()).ToString();
                if (action.Range > 1) {
                    RangeImage.gameObject.SetActive(true);
                    RangeValue.gameObject.SetActive(true);
                    RangeValue.text = (action.Range + character.GetDexterity()).ToString();
                } else
                {
                    RangeImage.gameObject.SetActive(false);
                    RangeValue.gameObject.SetActive(false);
                }
                break;
            case ActionType.Movement:
                MainImage.sprite = MoveSprite;
                MainValue.text = (action.Range + character.GetAgility()).ToString();
                RangeImage.gameObject.SetActive(false);
                RangeValue.gameObject.SetActive(false);
                break;
            case ActionType.Heal:
                MainImage.sprite = HealSprite;
                MainValue.text = action.thisAOE.Damage.ToString();
                if (action.Range > 1)
                {
                    RangeImage.gameObject.SetActive(true);
                    RangeValue.gameObject.SetActive(true);
                    RangeValue.text = action.Range.ToString();
                }
                else
                {
                    RangeImage.gameObject.SetActive(false);
                    RangeValue.gameObject.SetActive(false);
                }
                break;
            case ActionType.Shield:
                MainImage.sprite = ShieldSprite;
                MainValue.text = action.thisAOE.Damage.ToString();
                if (action.Range > 1)
                {
                    RangeImage.gameObject.SetActive(true);
                    RangeValue.gameObject.SetActive(true);
                    RangeValue.text = action.Range.ToString();
                }
                else
                {
                    RangeImage.gameObject.SetActive(false);
                    RangeValue.gameObject.SetActive(false);
                }
                break;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
