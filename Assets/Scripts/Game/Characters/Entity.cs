using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public Hex HexOn;

    public void StartOnHex(Hex hex)
    {
        LinktoHex(hex);
    }

    public void LinktoHex(Hex hex)
    {
        if (hex.EntityHolding == null) { hex.AddEntityToHex(this); }
        HexOn = hex;
    }

    public void RemoveLinkFromHex()
    {
        if (HexOn.EntityHolding == this)
        {
            HexOn.RemoveEntityFromHex();
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
