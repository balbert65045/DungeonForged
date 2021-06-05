﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {

    public List<PlayerCharacter> myCharacters = new List<PlayerCharacter>();
    public PlayerCharacter SelectPlayerCharacter;

    private EndActionButton endActionButton;
    private EndTurnButton endTurnButton;
    private PlayerActionButton actionButton;
    private MyCameraController myCamera;
    private CameraRaycaster raycaster;
    private HexVisualizer hexVisualizer;
    private TurnOrder turnOrder;

    public List<Hex> HexesMovingTo = new List<Hex>();
    public void AddHexMovingTo(Hex hex) { HexesMovingTo.Add(hex); }
    public void ClearHexesMovingTo() { HexesMovingTo.Clear(); }

    public int GoldHolding = 0;

    public bool CardsPlayable = true;
    public List<Action> CurrentActions;
    public Action CurrentAction;


    public void ShowStagedAction(List<Action> actions)
    {
        CurrentActions = actions;
        if (actions.Count == 0)
        {
            endActionButton.gameObject.SetActive(false);
            SelectPlayerCharacter.ClearActions();
            RemoveArea();
            return;
        }

        CurrentAction = actions[0];
        ShowArea();
        endActionButton.gameObject.SetActive(true);
        SelectPlayerCharacter.ShowActionOnHealthBar(CurrentActions);
    }

    void ShowArea()
    {
        if (CurrentActions.Count == 0) { return; }
        if (CurrentAction.thisActionType == ActionType.Movement)
        {
            SelectPlayerCharacter.ShowMoveDistance(CurrentAction.Range);
        }
        else
        {
            SelectPlayerCharacter.ShowAction(CurrentAction.Range, CurrentAction.thisActionType);
        }
    }

    public void BeginGame()
    {
        StartCoroutine("StartGame");
    }

    private void Start()
    {
        hexVisualizer = FindObjectOfType<HexVisualizer>();
        raycaster = FindObjectOfType<CameraRaycaster>();
        myCamera = FindObjectOfType<MyCameraController>();
        endTurnButton = FindObjectOfType<EndTurnButton>();
        endActionButton = FindObjectOfType<EndActionButton>();
        actionButton = FindObjectOfType<PlayerActionButton>();
        turnOrder = FindObjectOfType<TurnOrder>();
        endActionButton.gameObject.SetActive(false);
        FindObjectOfType<PlayerTopBar>().HideBar();
        //..
        //StartCoroutine("StartGame");
        //actionButton.gameObject.SetActive(false);
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(.5f);
        //myCharacters.Clear();
        //myCharacters.AddRange(FindObjectsOfType<PlayerCharacter>());
        //FindObjectOfType<PlayerCurrency>().SetGoldValue(GoldHolding);
        FindObjectOfType<EnemyController>().StartFirstActions();
        AllowNewTurns();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (enemySelected == null)
            {
                UseAction(CurrentAction);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (!usingAction) 
            {
                SelectEnemy();
            }
        }
    }

    bool OverCard()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public EnemyCharacter enemySelected = null;
    void SelectEnemy()
    {
        if (enemySelected != null) { enemySelected.RemoveAreas(); }
        if (OverCard()) {
            enemySelected = null;
            return; 
        }
        Transform HexHit = raycaster.HexRaycast();
        Hex hexSelected = null;
        if (HexHit != null && HexHit.GetComponent<Hex>()) { hexSelected = HexHit.GetComponent<Hex>(); }
        if (hexSelected == null) {
            enemySelected = null;
            ShowArea();
            return; 
        }
        if (!hexSelected.HasEnemy()) {
            enemySelected = null;
            ShowArea();
            return;
        }
        else
        {
            RemoveArea();
            enemySelected = hexSelected.GetEnemy();
            enemySelected.Selected();
        }
    }

    public void UseUnstagedAction(Action action)
    {
        if (action.thisActionType == ActionType.LoseHealth)
        {
            SelectPlayerCharacter.TakeTrueDamage(action.thisAOE.Damage);
        }
        else if (action.thisActionType == ActionType.DrawCard)
        {
            SelectPlayerCharacter.GetMyNewHand().DrawCards(action.thisAOE.Damage);
        }
    }

    void UseImmediateAction(Action action)
    {
        if (action.thisActionType == ActionType.LoseHealth)
        {
            SelectPlayerCharacter.TakeTrueDamage(action.thisAOE.Damage);
            RemoveActionUsed();
            ActionFinished();
        }
        else if (action.thisActionType == ActionType.DrawCard)
        {
            SelectPlayerCharacter.GetMyNewHand().DrawCards(action.thisAOE.Damage);
            RemoveActionUsed();
            ActionFinished();
        }
    }

    bool usingAction = false;

    List<Character> charactersAttacking = new List<Character>();
    void UseAction(Action action)
    {
        if (usingAction) { return; }
        if (action.thisActionType == ActionType.None) { return; }
        Transform HexHit = raycaster.HexRaycast();
        Hex hexSelected = null;
        if (HexHit != null && HexHit.GetComponent<Hex>()) { hexSelected = HexHit.GetComponent<Hex>(); }
        if (hexSelected == null) { return; }
        if (action.thisActionType == ActionType.Movement)
        {
            usingAction = CheckToMoveOrInteract();
            if (usingAction) { RemoveArea(); }
        }
        else if (action.thisActionType == ActionType.Attack)
        {
            charactersAttacking = CheckForNegativeAction(action, SelectPlayerCharacter, hexSelected);
            if (charactersAttacking != null && charactersAttacking.Count != 0)
            {
                usingAction = true;
                hexVisualizer.ResetPredication();
                PerformAction(action, charactersAttacking);
            }
        }
        else
        {
            List<Character> charactersActingOn = CheckForPositiveAction(action, SelectPlayerCharacter, hexSelected);
            if (charactersActingOn == null) { return; }
            else if (charactersActingOn.Count != 0)
            {
                usingAction = true;
                PerformAction(action, charactersActingOn);
            }
        }

        if (usingAction)
        {
            FindObjectOfType<NewHand>().HideHand();
            FindObjectOfType<StagingArea>().DiscardUsedCards();
            endActionButton.gameObject.SetActive(false);
            DisableEndTurn();
        }
    }

    bool CheckToMoveOrInteract()
    {
        if (SelectPlayerCharacter.GetMoving()) { return false; }
        Transform WallHit = raycaster.WallRaycast();
        if (WallHit != null)
        {
            if (WallHit.GetComponent<DoorObject>() != null && !WallHit.GetComponent<DoorObject>().door.isOpen)
            {
                MoveToDoor(WallHit.GetComponent<DoorObject>().door);
                return true;
            }
        }

        Transform HexHit = raycaster.HexRaycast();
        if (HexHit != null && HexHit.GetComponent<Hex>())
        {
            Hex hexSelected = HexHit.GetComponent<Hex>();
            if (hexSelected == null || !hexSelected.HexNode.Shown) { return false; }
            if (!SelectPlayerCharacter.HexInMoveRange(hexSelected, CurrentAction.Range)) { return false; }
            if (hexSelected.GetComponent<Door>() != null && !hexSelected.GetComponent<Door>().isOpen)
            {
                MoveToDoor(hexSelected.GetComponent<Door>());
                return true;
            }
            if (hexSelected.EntityHolding == null && !hexSelected.MovedTo)
            {
                myCamera.SetTarget(SelectPlayerCharacter.transform);
                SelectPlayerCharacter.MoveOnPath(hexSelected);
                return true;
            }
        }
        return false;
    }

    void MoveToDoor(Door doorHex)
    {
        if (SelectPlayerCharacter.GetMoving()) { return; }
        if (!SelectPlayerCharacter.HexInMoveRange(doorHex.GetComponent<Hex>(), CurrentAction.Range)) { return; }
        if (doorHex.GetComponent<Hex>().EntityHolding == null && !doorHex.GetComponent<Hex>().MovedTo)
        {
            myCamera.SetTarget(SelectPlayerCharacter.transform);
            SelectPlayerCharacter.SetDoorToOpen(doorHex);
            SelectPlayerCharacter.MoveOnPath(doorHex.GetComponent<Hex>());
        }
    }

    public void RemoveActionUsed()
    {
        if (CurrentActions.Count == 0) { return; }
        int amountOfActions = CombinedActions().Count;
        for(int i = 0; i < amountOfActions; i++)
        {
            CurrentActions.RemoveAt(0);
        }
        SelectPlayerCharacter.ShowActionOnHealthBar(CurrentActions);
    }

    void PerformAction(Action action, List<Character> characters)
    {
        RemoveArea();
        if (action.thisActionType == ActionType.Attack) {
            PerformAttack(CombinedActions(), characters);
        }
        else { SelectPlayerCharacter.GetComponent<CharacterAnimationController>().DoBuff(action.thisActionType, action.thisAOE.Damage, CurrentAction.thisDeBuff, characters); }
    }

    void PerformAttack(List<Action> actions, List<Character> characters)
    {
        RemoveArea();
        SelectPlayerCharacter.PlayerAttack(actions, characters.ToArray());
    }

    List<Character> CheckForNegativeAction(Action action, Character character, Hex hexSelected)
    {
        List<Node> nodes = FindObjectOfType<HexMapController>().GetAOE(action.thisAOE.thisAOEType, character.HexOn.HexNode, hexSelected.HexNode);
        List<Character> characterActingUpon = new List<Character>();

        if (!character.HexInActionRange(hexSelected)) { return null; }
        foreach (Node node in nodes)
        {
            if (node == null) { break; }
            if (character.HexNegativeActionable(node.NodeHex))
            {
                UnHighlightHexes();
                foreach (Node node_highlight in nodes)
                {
                    hexVisualizer.HighlightActionPointHex(node_highlight.NodeHex, action.thisActionType);
                }
                characterActingUpon.Add(node.NodeHex.EntityHolding.GetComponent<Character>());
            }
        }
        return characterActingUpon;
    }

    List<Character> CheckForPositiveAction(Action action, Character character, Hex hexSelected)
    {
        List<Node> nodes = FindObjectOfType<HexMapController>().GetAOE(action.thisAOE.thisAOEType, character.HexOn.HexNode, hexSelected.HexNode);
        List<Character> characterActingUpon = new List<Character>();

        if (!character.HexInActionRange(hexSelected)) { return null; }
        foreach (Node node in nodes)
        {
            if (node == null) { break; }
            if (character.HexPositiveActionable(node.NodeHex))
            {
                UnHighlightHexes();
                foreach (Node node_highlight in nodes)
                {
                    hexVisualizer.HighlightActionPointHex(node_highlight.NodeHex, action.thisActionType);
                }
                characterActingUpon.Add(node.NodeHex.EntityHolding.GetComponent<Character>());
            }
        }
        return characterActingUpon;
    }

    public void AddGold(int gold)
    {
        FindObjectOfType<PlayerCurrency>().SetGoldValue(FindObjectOfType<PlayerCurrency>().GoldHolding() + gold);
    }

    public void RemoveGold(int gold)
    {
        GoldHolding -= gold;
        FindObjectOfType<PlayerCurrency>().SetGoldValue(GoldHolding);
    }

    public void RemoveArea()
    {
        GetComponentInChildren<MeshGenerator>().DeleteMesh();
        GetComponentInChildren<EdgeLine>().DestroyLine();
    }

    public void CreateArea(List<Vector3> points, ActionType type)
    {
        GetComponentInChildren<MeshGenerator>().CreateMesh(points);
        GetComponentInChildren<MeshGenerator>().SetCurrentMaterial(type);
        GetComponentInChildren<EdgeLine>().CreateLine(points.ToArray());
        GetComponentInChildren<EdgeLine>().SetCurrentMaterial(type);
    }

    public void SelectCharacterByName(string characterName)
    {
        foreach(PlayerCharacter character in myCharacters)
        {
            if (character.CharacterName == characterName)
            {
                SelectCharacter(character);
            }
        }
    }

    public void AllowNewTurns()
    {
        StartCoroutine("NewTurns");
    }

    IEnumerator NewTurns()
    {
        yield return new WaitForSeconds(.1f);
        SelectPlayerCharacter = (PlayerCharacter)turnOrder.GetCurrentCharacter();
        myCamera.SetTarget(SelectPlayerCharacter.transform);
        SelectPlayerCharacter.MyNewDeck.SetActive(true);
        SelectPlayerCharacter.GetMyNewHand().DrawNewHand();
        SelectPlayerCharacter.BeginTurn();
        yield return new WaitForSeconds(.8f);
        if (turnOrder.TurnNumber == 0)
        {
            if (SelectPlayerCharacter.hasArtifact(ArtifactType.MoveStart))
            {
                FindObjectOfType<NewHand>().HideHand();
                CurrentActions.Add(new Action(ActionType.Movement, new AOE(AOEType.SingleTarget, 0), 2, new DeBuff(DeBuffType.None, 0)));
                CurrentAction = CurrentActions[0];
                SelectPlayerCharacter.ShowMoveDistance(CurrentAction.Range);
                SelectPlayerCharacter.ShowActionOnHealthBar(CurrentActions);
                endActionButton.gameObject.SetActive(true);
            }
        }
        if (SelectPlayerCharacter.MyDeBuffsHas(DeBuffType.Stun)) { EndPlayerTurn(); }
        else { AllowEndTurn(); }
    }

    public void EndPlayerTurn()
    {
        endActionButton.gameObject.SetActive(false);
        RemoveArea();
        FindObjectOfType<StagingArea>().DiscardCards();
        SelectPlayerCharacter.GetMyNewHand().DiscardHand();
        StartCoroutine("StartNewTurn");
    }

    IEnumerator StartNewTurn()
    {
        DisableEndTurn();
        yield return new WaitForSeconds(.8f);
        SelectPlayerCharacter.MyNewDeck.SetActive(false);
        SelectPlayerCharacter.EndTurn();
        TurnOrder turnOrder = FindObjectOfType<TurnOrder>();
        turnOrder.EndTurn();

        if (turnOrder.GetCurrentCharacter().IsPlayer())
        {
            AllowNewTurns();
        }
        else
        {
            FindObjectOfType<EnemyController>().DoEnemyActions();
        }
    }

    public void DiscardAction()
    {
        RemoveActionUsed();
        ActionFinished();
    }

    void ActionFinished()
    {
        usingAction = false;
        if (CurrentActions.Count > 0)
        {
            switch (CurrentActions[0].thisActionType)
            {
                case ActionType.DrawCard:
                    UseImmediateAction(CurrentActions[0]);
                    break;
                case ActionType.LoseHealth:
                    UseImmediateAction(CurrentActions[0]);
                    break;
                default:
                    CardsPlayable = true;
                    endActionButton.gameObject.SetActive(true);
                    ShowStagedAction(CurrentActions);
                    hexVisualizer.HexChange();
                    break;
            }
        }
        else
        {
            AllowNewActions();
        }
    }

    public List<Action> CombinedActions()
    {
        if (CurrentActions.Count == 0) { return null; }
        List<Action> actions = new List<Action>();
        Action firstAction = CurrentActions[0];
        actions.Add(firstAction);
        if (CurrentActions.Count < 2) { return actions; }
        for (int i = 1; i < CurrentActions.Count; i++)
        {
            if(CurrentActions[i].thisActionType == firstAction.thisActionType && CurrentActions[i].Range == firstAction.Range && 
                CurrentActions[i].thisAOE.thisAOEType == firstAction.thisAOE.thisAOEType)
            {
                actions.Add(CurrentActions[i]);
            }
            else
            {
                break;
            }
        }

        return actions;
    }

    public void AllowNewActions()
    {
        endActionButton.gameObject.SetActive(false);
        myCamera.UnLockCamera();
        if (FindObjectOfType<StagingArea>() == null) { return; }
        FindObjectOfType<NewHand>().ShowHand(); 
        FindObjectOfType<StagingArea>().DiscardUsedCards();
        FindObjectOfType<StagingArea>().ClearStagedAction();
        CurrentAction = new Action(ActionType.None, new AOE(AOEType.SingleTarget, 0), 0, new DeBuff(DeBuffType.None, 0));
        AllowEndTurn();
        UnHighlightHexes();
    }

    void AddMovement(PlayerCharacter character)
    {
        int amount = 1;
        if (character.hasArtifact(ArtifactType.HexMoveIncrease)) {
            amount = 2;
        }
        CurrentActions.Add(new Action(ActionType.Movement, new AOE(AOEType.SingleTarget, 0), amount, new DeBuff(DeBuffType.None, 0)));
    }

    void AddShield(PlayerCharacter character)
    {
        int amount = 3;
        if (character.hasArtifact(ArtifactType.HexShieldIncrease))
        {
            amount = 6;
        }
        List<Character> charactersShielding = new List<Character>();
        charactersShielding.Add(character);
        RemoveActionUsed();
        hexVisualizer.HexChange();
        PerformAction(new Action(ActionType.Shield, new AOE(AOEType.SingleTarget, amount), amount, new DeBuff(DeBuffType.None, 0)), charactersShielding);
    }

    void AddDraw(PlayerCharacter character)
    {
        int amountDrawn = 1;
        if (character.hasArtifact(ArtifactType.HexDrawIncrease))
        {
            amountDrawn = 2;
        }
        RemoveActionUsed();
        hexVisualizer.HexChange();
        UseImmediateAction(new Action(ActionType.DrawCard, new AOE(AOEType.SingleTarget, amountDrawn), amountDrawn, new DeBuff(DeBuffType.None, 0)));
    }

    public void AddModifier(ModifierTypes modifier, PlayerCharacter character)
    {
        switch (modifier)
        {
            case ModifierTypes.Attack:
                character.IncreaseAttack();
                RemoveActionUsed();
                ActionFinished();
                break;
            case ModifierTypes.Move:
                AddMovement(character);
                RemoveActionUsed();
                ActionFinished();
                break;
            case ModifierTypes.Shield:
                AddShield(character);
                break;
            case ModifierTypes.Draw:
                AddDraw(character);
                break;
            case ModifierTypes.Heal:
                int amountHealed = 3;
                List<Character> charactersHealing = new List<Character>();
                charactersHealing.Add(character);
                RemoveActionUsed();
                hexVisualizer.HexChange();
                PerformAction(new Action(ActionType.Heal, new AOE(AOEType.SingleTarget, amountHealed), amountHealed, new DeBuff(DeBuffType.None, 0)), charactersHealing);
                break;
            case ModifierTypes.Money:
                int goldAmount = Random.Range(10, 30);
                character.PickUpGold(goldAmount);
                RemoveActionUsed();
                ActionFinished();
                break;
        }
    }

    //Callbacks
    public void FinishedMoving(PlayerCharacter characterFinished)
    {
        if (characterFinished.HexOn.myHexModifier != null)
        {
            AddModifier(characterFinished.HexOn.myHexModifier.myModifier, characterFinished);
            Destroy(characterFinished.HexOn.myHexModifier.gameObject);
            hexVisualizer.UnhighlightHexes();
        }
        else
        {
            RemoveActionUsed();
            ActionFinished();
        }
    }

    public void FinishedAttacking(PlayerCharacter characterFinished)
    {
        RemoveActionUsed();
        ActionFinished();
    }

    public void FinishedBuffing(PlayerCharacter characterFinished)
    {
        RemoveActionUsed();
        ActionFinished();
    }

    public void FinishedHealing(PlayerCharacter characterFinished)
    {
        RemoveActionUsed();
        ActionFinished();
    }

    public void FinishedShielding(PlayerCharacter characterFinished)
    {
        RemoveActionUsed();
        ActionFinished();
    }

    //Character selection
    public void selectAnotherCharacter()
    {
        for(int i = 0; i < myCharacters.Count; i++)
        {
            if (myCharacters[i] == SelectPlayerCharacter)
            {
                int nextCharindex = i + 1 >= myCharacters.Count ? 0 : i + 1;
                SelectCharacter(myCharacters[nextCharindex]);
                return;
            }
        }
    }

    public void EnemyVanquished(float XP)
    {
        for (int i = 0; i < myCharacters.Count; i++)
        {
            myCharacters[i].GainXP(XP);
        }
    }

    public void selectNextAvailableCharacter()
    {
        foreach (PlayerCharacter character in myCharacters)
        {
            SelectCharacter(character);
        }
    }

    public void SelectCharacter(PlayerCharacter playerCharacter)
    {
       
    }

    public Character CheckIfOverCharacter()
    {
        Transform HexHit = raycaster.HexRaycast();
        if (HexHit != null && HexHit.GetComponent<Hex>())
        {
            Hex hex = HexHit.GetComponent<Hex>();
            if (hex.EntityHolding != null && hex.EntityHolding.GetComponent<Character>())
            {
                return hex.EntityHolding.GetComponent<Character>();
            }
        }
        return null;
    }

    public void CheckToSelectCharacter()
    {
        Transform HexHit = raycaster.HexRaycast();
        if (HexHit != null && HexHit.GetComponent<Hex>())
        {
            Hex hex = HexHit.GetComponent<Hex>();
            if (hex.EntityHolding != null && hex.EntityHolding.GetComponent<PlayerCharacter>())
            {
                SelectCharacter(hex.EntityHolding.GetComponent<PlayerCharacter>());
            }
        }
    }

    //Group character check
    bool AnyCharacterMoving()
    {
        foreach (PlayerCharacter character in myCharacters)
        {
            if (character.GetMoving()) { return true; }
        }
        return false;
    }

    //Map
    public void UnHighlightHexes() { hexVisualizer.UnhighlightHexes(); }

    public void ExitLevel()
    {
        actionButton.gameObject.SetActive(false);
        FindObjectOfType<LevelClearedPanel>().TurnOnPanel();
    }

    //End Turn Button
    public void AllowEndTurn()
    {
        CardsPlayable = true;
        SelectPlayerCharacter.GetMyNewHand().MakeAllCardsPlayable();
        endTurnButton.AllowEndTurn();
    }

    public void DisableEndTurn()
    {
        CardsPlayable = false;
        SelectPlayerCharacter.GetMyNewHand().MakeAllCardsUnPlayable();
        endTurnButton.DisableEndTurn();
    }

    public void CharacterDied(PlayerCharacter character)
    {
        myCharacters.Remove(character);
        StartCoroutine("CheckForLose");
    }

    IEnumerator CheckForLose()
    {
        yield return new WaitForEndOfFrame();
        if (FindObjectsOfType<PlayerCharacter>().Length == 0)
        {
            FindObjectOfType<LostScreen>().TurnOnPanel();
        }
    }

    public void ChestOpenedFor(List<Card> cards)
    {
        
    }
}
