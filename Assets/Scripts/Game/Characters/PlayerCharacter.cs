using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerCharacterType
{
    All = 1,
    Knight = 2,
    Barbarian = 3,
    Mage = 4,
    Crossbow = 5,
}

public class PlayerCharacter : Character
{
    public int CharacterLevel = 1;
    public float CurrentXP = 0;
    public float XpUntilNextLevel { get { return CharacterLevel * 100 + (CharacterLevel - 1) * 50; } }

    public PlayerCharacterType myType;

    public Sprite characterSymbol;
    public string CharacterName;

    public GameObject MyNewDeckPrefab;
    public GameObject MyNewDeck;
    public NewHand GetMyNewHand() { return MyNewDeck.GetComponentInChildren<NewHand>(); }
    public StagingArea GetMyStagingArea() { return MyNewDeck.GetComponentInChildren<StagingArea>(); }
    public DrawPile GetMyDrawPile() { return MyNewDeck.GetComponentInChildren<DrawPile>(); }
    public DiscardPile GetMyDiscardPile() { return MyNewDeck.GetComponentInChildren<DiscardPile>(); }

    private PlayerController playerController;

    public Door doorToOpen;
    public void SetDoorToOpen(Door door) { doorToOpen = door; }

    private CardChest ChestToOpen;
    public void SetChestToOpen(CardChest chest) { ChestToOpen = chest; }

    bool EntityIsOnPositionAndMoving(Hex hex)
    {
        if (hex.EntityHolding.GetComponent<PlayerCharacter>() != null)
        {
            PlayerCharacter character = hex.EntityHolding.GetComponent<PlayerCharacter>();
            return character.GetMoving();
        }
        return false;
    }

    public override void MoveOnPath(Hex hex)
    {
        Hex HexMovingFrom = HexOn;
        List<Node> nodes = GetPath(hex.HexNode);
        List<Node> totalPath = new List<Node>();
        foreach(Node node in nodes) { totalPath.Add(node); }
        if (totalPath.Count == 0) {
            FinishedMoving(HexOn, true);
            return;
        }

        NodesInWalkingDistance.Clear();
        HexMovingTo = hex;
        HexMovingTo.CharacterMovingToHex();
        RemoveLinkFromHex();
        if (hex == HexOn) { FinishedMoving(hex); }
        if (nodes[0] == null) { return; }
        Node HexToMoveTo = nodes[0];
        playerController.AddHexMovingTo(nodes[nodes.Count - 1].NodeHex);
        nodes.Remove(HexToMoveTo);

        GetComponent<CharacterAnimationController>().MoveTowards(HexToMoveTo.NodeHex, nodes, HexMovingFrom);
    }

    public override void ShowMoveDistance(int moveRange)
    {
        CurrentMoveRange = moveRange;
        if (MyDeBuffsHas(DeBuffType.Immobelized)) { CurrentMoveRange = 0; }
        else if (MyDeBuffsHas(DeBuffType.Slow)) { CurrentMoveRange--; }
        List<Node> nodesInDistance = aStar.Diskatas(HexOn.HexNode, CurrentMoveRange, myCT);
        NodesInWalkingDistance.Clear();
        List<Node> nodesInArea = new List<Node>();
        foreach (Node node in nodesInDistance)
        {
            if (!node.Shown || node.edge) { continue; }
            nodesInArea.Add(node);
            if (node.NodeHex.EntityHolding != null) { continue; }
            NodesInWalkingDistance.Add(node);
        }
        ShowHexesOnEdgeOfRange(nodesInArea);
    }

    public void ShowHexesOnEdgeOfRange(List<Node> nodes)
    {
        List<Vector3> points = HexMap.GetHexesSurrounding(HexOn.HexNode, nodes);
        playerController.CreateArea(points, ActionType.Movement);
    }

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        myHealthBar = GetComponentInChildren<HealthBar>();
        if (FindObjectOfType<PlayersDecks>() == null) { return; }
        if (FindObjectOfType<NewGroupStorage>().GetCurrentHealth(CharacterName) == 0)
        {
            Destroy(this.gameObject);
            return;
        }
        BuildNewDeck();
        if (myHealthBar != null)
        {
            myHealthBar.CreateHealthBar(health, maxHealth);
            if (hasArtifact(ArtifactType.ShieldStart))
            {
                myHealthBar.SetShield(10);
            }
            if (hasArtifact(ArtifactType.AttackStart))
            {
                int amount = 5;
                DeBuff newDeBuff = new DeBuff(DeBuffType.IncreaseAttack, amount);
                if (!MyDeBuffsHas(DeBuffType.IncreaseAttack))
                {
                    myDeBuffs.Add(newDeBuff);
                    myHealthBar.ShowDeBuff(newDeBuff);
                }
                else
                {
                    DeBuff myDeBuff = FindDeBuff(DeBuffType.IncreaseAttack);
                    int newAmount = myDeBuff.Amount + amount;
                    SetDeBuffAmount(newAmount, myDeBuff);
                    myHealthBar.SetDeBuffAmount(myDeBuff.thisDeBuffType, newAmount);
                }
            }
        }
        GetComponent<CharacterAnimationController>().SwitchCombatState(true);
        FindObjectOfType<TurnOrder>().AddCharacter(this);
    }

    void BuildNewDeck()
    {
        Transform deckParent = FindObjectOfType<PlayersDecks>().transform;
        MyNewDeck = Instantiate(MyNewDeckPrefab, deckParent);
        MyNewDeck.name = name + " Deck";
        health = FindObjectOfType<NewGroupStorage>().GetCurrentHealth(CharacterName);
        maxHealth = FindObjectOfType<NewGroupStorage>().GetMaxHealth(CharacterName);
        List<GameObject> cards = FindObjectOfType<NewGroupStorage>().GetDeck(CharacterName);
        MyNewDeck.GetComponentInChildren<DrawPile>().SetInitialDeck(cards);
        FindObjectOfType<CharacterCardViewers>().AddCharacter(this);
        MyNewDeck.SetActive(false);
    }

    //VIEW//
    public void ShowPath(Node NodeToMoveTo)
    {
        Node StartNode = HexOn.HexNode;
        Node EndNode = NodeToMoveTo;
        List<Node> NodePath = FindObjectOfType<AStar>().FindPath(StartNode, EndNode, myCT);
        foreach (Node node in NodePath)
        {
            hexVisualizer.HighlightMoveRangeHex(node.NodeHex);
        }
    }

    public void Selected()
    {
    }

    public void PickUpGold(int amount)
    {
        GetComponent<CharacterAnimationController>().DoPickup(amount);
    }

    public override void FinishedMoving(Hex hex, bool fight = false, Hex hexMovingFrom = null)
    {
        playerController.ClearHexesMovingTo();
        if (HexMovingTo != null) { HexMovingTo.CharacterArrivedAtHex(); }
        int goldPickedUp = HexMovingTo.PickUpMoney();
        if (goldPickedUp > 0)
        {
            GetComponent<CharacterAnimationController>().DoPickup(goldPickedUp);
        }
        if (doorToOpen != null)
        {
            StartCoroutine("OpenDoor");
            doorToOpen = null;
            return;
        }
        else if (ChestToOpen != null && !fight)
        {
            ChestToOpen.OpenChest();
            ChestToOpen = null;
        }
        FindObjectOfType<PlayerController>().FinishedMoving(this);
    }

    public void CollectGold(int amount)
    {
        playerController.AddGold(amount);
        myHealthBar.AddGold(amount);
    }

    public override void FinishedPerformingBuff()
    {
        FindObjectOfType<PlayerController>().FinishedBuffing(this);
    }

    public override void FinishedAttacking(bool dead = false)
    {
        base.FinishedAttacking();
        if (CharactersFinishedTakingDamage >= charactersAttackingAt.Count || charactersAttackingAt.Count == 0) {
            if (!dead) { StartCoroutine("FinisheAttackDelay"); }
            else { FindObjectOfType<PlayerController>().FinishedAttacking(this); }
        }
    }
    IEnumerator FinisheAttackDelay()
    {
        yield return new WaitForSeconds(.5f);
        FindObjectOfType<PlayerController>().FinishedAttacking(this);
    }

    public override void FinishedPerformingHealing()
    {
        FindObjectOfType<PlayerController>().FinishedHealing(this);
    }

    public override void FinishedPerformingShielding()
    {
        FindObjectOfType<PlayerController>().FinishedHealing(this);
    }

    //DOOR
    public IEnumerator OpenDoor()
    {
        yield return new WaitForSeconds(.1f);
        if (HexOn.GetComponent<Door>() != null)
        {
            HexOn.GetComponent<Door>().OpenHexes(HexOn.HexNode.RoomName[0]);
        }
        FindObjectOfType<PlayerController>().FinishedMoving(this);
        yield return null;
    }

    //Damage
    public override void GetHit()
    {
        base.GetHit();
    }

    public override void Die()
    {
        FindObjectOfType<PlayerController>().CharacterDied(this);
        base.Die();
    }

    float XpGaining = 0;

    public void UpdateHealth(int amount)
    {
        FindObjectOfType<CCSSelectionButton>().SetCurrentHp(amount);
    }

    public void GainXP(float XP)
    {
        if (XpGaining == 0)
        {
            StartCoroutine("GainXp");
        }
        XpGaining += XP;
    }

    IEnumerator GainXp()
    {
        yield return new WaitForSeconds(.1f);
        CurrentXP += XpGaining;
        if (CurrentXP > XpUntilNextLevel)
        {
            CurrentXP = CurrentXP - XpUntilNextLevel;
            CharacterLevel++;
           // myHealthBar.LevelUpAndGainXP(CurrentXP / XpUntilNextLevel);
        }
        else
        {
          //  myHealthBar.GainXP(CurrentXP / XpUntilNextLevel);
        }
        XpGaining = 0;
    }

    public override void SlayedEnemy(float XP)
    {
        FindObjectOfType<PlayerController>().EnemyVanquished(XP);
        base.SlayedEnemy(XP);
    }
}
