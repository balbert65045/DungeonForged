using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModifierController : MonoBehaviour {

    public string[] modifiers;

    public string GetRandomModifier()
    {
        return modifiers[Random.Range(0, modifiers.Length)];
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
