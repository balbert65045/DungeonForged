using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour {


    Rigidbody myRigidbody;
    Animator myAnimator;
    Vector3 movePosition;
    Hex hexMovingTo;
    Hex HexMovingFrom;
    List<Node> nodesMovingOn;

    Character myCharacter;
    bool MovingToFight = false;
    public void SetMovingToFight(bool value) { MovingToFight = value; }

    int GoldAmount;
    public GameObject RightHandWeapon;

    ActionType ActionPerforming;
    int AmountOfAction;
    int DurationOfAction;
    List<Character> CharactersAffected;
    // Use this for initialization
    void Awake() {
        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        myRigidbody.constraints = RigidbodyConstraints.FreezePosition;
        myCharacter = GetComponent<Character>();
    }

    public void HideWeapon()
    {
        if (RightHandWeapon != null) { RightHandWeapon.SetActive(false); }
    }

    public void RevealWeapon()
    {
        if (RightHandWeapon != null) { RightHandWeapon.SetActive(true); }
    }

    public void SetStealthState(bool stealthed)
    {
        myAnimator.SetBool("Stealthed", stealthed);
    }

    public void SwitchCombatState(bool inCombat)
    {
        myAnimator.SetBool("InCombat", inCombat);
    }

    public void FinishedDying()
    {
        GetComponent<Character>().Die();
    }

    public void Die()
    {
        myAnimator.SetTrigger("Die");
    }

    public void GetHit()
    {
        myRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        myAnimator.SetTrigger("Hit");
    }

    public void DoPickup(int amount)
    {
        GoldAmount = amount;
        myAnimator.SetTrigger("PickUp");
    }

    public void PickUp()
    {
        GetComponent<PlayerCharacter>().CollectGold(GoldAmount);
    }

    public void DoBuff(ActionType action, int Amount, int Duration, List<Character> charactersEffecting)
    {
       ActionPerforming = action;
       AmountOfAction = Amount;
       DurationOfAction = Duration;
       CharactersAffected = charactersEffecting;
       myAnimator.SetTrigger("Buff");
    }

    public void Buff()
    {
        switch (ActionPerforming)
        {
            case ActionType.Heal:
                GetComponent<Character>().PerformHeal(AmountOfAction, CharactersAffected);
                break;
            case ActionType.Shield:
                GetComponent<Character>().PerformShield(AmountOfAction, CharactersAffected);
                break;
            case ActionType.BuffArmor:
                GetComponent<Character>().GiveBuff(AmountOfAction, DurationOfAction, BuffType.Armor, CharactersAffected);
                break;
            case ActionType.BuffAttack:
                GetComponent<Character>().GiveBuff(AmountOfAction, DurationOfAction, BuffType.Strength, CharactersAffected);
                break;
            case ActionType.BuffMove:
                GetComponent<Character>().GiveBuff(AmountOfAction, DurationOfAction, BuffType.Agility, CharactersAffected);
                break;
            case ActionType.BuffRange:
                GetComponent<Character>().GiveBuff(AmountOfAction, DurationOfAction, BuffType.Dexterity, CharactersAffected);
                break;
        }
    }

    public void Hit()
    {
        myCharacter.SetAttacking(false);
        myCharacter.HitOpponent();
    }

    public void Attack()
    {
        if (!myCharacter.GetAttacking())
        {
            myAnimator.SetTrigger("Attack");
            myCharacter.SetAttacking(true);
        }
    }

    public void Stop()
    {
        nodesMovingOn.Clear();
    }

    float oldDifference;
    public void MoveTowards(Hex hex, List<Node> nextNodes, Hex hexMovingFrom)
    {
        oldDifference = 10000f;
        myCharacter.SetMoving(true);
        transform.LookAt(new Vector3(hex.transform.position.x, transform.position.y, hex.transform.position.z));
        myAnimator.SetBool("moving", true);
        hexMovingTo = hex;
        HexMovingFrom = hexMovingFrom;
        nodesMovingOn = nextNodes;
        myRigidbody.constraints = RigidbodyConstraints.None;
    }

	void Update () {
		if (myCharacter.GetMoving())
        {
            movePosition = new Vector3(hexMovingTo.transform.position.x, transform.position.y, hexMovingTo.transform.position.z);
            float difference = (transform.position - movePosition).magnitude;
            if (difference <= .3f)
            {
                MoveToNextPosition();
            }
            else
            {
                oldDifference = difference;
            }
        }
	}

    void MoveToNextPosition()
    {
        myCharacter.LinktoHex(hexMovingTo);
        if (myCharacter.GetStealthed()) { myCharacter.ReduceStealthDuration(); }
        myCharacter.ShowViewArea(hexMovingTo, GetComponent<Character>().ViewDistance);

        //if (!myCharacter.GetStealthed()) { myCharacter.CheckToFight(); }

        if (nodesMovingOn.Count == 0)
        {
            myCharacter.SetMoving(false);
            myRigidbody.constraints = RigidbodyConstraints.FreezePosition;
            myAnimator.SetBool("moving", false);
            myCharacter.FinishedMoving(hexMovingTo, MovingToFight, HexMovingFrom);
            MovingToFight = false;
        }
        else
        {
            myCharacter.MovingOnPath();
            Node NextHex = nodesMovingOn[0];
            nodesMovingOn.Remove(NextHex);
            MoveTowards(NextHex.NodeHex, nodesMovingOn, hexMovingTo);
        }
    }
}
