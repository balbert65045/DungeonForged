using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    Strength = 1,
    Agility = 2,
    Dexterity = 3,
    Armor = 4,
    None = 5,
}

[System.Serializable]
public class Buff
{
    public int Amount;
    public int Duration;
    public BuffType myBuffType;

    public Buff(int amount, int duration, BuffType buffType)
    {
        Amount = amount;
        Duration = duration;
        myBuffType = buffType;
    }
}

public class Character : Entity {

    public Sprite characterIcon;

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
    protected int CurrentAttackRange = 0;
    public int GetCurrentAttackRange() { return CurrentAttackRange; }
    public void SetCurrentAttackRange(int range) { CurrentAttackRange = range; }
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

    private List<Buff> Buffs = new List<Buff>();
    public List<Buff> GetBuffs() { return Buffs; }

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

    public bool IsPlayer() { return GetComponent<PlayerCharacter>() != null; }
    public bool IsEnemy() { return GetComponent<EnemyCharacter>() != null; }

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
        foreach (Node node in nodes)
        {
            if (!node.Shown) { continue; }
            if (node.edge) { continue; }
            NodesInActionRange.Add(node);
        }
        if (action == ActionType.Attack && NodesInActionRange.Contains(HexOn.HexNode)) { NodesInActionRange.Remove(HexOn.HexNode); }

        //Have to do this because of silly variable being added
        List<Node> nodesToSurround = new List<Node>();
        foreach(Node node in NodesInActionRange) { nodesToSurround.Add(node); }

        List<Vector3> points = new List<Vector3>();
        points = HexMap.GetHexesSurrounding(HexOn.HexNode, nodesToSurround);
        FindObjectOfType<PlayerController>().CreateArea(points, action);
    }

    public void ShowActionOnHealthBar(List<Action> actions)
    {
        myHealthBar.ShowActions(actions);
    }

    public bool HexInActionRange(Hex hex) { return NodesInActionRange.Contains(hex.HexNode); }

    public bool HexPositiveActionable(Hex hex) { return hex.EntityHolding != null && hex.EntityHolding.GetComponent<Character>() != null && hex.EntityHolding.GetComponent<Character>().myCT == myCT; }

    public bool HexNegativeActionable(Hex hex) { return hex.EntityHolding != null && hex.EntityHolding.GetComponent<Character>() != null && hex.EntityHolding.GetComponent<Character>().myCT != myCT; }

    public void GiveBuff(int value, int duration, BuffType buffType, List<Character> charactersGivingBuffTo)
    {
        foreach (Character character in charactersGivingBuffTo)
        {
            character.ApplyBuff(value, duration, buffType);
        }
        FinishedPerformingBuff();
    }

    public void ApplyBuff(int value, int duration, BuffType buffType)
    {
        switch (buffType)
        {
            case BuffType.Strength:
                AddBuff(value, duration, buffType, Strength, baseStrength);
                Strength += value;
                break;
            case BuffType.Armor:
                AddBuff(value, duration, buffType, Armor, baseArmor);
                Armor += value;
                Shield(value, this);
                break;
            case BuffType.Agility:
                AddBuff(value, duration, buffType, Agility, baseAgility);
                Agility += value;
                break;
            case BuffType.Dexterity:
                AddBuff(value, duration, buffType, Dexterity, baseDexterity);
                Dexterity += value;
                break;
        }
    }

    void AddBuff(int value, int duration, BuffType buffType, int Attribute, int baseAttribute)
    {
        Buff buffApplied = new Buff(value, duration, buffType);
        Buffs.Add(buffApplied);
        bool AttributeAlreadyBuffed = Attribute > baseAttribute;
        if (!AttributeAlreadyBuffed) { myHealthBar.AddBuff(buffType); }
    }

    public void DecreaseBuffsDuration()
    {
        List<Buff> buffsToRemove = new List<Buff>();
        foreach (Buff buff in Buffs)
        {
            buff.Duration--;
            if (buff.Duration <= 0) { buffsToRemove.Add(buff); }

        }
        foreach (Buff buff in buffsToRemove)
        {
            switch (buff.myBuffType)
            {
                case BuffType.Strength:
                    Strength = Strength - buff.Amount;
                    if (Strength == baseStrength) { myHealthBar.RemoveBuff(BuffType.Strength); }
                    break;
                case BuffType.Agility:
                    Agility = Agility - buff.Amount;
                    if (Agility == baseAgility) { myHealthBar.RemoveBuff(BuffType.Agility); }
                    break;
                case BuffType.Dexterity:
                    Dexterity = Dexterity - buff.Amount;
                    if (Dexterity == baseDexterity) { myHealthBar.RemoveBuff(BuffType.Dexterity); }
                    break;
                case BuffType.Armor:
                    Armor = Armor - buff.Amount;
                    if (Armor == baseArmor) { myHealthBar.RemoveBuff(BuffType.Armor); }
                    break;
            }
            Buffs.Remove(buff);
        }
    }

    void Awake()
    {
        hexVisualizer = FindObjectOfType<HexVisualizer>();
        HexMap = FindObjectOfType<HexMapController>();
        aStar = FindObjectOfType<AStar>();

        Strength = baseStrength;
        Agility = baseAgility;
        Dexterity = baseDexterity;
        Armor = baseArmor;
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

    // CALLBACKS
    public virtual void ShowStats() { }

    public virtual void ShowViewArea(Hex hex, int distance) { }

    public virtual bool CheckToFight() { return false; }

    public virtual bool SavingThrow() { return false; }

    public virtual void FinishedMoving(Hex hex, bool fighting = false, Hex HexMovingFrom = null) { }

    public virtual void FinishedAttacking() { CharactersFinishedTakingDamage++; }

    public void FinishedHealing() { characterThatHealingMe.FinishedPerformingHealing(); }

    public virtual void FinishedPerformingHealing() { }

    public void FinishedShielding() { characterShieldingMe.FinishedPerformingShielding(); }

    public virtual void FinishedPerformingShielding() { }

    public virtual void FinishedPerformingBuff() { }

    public virtual void GetHit()
    {
        transform.LookAt(characterThatAttackedMe.transform);
        GetComponent<CharacterAnimationController>().GetHit();

        int healthBeforeDamage = health;
        health -= Mathf.Clamp((TotalHealthLosing - CurrentArmor), 0, 1000);
        if (health <= 0) { GoingToDie = true; }
        myHealthBar.LoseHealth(TotalHealthLosing);
    }

    public void TakeTrueDamage(int amount)
    {
        characterThatAttackedMe = null;
        GetComponent<CharacterAnimationController>().GetHit();
        int healthBeforeDamage = health;
        TotalHealthLosing = amount;
        health -= Mathf.Clamp(amount, 0, 1000);
        if (health <= 0) { GoingToDie = true; }
        myHealthBar.LoseHealth(TotalHealthLosing);
    }

    public void SwitchCombatState(bool InCombat)
    {
        GetComponent<CharacterAnimationController>().SwitchCombatState(InCombat);
    }

    public void SavedBySavingThrow() { health = 1; }
    public void DeadBySavingThrow() { GoingToDie = true; }

    //HEALING
    public void PerformHeal(int amount, List<Character> charactersHealing)
    {
        foreach (Character character in charactersHealing)
        {
            character.Heal(amount, this);
        }
    }

    public void Heal(int amount, Character character)
    {
        characterThatHealingMe = character;
        myHealthBar.AddHealth(amount);
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth + 1);
    }

    //SHIELD

    public void PerformShield(int amount, List<Character> charactersShielding)
    {
        foreach (Character character in charactersShielding)
        {
            character.Shield(amount, this);
        }
    }

    public void resetShield(int amount)
    {
        int shieldLoss = CurrentArmor - amount;
        CurrentArmor = amount;
        myHealthBar.RemoveShield(shieldLoss);
    }

    public void Shield(int amount, Character character)
    {
        characterShieldingMe = character;
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
            characterThatAttackedMe.FinishedAttacking();
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

    public void HitOpponent()
    {
        foreach (Character character in charactersAttackingAt)
        {
            character.GetHit();
        }
    }

    public void LetAttackerAttack()
    {
        characterThatAttackedMe.MakeAttack();
    }

    public void MakeAttack()
    {
        GetComponent<CharacterAnimationController>().Attack();
    }

    public void Attack(int damage, Character[] characters)
    {
        charactersAttackingAt.Clear();
        CharactersFinishedTakingDamage = 0;
        foreach(Character character in characters)
        {
            if (character.GetGoingToDie() == false) { charactersAttackingAt.Add(character); }
        }
        if (charactersAttackingAt.Count == 0) {
            FinishedAttacking();
            return;
        }
        transform.LookAt(charactersAttackingAt[0].transform);
        foreach (Character character in charactersAttackingAt)
        {
            character.transform.LookAt(transform);
            character.TakeDamage(damage, this);
        }
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
        List<Node> nodesInDistance = aStar.Diskatas(HexOn.HexNode, moveRange, myCT);
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
