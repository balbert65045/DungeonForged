using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRaycaster : MonoBehaviour {

    public LayerMask HexLayer;
    float maxRaycastDepth = 100f; // Hard coded value

    // Setup delegates for broadcasting layer changes to other classes
    public delegate void OnCursorOverHex(Hex hex); // declare new delegate type
    public event OnCursorOverHex notifyCursorOverHexObservers; // instantiate an observer set

    public LayerMask WallMask;

    public LayerMask InterActionMask;
    public GameObject InteractableObjectOver;

    public Sprite DoorSprite;
    public Sprite ChestSprite;
    public Sprite Pointer;
    Image cursorImage;
    HexVisualizer hexVisualizer;
    PlayerController playerController;

    Vector2 cursorPoint;

    // Use this for initialization
    void Awake()
    {
        Cursor.visible = false;
        cursorImage = GetComponentInChildren<Image>();
        cursorImage.sprite = Pointer;
        hexVisualizer = FindObjectOfType<HexVisualizer>();
        playerController = FindObjectOfType<PlayerController>();
    }

    public Transform HexRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit Hit;
        if (Physics.Raycast(ray, out Hit, 100f, HexLayer)) { return Hit.transform;}
        return null;
    }

    public Transform InteractableRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit Hit;
        if (Physics.Raycast(ray, out Hit, 100f, InterActionMask)) { return Hit.transform; }
        return null;
    }

    public Transform WallRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit Hit;
        if (Physics.Raycast(ray, out Hit, 100f, WallMask)) { return Hit.transform; }
        return null;
    }

    bool ObjectInCharacterRange(GameObject Object)
    {
        return playerController.SelectPlayerCharacter.HexInMoveRange(Object.GetComponent<Hex>(), playerController.SelectPlayerCharacter.CurrentMoveDistance); 
    }

    void CombatRaycast()
    {
        Transform HexFound = HexRaycast();
        if (HexFound != null && HexFound.GetComponent<Hex>())
        {
            notifyCursorOverHexObservers(HexFound.GetComponent<Hex>());
            if (!ObjectInCharacterRange(HexFound.gameObject))
            {
                cursorImage.sprite = Pointer;
                return;
            }
            if (HexFound.GetComponent<Door>() != null && !HexFound.GetComponent<Door>().isOpen) { cursorImage.sprite = DoorSprite; }
            else { cursorImage.sprite = Pointer; }
            return;
        }
    }

    void OutOfCombatRaycast()
    {
        //if (playerController.SelectPlayerCharacter.OutOfActions()) {
        //    cursorImage.sprite = Pointer;
        //    return;
        //}
        Transform ActionHit = null;
        ActionHit = WallRaycast();
        if (ActionHit != null)
        {
            if (ActionHit.GetComponent<DoorObject>() != null && !ActionHit.GetComponent<DoorObject>().door.isOpen)
            {
                if (InteractableObjectOver != ActionHit.gameObject)
                {
                    if (!ObjectInCharacterRange(ActionHit.GetComponent<DoorObject>().door.gameObject))
                    {
                        cursorImage.sprite = Pointer;
                        return;
                    }
                    cursorImage.sprite = DoorSprite;
                    hexVisualizer.ShowDoorPath(ActionHit.GetComponent<DoorObject>().door);
                    InteractableObjectOver = ActionHit.gameObject;
                }
                return;
            }
        }

        ActionHit = InteractableRaycast();
        if (ActionHit != null)
        {
            if (ActionHit.GetComponent<CardChest>() && !ActionHit.GetComponent<CardChest>().isOpen)
            {
                if (InteractableObjectOver != ActionHit.gameObject)
                {
                    List<Node> AdjacentNodes = FindObjectOfType<HexMapController>().GetNodesAdjacent(ActionHit.GetComponent<Entity>().HexOn.HexNode);
                    Node ClosestNode = hexVisualizer.GetClosestPathToAdjacentHexes(playerController.SelectPlayerCharacter, ActionHit.GetComponent<Entity>().HexOn);
                    if (!ObjectInCharacterRange(ClosestNode.gameObject) && !AdjacentNodes.Contains(playerController.SelectPlayerCharacter.HexOn.HexNode))
                    {
                        cursorImage.sprite = Pointer;
                        return;
                    }
                    cursorImage.sprite = ChestSprite;
                    hexVisualizer.ShowChestPath(ActionHit.GetComponent<Entity>().HexOn);
                    InteractableObjectOver = ActionHit.gameObject;
                }
                return;
            }
        }
        InteractableObjectOver = null;

        Transform HexHit = HexRaycast();
        if (HexHit != null && HexHit.GetComponent<Hex>() != null)
        {
            notifyCursorOverHexObservers(HexHit.GetComponent<Hex>());
            if (!ObjectInCharacterRange(HexHit.gameObject))
            {
                cursorImage.sprite = Pointer;
                return;
            }
            if (HexHit.GetComponent<Door>() != null && !HexHit.GetComponent<Door>().isOpen) { cursorImage.sprite = DoorSprite; }
            else { cursorImage.sprite = Pointer; }
            return;
        }
        cursorImage.sprite = Pointer;
    }

    // Update is called once per frame
    void Update () {

        cursorPoint = Input.mousePosition;
        transform.position = cursorPoint;

        if (playerController.SelectPlayerCharacter == null) { return; }

        CombatRaycast();
        //if (playerController.SelectPlayerCharacter.InCombat()) {
        //    CombatRaycast();
        //}
        //else {
        //    OutOfCombatRaycast();
        //}
    }
}
