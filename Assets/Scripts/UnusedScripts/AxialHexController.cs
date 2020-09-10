using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxialHexController : MonoBehaviour {

    public Hashtable HexMap = new Hashtable();

    public void AddHex(Node2 node) { HexMap.Add(GetHexHash(node.q, node.r), node); }
    public string GetHexHash(int x, int y) { return x.ToString() + "," + y.ToString(); }
    public Node2 GetNode(int x, int y) { return (Node2)HexMap[GetHexHash(x, y)]; }

    public Node2[] GetNeighbors(Node2 node)
    {
        return new Node2[] {
            (Node2)HexMap[GetHexHash(node.q + 1, node.r)],
            (Node2)HexMap[GetHexHash(node.q + 1, node.r - 1)],
            (Node2)HexMap[GetHexHash(node.q, node.r - 1)],
            (Node2)HexMap[GetHexHash(node.q - 1, node.r)],
            (Node2)HexMap[GetHexHash(node.q - 1, node.r + 1)],
            (Node2)HexMap[GetHexHash(node.q, node.r + 1)],
        };
    }

    public Vector2[] GetDirections()
    {
        return new Vector2[] {
            //East
            new Vector2(1, 0),
            //NorthEast
            new Vector2(1, -1),
            //NorthWest
            new Vector2(0, -1),
            //West
            new Vector2(-1, 0),
            //SouthWest
            new Vector2(-1, +1),
            //SouthEast
            new Vector2(0, +1),
        };
    }

    public int HexDistance(Node2 a, Node2 b)
    {
        return (Mathf.Abs(a.q - b.q) + Mathf.Abs(a.q + a.r - b.q - b.r) + Mathf.Abs(a.r - b.r)) / 2;
    }

    //List<Node2> GetNodesAdjacent(Node2 node, bool edge)
    //{
    //    Node2[] Neighbors = GetNeighbors(node);
    //    List<Node2> AdjacentNodesAvailable = new List<Node2>();
    //    foreach(Node2 aNode in AdjacentNodesAvailable)
    //    {
    //        if ()
    //    }
    //}

    //bool NodeIsAvailableToNode(Node2 a, Node2 b)
    //{

    //}

    //public List<Node2> GetNodesAtDistanceFromNode(Node2 StartNode, int distance, bool edge = false)
    //{
    //    List<Node2> NodesChecked = new List<Node2>();
    //    List<Node2> NodesToCheckInDistance = new List<Node2>();
    //    List<Node2> NodesinInDistance = new List<Node2>();
    //    NodesToCheckInDistance.Add(StartNode);
    //    for (int i = 0; i < distance; i++)
    //    {
    //        int CurrentAmountToCheck = NodesToCheckInDistance.Count;
    //        for (int j = 0; j < CurrentAmountToCheck; j++)
    //        {
    //            List<Node2> AdjacentNodes = GetNodesAdjacent(NodesToCheckInDistance[0], edge);
    //            foreach (Node2 node in AdjacentNodes)
    //            {
    //                if (NodesChecked.Contains(node) || NodesToCheckInDistance.Contains(node)) { continue; }
    //                NodesinInDistance.Add(node);
    //                NodesToCheckInDistance.Add(node);
    //            }
    //            NodesChecked.Add(NodesToCheckInDistance[0]);
    //            NodesToCheckInDistance.Remove(NodesToCheckInDistance[0]);
    //        }
    //    }
    //    return NodesinInDistance;
    //}


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
