using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    public enum DoorLocation
    {
        Right = 1,
        Left = 2,
        Middle = 3
    }

    public GameObject RoomComingFrom;
    public GameObject RoomOpeningTo;

    public DoorLocation myDoorLocation;
    public RoomSide RoomSideToBuild;

    public bool isOpen = false;
    public bool RoomShown = false;

    public List<Hex> HexesInRoom;
    public List<Hex> hexesToOpenTo;

    public GameObject door;

    public int heightOfRoom;
    public int widthOfRoom;

    public string RoomNameToBuild;

    public void OpenedRoom(string Room)
    {
        if (RoomNameToBuild == Room) { RoomShown = true; }
    }

    public bool CanMoveOnDoor()
    {
        if (isOpen) { return true; }
        else
        {
            if (GetComponent<doorConnectionHex>() != null)
            {
                return true;
            }
        }
        return false;
    }

    public void BuildDoor()
    {
        HexMapController controller = FindObjectOfType<HexMapController>();
        controller.CreateTable();
        InteractionObjects parent = FindObjectOfType<InteractionObjects>();
        GameObject doorMade = Instantiate(GetComponent<Hex>().DoorPrefab, parent.transform);
        GetComponent<HexAdjuster>().SetHexToFull();
        doorConnectionHex connectionHex = null;
        if (GetComponent<doorConnectionHex>() == null) { connectionHex = GetComponent<Node>().gameObject.AddComponent<doorConnectionHex>(); }
        else { connectionHex = GetComponent<doorConnectionHex>(); }
        connectionHex.door = this;
        doorMade.transform.localPosition = transform.position + (Vector3.up * .9f);
        switch (myDoorLocation)
        {
            case DoorLocation.Left:
                break;
            case DoorLocation.Right:
                break;
            case DoorLocation.Middle:
                doorMade.transform.rotation = Quaternion.Euler(Vector3.zero);
                break;
        }
        door = doorMade.GetComponentInChildren<DoorObject>().gameObject;
        door.GetComponent<DoorObject>().door = this;
        door.GetComponent<DoorObject>().DoorWall = doorMade;
        GetHexesInRoom(GetComponent<Node>().RoomName[0], false);
        GetHexesInRoom(RoomNameToBuild, true);
    }

    public void GetHexesInRoom(string room, bool RoomOpening)
    {
        HexMapController controller = FindObjectOfType<HexMapController>();
        if (RoomOpening)
        {
            if (hexesToOpenTo != null) { hexesToOpenTo.Clear(); };
            //Get if Any hex has that room and edges
            hexesToOpenTo = controller.GetAllHexesInThisRoom(room, GetComponent<Node>());
        }
        else
        {
            if (HexesInRoom != null) { HexesInRoom.Clear(); }
            HexesInRoom = controller.GetAllHexesInThisRoom(room, GetComponent<Node>());
        }
    }

    public void BuildRoomBySize()
    {
        HexRoomBuilder builder = FindObjectOfType<HexRoomBuilder>();
        hexesToOpenTo = builder.BuildRoomBySize(GetComponent<Node>(), heightOfRoom, widthOfRoom, RoomNameToBuild, RoomSideToBuild);
        if (hexesToOpenTo != null)
        {
            ShowHexes();
            if (!GetComponent<Node>().RoomName.Contains(RoomNameToBuild))
            {
                GetComponent<Node>().RoomName.Add(RoomNameToBuild);
            }
        }
    }


    public void OpenHexes(string RoomComingFrom)
    {
        if (isOpen) { return; }
        isOpen = true;
        GetComponent<Node>().isAvailable = true;
        //door.GetComponent<Animator>().SetTrigger("Open");
        Debug.Log("Opening Door");
        door.GetComponent<DoorNew>().OpenDoor();
        if (!RoomShown)
        {
            GetComponent<HexAdjuster>().SetHexToFull();
            RoomOpeningTo.SetActive(true);
            foreach (Hex hex in hexesToOpenTo)
            {
                hex.OpenHex(RoomNameToBuild);
                if (hex.GetComponent<Door>() != null && hex.GetComponent<Door>() != this ){
                    hex.GetComponent<Door>().OpenedRoom(RoomNameToBuild);
                    if (hex.GetComponent<Door>() != this) { hex.GetComponent<Door>().door.transform.parent.gameObject.SetActive(true); }
                }
            }
        }
    }

    public void ShowHexes()
    {
        ShowHexSet(hexesToOpenTo, RoomNameToBuild);
    }

    public void HideHexes()
    {
        HideHexSet(hexesToOpenTo, RoomNameToBuild);
    }

    public void ShowHexesInRoom()
    {
        ShowHexSet(HexesInRoom, GetComponent<Node>().RoomName[0]);
    }

    public void HideHexesInRoom()
    {
        HideHexSet(HexesInRoom, GetComponent<Node>().RoomName[0]);
    }

    void HideHexSet(List<Hex> hexes, string Room)
    {
        foreach (Hex hex in hexes)
        {
            hex.GetComponent<HexAdjuster>().RemoveRoom(Room);
            hex.GetComponent<HexAdjuster>().HideRoomEdge();
            //hex.GetComponent<HexWallAdjuster>().HideWall();
            if (hex.GetComponent<Door>() != null && hex.GetComponent<Door>().door != null)
            {
                if (hex.GetComponent<Door>().door.transform.parent.GetComponentInParent<DoorWall>() == null || !hex.GetComponent<Door>().door.transform.parent.GetComponentInParent<DoorWall>().onStart)
                {
                    hex.GetComponent<Door>().door.transform.parent.gameObject.SetActive(false);
                }
            }
            if (hex.GetComponent<ExitHex>() != null)
            {
                hex.GetComponent<ExitHex>().HideExit();
            }
            if (hex.GetComponent<HexAdjuster>().StillShowingRoom()) { continue; }
            else
            {
                hex.HideHexEditor();
                hex.GetComponent<Node>().isAvailable = false;
                if (hex.EntityHolding != null)
                {
                    DestroyImmediate(hex.EntityHolding.gameObject);
                }
                hex.HideHexEnd();
            }
        }
    }

    void ShowHexSet(List<Hex> hexes, string Room)
    {
        foreach (Hex hex in hexes)
        {
            hex.GetComponent<HexAdjuster>().AddRoomShown(Room);
            hex.GetComponent<HexAdjuster>().RevealRoomEdge();
            //hex.GetComponent<HexWallAdjuster>().ShowWall();
            if (hex.GetComponent<Door>() != null && hex.GetComponent<Door>().door != null)
            {
                hex.GetComponent<Door>().door.transform.parent.gameObject.SetActive(true);
            }
            if (hex.GetComponent<ExitHex>() != null)
            {
                hex.GetComponent<ExitHex>().ShowExit();
            }
            if (hex.GetComponent<HexAdjuster>().IsApartOfBothRooms(GetComponent<Node>().RoomName)) { continue; }
            else
            {
                hex.ShowHexEditor();
                hex.ShowHexEnd();
                hex.GetComponent<Node>().isAvailable = true;
                if (hex.EntityToSpawn != null)
                {
                    hex.GenerateCharacter();
                }
            }
        }
    }
}
