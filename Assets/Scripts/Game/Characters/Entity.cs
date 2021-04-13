using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public Sprite characterIcon;

    public Hex HexOn;

    public bool IsPlayer() { return GetComponent<PlayerCharacter>() != null; }
    public bool IsEnemy() { return GetComponent<EnemyCharacter>() != null; }

    public bool IsEnemySpawner() { return GetComponent<EnemySpawner>() != null; }

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
