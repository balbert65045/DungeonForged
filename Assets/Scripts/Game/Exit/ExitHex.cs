using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitHex : MonoBehaviour {

    public enum ExitLocation
    {
        Right = 1,
        Left = 2,
        Middle = 3
    }
    public ExitLocation myExitLocation;

    public GameObject exit;

    public void BuildExit()
    {
        InteractionObjects parent = FindObjectOfType<InteractionObjects>();
        exit = Instantiate(GetComponent<Hex>().ExitPrefab, parent.transform);
        exit.transform.localPosition = transform.position + (Vector3.up * .8f);
        switch (myExitLocation)
        {
            case ExitLocation.Left:
                exit.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                break;
            case ExitLocation.Right:
                exit.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                break;
            case ExitLocation.Middle:
                exit.transform.rotation = Quaternion.Euler(Vector3.zero);
                break;
        }
    }

    public void ShowExit()
    {
        exit.gameObject.SetActive(true);
        gameObject.AddComponent<ExitAreaHex>();
    }

    public void ShowWinArea()
    {
        Node node = GetComponent<Node>();
        HexMapController map = FindObjectOfType<HexMapController>();
        WinArea winArea = FindObjectOfType<WinArea>();
        MeshGenerator meshGen = winArea.GetComponentInChildren<MeshGenerator>();
        EdgeLine edgeLine = winArea.GetComponentInChildren<EdgeLine>();
        List<Node> ExitNodes = new List<Node>();
        List<Node> AdjacentNodes = map.GetNodesAtDistanceFromNode(node, 2);
        foreach (Node Anode in AdjacentNodes) {
            if (Anode.Shown && !Anode.edge)
            {
                ExitNodes.Add(Anode);
                Anode.gameObject.AddComponent<ExitAreaHex>();
            }
        }
        List<Vector3> points = map.GetHexesSurrounding(node, ExitNodes);
        meshGen.CreateMesh(points);
        edgeLine.CreateLine(points.ToArray());
    }

    public void HideExit()
    {
        exit.gameObject.SetActive(false);
    }
}
