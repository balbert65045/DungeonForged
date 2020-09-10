using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexVisualizer : MonoBehaviour {

    private CameraRaycaster cameraRaycaster;
    private PlayerController playerController;
    private HexMapController hexMap;
    private List<Hex> LastHexesChanged = new List<Hex>();
    private Hex LastHexOver;

    public Material SelectionHighlightMaterial;
    public Material MovePointHighlightMaterial;
    public Material MoveRangeHighlightMaterial;
    public Material AttackAreaHighlightMaterial;
    public Material AttackRangeHighlightMaterial;
    public Material AttackViewHighlightMaterial;
    public Material HealPointHighlightMaterial;
    public Material HealRangeHighlightMaterial;
    public Material ShieldRangeHighlightMaterial;
    public Material ShieldPointHighlightMaterial;

    public void UnhighlightHexes()
    {
        Hex[] hexes = hexMap.AllHexes;
        foreach (Hex hex in hexes)
        {
            if (hex.HexNode.Shown)
            {
                hex.UnHighlight();
            }
        }
    }

    public void ReturntHexesToPreviousColor()
    {
        Hex[] hexes = hexMap.AllHexes;
        foreach (Hex hex in hexes)
        {
            if (hex.HexNode.Shown)
            {
                hex.returnToPreviousColor();
            }
        }
    }

    public void UnHighlightHex(Hex hex)
    {
        hex.UnHighlight();
    }

    public void DeactivateHex(Hex hex)
    {
        hex.GetComponent<MeshRenderer>().material = ShieldRangeHighlightMaterial;
        hex.previousMaterial = ShieldRangeHighlightMaterial;
        hex.OGMaterial = ShieldRangeHighlightMaterial;
    }

    public void HighlightActionRangeHex(Hex hex, ActionType action)
    {
        switch (action)
        {
            case ActionType.Attack:
                HighlightAttackRangeHex(hex);
                break;
            case ActionType.Heal:
                HighlightHealRangeHex(hex);
                break;
            case ActionType.Shield:
                HighlightArmorRangeHex(hex);
                break;
            case ActionType.BuffArmor:
                HighlightBuffRangeHex(hex);
                break;
            case ActionType.BuffAttack:
                HighlightBuffRangeHex(hex);
                break;
            case ActionType.BuffMove:
                HighlightBuffRangeHex(hex);
                break;
            case ActionType.BuffRange:
                HighlightBuffRangeHex(hex);
                break;
        }
    }

    public void HighlightActionPointHex(Hex hex, ActionType action)
    {
        switch (action)
        {
            case ActionType.Attack:
                HighlightAttackAreaHex(hex);
                break;
            case ActionType.Heal:
                HighlightHealPointHex(hex);
                break;
            case ActionType.Shield:
                HighlightArmorPointHex(hex);
                break;
            case ActionType.BuffArmor:
                HighlightBuffPointHex(hex);
                break;
            case ActionType.BuffAttack:
                HighlightBuffPointHex(hex);
                break;
            case ActionType.BuffMove:
                HighlightBuffPointHex(hex);
                break;
            case ActionType.BuffRange:
                HighlightBuffPointHex(hex);
                break;
        }
    }

    public void HighlightMoveRangeHex(Hex hex)
    {
        hex.GetComponent<MeshRenderer>().material = MoveRangeHighlightMaterial;
    }

    public void HighlightMovePointHex(Hex hex)
    {
        hex.GetComponent<MeshRenderer>().material = MovePointHighlightMaterial;
    }

    public void HighlightAttackRangeHex(Hex hex)
    {
        hex.GetComponent<MeshRenderer>().material = AttackRangeHighlightMaterial;
    }

    public void HighlightAttackAreaHex(Hex hex)
    {
        hex.GetComponent<MeshRenderer>().material = AttackAreaHighlightMaterial;
    }

    public void HighlightAttackViewArea(Hex hex)
    {
        hex.GetComponent<MeshRenderer>().material = AttackViewHighlightMaterial;
        hex.previousMaterial = AttackViewHighlightMaterial;
    }

    public void HighlightHealRangeHex(Hex hex)
    {
        hex.GetComponent<MeshRenderer>().material = HealRangeHighlightMaterial;
    }

    public void HighlightHealPointHex(Hex hex)
    {
        hex.GetComponent<MeshRenderer>().material = HealPointHighlightMaterial;
    }

    public void HighlightArmorPointHex(Hex hex)
    {
        hex.GetComponent<MeshRenderer>().material = ShieldPointHighlightMaterial;
    }

    public void HighlightArmorRangeHex(Hex hex)
    {
        hex.GetComponent<MeshRenderer>().material = ShieldRangeHighlightMaterial;
    }

    public void HighlightBuffRangeHex(Hex hex)
    {
        hex.GetComponent<MeshRenderer>().material = HealRangeHighlightMaterial;
    }

    public void HighlightBuffPointHex(Hex hex)
    {
        hex.GetComponent<MeshRenderer>().material = HealPointHighlightMaterial;
    }


    public void HighlightSelectionHex(Hex hex)
    {
        hex.GetComponent<MeshRenderer>().material = SelectionHighlightMaterial;
    }

    public void ShowChestPath(Hex ChestHex)
    {
        LastHexOver = null;
        PlayerCharacter myCharacter = playerController.SelectPlayerCharacter;
        if (myCharacter.GetMoving()) { return; }
        Node ClosestNode = GetClosestPathToAdjacentHexes(myCharacter, ChestHex);
        if (ClosestNode != null)
        {
            ReturnMoveHexes(myCharacter);
            HighlightMovePath(ClosestNode.NodeHex);
        }
    }

    public void ShowDoorPath(Door doorHex)
    {
        LastHexOver = null;
        PlayerCharacter myCharacter = playerController.SelectPlayerCharacter;
        if (myCharacter == null || myCharacter.GetMoving()) { return; }
        if (!myCharacter.HexInMoveRange(doorHex.GetComponent<Hex>(), myCharacter.CurrentMoveDistance)) { return; }
        ReturnMoveHexes(myCharacter);
        if (myCharacter.HexOn == doorHex.GetComponent<Hex>()) { return; }
        if (doorHex.GetComponent<doorConnectionHex>() != null)
        {
            if (doorHex.GetComponent<Hex>().EntityHolding == null && !doorHex.GetComponent<Hex>().MovedTo) { HighlightMovePath(doorHex.GetComponent<Hex>()); }
        }
        else
        {
            Node ClosestNode = GetClosestPathToAdjacentHexes(myCharacter, doorHex.GetComponent<Hex>());
            if (ClosestNode != null)
            {
                HighlightMovePath(ClosestNode.NodeHex);
            }
        }
    }

    public Node GetClosestPathToAdjacentHexes(Character character, Hex hex)
    {
        Node closestNode = FindObjectOfType<HexMapController>().GetClosestNodeFromNeighbors(hex, character);
        if (closestNode == character.HexOn.HexNode) { ClearLastChangedHexes(); }
        return closestNode;
    }

    public void HighlightMovePath(Hex hex)
    {
        if (hex.GetComponent<Door>() != null && !hex.GetComponent<Door>().CanMoveOnDoor()) { return; }
        PlayerCharacter myCharacter = playerController.SelectPlayerCharacter;
        List<Node> NodePath = myCharacter.GetPath(hex.HexNode);
        if (NodePath == null) { return; }
        foreach (Node node in NodePath)
        {
            LastHexesChanged.Add(node.NodeHex);
            HighlightMovePointHex(node.NodeHex);
        }
    }

    public void ClearLastChangedHexes()
    {
        if (LastHexesChanged.Count != 0)
        {
            foreach (Hex lastHex in LastHexesChanged)
            {
                if (lastHex == playerController.SelectPlayerCharacter.HexOn) { continue; }
                lastHex.returnToPreviousColor();
            }
            LastHexesChanged.Clear();
        }
    }

    void ClearLastChangedHexSelf()
    {
        if (LastHexesChanged.Count != 0)
        {
            foreach (Hex lastHex in LastHexesChanged)
            {
                if (lastHex == playerController.SelectPlayerCharacter.HexOn) {
                    HighlightSelectionHex(lastHex);
                }
                else { lastHex.returnToPreviousColor(); }
            }
            LastHexesChanged.Clear();
        }
    }

    void ShowActionArea(Character myCharacter, Hex hex, ActionType type)
    {
        if (myCharacter.HexInActionRange(hex))
        {
            if (LastHexesChanged.Count != 0)
            {
                foreach (Hex lastHex in LastHexesChanged)
                {
                    lastHex.UnHighlight();
                }
                LastHexesChanged.Clear();
            }
            HighlightActionArea(hex, type);
        }
        else
        {
            if (LastHexesChanged.Count != 0)
            {
                foreach (Hex lastHex in LastHexesChanged)
                {
                    lastHex.UnHighlight();
                }
                LastHexesChanged.Clear();
            }
        }
    }

    public void HighlightActionArea(Hex hex, ActionType type)
    {
        PlayerCharacter myCharacter = playerController.SelectPlayerCharacter;
        List<Node> nodesInAOE = FindObjectOfType<HexMapController>().GetAOE(playerController.CurrentAction.thisAOE.thisAOEType, myCharacter.HexOn.HexNode, hex.HexNode);
        foreach (Node node in nodesInAOE)
        {
            if (node == null) { break; }
            HighlightActionPointHex(node.NodeHex, type);
            LastHexesChanged.Add(node.NodeHex);
        }
    }

    public void ResetLastHex() { LastHexOver = null; }

    void ReturnMoveHexes(Character myCharacter)
    {
        if (LastHexesChanged.Count != 0)
        {
            foreach (Hex lastHex in LastHexesChanged)
            {
                if (myCharacter.HexOn == lastHex) { continue; }
                lastHex.UnHighlight();
            }
            LastHexesChanged.Clear();
        }
    }

    public void OnHexChanged(Hex hex)
    {
        
        if (LastHexOver == hex) {return;}
        LastHexOver = hex;

        PlayerCharacter myCharacter = playerController.SelectPlayerCharacter;
        if (myCharacter == null) { return; }
        //if (!myCharacter.InCombat() && combatcontroller.PickingCards())
        //{
        //    Debug.Log("Hex Changed Combat");
        //    if (myCharacter.GetMoving()) { return; }
        //    if (outOfCombatcontroller.LookingInChest) { return; }
        //    if (outOfCombatcontroller.cardUsing == null)
        //    {
        //        if (hex == null || !hex.HexNode.Shown) { return; }
        //        if(myCharacter.GetMoving()) { return; }
        //        ReturnMoveHexes(myCharacter);
        //        if (myCharacter.HexInMoveRange(hex, myCharacter.GetCurrentMoveRange()))
        //        {
        //            HighlightMovePath(hex);
        //        }
        //    }
        //    else
        //    {
        //        switch (outOfCombatcontroller.cardUsing.cardAbility.Actions[0].thisActionType)
        //        {
        //            case ActionType.Scout:
        //                ClearLastChangedHexSelf();
        //                if (hex == myCharacter.HexOn)
        //                {
        //                    HighlightHealPointHex(hex);
        //                    LastHexesChanged.Add(hex);
        //                }
        //                break;
        //            case ActionType.Stealth:
        //                ClearLastChangedHexSelf();
        //                if (hex == myCharacter.HexOn)
        //                {
        //                    HighlightHealPointHex(hex);
        //                    LastHexesChanged.Add(hex);
        //                }
        //                break;
        //            case ActionType.BuffAttack:
        //                ShowActionArea(myCharacter, hex, ActionType.BuffAttack);
        //                break;
        //            case ActionType.BuffMove:
        //                ShowActionArea(myCharacter, hex, ActionType.BuffMove);
        //                break;
        //            case ActionType.BuffRange:
        //                ShowActionArea(myCharacter, hex, ActionType.BuffRange);
        //                break;
        //            case ActionType.BuffArmor:
        //                ShowActionArea(myCharacter, hex, ActionType.BuffArmor);
        //                break;
        //            case ActionType.Heal:
        //                ShowActionArea(myCharacter, hex, ActionType.Heal);
        //                break;
        //            case ActionType.Attack:
        //                ShowActionArea(myCharacter, hex, ActionType.Attack);
        //                break;
        //        }
        //    }
        //}
        ////else if (myCharacter.InCombat() && combatcontroller.GetCombatState() == CombatActionController.CombatState.UsingCombatCards)
        //else
        if (hex == null || !hex.HexNode.Shown) { return; }
        switch (playerController.CurrentAction.thisActionType)
        {
            case ActionType.Movement:
            {
                    if (myCharacter.GetMoving()) { return; }
                    if (LastHexesChanged.Count != 0)
                    {
                        foreach (Hex lastHex in LastHexesChanged)
                        {
                            lastHex.UnHighlight();
                        }
                        LastHexesChanged.Clear();
                    }
                    if (myCharacter.HexInMoveRange(hex, myCharacter.GetCurrentMoveRange()))
                    {
                        HighlightMovePath(hex);
                    }
                    break;
            }
            case ActionType.Attack:
            {
                if (!playerController.CardsPlayable) { return; }
                ShowActionArea(myCharacter, hex, ActionType.Attack);
                break;
            }
            case ActionType.Heal:
            {
                ShowActionArea(myCharacter, hex, ActionType.Heal);
                break;
            }
            case ActionType.Shield:
            {
                ShowActionArea(myCharacter, hex, ActionType.Shield);
                break;
            }
            case ActionType.BuffAttack:
            {
                ShowActionArea(myCharacter, hex, ActionType.BuffAttack);
                break;
            }
            case ActionType.BuffArmor:
            {
                ShowActionArea(myCharacter, hex, ActionType.BuffArmor);
                break;
            }
            case ActionType.BuffMove:
            {
                ShowActionArea(myCharacter, hex, ActionType.BuffMove);
                break;
            }
            case ActionType.BuffRange:
            {
                ShowActionArea(myCharacter, hex, ActionType.BuffRange);
                break;
            }
        }
    }

    // Use this for initialization
    void Start () {
        playerController = GetComponent<PlayerController>();
        hexMap = FindObjectOfType<HexMapController>();
        cameraRaycaster = FindObjectOfType<CameraRaycaster>();
        cameraRaycaster.notifyCursorOverHexObservers += OnHexChanged;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
