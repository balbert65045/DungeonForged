using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexAdjuster : MonoBehaviour {

    public Mesh FullHexOutline;
    public Mesh HalfHexOutline;
    public Mesh PartialHexOutline;
    public Mesh DoublePartialHexOutline;

    public Mesh FullHex;
    public Mesh HalfHex;
    public Mesh FragmentHex;
    public Mesh DoubleFragmentAcross;
    public Mesh DoubleFragmentSide;

    public string LeftRoomSide;
    public string RightRoomSide;
    public string UpRoomSide;
    public string DownRoomSide;

    bool CompletelyShown = false;
    Node myNode;

    private void Start()
    {
        hexMap = FindObjectOfType<HexMapController>();
        myNode = GetComponent<Node>();
    }

    public void ClearSides()
    {
        LeftRoomSide = "";
        RightRoomSide = "";
        UpRoomSide = "";
        DownRoomSide = "";
    }

    public bool FullyShown()
    {
        if (RoomsShown.Count > 1) {return CompletelyShown;}
        return true;
    }

    public List<string> RoomsShown = new List<string>();
    public void AddRoomShown(string room)
    {
        if (!RoomsShown.Contains(room))
        {
            RoomsShown.Add(room);
        }
    }
    public void RemoveRoom(string room)
    {
        if (RoomsShown.Contains(room))
        {
            RoomsShown.Remove(room);
        }
    }

    public bool IsApartOfBothRooms(List<string> rooms)
    {
        if (LeftRoomSide.Length != 0 && RightRoomSide.Length != 0)
        {
            if (rooms.Contains(LeftRoomSide) && rooms.Contains(RightRoomSide)) { return true; }
        }
        else if (UpRoomSide.Length != 0 && DownRoomSide.Length != 0)
        {
            if (rooms.Contains(UpRoomSide) && rooms.Contains(DownRoomSide)) { return true; }
        }
        return false;
    }

    public bool StillShowingRoom()
    {
        return RoomsShown.Count != 0;
    }

    public bool HideRoomEdge()
    {
        CompletelyShown = false;
        if (RoomsShown.Count == 0) { return false; }
        if (RoomsShown.Contains(UpRoomSide))
        {
            SetHexToFragment();
            RotateHexToTopMiddle();
        }
        else if (RoomsShown.Contains(DownRoomSide))
        {
            SetHexToFragment();
            RotateHexToBottomMiddle();
        }
        else if (RoomsShown.Contains(LeftRoomSide))
        {
            SetHexToHalf();
            RotateHexToBottomLeft();
        }
        else if (RoomsShown.Contains(RightRoomSide))
        {
            SetHexToHalf();
            RotateHexToTopRight();
        }
        return true;
    }

    HexMapController hexMap;
    public void RevealEdgeHexes()
    {
        if (GetComponent<Door>() != null) { return; }
        if (hexMap == null) { hexMap = FindObjectOfType<HexMapController>(); }
        List<Node> nodes = hexMap.GetNeighborsNoRoom(myNode);
        foreach (Node node in nodes)
        {
            if (node.edge)
            {
                if (!node.GetComponent<HexAdjuster>().CompletelyShown && node.isConnectedToRoom(myNode)) {
                    node.GetComponent<HexAdjuster>().RevealRoomEdge();
                    //node.NodeHex.ShowHex();
                   // node.GetComponent<HexWallAdjuster>().ShowWall();
                }
            }
        }
    }

    public void RevealRoomEdge()
    {
        if (RoomsShown.Contains(UpRoomSide) && RoomsShown.Contains(DownRoomSide))
        {
            CompletelyShown = true;
            SetHexToDoubleAcross();
        }
        else if (RoomsShown.Contains(LeftRoomSide) && RoomsShown.Contains(RightRoomSide))
        {
            CompletelyShown = true;
            SetHexToEdgeFull();
        }
        else if (RoomsShown.Contains(UpRoomSide))
        {
            SetHexToFragment();
            RotateHexToTopMiddle();
        }
        else if (RoomsShown.Contains(DownRoomSide))
        {
            SetHexToFragment();
            RotateHexToBottomMiddle();
        }
        else if (RoomsShown.Contains(LeftRoomSide))
        {
            SetHexToHalf();
            RotateHexToBottomLeft();
        }
        else if (RoomsShown.Contains(RightRoomSide))
        {
            SetHexToHalf();
            RotateHexToTopRight();
        }
    }

    public void SetHexToHalf()
    {
        GetComponent<MeshFilter>().mesh = HalfHex;
        if (GetComponent<Door>() == null)
        {
            GetComponent<Node>().edge = true;
        }
      //  GetComponent<Hex>().OutLine.SetActive(false);
        //GetComponent<Hex>().OutLine.GetComponent<MeshFilter>().mesh = HalfHexOutline;
    }

    public void SetHexToFragment()
    {
     //   GetComponent<Hex>().OutLine.SetActive(false);
       // GetComponent<Hex>().OutLine.GetComponent<MeshFilter>().mesh = PartialHexOutline;
        if (GetComponent<MeshFilter>().sharedMesh == DoubleFragmentAcross){ RotateWithoutChild(true); }
        GetComponent<MeshFilter>().mesh = FragmentHex;
        if (GetComponent<Door>() == null)
        {
            GetComponent<Node>().edge = true;
        }
    }

    public void SetHexToEdgeFull()
    {
        GetComponent<MeshFilter>().mesh = FullHex;
      //  GetComponent<Hex>().OutLine.SetActive(false);
       // GetComponent<Hex>().OutLine.GetComponent<MeshFilter>().mesh = FullHexOutline;
    }

    public void SetHexToFull()
    {
        transform.rotation = Quaternion.Euler(90, 0, 0);
        GetComponent<MeshFilter>().mesh = FullHex;
        GetComponent<Hex>().OutLine.GetComponent<MeshFilter>().mesh = FullHexOutline;
        GetComponent<Node>().edge = false;
    }

    public void SetHexToDoubleAcross()
    {
        RotateWithoutChild(false);
        GetComponent<MeshFilter>().mesh = DoubleFragmentAcross;
        //  GetComponent<Hex>().OutLine.SetActive(false);
        //  GetComponent<Hex>().OutLine.GetComponent<MeshFilter>().mesh = DoublePartialHexOutline;
        if (GetComponent<Door>() == null)
        {
            GetComponent<Node>().edge = true;
        }
    }

    void RotateWithoutChild(bool forward)
    {
        //int childrenCount = transform.childCount;
        //List<Transform> children = new List<Transform>();
        //for (int i = 0; i < childrenCount; i++)
        //{
        //    children.Add(transform.GetChild(0));
        //    transform.GetChild(0).SetParent(null);
        //}
        //if (forward) { Rotate60Forward(); }
        //else { Rotate60Backward(); }
        //foreach (Transform tranform in children)
        //{
        //    tranform.SetParent(this.transform);
        //}
    }

    public void SetHexToDoubleSide()
    {
        GetComponent<MeshFilter>().mesh = DoubleFragmentSide;
        //GetComponent<Hex>().OutLine.GetComponent<MeshFilter>().mesh = DoublePartialHexOutline;
        //GetComponent<Hex>().OutLine.SetActive(false);
        if (GetComponent<Door>() == null)
        {
            GetComponent<Node>().edge = true;
        }
    }

    public void RotateHexToTopRight()
    {
        transform.rotation = Quaternion.Euler(90, 0, 180);
    }

    public void RotateHexToTopMiddle()
    {
        int childrenCount = transform.childCount;
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < childrenCount; i++)
        {
            children.Add(transform.GetChild(0));
            transform.GetChild(0).SetParent(null);
        }
        transform.rotation = Quaternion.Euler(90, 0, 0);
        foreach (Transform tranform in children)
        {
            tranform.SetParent(this.transform);
        }
        //GetComponent<Hex>().OutLine.transform.localRotation = Quaternion.Euler(0, -180, 240);
    }

    public void RotateHexToTopLeft()
    {
        transform.rotation = Quaternion.Euler(90, 0, -60);
    }

    public void RotateHexToBottomLeft()
    {
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    public void RotateHexToBottomMiddle()
    {
        int childrenCount = transform.childCount;
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < childrenCount; i++)
        {
            children.Add(transform.GetChild(0));
            transform.GetChild(0).SetParent(null);
        }
        transform.rotation = Quaternion.Euler(90, 0, 180);
        foreach (Transform tranform in children)
        {
            tranform.SetParent(this.transform);
        }
    }

    public void RotateHexToBottomRight()
    {
        transform.rotation = Quaternion.Euler(90, 0, 120);
    }


    public void Rotate60Forward()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 60);
    }

    public void Rotate60Backward()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - 60);
    }

    public void Rotate180()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + 180);
    }

}
