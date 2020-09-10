using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<string> RoomName;
    public void SetRoomName(string name)
    {
        RoomName.Clear();
        RoomName.Add(name);
    }
    public void AddRoomName(string name)
    {
        if (!RoomName.Contains(name)) { RoomName.Add(name); }
    }

    public bool Used = false;
    public bool edge = false;
    public bool isAvailable = true;
    public bool Shown = false;

    public int q;
    public int r;
    public int s;

    public bool IsWalkable { get; set; }
    public float G { get; set; }
    public float H { get; set; }
    public float F { get { return this.G + this.H; } }
    public NodeState State { get; set; }
    public Node ParentNode { get; set; }

    public Hex NodeHex { get; set; }

    public bool isConnectedToRoom(Node node)
    {
        if (node.isAvailable || node.Used)
        {
            if (node.RoomName.Count == this.RoomName.Count && node.RoomName[0] == this.RoomName[0])
            {
                return true;
            }
            //Moving to door
            else if (node.RoomName.Count > this.RoomName.Count && node.RoomName.Contains(this.RoomName[0]))
            {
                return true;
            }
            //Moving off door
            else if (this.RoomName.Count > node.RoomName.Count && this.RoomName.Contains(node.RoomName[0]))
            {
                return true;
            }
        }
        return false;
    }

    //EnviromentTile Tile;
    private void Start()
    {
        NodeHex = GetComponent<Hex>();
    }

    public static float GetTraversalCost(Node startLocation, Node endLocation)
    {
        return (startLocation.transform.position - endLocation.transform.position).magnitude;
    }

    //public void CalculateG(Node StartPoint)
    //{
    //    G = (StartPoint.transform.position - transform.position).magnitude;
    //}
    

    public void CalculateH(Node EndPoint)
    {
        H  = (EndPoint.transform.position - transform.position).magnitude;
    }


    public void SetNode(int r_, int q_)
    {
        r = r_;
        q = q_;
        s = -q_ - r_;
    }

}

public enum NodeState { Untested, Open, Closed }
