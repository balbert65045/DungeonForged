using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character {

    public int EnemyChallengeRating = 1;
    public float XpOnDeath = 10;
    public int GoldHolding = 5;

    public string CharacterName;
    public Sprite enemySprite;

    private int HealAmount = 0;
    private int ShieldAmount = 0;

    private Hex TargetHex;
    PlayerCharacter ClosestCharacter;

    ActionSet currentActionSet;

    public EnemyGroup GetGroup()
    {
        return FindObjectOfType<EnemyController>().GetGroupFromCharacter(this);
    }

    void Start()
    {
        myHealthBar = GetComponentInChildren<HealthBar>();
        maxHealth = health;
        if (myHealthBar != null)
        {
            myHealthBar.CreateHealthBar(health, maxHealth);
        }
        FindObjectOfType<EnemyController>().LinkSpawnedCharacter(this);
        Shield(baseArmor, this);
        HexMap = FindObjectOfType<HexMapController>();
        GetComponent<CharacterAnimationController>().SwitchCombatState(true);
        ShowNewAction();
        FindObjectOfType<TurnOrder>().AddCharacter(this);
    }

    public void ShowNewAction()
    {
        currentActionSet = GetGroup().GetRandomActionSet();
        myHealthBar.ShowActions(currentActionSet.Actions, myDeBuffs);
    }

    //OTHER//

    void UnShowPath()
    {
        hexVisualizer.UnhighlightHexes();
    }

    public void ResetBuffs() { resetShield(GetArmor()); }

    public override void finishedTakingDamage()
    {
        base.finishedTakingDamage();
    }

    //Callbacks
    public override void Die()
    {
        GetGroup().UnLinkCharacterToGroup(this);
        FindObjectOfType<ObjectiveArea>().EnemyDied();
        HexOn.goldHolding += GoldHolding;
        HexOn.ShowMoney();
        if (FindObjectOfType<TurnOrder>().GetCurrentCharacter() == this) { finishedActions(); }
        base.Die();
    }

    public override void FinishedPerformingShielding()
    {
        UnShowPath();
        finishedAction();
    }

    public override void FinishedPerformingHealing()
    {
        UnShowPath();
        finishedAction();
    }

    public override void FinishedAttacking(bool dead = false)
    {
        base.FinishedAttacking();
        if (charactersAttackingAt == null || CharactersFinishedTakingDamage >= charactersAttackingAt.Count) 
        { 
            if (!dead) { StartCoroutine("FinisheAttackDelay"); }
            else { finishedAction(); }
        }
    }

    IEnumerator FinisheAttackDelay()
    {
        yield return new WaitForSeconds(.5f);
        finishedAction();
    }

    public override void FinishedMoving(Hex hex, bool fight = false, Hex HexMovingFrom = null)
    {
        if (HexMovingTo != null) { HexMovingTo.CharacterArrivedAtHex(); }
        UnShowPath();
        LinktoHex(hex);
        finishedAction();
    }


    //Actions//

    void finishedAction()
    {
        myHealthBar.RemoveAction();
        actionSetIndex++;
        if (actionSetIndex >= currentActionSet.Actions.Count) { finishedActions(); }
        else { DoNextAction(); }
    }

    void finishedActions()
    {
        UnShowPath();
        EndTurn();
        FindObjectOfType<EnemyController>().CharacterEndedTurn();
    }

    int actionSetIndex = 0;
    public void PerformActionSet()
    {
        ClosestCharacter = null;
        actionSetIndex = 0;
        BeginTurn();
        if (myDeBuffs.Contains(DeBuff.Stun)) { 
            finishedActions();
            return;
        }
        if (health <= 0) { return; }
        DoNextAction();
    }

    void DoNextAction()
    {
        if (FindObjectsOfType<PlayerCharacter>().Length == 0) { return; }
        Action CurrentAction = currentActionSet.Actions[actionSetIndex];
        if (CurrentAction.thisActionType == ActionType.Movement)
        {
            UseMove(CurrentAction);
        }
        else if(CurrentAction.thisActionType == ActionType.Attack)
        {
            UseAttack(CurrentAction);
        }
        else
        {
            GetComponent<CharacterAnimationController>().DoBuff(ActionType.Shield, CurrentAction.thisAOE.Damage, new List<Character>() { this });
        }
    }

    DeBuff deBuffApplying;
    void UseAttack(Action action)
    {
        if (ClosestCharacter == null) { ClosestCharacter = BreadthFirst(); }
        TargetHex = ClosestCharacter.HexOn;
        GetAttackHexes(action.Range);
        if (HexInActionRange(TargetHex) && !myDeBuffs.Contains(DeBuff.Disarm)) {
            CurrentAttack = action.thisAOE.Damage;
            deBuffApplying = action.thisDeBuff;
            StartCoroutine("ShowAttack");
        }
        else { FinishedAttacking(); }
    }

    void UseMove(Action action)
    {
        //Adjust this to check if using positive action or negative action and move to the target looking for
        CurrentAttackRange = 1;
        for (int i = actionSetIndex + 1; i < currentActionSet.Actions.Count; i++)
        {
            if (currentActionSet.Actions[i].thisActionType == ActionType.Attack)
            {
                CurrentAttackRange = currentActionSet.Actions[i].Range;
            }
        }
        ClosestCharacter = BreadthFirst();
        if (ClosestCharacter == null)
        {
            Debug.Log("No character to attack");
            finishedAction();
            return;
        }
        TargetHex = ClosestCharacter.HexOn;
        GetAttackHexes(CurrentAttackRange);
        if (HexInActionRange(TargetHex) || myDeBuffs.Contains(DeBuff.Immobelized))
        {
            finishedAction();
        } else
        {
            CurrentMoveRange = action.Range;
            if (myDeBuffs.Contains(DeBuff.Slow)) { CurrentMoveRange--; }
            StartCoroutine("Move");
        }
    }

    IEnumerator Move()
    {
        List<Node> nodePath = getPathToTargettoAttack(TargetHex, CurrentAttackRange);
        if (nodePath.Count == 0) { FinishedMoving(HexOn); }
        else
        {
            hexVisualizer.HighlightSelectionHex(HexOn);
            yield return new WaitForSeconds(.5f);
            int distanceToTravel = nodePath.Count;
            if (nodePath.Count > CurrentMoveRange) { distanceToTravel = CurrentMoveRange; }
            Hex hexToMoveTo = null;
            //Loop and eliminate last node if it has something on it
            for (int i = distanceToTravel - 1; i > 0; i--)
            {
                if (nodePath[i].NodeHex.EntityHolding != null){ distanceToTravel--;}
                else { break; }
            }
            hexToMoveTo = nodePath[distanceToTravel - 1].NodeHex;

            if (nodePath.Count > distanceToTravel) { nodePath = nodePath.GetRange(0, distanceToTravel); }
            if (hexToMoveTo != null) { MoveOnPathFound(hexToMoveTo, nodePath); }
            else { Debug.LogWarning("No hex to move to"); }
        }
        yield return null;
    }

    //Change this to actual heal action
    IEnumerator DoHealThenOtherActions()
    {
        //hexVisualizer.HighlightHealRangeHex(HexOn);
        //if (health != maxHealth)
        //{
        //    yield return new WaitForSeconds(.5f);
        //    //TODO Change this to be able to allow someone else to heal
        //    Heal(HealAmount, this);
        //}
        //else
        //{
        //    yield return new WaitForSeconds(.1f);
        //    FinishedHealing();
        //}
        yield return null;
    }

    PlayerCharacter BreadthFirst()
    {
        List<Hex> frontier = new List<Hex>();
        frontier.Add(HexOn);
        List<Hex> visited = new List<Hex>();
        return BreadthFirstSearch(frontier, visited);
    }


    PlayerCharacter BreadthFirstSearch(List<Hex> Frontier, List<Hex> Visited)
    {
        if (Frontier.Count == 0) { return null; }
        List<Hex> newFrontier = new List<Hex>();
        foreach (Hex current in Frontier)
        {
            foreach (Node next in HexMap.GetRealNeighbors(current.HexNode))
            {
                if (!Visited.Contains(next.NodeHex))
                {
                    if (next.NodeHex.EntityHolding != null && next.NodeHex.EntityHolding.GetComponent<PlayerCharacter>() != null)
                    {
                        return next.NodeHex.EntityHolding.GetComponent<PlayerCharacter>();
                    }
                    newFrontier.Add(next.NodeHex);
                    Visited.Add(next.NodeHex);
                }
            }
        }
        return BreadthFirstSearch(newFrontier, Visited);
    }

    void MoveOnPathFound(Hex hexMovingTo, List<Node> nodePath)
    {
        hexMovingTo.CharacterMovingToHex();
        Hex HexCurrentlyOn = HexOn;
        RemoveLinkFromHex();
        if (hexMovingTo == HexOn) { FinishedMoving(hexMovingTo); }
        Node NodeToMoveTo = nodePath[0];
        nodePath.Remove(NodeToMoveTo);
        GetComponent<CharacterAnimationController>().MoveTowards(NodeToMoveTo.NodeHex, nodePath, HexCurrentlyOn);
    }

    IEnumerator ShowAttack()
    {
        hexVisualizer.HighlightAttackRangeHex(HexOn);
        yield return new WaitForSeconds(.5f);
        List<Character> charactersAttacking = new List<Character>();
        charactersAttacking.Add(TargetHex.EntityHolding.GetComponent<Character>());
        foreach(Character character in charactersAttacking)
        {
            hexVisualizer.HighlightAttackAreaHex(character.HexOn);
        }
        Attack(CurrentAttack, deBuffApplying, charactersAttacking.ToArray());
    }

    public void GetAttackHexes(int Range)
    {
        SetCurrentAttackRange(Range);
        List<Node> nodes = HexMap.GetNodesInLOS(HexOn.HexNode, Range);
        NodesInActionRange.Clear();
        foreach (Node node in nodes)
        {
            if (!node.Shown) { continue; }
            NodesInActionRange.Add(node);
        }
    }

    //PATHING
    public List<Node> getPathToTargettoAttack(Hex target, int range)
    {
        List<Node> possibleNodes = HexMap.GetNodesInLOS(target.HexNode, range);
        if (possibleNodes.Contains(HexOn.HexNode)){ return new List<Node> { HexOn.HexNode }; }
        Node ClosestNode = FindObjectOfType<AStar>().DiskatasWithArea(HexOn.HexNode, possibleNodes, myCT);
        if (ClosestNode != null) { return FindObjectOfType<AStar>().FindPathWithMoveLimit(HexOn.HexNode, ClosestNode, myCT, CurrentMoveRange); }
        else { return new List<Node>(); }
    }
}
