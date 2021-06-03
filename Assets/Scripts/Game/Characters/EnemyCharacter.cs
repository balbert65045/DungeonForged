using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character {


    public GameObject PreviewCharacter;
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
        if (baseArmor > 0) { myHealthBar.AddShield(baseArmor); }
        HexMap = FindObjectOfType<HexMapController>();
        GetComponent<CharacterAnimationController>().SwitchCombatState(true);
        //ShowNewAction();
        if (!FindObjectOfType<EnemyController>().GameInPlay)
        {
            FindObjectOfType<TurnOrder>().AddCharacter(this);
        }
        else
        {
            FindObjectOfType<TurnOrder>().NewCharacter(this);
        }
    }

    public void ShowNewAction()
    {
        ClosestCharacter = BreadthFirst();
        int distance = FindObjectOfType<HexMapController>().GetDistance(ClosestCharacter.HexOn.HexNode, HexOn.HexNode);
        if (distance > GetGroup().MaxIdealRange) { currentActionSet = GetGroup().GetRandomEngageActionSet(); }
        else if (distance < GetGroup().MinIdealRange) { currentActionSet = GetGroup().GetRandomDisengageActionSet(); }
        else { currentActionSet = GetGroup().GetRandomActionSet(currentActionSet); }
        myHealthBar.ShowActions(currentActionSet.Actions, myDeBuffs);
        if (currentActionSet.Actions[0].thisActionType == ActionType.Attack && currentActionSet.Actions[0].thisAOE.thisAOEType != AOEType.SingleTarget)
        {
            attackAOEType = currentActionSet.Actions[0].thisAOE.thisAOEType;
            AoeTargetHex = ClosestCharacter.HexOn;
            Node nodeInBestDirection = null;
            if (currentActionSet.Actions[0].thisAOE.thisAOEType == AOEType.Circle)
            {
                nodeInBestDirection = AoeTargetHex.HexNode;
            }
            else
            {
                nodeInBestDirection = HexMap.GetNodeInDirection(HexMap.GetBestDirection(HexOn.HexNode, AoeTargetHex.HexNode, currentActionSet.Actions[0].thisAOE.thisAOEType), HexOn.HexNode);
            }
            transform.LookAt(new Vector3(AoeTargetHex.transform.position.x, transform.position.y, AoeTargetHex.transform.position.z));
            List<Node> nodesInAOE = FindObjectOfType<HexMapController>().GetAOE(currentActionSet.Actions[0].thisAOE.thisAOEType, HexOn.HexNode, nodeInBestDirection);
            foreach(Node node in nodesInAOE)
            {
                node.NodeHex.SetToAOE();
            }
        }
        else
        {
            attackAOEType = AOEType.SingleTarget;
        }
    }

    public void Selected()
    {
        previewMoveHexes.Clear();
        for (int i = 0; i < currentActionSet.Actions.Count; i++)
        {
            int range = currentActionSet.Actions[i].Range;
            if (currentActionSet.Actions[i].thisActionType == ActionType.Movement)
            {
                ShowMove(range);
            }
            else if (currentActionSet.Actions[i].thisActionType == ActionType.Attack)
            {
                attackAOEType = currentActionSet.Actions[i].thisAOE.thisAOEType;
                previewAttackRange = range;
                ShowAttackOnHexOn();
            }
        }
    }

    Hex AoeTargetHex;
    AOEType attackAOEType = AOEType.SingleTarget;
    int previewAttackRange = 0;
    GameObject Preview = null;

    public void RemoveAreas()
    {
        if (Preview != null) { Destroy(Preview); }
        FindObjectOfType<EnemyController>().RemoveAreas();
    }

    public void VisualizeAttack(Hex hex)
    {
        if (Preview != null) { Destroy(Preview); }
        if (hex.EntityHolding != null) { return; }
        Preview = hex.SpawnPreview(PreviewCharacter);
        ShowPreviewAttack(hex);
    }

    public void ShowAttackOnHexOn()
    {
        if (Preview != null) { Destroy(Preview); }
        ShowPreviewAttack(HexOn);
    }

    public void ShowPreviewAttack( Hex hex)
    {
        if (previewAttackRange == 0 && attackAOEType == AOEType.SingleTarget) { return; }
        List<Node> nodes = null;
        if (attackAOEType == AOEType.SingleTarget) { nodes = HexMap.GetNodesInLOS(hex.HexNode, previewAttackRange); }
        else { nodes = HexMap.GetEnemyAOE(attackAOEType, HexOn.HexNode, AoeTargetHex.HexNode); }
        List<Node> nodesInRange = new List<Node>();
        if (MyDeBuffsHas(DeBuffType.Disarm)) { }
        else
        {
            foreach (Node node in nodes)
            {
                if (!node.Shown) { continue; }
                if (node.edge) { continue; }
                nodesInRange.Add(node);
            }
            if (nodesInRange.Contains(hex.HexNode)) { nodesInRange.Remove(hex.HexNode); }
        }

        //Have to do this because of silly variable being added
        List<Node> nodesToSurround = new List<Node>();
        foreach (Node node in nodesInRange) { nodesToSurround.Add(node); }
        if ((NodesInActionRange.Count == 0 && attackAOEType != AOEType.Circle) || attackAOEType == AOEType.Surounding) { nodesToSurround.Add(hex.HexNode); }

        List<Vector3> points = new List<Vector3>();
        if (attackAOEType == AOEType.Circle)
        {
            hex = AoeTargetHex;
            nodesToSurround.Remove(hex.HexNode);
        }
        points = HexMap.GetHexesSurrounding(hex.HexNode, nodesToSurround);
        FindObjectOfType<EnemyController>().CreateOtherArea(points, ActionType.Attack);
    }

    public List<Node> previewMoveHexes;
    void ShowMove(int moveRange)
    {
        if (MyDeBuffsHas(DeBuffType.Immobelized)) { moveRange = 0; }
        else if (MyDeBuffsHas(DeBuffType.Slow)) { moveRange--; }
        List<Node> nodesInDistance = aStar.Diskatas(HexOn.HexNode, moveRange, myCT);
        previewMoveHexes = new List<Node>();
        foreach (Node node in nodesInDistance)
        {
            if (!node.Shown || node.edge) { continue; }
            previewMoveHexes.Add(node);
            if (node.NodeHex.EntityHolding != null) { continue; }
        }
        previewMoveHexes.Add(HexOn.HexNode);
        ShowMoveArea(previewMoveHexes);
    }

    public void ShowMoveArea(List<Node> nodes)
    {
        List<Vector3> points = HexMap.GetHexesSurrounding(HexOn.HexNode, nodes);
        FindObjectOfType<EnemyController>().CreateMoveArea(points, ActionType.Movement);
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
        GetAttackHexes(1);
        if (attackAOEType != AOEType.SingleTarget)
        {
            foreach (Node node in NodesInActionRange)
            {
                node.NodeHex.UnSetAOE();
            }
        }
        FindObjectOfType<ObjectiveArea>().EnemyDied();
        //HexOn.goldHolding += GoldHolding;
        //HexOn.ShowMoney();
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
        if (attackAOEType != AOEType.SingleTarget)
        {
            foreach(Node node in NodesInActionRange)
            {
                node.NodeHex.UnSetAOE();
            }
        }
        if (charactersAttackingAt == null || charactersAttackingAt.Count == 0 || CharactersFinishedTakingDamage >= charactersAttackingAt.Count) 
        {
            charactersAttackingAt.Clear();
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
        if (MyDeBuffsHas(DeBuffType.Stun)) { 
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
            GetComponent<CharacterAnimationController>().DoBuff(ActionType.Shield, CurrentAction.thisAOE.Damage, CurrentAction.thisDeBuff, new List<Character>() { this });
        }
    }

    DeBuff deBuffApplying;
    bool RangedAttack = false;
    string TriggerAnimation;
    void UseAttack(Action action)
    {
        if (ClosestCharacter == null) { ClosestCharacter = BreadthFirst(); }
        TargetHex = ClosestCharacter.HexOn;
        attackAOEType = action.thisAOE.thisAOEType;
        GetAttackHexes(action.Range);
        if (HexInActionRange(TargetHex) && !MyDeBuffsHas(DeBuffType.Disarm)) {
            CurrentAttack = action.thisAOE.Damage;
            deBuffApplying = action.thisDeBuff;
            RangedAttack = action.Range > 1;
            TriggerAnimation = action.TriggerAnimation;
            StartCoroutine("ShowAttack");
        }
        else if (attackAOEType != AOEType.SingleTarget)
        {
            TriggerAnimation = action.TriggerAnimation;
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
            finishedAction();
            return;
        }
        TargetHex = ClosestCharacter.HexOn;
        GetAttackHexes(CurrentAttackRange);
        if ((HexInActionRange(TargetHex) && !TakingDisadvantageWithRangedAttack()) || MyDeBuffsHas(DeBuffType.Immobelized))
        {
            finishedAction();
        } else
        {
            CurrentMoveRange = action.Range;
            if (MyDeBuffsHas(DeBuffType.Slow)) { CurrentMoveRange--; }
            StartCoroutine("Move");
        }
    }

    bool TakingDisadvantageWithRangedAttack()
    {
        if (CurrentAttackRange > 1 && InMeleeRange(ClosestCharacter))
        {
            return true;
        }
        return false;
    }

    IEnumerator Move()
    {
        List<Node> nodePath = null;
        if (TakingDisadvantageWithRangedAttack()) { 
            nodePath = MoveAwayFromTarget(TargetHex);
            Debug.Log(nodePath.Count);
        }
        else { nodePath = getPathToTargettoAttack(TargetHex, CurrentAttackRange); }
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
        if (NodesInActionRange.Contains(TargetHex.HexNode)) 
        {
            charactersAttacking.Add(TargetHex.EntityHolding.GetComponent<Character>());
            foreach (Character character in charactersAttacking)
            {
                hexVisualizer.HighlightAttackAreaHex(character.HexOn);
            }
        }
        if (attackAOEType != AOEType.SingleTarget)
        {
            foreach(Node node in NodesInActionRange)
            {
                hexVisualizer.HighlightAttackAreaHex(node.NodeHex);
            }
        }
        if (charactersAttacking.Count == 0)
        {
            transform.LookAt(new Vector3(AoeTargetHex.transform.position.x, transform.position.y, AoeTargetHex.transform.position.z));
            GetComponent<CharacterAnimationController>().AnimationTrigger = TriggerAnimation;
            MakeAttack();
        }
        else
        {
            Attack(CurrentAttack, deBuffApplying, charactersAttacking.ToArray(), RangedAttack, TriggerAnimation);
        }
    }

    public void GetAttackHexes(int Range)
    {
        SetCurrentAttackRange(Range);
        List<Node> nodes = null;
        if (attackAOEType == AOEType.SingleTarget) { nodes = HexMap.GetNodesInLOS(HexOn.HexNode, Range); }
        else { nodes = HexMap.GetEnemyAOE(attackAOEType, HexOn.HexNode, AoeTargetHex.HexNode); }
        NodesInActionRange.Clear();
        foreach (Node node in nodes)
        {
            if (!node.Shown) { continue; }
            NodesInActionRange.Add(node);
        }
    }

    Node BreadthFirstAway(Node target)
    {
        List<Hex> frontier = new List<Hex>();
        frontier.Add(HexOn);
        List<Hex> visited = new List<Hex>();
        return BreadthFirstAwaySearch(frontier, visited, target);
    }


    Node BreadthFirstAwaySearch(List<Hex> Frontier, List<Hex> Visited, Node target)
    {
        if (Frontier.Count == 0) { return null; }
        List<Hex> newFrontier = new List<Hex>();
        foreach (Hex current in Frontier)
        {
            List<Node> neihbors = HexMap.GetRealNeighbors(current.HexNode);
            if (neihbors.Contains(target)) { neihbors.Remove(target); }
            foreach (Node next in neihbors)
            {
                if (!Visited.Contains(next.NodeHex))
                {
                    if (!HexMap.GetRealNeighbors(next).Contains(target))
                    {
                        return next;
                    }
                    newFrontier.Add(next.NodeHex);
                    Visited.Add(next.NodeHex);
                }
            }
        }
        return BreadthFirstAwaySearch(newFrontier, Visited, target);
    }

    //PATHING
    public List<Node> MoveAwayFromTarget(Hex target)
    {
        Node NodeLookingToMoveTo = BreadthFirstAway(target.HexNode);
        if (NodeLookingToMoveTo != null) { return FindObjectOfType<AStar>().FindPathWithMoveLimit(HexOn.HexNode, NodeLookingToMoveTo, myCT, CurrentMoveRange); }
        else { return new List<Node>(); }
    }

    public List<Node> getPathToTargettoAttack(Hex target, int range)
    {
        List<Node> possibleNodes = HexMap.GetNodesInLOS(target.HexNode, range);
        if (possibleNodes.Contains(HexOn.HexNode)){ return new List<Node> { HexOn.HexNode }; }
        Node ClosestNode = FindObjectOfType<AStar>().DiskatasWithArea(HexOn.HexNode, possibleNodes, myCT);
        if (ClosestNode != null) { return FindObjectOfType<AStar>().FindPathWithMoveLimit(HexOn.HexNode, ClosestNode, myCT, CurrentMoveRange); }
        else { return new List<Node>(); }
    }
}
