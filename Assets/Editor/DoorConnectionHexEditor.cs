using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(doorConnectionHex))]
public class DoorConnectionHexEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        doorConnectionHex connectionHex = (doorConnectionHex)target;
        if (GUILayout.Button("Show Door Room"))
        {
            connectionHex.ShowHexesInRoom();
        }
        if (GUILayout.Button("Hide Door Room"))
        {
            connectionHex.HideHexesInRoom();
        }
    }
}
