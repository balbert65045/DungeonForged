using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyChest : Entity
{

    public int goldHolding = 20;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<TurnOrder>().AddCharacter(GetComponent<Entity>());
    }

    public void TickDownTimer()
    {
        TurnsLeft--;
        StartCoroutine("TickDown");
    }

    public void Use()
    {
        FindObjectOfType<TurnOrder>().CharacterRemoved(this);
        Destroy(gameObject);
    }

    IEnumerator TickDown()
    {
        Hex HexOn = GetComponent<Entity>().HexOn;
        FindObjectOfType<MyCameraController>().SetTarget(transform);
        yield return new WaitForSeconds(.5f);
        bool died = false;
        if (TurnsLeft == 0)
        {
            died = true;
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            FindObjectOfType<TurnOrder>().SetTurnsLeft(this);
        }
        yield return new WaitForSeconds(.5f);
        FindObjectOfType<EnemyController>().CharacterEndedTurn();
        if (died)
        {
            FindObjectOfType<TurnOrder>().CharacterRemoved(this);
            Destroy(gameObject);
        }
    }
}
