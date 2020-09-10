using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class AStar : MonoBehaviour
{

    // For more detail on the A* algorithm follow this link http://blog.two-cats.com/2014/06/a-star-example/ 

    HexMapController controller;

    private Node[] nodes;
    private Node startNode;
    private Node endNode;
    private List<Node> OpenNodes;
    private List<Node> ClosedNodes;

    private void Start()
    {
        controller = GetComponent<HexMapController>();
        nodes = GetComponentsInChildren<Node>();
    }

    public bool PathAvailable(Node Begin, Node End, Character.CharacterType CT)
    {
        startNode = Begin;
        endNode = End;
        startNode.G = 0;
        foreach (Node node in nodes)
        {
            node.CalculateH(endNode);
            node.State = NodeState.Untested;
            node.ParentNode = null;

            if (node.GetComponent<Hex>().EntityHolding == null || node.GetComponent<Hex>().HasSameType(CT))
            {
                node.IsWalkable = true;
            }
            else
            {
                node.IsWalkable = false;
            }

        }
        OpenNodes = new List<Node>();
        ClosedNodes = new List<Node>();

        // Add Starting point into list to become closed
        OpenNodes.Add(startNode);
        return (Search(startNode));
    }

    public List<Node> GetRawPath(Node Begin, Node End)
    {
        // Initialize values and nodes
        startNode = Begin;
        endNode = End;
        startNode.G = 0;
        foreach (Node node in nodes)
        {
            node.CalculateH(endNode);
            node.State = NodeState.Untested;
            node.ParentNode = null;
            node.IsWalkable = true;

        }
        OpenNodes = new List<Node>();
        ClosedNodes = new List<Node>();
        List<Node> path = new List<Node>();

        // Add Starting point into list to become closed
        OpenNodes.Add(startNode);
        // Search for a path between start point and end point and return true if possible 
        bool success = Search(startNode);
        if (success)
        {
            // Reverse the path list for path from start to end
            Node node = this.endNode;
            while (node.ParentNode != null)
            {
                path.Add(node);
                node = node.ParentNode;
            }
            path.Reverse();
        }
        return path;
    }


    public List<Node> ShowPathDistance(Node Begin, Node End)
    {
        // Initialize values and nodes
        startNode = Begin;
        endNode = End;
        startNode.G = 0;
        foreach (Node node in nodes)
        {
            node.CalculateH(endNode);
            node.State = NodeState.Untested;
            node.ParentNode = null;

            if (node.GetComponent<Hex>().EntityHolding == null || node == endNode)
            {
                node.IsWalkable = true;
            }
            else
            {
                node.IsWalkable = false;
            }
        }
        OpenNodes = new List<Node>();
        ClosedNodes = new List<Node>();
        List<Node> path = new List<Node>();

        // Add Starting point into list to become closed
        OpenNodes.Add(startNode);
        // Search for a path between start point and end point and return true if possible 
        bool success = Search(startNode);
        if (success)
        {
            // Reverse the path list for path from start to end
            Node node = this.endNode;
            while (node.ParentNode != null)
            {
                path.Add(node);
                node = node.ParentNode;
            }
            path.Reverse();
        }
        return path;
    }


    int index = 0;
    // Finds a path between Tile A and B using the A* algorithm
    public List<Node> FindPath(Node Begin, Node End, Character.CharacterType CT)
    {
        index = 0;
        List<Node> path = new List<Node>();
        // Initialize values and nodes
        if (Begin == End) { return null; }

        startNode = Begin;
        endNode = End;
        startNode.G = 0;
        foreach (Node node in nodes)
        {
            node.CalculateH(endNode);
            node.State = NodeState.Untested;
            node.ParentNode = null;

            if (node.GetComponent<Hex>().EntityHolding == null || node.GetComponent<Hex>().HasSameType(CT))
            {
                node.IsWalkable = true;
            }
            else
            {
                node.IsWalkable = false;
            }

        }
        OpenNodes = new List<Node>();
        ClosedNodes = new List<Node>();

        // Add Starting point into list to become closed
        OpenNodes.Add(startNode);
        // Search for a path between start point and end point and return true if possible 
        bool success = Search(startNode);
        if (success)
        {
            // Reverse the path list for path from start to end
            Node node = this.endNode;
            while (node.ParentNode != null)
            {
                path.Add(node);
                node = node.ParentNode;
            }
            path.Reverse();
        }
        return path;
    }

    public List<Node> FindPathWithMoveLimit(Node Begin, Node End, Character.CharacterType CT, int Distance)
    {
        index = 0;
        List<Node> path = new List<Node>();
        // Initialize values and nodes
        if (Begin == End) { return null; }

        startNode = Begin;
        endNode = End;
        startNode.G = 0;
        foreach (Node node in nodes)
        {
            node.CalculateH(endNode);
            node.State = NodeState.Untested;
            node.ParentNode = null;

            if (node.GetComponent<Hex>().EntityHolding == null || node.GetComponent<Hex>().HasSameType(CT))
            {
                node.IsWalkable = true;
            }
            else
            {
                node.IsWalkable = false;
            }

        }
        OpenNodes = new List<Node>();
        ClosedNodes = new List<Node>();

        // Add Starting point into list to become closed
        OpenNodes.Add(startNode);
        // Search for a path between start point and end point and return true if possible 
        bool success = Search(startNode, Distance);
        if (success)
        {
            // Reverse the path list for path from start to end
            Node node = this.endNode;
            while (node.ParentNode != null)
            {
                path.Add(node);
                node = node.ParentNode;
            }
            path.Reverse();
        }
        return path;
    }


    private bool Search(Node currentNode, int limit = 1000)
    {
        index++;
        //Make the current node closed since its being evaluated
        currentNode.State = NodeState.Closed;
        OpenNodes.Remove(currentNode);
        ClosedNodes.Add(currentNode);

        // Add the viable nodes that are adjacent to the current one to the Open list
        OpenNodes.AddRange(GetAdjacentWalkableNodes(currentNode, limit));

        if (OpenNodes.Contains(endNode)) { return true; }
        // Check if any open nodes available, if not then no path possible 
        if (OpenNodes.Count > 0)
        {
            // Find the next node with the lowest F value
            Node minFNode = OpenNodes[0];
            foreach (var nextNode in OpenNodes)
            {
                if (nextNode.F < minFNode.F)
                {
                    minFNode = nextNode;
                }
            }

            // If not repeat the process
            if (Search(minFNode, limit)) // Note: Recurses back into Search(Node)
                return true;
        }
        return false;
    }

    public Node DiskatasWithArea(Node fromNode, List<Node> nodesToReach, Character.CharacterType CT)
    {
        List<Node> NodesInRange = new List<Node>();
        List<Node> Visited = new List<Node>();
        List<Node> Queue = new List<Node>();
        Queue.Add(fromNode);
        Visited.Add(fromNode);
        while ((Queue.Count > 0))
        {
            if (nodesToReach.Contains(Queue[0]) && Queue[0].NodeHex.EntityHolding == null) { break; }
            Node node = Queue[0];
            List<Node> adjacentNodes = GetDiskatasAdjacentWalkableNodes(node, CT);
            foreach (Node adjacentNode in adjacentNodes)
            {
                if (!Visited.Contains(adjacentNode))
                {
                    NodesInRange.Add(adjacentNode);
                    Visited.Add(adjacentNode);
                    Queue.Add(adjacentNode);
                }
            }
            Queue.Remove(node);
        }
        Node NodeClosest = null;
        if (Queue.Count > 0) { NodeClosest = Queue[0]; }
        return NodeClosest;
    }

    public List<Node> Diskatas(Node fromNode, int distance, Character.CharacterType CT)
    {
        List<Node> NodesInRange = new List<Node>();
        List<Node> Visited = new List<Node>();
        List<Node> Queue = new List<Node>();
        fromNode.G = 0;
        Queue.Add(fromNode);
        Visited.Add(fromNode);
        while ((Queue.Count > 0))
        {
            //Debug.Log(Queue.Count);
            Node node = Queue[0];
            List<Node> adjacentNodes = GetDiskatasAdjacentWalkableNodes(node, CT);
            foreach(Node adjacentNode in adjacentNodes)
            {
                if (!Visited.Contains(adjacentNode))
                {
                    adjacentNode.G = node.G + 1;
                    NodesInRange.Add(adjacentNode);
                    Visited.Add(adjacentNode);
                    if (adjacentNode.G < distance) { Queue.Add(adjacentNode); }
                }
            }
            Queue.Remove(node);

        }
        return NodesInRange;
    }

    private List<Node> GetDiskatasAdjacentWalkableNodes(Node fromNode, Character.CharacterType myCT)
    {
        List<Node> walkableNodes = new List<Node>();

        //ClosedDoor
        if (fromNode.GetComponent<Door>() != null && fromNode.GetComponent<Door>().isOpen == false) { return walkableNodes; }

        Node[] nextLocations = controller.GetNeighbors(fromNode);
        foreach (Node node in nextLocations)
        {
            if (node == null) { continue; }
            //Enemy Character
            if (node.NodeHex.EntityHolding != null && node.NodeHex.EntityHolding.GetComponent<Character>() != null && node.NodeHex.EntityHolding.GetComponent<Character>().myCT != myCT)
            {
                continue;
            }
            //Object
            if (node.NodeHex.EntityHolding != null && node.NodeHex.EntityHolding.GetComponent<Character>() == null)
            {
                continue;
            }

            // Ignore non-walkable nodes
            if (!node.isAvailable) { continue; }
            if (node.edge) { continue; }
            // Ignore already-closed nodes
            if (!Connected(fromNode.RoomName, node.RoomName)) { continue; }
            walkableNodes.Add(node);
        }

        return walkableNodes;
    }


    private List<Node> GetAdjacentWalkableNodes(Node fromNode, int limit = 1000)
    {
        List<Node> walkableNodes = new List<Node>();

        //ClosedDoor
        if (fromNode.GetComponent<Door>() != null && fromNode.GetComponent<Door>().isOpen == false) { return walkableNodes; }
        //List<Node> nextLocations = controller.GetRealNeighbors(fromNode);
        Node[] nextLocations = controller.GetNeighbors(fromNode);

        foreach (Node node in nextLocations)
        {
            if (ClosedNodes.Contains(node)){ continue; }
            if (node == null) { continue; }
            // Ignore non-walkable nodes
            if (!node.IsWalkable) { continue; }
            if (!node.isAvailable) { continue; }
            if (node.edge) { continue; }
            // Ignore already-closed nodes
            if (node.State == NodeState.Closed) { continue; }
            if (!Connected(fromNode.RoomName, node.RoomName)) { continue; }
            if ((fromNode.G + 1) == limit && node.NodeHex.EntityHolding != null) { continue;}

            // Already-open nodes are only added to the list if their G-value is lower going via this route.
            if (node.State == NodeState.Open)
            {
                float traversalCost = 1;
                float gTemp = fromNode.G + traversalCost;
                if (gTemp <= node.G)
                {
                    node.G = gTemp;
                    node.ParentNode = fromNode;
                    walkableNodes.Add(node);
                }
            }
            else
            {
                // If it's untested, set the parent and flag it as 'Open' for consideration
                float traversalCost = 1;
                float gTemp = fromNode.G + traversalCost;
                node.G = gTemp;
                node.ParentNode = fromNode;
                node.State = NodeState.Open;
                walkableNodes.Add(node);
            }
        }

        return walkableNodes;
    }

    bool Connected(List<string> Room1, List<string> Room2)
    {
        return Room1.Contains(Room2[0]) || Room2.Contains(Room1[0]);
    }


}
