using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterTurnIndicator : MonoBehaviour, IPointerDownHandler
{

    public Entity characterLinkedTo;
    public Image CharacterImage;
    public Text TurnsLeft;

    public void ReduceTurns()
    {
        TurnsLeft.text = (int.Parse(TurnsLeft.text) - 1).ToString();
    }
    public void SetTurnsLeft(int amount)
    {
        TurnsLeft.gameObject.SetActive(true);
        TurnsLeft.text = amount.ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        FindObjectOfType<MyCameraController>().LookAt(characterLinkedTo.transform);
        FindObjectOfType<PlayerController>().SelectEnemyViaPortait((Character)characterLinkedTo);
    }

     public void SetCharacter(Entity character)
    {
        characterLinkedTo = character;
        SetCharacterImage(character.characterIcon);
    }

    void SetCharacterImage(Sprite sprite)
    {
        CharacterImage.sprite = sprite;
    }

    public void SetPosition()
    {
        transform.localPosition = Vector3.zero;
    }

    public void SetAsActivePositionSize()
    {
        transform.localScale = new Vector3(1.268284f, 1.268284f, 1.268284f);
    }

    bool Shift = false;
    public void SetShift() { Shift = true; }

    public void Grow()
    {
        StartCoroutine("Growing");
    }

    IEnumerator Growing()
    {
        while(transform.localScale.x <= 1.268284f)
        {
            transform.localScale = new Vector3(transform.localScale.x + .01f, transform.localScale.y + .01f, transform.localScale.z + .01f);
            yield return new WaitForEndOfFrame();
        }
    }

    public void ShrinkAndDestroy()
    {
        StartCoroutine("ShrinkingAndDestroy");
    }

    IEnumerator ShrinkingAndDestroy()
    {
        while (transform.localScale.x > 1f)
        {
            transform.localScale = new Vector3(transform.localScale.x - .01f, transform.localScale.y - .01f, transform.localScale.z - .01f);
            yield return new WaitForEndOfFrame();
        }
        Destroy(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        if (Shift)
        {
            if (transform.localPosition.magnitude > 1f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, .04f);
            }
            else
            {
                Shift = false;
            }
        }
	}
}
