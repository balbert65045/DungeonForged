using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity {

    public enum CharacterType { Good, Bad }
    public CharacterType myCT;

    public int MaxMoveDistance = 15;
    public int CurrentMoveDistance = 0;

    public int health = 10;
    public int ViewDistance = 4;

    public int baseStrength = 0;
    public int baseAgility = 0;
    public int baseDexterity = 0;
    public int baseArmor = 0;

    protected HexVisualizer hexVisualizer;

    protected HealthBar myHealthBar;
    protected int maxHealth;

    public List<Node> NodesInWalkingDistance = new List<Node>();
    public List<Node> NodesInActionRange = new List<Node>();

    protected HexMapController HexMap;
    protected AStar aStar;

    protected Hex HexMovingTo;

    protected int CurrentArmor = 0;
    public int GetCurrentArmor() { return CurrentArmor; }
    protected int CurrentAttack = 0;
    public int GetCurrentAttack() { return CurrentAttack; }
    
    protected int CurrentActionRange = 0;
    public int GetCurrentActionRange() { return CurrentActionRange; }
    public void SetCurrentActionRange(int range) { CurrentActionRange = range; }
    protected int CurrentMoveRange = 0;
    public int GetCurrentMoveRange() { return CurrentMoveRange; }

    private int Agility = 0;
    public int GetAgility() { return Agility; }
    private int Strength = 0;
    public int GetStrength() { return Strength; }
    private int Dexterity = 0;
    public int GetDexterity() { return Dexterity; }
    private int Armor = 0;
    public int GetArmor() { return Armor; }

    private bool summonSickness = false;
    public bool GetSummonSickness() { return summonSickness; }
    public void SetSummonSickness(bool value) { summonSickness = value; }

    private Character characterShieldingMe;
    private Character characterThatHealingMe;
    protected Character characterThatAttackedMe;
    protected List<Character> charactersAttackingAt = new List<Character>();
    protected int CharactersFinishedTakingDamage = 0;

    private int TotalHealthLosing;
    private bool GoingToDie = false;
    public bool GetGoingToDie() { return GoingToDie; }

    private bool Stealthed = false;
    public bool GetStealthed() { return Stealthed; }
    private int StealthDuration = 0;

    private bool Moving = false;
    public bool GetMoving() { return Moving; }
    public void SetMoving(bool value) { Moving = value; }

    private bool Attacking = false;
    public bool GetAttacking() { return Attacking; }
    public void SetAttacking(bool value) { Attacking = value; }

    public List<Node> CombatNodes = new List<Node>();

    public void ClearActions()
    {
        NodesInActionRange.Clear();
        NodesInWalkingDistance.Clear();
        myHealthBar.ClearActions();
    }

    public void ShowAction(int Range, ActionType action)
    {
        List<Node> nodes = HexMap.GetNodesInLOS(HexOn.HexNode, Range);
        NodesInActionRange.Clear();
        if (action == ActionType.Attack && MyDeBuffsHas(DeBuffType.Disarm)) { }
        else
        {
            foreach (Node node in nodes)
            {
                if (!node.Shown) { continue; }
                if (node.edge) { continue; }
                NodesInActionRange.Add(node);
            }
            if (action == ActionType.Attack && NodesInActionRange.Contains(HexOn.HexNode)) { NodesInActionRange.Remove(HexOn.HexNode); }
        }

        //Have to do this because of silly variable being added
        List<Node> nodesToSurround = new List<Node>();
        foreach(Node node in NodesInActionRange) { nodesToSurround.Add(node); }
        if (NodesInActionRange.Count == 0) { nodesToSurround.Add(HexOn.HexNode); }

        List<Vector3> points = new List<Vector3>();
        points = HexMap.GetHexesSurrounding(HexOn.HexNode, nodesToSurround);
        FindObjectOfType<PlayerController>().CreateArea(points, action);
    }

    public void ShowActionOnHealthBar(List<Action> actions)
    {
        myHealthBar.ShowActions(actions, myDeBuffs, IsPlayer());
    }

    public bool HexInActionRange(Hex hex) { return NodesInActionRange.Contains(hex.HexNode); }

    public bool HexPositiveActionable(Hex hex) { return hex.EntityHolding != null && hex.EntityHolding.GetComponent<Character>() != null && hex.EntityHolding.GetComponent<Character>().myCT == myCT; }

    public bool HexNegativeActionable(Hex hex) { return hex.EntityHolding != null && hex.EntityHolding.GetComponent<Character>() != null && hex.EntityHolding.GetComponent<Character>().myCT != myCT; }

    IEnumerator GivingBuffDelay()
    {
        yield return new WaitForSeconds(.3f);
        FinishedPerformingBuff();
    }

    void Awake()
    {
        hexVisualizer = FindObjectOfType<HexVisualizer>();
        HexMap = FindObjectOfType<HexMapController>();
        aStar = FindObjectOfType<AStar>();

        Strength = baseStrength;
        Agility = baseAgility;
        Dexterity = baseDexterity;
        CurrentArmor = baseArmor;
        CurrentMoveDistance = MaxMoveDistance;
    }

    private void Start()
    {
        myHealthBar = GetComponentInChildren<HealthBar>();
        maxHealth = health;
        if (myHealthBar != null)
        {
            myHealthBar.CreateHealthBar(health, maxHealth);
        }
        Shield(baseArmor, this);
    }

    public virtual void DoPositiveAction()
    {

    }

    public void SelectCharacter()
    {

    }

    // CALLBACKS
    public virtual void ShowStats() { }

    public virtual void ShowViewArea(Hex hex, int distance) { }

    public virtual bool CheckToFight() { return false; }

    public virtual bool SavingThrow() { return false; }

    public virtual void FinishedMoving(Hex hex, bool fighting = false, Hex HexMovingFrom = null) { }

    public virtual void FinishedAttacking(bool dead = false) { 
        CharactersFinishedTakingDamage++; 
    }

    public void FinishedHealing() { characterThatHealingMe.FinishedPerformingHealing(); }

    public virtual void FinishedPerformingHealing() { }

    public void FinishedShielding() { 
        if (characterShieldingMe != null) { characterShieldingMe.FinishedPerformingShielding(); }
    }

    public virtual void FinishedPerformingShielding() { }

    public virtual void FinishedPerformingBuff() { }

    int AddPoisonDamage(int initialHealthLost)
    {
        if (MyDeBuffsHas(DeBuffType.Poison) && deBuffApplied.thisDeBuffType != DeBuffType.Poison)
        {
            return Mathf.FloorToInt(initialHealthLost * 1.25f);
        }
        return initialHealthLost;
    }

    public virtual void GetHit()
    {
        transform.LookAt(characterThatAttackedMe.transform);
        GetComponent<CharacterAnimationController>().GetHit();
        TotalHealthLosing = AddPoisonDamage(TotalHealthLosing);
        ShowDeBuff();
        int healthBeforeDamage = health;
        health -= Mathf.Clamp((TotalHealthLosing - CurrentArmor), 0, 1000);
        CurrentArmor = Mathf.Clamp((CurrentArmor - TotalHealthLosing), 0, 1000);
        if (health <= 0) { GoingToDie = true; }
        myHealthBar.LoseHealth(TotalHealthLosing);
        if (GetComponent<PlayerCharacter>() != null) { GetComponent<PlayerCharacter>().UpdateHealth(health); }
    }

    public void PredictDamage(int damage)
    {
        damage = AddPoisonDamage(damage);
        myHealthBar.PredictDamage(damage);
    }

    public int DamageDealing(int damage, bool firstAttack)
    {
        damage = CalculateDeBuffs(damage, firstAttack);
        return damage;
    }

    public void HidePredication()
    {
        myHealthBar.RemovePredication();
    }

    public void TakeTrueDamage(int amount)
    {
        characterThatAttackedMe = null;
        GetComponent<CharacterAnimationController>().GetHit();
        TotalHealthLosing = amount;
        health -= Mathf.Clamp(amount, 0, 1000);
        if (health <= 0) { GoingToDie = true; }
        myHealthBar.LoseHealth(TotalHealthLosing);
        if (GetComponent<PlayerCharacter>() != null) { GetComponent<PlayerCharacter>().UpdateHealth(health); }
    }

    public void SwitchCombatState(bool InCombat)
    {
        GetComponent<CharacterAnimationController>().SwitchCombatState(InCombat);
    }

    public void SavedBySavingThrow() { health = 1; }
    public void DeadBySavingThrow() { GoingToDie = true; }

    //HEALING
    public void PerformHeal(int amount, List<Character> charactersHealing, DeBuff deBuffApplying)
    {
        foreach (Character character in charactersHealing)
        {
            character.Heal(amount, this);
            if (deBuffApplying.thisDeBuffType != DeBuffType.None)
            {
                character.AddDeBuff(deBuffApplying);
                character.ShowDeBuff();
            }
        }
    }

    public void Heal(int amount, Character character)
    {
        characterThatHealingMe = character;
        if (MyDeBuffsHas(DeBuffType.Bleed)){RemoveStatus(DeBuffType.Bleed);}
        myHealthBar.AddHealth(amount);
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth + 1);
        if (GetComponent<PlayerCharacter>() != null) { GetComponent<PlayerCharacter>().UpdateHealth(health); }
    }

    //SHIELD

    public void PerformShield(int amount, List<Character> charactersShielding, DeBuff deBuffApplying)
    {
        foreach (Character character in charactersShielding)
        {
            character.Shield(amount, this);
            if (deBuffApplying.thisDeBuffType != DeBuffType.None)
            {
                character.AddDeBuff(deBuffApplying);
                character.ShowDeBuff();
            }
        }
    }

    public void resetShield(int amount)
    {
        CurrentArmor = amount;
        myHealthBar.ResetShield(amount);

    }

    public void Shield(int amount, Character character)
    {
        characterShieldingMe = character;
        if (MyDeBuffsHas(DeBuffType.IncreaseShield)) { amount += FindDeBuff(DeBuffType.IncreaseShield).Amount; }
        CurrentArmor += amount;
        if (amount > 0) { myHealthBar.AddShield(amount); }
    }

    public virtual void SlayedEnemy(float XP)
    {
        FinishedAttacking();
    }

    //ATACKING
    public virtual void finishedTakingDamage()
    {
        if (GoingToDie) {
            GetComponent<CharacterAnimationController>().Die();
        }
        else
        {
            if (characterThatAttackedMe != null)
            {
                characterThatAttackedMe.FinishedAttacking();
            }
        }
    }

    public virtual void ShowNewMoveArea()
    {

    }

    public virtual void Die()
    {
        if (characterThatAttackedMe != null)
        {
            characterThatAttackedMe.FinishedAttacking(true);
        }
        HexOn.RemoveEntityFromHex();
        FindObjectOfType<TurnOrder>().CharacterRemoved(this);
        //characterThatAttackedMe.ShowNewMoveArea();
        Destroy(this.gameObject);
    }

    public void TakeDamage(int damage, Character characterThatAttacked)
    {
        characterThatAttackedMe = characterThatAttacked;
        TotalHealthLosing = damage;
        LetAttackerAttack();
    }

    public void BeginTurn()
    {
        if (FindObjectOfType<TurnOrder>().TurnNumber != 0) {
            resetShield(baseArmor);
        }
        if (MyDeBuffsHas(DeBuffType.Bleed))
        {
            Bleed();
        }
    }

    public bool hasArtifact(ArtifactType type)
    {
        if (!IsPlayer()) { return false; }
        return FindObjectOfType<NewGroupStorage>().MyGroupCardStorage[0].ArtifactsHolding().Contains(type);
    }

    public void IncreaseMove()
    {
        int amount = 1;
        if (hasArtifact(ArtifactType.HexMoveIncrease)) { amount = 2; }
        DeBuff newDeBuff = new DeBuff(DeBuffType.IncreaseMove, amount);
        AddDeBuff(newDeBuff);
        ShowDeBuff();
    }

    public void IncreaseShield()
    {
        int amount = 3;
        if (hasArtifact(ArtifactType.HexShieldIncrease)) { amount = 6; }
        DeBuff newDeBuff = new DeBuff(DeBuffType.IncreaseShield, amount);
        AddDeBuff(newDeBuff);
        ShowDeBuff();
    }

    public void IncreaseAttack()
    {
        int amount = 3;
        if (hasArtifact(ArtifactType.HexAttackIncrease)){ amount = 6; }
        DeBuff newDeBuff = new DeBuff(DeBuffType.IncreaseAttack, amount);
        AddDeBuff(newDeBuff);
        ShowDeBuff();
        //if (!MyDeBuffsHas(DeBuffType.IncreaseAttack))
        //{
        //    myDeBuffs.Add(newDeBuff);
        //    myHealthBar.ShowDeBuff(newDeBuff);
        //}
        //else
        //{
        //    DeBuff myDeBuff = FindDeBuff(DeBuffType.IncreaseAttack);
        //    int newAmount = myDeBuff.Amount + amount;
        //    SetDeBuffAmount(newAmount, myDeBuff);
        //    myHealthBar.SetDeBuffAmount(myDeBuff.thisDeBuffType, newAmount);
        //}
    }

    public void EndTurn()
    {
        if (MyDeBuffsHas(DeBuffType.Poison)) { DecreaseDebuff(DeBuffType.Poison); }
        if (MyDeBuffsHas(DeBuffType.Immobelized)) { DecreaseDebuff(DeBuffType.Immobelized); }
        if (MyDeBuffsHas(DeBuffType.Weaken)) { DecreaseDebuff(DeBuffType.Weaken); }
        if (MyDeBuffsHas(DeBuffType.Stun)) { DecreaseDebuff(DeBuffType.Stun); }
        if (MyDeBuffsHas(DeBuffType.Disarm)) { DecreaseDebuff(DeBuffType.Disarm); }
        if (MyDeBuffsHas(DeBuffType.Slow)) { DecreaseDebuff(DeBuffType.Slow); }
    }
    void DecreaseDebuff(DeBuffType DeBuffType)
    {
        DeBuff deBuffToDecrease = FindDeBuff(DeBuffType);
        int newAmount = deBuffToDecrease.Amount - 1;
        SetDeBuffAmount(newAmount, deBuffToDecrease);
        if (newAmount <= 0)
        {
            RemoveStatus(DeBuffType);
        }
        else
        {
            myHealthBar.SetDeBuffAmount(DeBuffType, newAmount);
        }
    }

    protected DeBuff FindDeBuff(DeBuffType DeBuffType)
    {
        foreach (DeBuff aDebuff in myDeBuffs)
        {
            if (aDebuff.thisDeBuffType == DeBuffType) { return aDebuff; }
        }
        return new DeBuff(DeBuffType.None, 0);
    }

    protected void RemoveStatus(DeBuffType debuff)
    {
        RemoveDeBuff(debuff);
        myHealthBar.RemoveDeBuff(debuff);
    }

    void Bleed()
    {
        TakeTrueDamage(1);
    }

    public bool MyDeBuffsHas(DeBuffType debuff)
    {
        foreach(DeBuff aDebuff in myDeBuffs)
        {
            if (aDebuff.thisDeBuffType == debuff) { return true; }
        }
        return false;
    }
    public void RemoveDeBuff(DeBuffType debuff)
    {
        int index = -1;
        for(int i = 0; i < myDeBuffs.Count; i++)
        {
            if (myDeBuffs[i].thisDeBuffType == debuff) 
            {
                index = i;
                break;
            }
        }

        if (index != -1)
        {
            myDeBuffs.RemoveAt(index);
        }
    }

    public List<DeBuff> myDeBuffs = new List<DeBuff>();
    public int InCreaseAmount = 0;
    public DeBuff deBuffApplied = new DeBuff(DeBuffType.None, 0);
    public void ShowDeBuff()
    {
        if (deBuffApplied.thisDeBuffType != DeBuffType.None && deBuffApplied.Amount != 0)
        {
            if (InCreaseAmount == 0)
            {
                myHealthBar.ShowDeBuff(deBuffApplied);
            }
            else
            {
                DeBuff myDeBuff = FindDeBuff(deBuffApplied.thisDeBuffType);
                int newAmount = myDeBuff.Amount + InCreaseAmount;
                SetDeBuffAmount(newAmount, myDeBuff);
                myHealthBar.SetDeBuffAmount(myDeBuff.thisDeBuffType, newAmount);
                InCreaseAmount = 0;
            }
        }
        deBuffApplied = new DeBuff(DeBuffType.None, 0);
    }

    protected void SetDeBuffAmount(int amount, DeBuff deBuff)
    {
        DeBuff NewDeBuff = new DeBuff(deBuff.thisDeBuffType, amount);
        myDeBuffs.Add(NewDeBuff);
        myDeBuffs.Remove(deBuff);
    }

    public void AddDeBuff(DeBuff deBuff)
    {
        if (!MyDeBuffsHas(deBuff.thisDeBuffType))
        {
            myDeBuffs.Add(deBuff);
            deBuffApplied = deBuff;
        }
        else
        {
            InCreaseAmount = deBuff.Amount;
            deBuffApplied = deBuff;
        }
    }

    protected bool AttackingAgain = false;
    public void HitOpponent()
    {
        if (charactersAttackingAt.Count == 0)
        {
            FinishedAttacking();
            return;
        }
        foreach (Character character in charactersAttackingAt)
        {
            character.GetHit();
        }
        if (AttackActions.Count != 0)
        {
            bool ranged = AttackActions[0].Range > 1;
            Attack(AttackActions[0].thisAOE.Damage, AttackActions[0].thisDeBuff, charactersAttackingAt.ToArray(), ranged, "StrikeAgain");
            AttackActions.RemoveAt(0);
            AttackingAgain = true;
            return;
        }
        AttackingAgain = false;
    }

    public void LetAttackerAttack()
    {
        characterThatAttackedMe.MakeAttack();
    }

    public void MakeAttack()
    {
        GetComponent<CharacterAnimationController>().Attack();
    }

    int CalculateDeBuffs(int amount, bool firstAttack = true)
    {
        int totalamount = amount;
        if (MyDeBuffsHas(DeBuffType.PowerUp)) { totalamount = totalamount + FindDeBuff(DeBuffType.PowerUp).Amount; }
        if (MyDeBuffsHas(DeBuffType.IncreaseAttack) && firstAttack) { totalamount = totalamount + FindDeBuff(DeBuffType.IncreaseAttack).Amount; }
        if (MyDeBuffsHas(DeBuffType.Weaken)) { totalamount = Mathf.FloorToInt(totalamount * .75f); }
        return totalamount;
    }

    protected List<Action> AttackActions = new List<Action>();
    public void PlayerAttack(List<Action> actions, Character[] characters)
    {
        AttackActions = actions;
        bool ranged = actions[0].Range > 1;
        Attack(actions[0].thisAOE.Damage, actions[0].thisDeBuff, characters, ranged, actions[0].TriggerAnimation);
        AttackActions.Remove(actions[0]);
    }

    public void Attack(int damage, DeBuff deBuff, Character[] characters, bool ranged, string TriggerAnimation)
    {
        charactersAttackingAt.Clear();
        CharactersFinishedTakingDamage = 0;
        GetComponent<CharacterAnimationController>().AnimationTrigger = TriggerAnimation;
        foreach(Character character in characters)
        {
            if (character.GetGoingToDie() == false) { charactersAttackingAt.Add(character); }
        }
        if (charactersAttackingAt.Count == 0) {
            FinishedAttacking();
            return;
        }
        transform.LookAt(charactersAttackingAt[0].transform);

        //DeBuffCalc
        damage = CalculateDeBuffs(damage);

        foreach (Character character in charactersAttackingAt)
        {
            character.transform.LookAt(transform);
            character.AddDeBuff(deBuff);
            int damageTaking = damage;
            if (ranged && InMeleeRange(character)) { damageTaking = damage / 2; }
            character.TakeDamage(damageTaking, this);
        }

        if (MyDeBuffsHas(DeBuffType.IncreaseAttack)) { RemoveStatus(DeBuffType.IncreaseAttack); }
    }

    public bool InMeleeRange(Character character)
    {
        List<Node> meleeNodes = FindObjectOfType<HexMapController>().GetNodesAdjacent(HexOn.HexNode);
        if (meleeNodes.Contains(character.HexOn.HexNode)) { return true; }
        return false;
    }

    //Stealth
    public void ReduceStealthDuration()
    {
        StealthDuration--;
        if (StealthDuration <= 0) {
            GetComponent<CharacterAnimationController>().SetStealthState(false);
            Stealthed = false;
        }
    }

    public void Stealth(int value)
    {
        Stealthed = true;
        StealthDuration = value;
        GetComponent<CharacterAnimationController>().SetStealthState(true);
    }

    // MOVEMENT//
    public bool HexInMoveRange(Hex hex, int Amount)
    {
        if (MyDeBuffsHas(DeBuffType.Immobelized)){ return false; }
        if (NodesInWalkingDistance.Contains(hex.HexNode)) { return true; }
        return false;

    }

    public List<Node> GetOpenNodes(List<Node> currentNodes)
    {
        List<Node> openNodes = new List<Node>();
        foreach (Node node in currentNodes)
        {
            if (node.NodeHex.EntityHolding == null) { openNodes.Add(node); }
        }

        return openNodes;
    }

    public virtual void ShowMoveDistance(int moveRange)
    {
        CurrentMoveRange = moveRange;
        List<Node> nodesInDistance = aStar.Diskatas(HexOn.HexNode, CurrentMoveRange, myCT);
        NodesInWalkingDistance.Clear();
        foreach (Node node in nodesInDistance)
        {
            if (!node.Shown || node.edge) { continue; }
            if (node.NodeHex.EntityHolding != null) { continue; }
            NodesInWalkingDistance.Add(node);
        }
    }

    public Node FindClosestNode(List<Node> nodes)
    {
        int shortestPath = 1000;
        Node nodeToMoveTo = null;
        foreach(Node node in nodes)
        {
            List<Node> path = aStar.FindPath(HexOn.HexNode, node, myCT);
            int distance = path.Count;
            if (distance < shortestPath && distance != 0) {
                nodeToMoveTo = node;
                shortestPath = distance;
            }
        }
        return nodeToMoveTo;
    }

    public List<Node> GetPath(Node NodeToMoveTo)
    {
        Node StartNode = HexOn.HexNode;
        Node EndNode = NodeToMoveTo;
        if (!NodeToMoveTo.isAvailable || NodeToMoveTo.edge) { return null; }
        if (NodeToMoveTo.NodeHex.EntityHolding != null) { return null; }
        return FindObjectOfType<AStar>().FindPath(StartNode, EndNode, myCT);
    }

    public virtual void MoveOnPath(Hex hex)
    {
        NodesInWalkingDistance.Clear();
        HexMovingTo = hex;
        Hex HexCurrentlyOn = HexOn;
        HexMovingTo.CharacterMovingToHex();
        RemoveLinkFromHex();
        if (hex == HexOn) { FinishedMoving(hex); }
        List<Node> nodes = GetPath(hex.HexNode);
        if (nodes[0] == null) { return; }
        Node HexToMoveTo = nodes[0];
        nodes.Remove(HexToMoveTo);
        GetComponent<CharacterAnimationController>().MoveTowards(HexToMoveTo.NodeHex, nodes, HexCurrentlyOn);
    }

    public virtual void MovingOnPath()
    {
        RemoveLinkFromHex();
    }
}
