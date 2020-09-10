using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DoorNew))]
public class DoorNewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DoorNew doorNew = (DoorNew)target;
        if (GUILayout.Button("Create Door"))
        {
            doorNew.CreateDoorHex("S");
        }
    }
}
