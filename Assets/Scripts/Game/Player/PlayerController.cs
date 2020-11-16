using System.Collections;
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
        if (CurrentAction.thisActionType == ActionType.Movement)
        {
            SelectPlayerCharacter.ShowMoveDistance(CurrentAction.Range);
        }
        else
        {
            SelectPlayerCharacter.ShowAction(CurrentAction.Range, CurrentAction.thisActionType);
        }
        endActionButton.gameObject.SetActive(true);
        SelectPlayerCharacter.ShowActionOnHealthBar(CurrentActions);
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
        AllowNewTurns();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            UseAction(CurrentAction);
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
        }
        else if (action.thisActionType == ActionType.Attack)
        {
            List<Character> charactersActingOn = CheckForNegativeAction(action, SelectPlayerCharacter, hexSelected);
            if (charactersActingOn != null && charactersActingOn.Count != 0)
            {
                usingAction = true;
                PerformAction(action, charactersActingOn);
            }
        }
        else
        {
            List<Character> charactersActingOn = CheckForPositiveAction(action, SelectPlayerCharacter, hexSelected);
            if (charactersActingOn.Count != 0)
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
        CurrentActions.RemoveAt(0);
        SelectPlayerCharacter.ShowActionOnHealthBar(CurrentActions);
    }

    void PerformAction(Action action, List<Character> characters)
    {
        RemoveArea();
        if (action.thisActionType == ActionType.Attack) { SelectPlayerCharacter.Attack(action.thisAOE.Damage, action.thisDeBuff, characters.ToArray()); }
        else { SelectPlayerCharacter.GetComponent<CharacterAnimationController>().DoBuff(action.thisActionType, action.thisAOE.Damage, characters); }
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
        AllowEndTurn();
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
                    break;
            }
        }
        else
        {
            AllowNewActions();
        }
    }

    public void AllowNewActions()
    {
        endActionButton.gameObject.SetActive(false);
        myCamera.UnLockCamera();
        if (FindObjectOfType<StagingArea>() == null) { return; }
        FindObjectOfType<NewHand>().ShowHand(); 
        FindObjectOfType<StagingArea>().DiscardUsedCards();
        FindObjectOfType<StagingArea>().ClearStagedAction();
        AllowEndTurn();
        UnHighlightHexes();
    }

    //Callbacks
    public void FinishedMoving(PlayerCharacter characterFinished)
    {
        RemoveActionUsed();
        ActionFinished();
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
