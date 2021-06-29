using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Hex))]
public class HexEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Hex myHex = (Hex)target;
        if (GUILayout.Button("Make Money Chest"))
        {
            myHex.CreateMoneyChest();
        }
        if (GUILayout.Button("GenerateCharacter"))
        {
            myHex.GenerateCharacter();
        }
    }
}
