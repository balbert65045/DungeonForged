using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node2 : MonoBehaviour {

    public AxialHex HexPoint;

    public int q { get { return HexPoint.q; } }
    public int r { get { return HexPoint.r; } }
    public int s { get { return HexPoint.s; } }


    public void SetNode(int q, int r)
    {
        HexPoint = new AxialHex(q, r);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
